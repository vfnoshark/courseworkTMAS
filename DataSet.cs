using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace courseworkTMAS
{
    // This class stores configuration data for generating a dataset used in modeling and estimation
    class DataSet
    {
        // True slope of the model function (a in Y = aX + b + noise)
        private readonly double _a;
        public double A { get => _a; }

        // True intercept of the model function (b in Y = aX + b + noise)
        private readonly double _b;
        public double B { get => _b; }

        // Step size between consecutive x values
        private readonly int _incriment;
        public int Incriment { get => _incriment; }

        // Standard deviation (σ) of the normal distribution used for noise
        private readonly int _standardDeviation;
        public int StandardDeviation { get => _standardDeviation; }

        // Number of elements in the dataset (number of data points)
        private readonly int _numberOfElements;
        public int NumberOfElements { get => _numberOfElements; }

        // Minimum X value (fixed at -25)
        private readonly int _minX = -25;
        public int MinX { get => _minX; }

        // Maximum X value (fixed at 25)
        private readonly int _maxX = 25;
        public int MaxX { get => _maxX; }

        // Constructor calculates a and b based on two input parameters N and M,
        // and sets other parameters directly
        public DataSet(double N, double M, int incriment, int n, int sigma)
        {
            // Calculate coefficient a as a function of N and M
            _a = M + (N / 7);
            // Calculate coefficient b as a function of N and M
            _b = (N / 3) + M / 2;

            _incriment = incriment;               // Step between x-values
            _standardDeviation = sigma;           // Standard deviation for the normal noise
            _numberOfElements = n;                // Number of data points
        }
    }
}
