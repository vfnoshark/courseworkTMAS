namespace courseworkTMAS
{
    // This class calculates and stores the values of a linear function with added noise.
    class FunctionY
    {
        // Stores the computed Y values (function results)
        private double[] _values;

        // Public getter for the Y values
        public double[] Values { get => _values; }

        // The data set that provides model parameters and configuration
        private readonly DataSet _dataSet;

        // Array of normally distributed random noise to be added to the function
        private readonly double[] _randomValuesOfNormalDistrubtion;

        // Constructor takes a dataset and random errors, then computes the Y values
        public FunctionY(DataSet dataSet, double[] randomValuesOfNormalDistrubtion)
        {
            _dataSet = dataSet;
            _values = new double[dataSet.NumberOfElements]; // initialize array
            _randomValuesOfNormalDistrubtion = randomValuesOfNormalDistrubtion;

            // Compute the function values immediately upon construction
            GetFunctionValues();
        }

        // Computes the Y values: Y = a * x + b + E, where E is the normal random error
        private void GetFunctionValues()
        {
            double[] values = new double[_values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                // Calculate x_i = MinX + i * Incriment
                // Then compute Y_i = a * x_i + b + noise
                values[i] = _dataSet.A * (_dataSet.MinX + i * _dataSet.Incriment)
                            + _dataSet.B
                            + _randomValuesOfNormalDistrubtion[i];
            }
            _values = values; // Store results
        }
    }
}
