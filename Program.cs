using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CourseworkTMAS
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var datasets = CreateTestDatasets();
            DisplayGraphsForDatasets(datasets);
        }

        private static List<DataSetConfiguration> CreateTestDatasets()
        {
            return new List<DataSetConfiguration>
            {
                new DataSetConfiguration(parameterN: 5.00, parameterM: 4.00, increment: 5, sampleCount: 11, noiseStandardDeviation: 1),
                new DataSetConfiguration(parameterN: 5.00, parameterM: 4.00, increment: 1, sampleCount: 51, noiseStandardDeviation: 1),
                new DataSetConfiguration(parameterN: 5.00, parameterM: 4.00, increment: 5, sampleCount: 11, noiseStandardDeviation: 3),
                new DataSetConfiguration(parameterN: 5.00, parameterM: 4.00, increment: 1, sampleCount: 51, noiseStandardDeviation: 3)
            };
        }

        private static void DisplayGraphsForDatasets(IEnumerable<DataSetConfiguration> datasets)
        {
            foreach (var dataset in datasets)
            {
                using (var form = CreateGraphForm(dataset))
                {
                    Application.Run(form);
                }
                Console.ReadKey();
            }
        }

        private static Form CreateGraphForm(DataSetConfiguration dataset)
        {
            var form = new Form
            {
                Text = $"Graph of function: {dataset.Slope}x + {dataset.Intercept}",
                Size = new Size(800, 600),
                StartPosition = FormStartPosition.CenterScreen
            };

            var graph = CreateGraphControl(dataset);
            form.Controls.Add(graph);

            return form;
        }

        private static Control CreateGraphControl(DataSetConfiguration dataset)
        {
            var dataTable = new DataTableBuilder(dataset);
            dataTable.BuildAndDisplayTable();

            var pointsCreator = new PointsListCreator(dataset, dataTable.FunctionValues);
            var graph = new LinearFunctionGraph(
                slopeCoefficient: (float)dataset.Slope,
                intercept: (float)dataset.Intercept,
                dataPoints: pointsCreator.Points)
            {
                Dock = DockStyle.Fill
            };

            return graph;
        }
    }
}
