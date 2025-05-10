using System;
using System.Linq;

namespace CourseworkTMAS
{
    /// <summary>
    /// Estimates parameters of a linear model (Y = aX + b + ε) and their confidence intervals
    /// </summary>
    public class LinearRegressionEstimator
    {
        // Critical values from Student's t-distribution (90% confidence)
        private const double TValueFor11Samples = 2.262; // df = 9 (n=11)
        private const double TValueFor51Samples = 2.010; // df = 49 (n=51)

        private readonly DataSetConfiguration _dataSet;
        private readonly double[] _xValues;
        private readonly double[] _yValues;
        private readonly int _sampleSize;

        // Estimation results
        public double EstimatedSlope { get; private set; }
        public double EstimatedIntercept { get; private set; }
        public double EstimatedSigma { get; private set; }

        // Standard errors
        public double SlopeStandardError { get; private set; }
        public double InterceptStandardError { get; private set; }

        // Confidence intervals
        public ConfidenceInterval SlopeConfidenceInterval { get; private set; }
        public ConfidenceInterval InterceptConfidenceInterval { get; private set; }
        public ConfidenceInterval SigmaConfidenceInterval { get; private set; }

        public LinearRegressionEstimator(DataSetConfiguration dataSet, double[] yValues)
        {
            _dataSet = dataSet ?? throw new ArgumentNullException(nameof(dataSet));
            _yValues = yValues ?? throw new ArgumentNullException(nameof(yValues));
            _sampleSize = dataSet.SampleCount;

            ValidateInputs();

            _xValues = GenerateXValues();
            EstimateParameters();
            CalculateConfidenceIntervals();
        }

        private void ValidateInputs()
        {
            if (_yValues.Length != _sampleSize)
                throw new ArgumentException("Y values count must match sample size");

            if (_sampleSize < 3)
                throw new ArgumentException("Sample size must be at least 3 for regression");
        }

        private double[] GenerateXValues()
        {
            return Enumerable.Range(0, _sampleSize)
                .Select(i => (double)(_dataSet.MinX + i * _dataSet.Increment))
                .ToArray();
        }

        private void EstimateParameters()
        {
            var xMean = _xValues.Average();
            var yMean = _yValues.Average();

            // Calculate slope (a)
            var covariance = _xValues.Zip(_yValues, (x, y) => (x - xMean) * (y - yMean)).Sum();
            var xVariance = _xValues.Sum(x => Math.Pow(x - xMean, 2));
            EstimatedSlope = covariance / xVariance;

            // Calculate intercept (b)
            EstimatedIntercept = yMean - EstimatedSlope * xMean;

            // Calculate residual standard deviation
            var residuals = _yValues.Zip(_xValues, (y, x) => y - (EstimatedSlope * x + EstimatedIntercept));
            EstimatedSigma = Math.Sqrt(residuals.Sum(r => r * r) / (_sampleSize - 2));
        }

        private void CalculateConfidenceIntervals()
        {
            var xMean = _xValues.Average();
            var xVariance = _xValues.Sum(x => Math.Pow(x - xMean, 2));

            // Standard errors
            SlopeStandardError = Math.Sqrt(EstimatedSigma / xVariance);
            InterceptStandardError = Math.Sqrt(EstimatedSigma * (1.0 / _sampleSize + xMean * xMean / xVariance));

            // Determine appropriate t-value based on sample size
            var tValue = _sampleSize == 11 ? TValueFor11Samples : TValueFor51Samples;

            // Confidence intervals for coefficients
            SlopeConfidenceInterval = new ConfidenceInterval(
                EstimatedSlope - tValue * SlopeStandardError,
                EstimatedSlope + tValue * SlopeStandardError);

            InterceptConfidenceInterval = new ConfidenceInterval(
                EstimatedIntercept - tValue * InterceptStandardError,
                EstimatedIntercept + tValue * InterceptStandardError);

            // Confidence interval for sigma (using chi-square distribution)
            var df = _sampleSize - 2;
            SigmaConfidenceInterval = new ConfidenceInterval(
                df * EstimatedSigma / ChiSquareQuantile(0.95, df),
                df * EstimatedSigma / ChiSquareQuantile(0.05, df));
        }

        /// <summary>
        /// Approximates the Chi-squared quantile using probit approximation
        /// </summary>
        private double ChiSquareQuantile(double probability, int degreesOfFreedom)
        {
            if (probability <= 0 || probability >= 1)
                throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0 and 1");

            double probit = probability > 0.5
                ? 2.0637 * Math.Pow(Math.Log(1 / (1 - probability)) - 0.16, 0.4274) - 1.5774
                : -2.0637 * Math.Pow(Math.Log(1 / probability) - 0.16, 0.4274) + 1.5774;

            double a = probit * Math.Sqrt(2);
            double b = (2 / 3.0) * (Math.Pow(probit, 2) - 1);
            double c = probit * (Math.Pow(probit, 2) - 7) / (9 * Math.Sqrt(2));
            double d = (6 * Math.Pow(probit, 4) + 14 * Math.Pow(probit, 2) - 32) / 405.0;
            double e = probit * (9 * Math.Pow(probit, 4) + 256 * Math.Pow(probit, 2) - 433) / (4860 * Math.Sqrt(2));

            return degreesOfFreedom + a * Math.Sqrt(2) + b + c / Math.Sqrt(degreesOfFreedom)
                   + d / degreesOfFreedom + e / (degreesOfFreedom * Math.Sqrt(degreesOfFreedom));
        }
    }

    /// <summary>
    /// Represents a confidence interval with lower and upper bounds
    /// </summary>
    public readonly struct ConfidenceInterval
    {
        public double LowerBound { get; }
        public double UpperBound { get; }

        public ConfidenceInterval(double lower, double upper)
        {
            LowerBound = lower;
            UpperBound = upper;
        }

        public override string ToString() => $"[{LowerBound:F4}, {UpperBound:F4}]";
    }
}