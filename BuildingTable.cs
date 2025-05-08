using System;

namespace courseworkTMAS
{
    // Class responsible for generating and displaying a table of function values with noise
    class BuildingTable
    {
        // Stores the normally distributed random errors
        private readonly double[] _normalDistrubtionValues;
        // Public getter for the random errors
        public double[] NormalDistrubtionValues { get => _normalDistrubtionValues; }

        // Stores the resulting Y values after applying the function to X values and adding noise
        private readonly double[] _functionYValues;
        // Public getter for the Y values
        public double[] FunctionYValues { get => _functionYValues; }

        // The dataset object containing parameters like X range, step, and model coefficients
        private readonly DataSet _dataSet;
        // Public getter for the dataset
        public DataSet DataSet { get => _dataSet; }

        // Constructor initializes internal arrays and computes Y values using the function and noise
        public BuildingTable(DataSet dataSet)
        {
            _dataSet = dataSet;

            // Create a normal distribution with mean 0 and standard deviation from the dataset
            NormalDistrubtion normalDistrubtion = new NormalDistrubtion(0, dataSet.StandardDeviation);
            // Generate random noise values for each data point
            _normalDistrubtionValues = normalDistrubtion.GetRandomValuesArray(dataSet.NumberOfElements);

            // Compute function Y = aX + b + noise, store result in _functionYValues
            FunctionY functionY = new FunctionY(dataSet, _normalDistrubtionValues);
            _functionYValues = functionY.Values;
        }

        // Builds and prints the table of computed data and statistical estimations
        public void BuildTable()
        {
            // Output the parameters used for building the dataset
            Console.WriteLine($"Параметры: x in [{_dataSet.MinX}, {_dataSet.MaxX}], h = {_dataSet.Incriment}, n = {_dataSet.NumberOfElements}, StandardDeviation = {_dataSet.StandardDeviation} Истинные параметры модели: a = {_dataSet.A}, b = {_dataSet.B}");

            // Print table header
            Console.WriteLine("   n   |   x   |   E   |   Y");
            Console.WriteLine("____________________________");

            // Iterate through data points and print index, X value, error (E), and Y value
            for (int i = 0; i < _dataSet.NumberOfElements; i++)
            {
                Console.WriteLine($"   {i}   |   {_dataSet.MinX + _dataSet.Incriment * i}   |   {_normalDistrubtionValues[i]}   |   {_functionYValues[i]}");
            }

            // Estimate parameters (a, b, standard deviation) based on Y values
            Estimation estimation = new Estimation(_dataSet, _functionYValues);

            // Print true values of model parameters
            Console.WriteLine($"Истинные значения: a = {_dataSet.A}, b = {_dataSet.B}, StandardDeviation = {_dataSet.StandardDeviation}");

            // Print estimated values of parameters
            Console.WriteLine($"Точечные оценки:  a = {estimation.EstimatedA.ToString()}, b = {estimation.EstimatedB}, StandardDeviation = {estimation.EstimatedStandardDeviation}");

            // Print 90% confidence intervals for each parameter
            Console.WriteLine($"90% ДИ для a: {estimation.confidenceIntervalA}");
            Console.WriteLine($"90% ДИ для b: {estimation.confidenceIntervalB}");
            Console.WriteLine($"90% ДИ для standartDeviation: {estimation.confidenceIntervalStandardDeviation}");
        }
    }
}
