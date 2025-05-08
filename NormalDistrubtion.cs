using System;

namespace courseworkTMAS
{
    // This class simulates a Normal (Gaussian) distribution for generating random values.
    class NormalDistrubtion
    {
        // Parameters for the normal distribution
        private readonly int _standardDeviation;  // Standard deviation (σ)
        private readonly int _expectation;        // Mean (μ)
        private readonly int _range;              // Total number of sample points

        private double[] _values; // Precomputed normal distribution density values
        private Random _rand;     // Random number generator

        // Constructor initializes the distribution parameters and precomputes the density function
        public NormalDistrubtion(int expectation, int standardDeviation)
        {
            _expectation = expectation;
            _standardDeviation = standardDeviation;

            // Range of samples (from -5σ to +5σ) with precision of 0.0001
            _range = ((standardDeviation + 5) * 2 + 1) * 10000;

            GetNormalDistrubtionFunctionValues(); // Fill _values array with density values

            _rand = new Random(); // Initialize RNG
        }

        // Fills _values with normal distribution probability density function (PDF) values
        private void GetNormalDistrubtionFunctionValues()
        {
            _values = new double[_range];
            for (int i = 0; i < _values.Length; i++)
            {
                // x ranges from -((range - 1) / 2) / 10000 to +((range - 1) / 2) / 10000
                double x = -1 * ((_range - 1) / 2) / 10000.0 + i / 10000.0;

                // Gaussian PDF: f(x) = (1 / √(2πσ²)) * exp(-(x - μ)² / (2σ²))
                _values[i] = (1 / Math.Sqrt(2 * Math.PI * Math.Pow(_standardDeviation, 2))) *
                             Math.Exp(-Math.Pow(x - _expectation, 2) / (2 * Math.Pow(_standardDeviation, 2)));
            }
        }

        // Returns a random value from the precomputed distribution values (density sampling)
        public double GetRandomValueOfNormalDistrubtion()
        {
            int index = _rand.Next(_values.Length); // Pick random index
            return _values[index];
        }

        // Returns an array of n random values from the distribution
        public double[] GetRandomValuesArray(int n)
        {
            double[] array = new double[n];
            for (int i = 0; i < n - 1; i++) // NOTE: Bug: should use i < n instead of i < n - 1
                array[i] = GetRandomValueOfNormalDistrubtion();

            return array;
        }
    }
}
