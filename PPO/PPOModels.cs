using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Numerics;

namespace ArcCore.PPO
{
    /// <summary>
    /// Модель актора (политики) для алгоритма PPO
    /// Отвечает за выбор действий на основе текущего состояния
    /// </summary>
    public class ActorModel
    {
        private readonly InferenceSession session;    // Сессия ONNX Runtime для выполнения модели
        private readonly int inputSize;               // Размер входного слоя
        private readonly int outputSize;              // Размер выходного слоя

        /// <summary>
        /// Инициализирует новую модель актора
        /// </summary>
        /// <param name="inputSize">Размер входного слоя</param>
        /// <param name="outputSize">Размер выходного слоя</param>
        /// <param name="options">Настройки сессии ONNX Runtime</param>
        public ActorModel(int inputSize, int outputSize, SessionOptions? options = null)
        {
            this.inputSize = inputSize;
            this.outputSize = outputSize;
            
            var modelPath = "C:\\git\\Arcam\\PPO\\models\\2025-06-20__3__window200\\actor_model_2025-06-20__3__window200.onnx";
            session = new InferenceSession(modelPath, options ?? new SessionOptions());
        }

        /// <summary>
        /// Возвращает текущую сессию ONNX Runtime
        /// </summary>
        /// <returns>Сессия ONNX Runtime</returns>
        public InferenceSession GetSession() => session;

        /// <summary>
        /// Выполняет прямой проход через модель актора
        /// </summary>
        /// <param name="state">Входное состояние</param>
        /// <returns>Кортеж (действие, логарифм вероятности)</returns>
        public (float[] action, float logProb) Forward(float[] state, float in_pos, float is_long, float price)
        {
            var inputTensor = new DenseTensor<float>(new[] { 1, inputSize });
            for (int i = 0; i < inputSize; i++)
            {
                inputTensor[0, i] = state[i];
            }
            var inposTensor = new DenseTensor<float>(new[] { 1, 1 }); 
            inposTensor[0,0] = in_pos;
            var islongTensor = new DenseTensor<float>(new[] { 1, 1 });
            islongTensor[0,0] = is_long;
            var priceTensor = new DenseTensor<float>(new[] { 1, 1 });
            priceTensor[0, 0] = price;

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", inputTensor),
                NamedOnnxValue.CreateFromTensor("in_position", inposTensor),
                NamedOnnxValue.CreateFromTensor("is_long", islongTensor),
                NamedOnnxValue.CreateFromTensor("enter_price", priceTensor)
            };

            using var results = session.Run(inputs);
            var output = results.First().AsTensor<float>();
            
            var action = new float[outputSize];
            var logProb = 0f;

            // Преобразуем выход модели в действия и вероятности
            for (int i = 0; i < outputSize; i++)
            {
                action[i] = output[0, i];
                logProb += (float)Math.Log(output[0, i] + 1e-8);
            }

            return (action, logProb);
        }
    }

    /// <summary>
    /// Модель критика (оценки ценности) для алгоритма PPO
    /// Отвечает за оценку ценности текущего состояния
    /// </summary>
    public class CriticModel
    {
        private readonly InferenceSession session;    // Сессия ONNX Runtime для выполнения модели
        private readonly int inputSize;               // Размер входного слоя

        /// <summary>
        /// Инициализирует новую модель критика
        /// </summary>
        /// <param name="inputSize">Размер входного слоя</param>
        /// <param name="options">Настройки сессии ONNX Runtime</param>
        public CriticModel(int inputSize, SessionOptions? options = null)
        {
            this.inputSize = inputSize;
            
            var modelPath = "C:\\git\\Arcam\\ArcamFullPick\\bin\\Debug\\net8.0\\models\\critic_model.onnx";
            session = new InferenceSession(modelPath, options ?? new SessionOptions());
        }

        /// <summary>
        /// Возвращает текущую сессию ONNX Runtime
        /// </summary>
        /// <returns>Сессия ONNX Runtime</returns>
        public InferenceSession GetSession() => session;

        /// <summary>
        /// Выполняет прямой проход через модель критика
        /// </summary>
        /// <param name="state">Входное состояние</param>
        /// <returns>Оценка ценности состояния</returns>
        public float Forward(float[] state)
        {
            var inputTensor = new DenseTensor<float>(new[] { 1, inputSize });
            for (int i = 0; i < inputSize; i++)
            {
                inputTensor[0, i] = state[i];
            }

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", inputTensor)
            };

            using var results = session.Run(inputs);
            var output = results.First().AsTensor<float>();
            
            return output[0, 0];
        }
    }
} 