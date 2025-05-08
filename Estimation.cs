using System;
using System.Linq;

namespace courseworkTMAS
{
    // This class is responsible for estimating parameters of a linear model and their confidence intervals.
    class Estimation
    {
        // Constants representing t-values from Student's t-distribution for given degrees of freedom
        private const double T_test51Elements = 2.0095752371292397; // for 49 df (n = 51)
        private const double T_test11Elements = 2.2621571628540993; // for 9 df (n = 11)

        private readonly DataSet _dataSet; // Dataset containing x range information
        private readonly int[] _x; // Array of x values calculated from the dataset
        private readonly double[] _functionYValues; // Array of y values provided for regression
        private readonly int _numberOfElements; // Number of data points

        // Estimated regression coefficients and statistics
        private double _estimatedA, _estimatedB, _estimatedStandardDeviation;
        private double _seA, _seB; // Standard errors
        private double _confidenceIntervalMinA, _confidenceIntervalMaxA;
        private double _confidenceIntervalMinB, _confidenceIntervalMaxB;
        private double _confidenceIntervalMinStandardDeviation, _confidenceIntervalMaxStandardDeviation;

        // Public properties to access estimates and confidence intervals
        public double EstimatedA { get => _estimatedA; }
        public double EstimatedB { get => _estimatedB; }
        public double EstimatedStandardDeviation { get => _estimatedStandardDeviation; }

        public string confidenceIntervalA { get => $"{_confidenceIntervalMinA}, {_confidenceIntervalMaxA}"; }
        public string confidenceIntervalB { get => $"{_confidenceIntervalMinB}, {_confidenceIntervalMaxB}"; }
        public string confidenceIntervalStandardDeviation { get => $"{_confidenceIntervalMinStandardDeviation}, {_confidenceIntervalMaxStandardDeviation}"; }

        // Constructor: Initializes data and triggers estimation procedures
        public Estimation(DataSet dataSet, double[] functionYValues)
        {
            _dataSet = dataSet;
            _numberOfElements = dataSet.NumberOfElements;
            _functionYValues = functionYValues;

            // Generate x values based on dataset min and increment
            _x = new int[_numberOfElements];
            for (int i = 0; i < _numberOfElements; i++)
                _x[i] = dataSet.MinX + i * dataSet.Incriment;

            // Perform all estimation steps
            EstimateA();
            EstimateB();
            EstimateStandardDeviation();
            CalculateSEs();
            CalculateConfidenceIsnterval();
        }

        // Approximates the Chi-squared quantile value using a probit approximation method
        private double CalculateChisquaredQuantile(double a, int numberOFElements)
        {
            int n = numberOFElements - 2;
            double probit;

            // Convert probability into a z-score-like value
            if (a > 0.5)
                probit = 2.0637 * Math.Pow((Math.Log(1 / (1 - a)) - 0.16), 0.4274) - 1.5774;
            else
                probit = -2.0637 * Math.Pow((Math.Log(1 / a) - 0.16), 0.4274) + 1.5774;

            // Use polynomial approximation formula for Chi-squared quantile
            double coefA = probit * Math.Sqrt(2);
            double coefB = (2 / 3.0) * (Math.Pow(probit, 2) - 1);
            double coefC = probit * ((Math.Pow(probit, 2) - 7) / 9.0 * Math.Sqrt(2));
            double coefD = (6 * Math.Pow(probit, 4) + 14 * Math.Pow(probit, 2) - 32) / 405.0;
            double coefE = probit * ((9 * Math.Pow(probit, 4) + 256 * Math.Pow(probit, 2) - 433) / 4860.0 * Math.Sqrt(2));

            return n + coefA * Math.Sqrt(2) + coefB + coefC / Math.Sqrt(n) + coefD / n + coefE / (n * Math.Sqrt(n));
        }

        // Estimate the slope 'A' of the linear regression using least squares
        private void EstimateA()
        {
            double[] sumTerms = new double[_numberOfElements];
            for (int i = 0; i < _numberOfElements; i++)
                sumTerms[i] = (_x[i] - _x.Average()) * (_functionYValues[i] - _functionYValues.Average());

            double sum = sumTerms.Sum();

            for (int i = 0; i < _numberOfElements; i++)
                sumTerms[i] = Math.Pow((_x[i] - _x.Average()), 2);

            _estimatedA = sum / sumTerms.Sum();
        }

        // Estimate the intercept 'B' of the regression
        private void EstimateB() =>
            _estimatedB = _functionYValues.Average();

        // Estimate the standard deviation of residuals
        private void EstimateStandardDeviation()
        {
            double[] sumTurms = new double[_numberOfElements];
            for (int i = 0; i < _numberOfElements; i++)
                sumTurms[i] = Math.Pow((_functionYValues[i] - _estimatedA * _x[i] - _estimatedB), 2);

            _estimatedStandardDeviation = sumTurms.Sum() / (_numberOfElements - 2);
        }

        // Calculate standard errors of coefficients A and B
        private void CalculateSEs()
        {
            double[] sumTerms = new double[_numberOfElements];
            for (int i = 0; i < _numberOfElements; i++)
                sumTerms[i] = Math.Pow((_x[i] - _x.Average()), 2);

            double sum = sumTerms.Sum();

            _seA = Math.Sqrt(_estimatedStandardDeviation / sum);
            _seB = Math.Sqrt(_estimatedStandardDeviation * (1.0 / _numberOfElements + Math.Pow(_x.Average(), 2) / sum));
        }

        // Calculate confidence intervals for A, B, and standard deviation
        private void CalculateConfidenceIsnterval()
        {
            _confidenceIntervalMinA = _estimatedA - T_test11Elements * _seA;
            _confidenceIntervalMaxA = _estimatedA + T_test11Elements * _seA;

            _confidenceIntervalMinB = _estimatedB - T_test51Elements * _seB;
            _confidenceIntervalMaxB = _estimatedB + T_test51Elements * _seB;

            _confidenceIntervalMinStandardDeviation = ((_numberOfElements - 2) * _estimatedStandardDeviation) / CalculateChisquaredQuantile(0.95, _numberOfElements);
            _confidenceIntervalMaxStandardDeviation = ((_numberOfElements - 2) * _estimatedStandardDeviation) / CalculateChisquaredQuantile(0.05, _numberOfElements);
        }
    }
}
