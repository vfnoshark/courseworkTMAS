using System;

namespace CourseworkTMAS
{
    /// <summary>
    /// Represents configuration data for generating a dataset used in linear modeling (Y = aX + b + noise)
    /// </summary>
    public class DataSetConfiguration
    {
        private const int DefaultMinX = -25;
        private const int DefaultMaxX = 25;

        /// <summary>
        /// True slope of the model function (a in Y = aX + b + noise)
        /// </summary>
        public double Slope { get; }

        /// <summary>
        /// True intercept of the model function (b in Y = aX + b + noise)
        /// </summary>
        public double Intercept { get; }

        /// <summary>
        /// Step size between consecutive x values
        /// </summary>
        public int Increment { get; }

        /// <summary>
        /// Standard deviation (σ) of the normal distribution used for noise
        /// </summary>
        public int NoiseStandardDeviation { get; }

        /// <summary>
        /// Number of elements in the dataset (number of data points)
        /// </summary>
        public int SampleCount { get; }

        /// <summary>
        /// Minimum X value in the dataset
        /// </summary>
        public int MinX { get; } = DefaultMinX;

        /// <summary>
        /// Maximum X value in the dataset
        /// </summary>
        public int MaxX { get; } = DefaultMaxX;

        /// <summary>
        /// Initializes a new dataset configuration
        /// </summary>
        /// <param name="parameterN">First modeling parameter</param>
        /// <param name="parameterM">Second modeling parameter</param>
        /// <param name="increment">Step between x-values</param>
        /// <param name="sampleCount">Number of data points</param>
        /// <param name="noiseStandardDeviation">Standard deviation for the normal noise</param>
        public DataSetConfiguration(double parameterN, double parameterM,
                                   int increment, int sampleCount, int noiseStandardDeviation)
        {
            if (increment <= 0)
                throw new ArgumentOutOfRangeException(nameof(increment), "Increment must be positive");
            if (sampleCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(sampleCount), "Sample count must be positive");
            if (noiseStandardDeviation <= 0)
                throw new ArgumentOutOfRangeException(nameof(noiseStandardDeviation), "Standard deviation must be positive");

            Slope = CalculateSlope(parameterN, parameterM);
            Intercept = CalculateIntercept(parameterN, parameterM);
            Increment = increment;
            SampleCount = sampleCount;
            NoiseStandardDeviation = noiseStandardDeviation;
        }

        private static double CalculateSlope(double parameterN, double parameterM)
        {
            return parameterM + (parameterN / 7);
        }

        private static double CalculateIntercept(double parameterN, double parameterM)
        {
            return (parameterN / 3) + (parameterM / 2);
        }
    }
}