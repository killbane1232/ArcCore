using Arcam.Data.DataTypes;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Numerics;
using System.Threading.Tasks;

namespace ArcCore.PPO
{
    /// <summary>
    /// Реализация алгоритма Proximal Policy Optimization (PPO) для обучения с подкреплением
    /// на данных свечей. Использует актор-критик архитектуру с оптимизациями для CPU.
    /// </summary>
    public class PPO
    {
        private readonly int inputSize = 200;        // Количество входных свечей
        private readonly int outputSize = 4;         // Количество выходных действий
        private readonly float learningRate = 0.0003f; // Скорость обучения
        private readonly float gamma = 0.99f;        // Коэффициент скидки для будущих наград
        private readonly float clipEpsilon = 0.2f;   // Параметр клиппинга для PPO
        private readonly int batchSize = 64;         // Размер батча для обучения
        private readonly int epochs = 10;            // Количество эпох обучения
        private readonly int numThreads = Environment.ProcessorCount; // Количество потоков для параллельной обработки

        private ActorModel actorModel;               // Модель актора (политики)
        private CriticModel criticModel;             // Модель критика (оценки ценности)
        private List<float[]> memoryStates = new();  // Память для хранения состояний
        private List<float[]> memoryActions = new(); // Память для хранения действий
        private List<float> memoryRewards = new();   // Память для хранения наград
        private List<float> memoryValues = new();    // Память для хранения оценок ценности
        private List<float> memoryLogProbs = new();  // Память для хранения логарифмов вероятностей

        /// <summary>
        /// Инициализирует новый экземпляр PPO с настроенными моделями актора и критика
        /// </summary>
        public PPO()
        {
            InitializeModels();
        }

        /// <summary>
        /// Инициализирует модели актора и критика с оптимизированными настройками для CPU
        /// </summary>
        private void InitializeModels()
        {
            var options = new SessionOptions();
            options.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;
            options.ExecutionMode = ExecutionMode.ORT_SEQUENTIAL;
            options.IntraOpNumThreads = numThreads;
            options.InterOpNumThreads = numThreads;

            actorModel = new ActorModel(inputSize * 5, outputSize, options);
            criticModel = new CriticModel(inputSize * 5, options);
        }

        /// <summary>
        /// Преобразует список свечей в массив признаков для нейронной сети
        /// </summary>
        /// <param name="candles">Список свечей для обработки</param>
        /// <returns>Массив признаков (OHLCV для каждой свечи)</returns>
        /// <exception cref="ArgumentException">Если количество свечей не соответствует ожидаемому</exception>
        public float[] PreprocessCandles(List<Candle> candles)
        {
            if (candles.Count != inputSize)
                throw new ArgumentException($"Expected {inputSize} candles, got {candles.Count}");

            var features = new float[inputSize * 5];
            Parallel.For(0, candles.Count, new ParallelOptions { MaxDegreeOfParallelism = numThreads }, i =>
            {
                var candle = candles[i];
                var baseIndex = i * 5;
                features[baseIndex] = (float)candle.Open;
                features[baseIndex + 1] = (float)candle.High;
                features[baseIndex + 2] = (float)candle.Low;
                features[baseIndex + 3] = (float)candle.Close;
                features[baseIndex + 4] = (float)candle.Volume;
            });
            return features;
        }

        /// <summary>
        /// Получает действие от модели актора для текущего состояния
        /// </summary>
        /// <param name="state">Текущее состояние в виде списка свечей</param>
        /// <returns>Массив действий</returns>
        public float[] GetAction(List<Candle> state, float in_pos, float is_long, float price)
        {
            var processedState = PreprocessCandles(state);
            var (action, _) = actorModel.Forward(processedState, in_pos, is_long, price);
            return action;
        }

        /// <summary>
        /// Вычисляет преимущества для каждого перехода в памяти
        /// </summary>
        /// <returns>Список преимуществ</returns>
        private List<float> CalculateAdvantages()
        {
            var advantages = new List<float>();
            var returns = new List<float>();
            var runningReturn = 0f;

            // Вычисляем дисконтированные возвраты
            for (int i = memoryRewards.Count - 1; i >= 0; i--)
            {
                runningReturn = memoryRewards[i] + gamma * runningReturn;
                returns.Insert(0, runningReturn);
            }

            // Вычисляем преимущества
            Parallel.For(0, memoryRewards.Count, new ParallelOptions { MaxDegreeOfParallelism = numThreads }, i =>
            {
                var advantage = returns[i] - memoryValues[i];
                lock (advantages)
                {
                    advantages.Add(advantage);
                }
            });

            return advantages;
        }

        /// <summary>
        /// Ограничивает значение в заданном диапазоне
        /// </summary>
        /// <param name="value">Исходное значение</param>
        /// <param name="min">Минимальное значение</param>
        /// <param name="max">Максимальное значение</param>
        /// <returns>Ограниченное значение</returns>
        private float Clip(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
