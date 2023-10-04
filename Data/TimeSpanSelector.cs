namespace Arcam.Data
{
    public class TimeSpanSelector
    {
        public static int GetTimeMultiplier(int timeSpan)
        {
            switch (timeSpan)
            {
                case -2:
                    return 1;
                case -1:
                    return 5;
                case 0:
                    return 60;
                case 1:
                    return 24 * 60;
            }
            throw new ArgumentException("WrongTimespanValue");
        }
    }
}
