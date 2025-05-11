using System;

namespace CourseworkTMAS
{
    /// <summary>
    /// Simulates a Normal (Gaussian) distribution for generating random values.
    /// </summary>
    public class NormalDistrubtion
    {
        private readonly double _increment;
        private const int StandardDeviationsRange = 5; // ±5σ range

        private readonly int _sampleCount;
        private readonly double _mean;
        private readonly double _standardDeviation;
        private readonly double[] _probabilityDensityValues;

        /// <summary>
        /// Initializes a new normal distribution with specified parameters.
        /// </summary>
        /// <param name="mean">The expectation (μ) of the distribution</param>
        /// <param name="standardDeviation">The standard deviation (σ) of the distribution</param>
        public NormalDistrubtion(double mean, double standardDeviation, DataSetConfiguration configuration)
        {
            if (standardDeviation <= 0)
                throw new ArgumentOutOfRangeException(nameof(standardDeviation), "Standard deviation must be positive");

            _mean = mean;
            _standardDeviation = standardDeviation;
            _increment = configuration.Increment;
            _sampleCount = configuration.SampleCount;
            _probabilityDensityValues = CalculateProbabilityDensityFunction();
        }



        /// <summary>
        /// Generates an array of random values from the normal distribution.
        /// </summary>
        /// <param name="count">Number of values to generate</param>

        public double[] CalculateProbabilityDensityFunction()
        {
            int sampleCount = _sampleCount;
            var values = new double[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                double x = CalculateXValue(i, sampleCount);
                values[i] = CalculateGaussianProbabilityDensity(x);
            }

            return values;
        }

        private double CalculateXValue(int index, int sampleCount)
        {
            double halfRange = (sampleCount - 1) * _increment / 2;
            return -halfRange + index * _increment;
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