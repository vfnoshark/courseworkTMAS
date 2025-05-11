using courseworkTMAS;
using System;

namespace CourseworkTMAS
{
    /// <summary>
    /// Generates and displays a table of function values with normally distributed noise
    /// </summary>
    public class DataTableBuilder
    {
        private const string Separator = "____________________________";

        public double[] NoiseValues { get; }
        public double[] FunctionValues { get; }
        public DataSetConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the data table builder
        /// </summary>
        /// <param name="configuration">Dataset configuration parameters</param>
        public DataTableBuilder(DataSetConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var noiseGenerator = new NormalDistrubtion(
                mean: 0,
                standardDeviation: configuration.NoiseStandardDeviation,
                configuration: configuration);

            NoiseValues = noiseGenerator.CalculateProbabilityDensityFunction();
            FunctionValues = new FunctionY(configuration, NoiseValues).Values;
        }

        /// <summary>
        /// Builds and displays the data table with statistical estimations
        /// </summary>
        public void BuildAndDisplayTable()
        {
            PrintConfigurationHeader();
            PrintTableHeader();
            PrintDataRows();
            PrintStatisticalEstimations();
        }

        private void PrintConfigurationHeader()
        {
            Console.WriteLine(
                $"Parameters: x : [{Configuration.MinX}, {Configuration.MaxX}], " +
                $"h = {Configuration.Increment}, n = {Configuration.SampleCount}, " +
                $"sigma = {Configuration.NoiseStandardDeviation} " +
                $"True model parameters: a = {Configuration.Slope}, b = {Configuration.Intercept}");
        }

        private void PrintTableHeader()
        {
            Console.WriteLine("   n   |   x   |   E   |   Y");
            Console.WriteLine(Separator);
        }

        private void PrintDataRows()
        {
            for (int i = 0; i < Configuration.SampleCount; i++)
            {
                double x = Configuration.MinX + Configuration.Increment * i;
                Console.WriteLine(
                    $"   {i,-4} |   {x,-5} |   {NoiseValues[i],-6:F14} |   {FunctionValues[i],-6:F14}");
            }
        }

        private void PrintStatisticalEstimations()
        {
            var estimator = new LinearRegressionEstimator(Configuration, FunctionValues);

            Console.WriteLine(
                $"True values: a = {Configuration.Slope}, " +
                $"b = {Configuration.Intercept}, " +
                $"sigma = {Configuration.NoiseStandardDeviation}");

            Console.WriteLine(
                $"Point estimates: a = {estimator.EstimatedSlope:F8}, " +
                $"b = {estimator.EstimatedIntercept:F8}, " +
                $"sigma = {estimator.EstimatedSigma:F8}");

            Console.WriteLine($"90% CI for a: {estimator.SlopeConfidenceInterval}");
            Console.WriteLine($"90% CI for b: {estimator.InterceptConfidenceInterval}");
            Console.WriteLine($"90% CI for sigma: {estimator.SigmaConfidenceInterval}");
        }
    }
}