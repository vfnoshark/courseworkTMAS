using System;

namespace CourseworkTMAS
{
    /// <summary>
    /// Simulates a Normal (Gaussian) distribution for generating random values.
    /// </summary>
    public class NormalDistrubtion
    {
        private const double Precision = 0.0001;
        private const int StandardDeviationsRange = 5; // ±5σ range

        private readonly double _mean;
        private readonly double _standardDeviation;
        private readonly double[] _probabilityDensityValues;
        private readonly Random _random;

        /// <summary>
        /// Initializes a new normal distribution with specified parameters.
        /// </summary>
        /// <param name="mean">The expectation (μ) of the distribution</param>
        /// <param name="standardDeviation">The standard deviation (σ) of the distribution</param>
        public NormalDistrubtion(double mean, double standardDeviation)
        {
            if (standardDeviation <= 0)
                throw new ArgumentOutOfRangeException(nameof(standardDeviation), "Standard deviation must be positive");

            _mean = mean;
            _standardDeviation = standardDeviation;
            _random = new Random();

            _probabilityDensityValues = CalculateProbabilityDensityFunction();
        }

        /// <summary>
        /// Generates a single random value from the normal distribution.
        /// </summary>
        public double GetRandomValue()
        {
            int randomIndex = _random.Next(_probabilityDensityValues.Length);
            return _probabilityDensityValues[randomIndex];
        }

        /// <summary>
        /// Generates an array of random values from the normal distribution.
        /// </summary>
        /// <param name="count">Number of values to generate</param>
        public double[] GetRandomValues(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive");

            var values = new double[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = GetRandomValue();
            }
            return values;
        }

        private double[] CalculateProbabilityDensityFunction()
        {
            int sampleCount = CalculateSampleCount();
            var values = new double[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                double x = CalculateXValue(i, sampleCount);
                values[i] = CalculateGaussianProbabilityDensity(x);
            }

            return values;
        }

        private int CalculateSampleCount()
        {
            double range = StandardDeviationsRange * _standardDeviation * 2;
            return (int)(range / Precision) + 1;
        }

        private double CalculateXValue(int index, int sampleCount)
        {
            double halfRange = (sampleCount - 1) * Precision / 2;
            return -halfRange + index * Precision;
        }

        private double CalculateGaussianProbabilityDensity(double x)
        {
            double variance = Math.Pow(_standardDeviation, 2);
            double exponent = -Math.Pow(x - _mean, 2) / (2 * variance);
            double normalizationFactor = 1 / Math.Sqrt(2 * Math.PI * variance);

            return normalizationFactor * Math.Exp(exponent);
        }
    }
}