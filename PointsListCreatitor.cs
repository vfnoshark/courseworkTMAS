using courseworkTMAS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace CourseworkTMAS
{
    class PointsListCreator
    {
        public IReadOnlyList<PointF> Points { get; }

        public PointsListCreator(DataSetConfiguration inputData, double[] yFunctionValues)
        {
            if (inputData == null) throw new ArgumentNullException(nameof(inputData));
            if (yFunctionValues == null) throw new ArgumentNullException(nameof(yFunctionValues));
            if (yFunctionValues.Length < inputData.SampleCount)
                throw new ArgumentException("Y values array length must be at least equal to number of elements");

            Points = CreatePoints(inputData, yFunctionValues);
        }

        private IReadOnlyList<PointF> CreatePoints(DataSetConfiguration inputData, double[] yFunctionValues)
        {
            var points = new List<PointF>(inputData.SampleCount);

            for (int i = 0; i < inputData.SampleCount; i++)
            {
                float x = inputData.MinX + i * inputData.Increment;
                float y = (float)yFunctionValues[i];
                points.Add(new PointF(x, y));
            }

            return points.AsReadOnly();
        }
    }
}