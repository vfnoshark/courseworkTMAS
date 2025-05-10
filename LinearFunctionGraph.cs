using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class LinearFunctionGraph : Control
{
    private const int XMin = -25;
    private const int XMax = 25;

    private readonly double _slopeCoefficient;
    private readonly double _intercept;
    private readonly List<PointF> _dataPoints;
   

    // Default visual settings
    private Color _functionColor = Color.Blue;
    private Color _pointsColor = Color.Red;
    private Color _gridColor = Color.LightGray;
    private Color _deviationColor = Color.Gray;
    private int _pointSize = 8;
    private Font _axisFont = new Font("Arial", 8);
    private int _axisStepX = 5;
    private int _axisStepY = 10;
    private bool _showPointInfo = true;
    private float _yPadding = 20;

    // Scaling and positioning fields
    private float _scaleX;
    private float _scaleY;
    private PointF _origin;

    public LinearFunctionGraph(double slopeCoefficient, double intercept, IEnumerable<PointF> dataPoints)
    {
        _slopeCoefficient = slopeCoefficient;
        _intercept = intercept;
        _dataPoints = new List<PointF>(dataPoints.Where(p => p.X >= XMin && p.X <= XMax));

        this.DoubleBuffered = true;
        CalculateScales();

        this.Resize += (sender, e) =>
        {
            CalculateScales();
            this.Invalidate();
        };
    }

    private void CalculateScales()
    {
        // Calculate X scale with fixed range
        float xRange = XMax - XMin;
        _scaleX = (this.Width - 40) / xRange; // 40 pixels for margins

        // Calculate Y scale with dynamic range
        float minY = _dataPoints.Count > 0 ? _dataPoints.Min(p => p.Y) : 0;
        float maxY = _dataPoints.Count > 0 ? _dataPoints.Max(p => p.Y) : 0;

        // Include function values at boundaries
        float functionAtXMin = (float)(_slopeCoefficient * XMin + _intercept);
        float functionAtXMax = (float)(_slopeCoefficient * XMax + _intercept);
        minY = Math.Min(minY, Math.Min(functionAtXMin, functionAtXMax));
        maxY = Math.Max(maxY, Math.Max(functionAtXMin, functionAtXMax));

        // Apply padding
        minY -= _yPadding;
        maxY += _yPadding;

        // Handle case where all values are equal
        if (Math.Abs(maxY - minY) < float.Epsilon)
        {
            minY -= 10;
            maxY += 10;
        }

        float yRange = maxY - minY;
        _scaleY = (this.Height - 40) / yRange;

        // Calculate origin point (where X=0 and Y=0)
        _origin = new PointF(
            -XMin * _scaleX + 20,
            this.Height - 20 - (0 - minY) * _scaleY);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var graphics = e.Graphics;
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        DrawGrid(graphics);
        DrawAxes(graphics);
        DrawFunction(graphics);
        DrawDataPoints(graphics);
    }

    private void DrawGrid(Graphics graphics)
    {
        using (var gridPen = new Pen(_gridColor, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
        {
            // Vertical grid lines (X axis)
            for (int x = XMin; x <= XMax; x += _axisStepX)
            {
                float xPos = _origin.X + x * _scaleX;
                graphics.DrawLine(gridPen, xPos, 20, xPos, this.Height - 20);
            }

            // Horizontal grid lines (Y axis)
            float minY = (20 - _origin.Y) / _scaleY;
            float maxY = (this.Height - 20 - _origin.Y) / _scaleY;

            int startY = (int)minY - _axisStepY;
            int endY = (int)maxY + _axisStepY;

            for (int y = startY; y <= endY; y += _axisStepY)
            {
                if (y == 0) continue; // Skip X axis line
                float yPos = _origin.Y + y * _scaleY;
                graphics.DrawLine(gridPen, 20, yPos, this.Width - 20, yPos);
            }
        }
    }

    private void DrawAxes(Graphics graphics)
    {
        using (var axisPen = new Pen(Color.Black, 2))
        {
            // X axis
            graphics.DrawLine(axisPen, 20, _origin.Y, this.Width - 20, _origin.Y);
            // Y axis
            graphics.DrawLine(axisPen, _origin.X, 20, _origin.X, this.Height - 20);

            // Draw arrowheads
            float arrowSize = 8;
            graphics.DrawLine(axisPen, this.Width - 20, _origin.Y, this.Width - 20 - arrowSize, _origin.Y - arrowSize);
            graphics.DrawLine(axisPen, this.Width - 20, _origin.Y, this.Width - 20 - arrowSize, _origin.Y + arrowSize);
            graphics.DrawLine(axisPen, _origin.X, 20, _origin.X - arrowSize, 20 + arrowSize);
            graphics.DrawLine(axisPen, _origin.X, 20, _origin.X + arrowSize, 20 + arrowSize);

            // Axis labels
            graphics.DrawString("X", _axisFont, Brushes.Black, this.Width - 30, _origin.Y + 10);
            graphics.DrawString("Y", _axisFont, Brushes.Black, _origin.X + 10, 10);

            // X axis ticks and labels
            for (int x = XMin; x <= XMax; x += _axisStepX)
            {
                float xPos = _origin.X + x * _scaleX;
                graphics.DrawLine(axisPen, xPos, _origin.Y - 5, xPos, _origin.Y + 5);

                string label = x.ToString();
                SizeF textSize = graphics.MeasureString(label, _axisFont);
                graphics.DrawString(label, _axisFont, Brushes.Black,
                    xPos - textSize.Width / 2, _origin.Y + 10);
            }

            // Y axis ticks and labels
            float currentY = (20 - _origin.Y) / _scaleY;
            float endY = (this.Height - 20 - _origin.Y) / _scaleY;

            int startY = (int)currentY - _axisStepY;
            endY = (int)endY + _axisStepY;

            for (int y = startY; y <= endY; y += _axisStepY)
            {
                if (y == 0) continue;

                float yPos = _origin.Y + y * _scaleY;
                graphics.DrawLine(axisPen, _origin.X - 5, yPos, _origin.X + 5, yPos);

                string label = y.ToString();
                SizeF textSize = graphics.MeasureString(label, _axisFont);
                graphics.DrawString(label, _axisFont, Brushes.Black,
                    _origin.X - textSize.Width - 10, yPos - textSize.Height / 2);
            }
        }
    }

    private void DrawFunction(Graphics graphics)
    {
        PointF startPoint = new PointF(
            _origin.X + XMin * _scaleX,
            _origin.Y - (float)(_slopeCoefficient * XMin + _intercept) * _scaleY);

        PointF endPoint = new PointF(
            _origin.X + XMax * _scaleX,
            _origin.Y - (float)(_slopeCoefficient * XMax + _intercept) * _scaleY);

        using (var functionPen = new Pen(_functionColor, 3))
        {
            graphics.DrawLine(functionPen, startPoint, endPoint);
        }
    }

    private void DrawDataPoints(Graphics graphics)
    {
        using (var pointPen = new Pen(Color.DarkRed, 2))
        {
            for (int i = 0; i < _dataPoints.Count; i++)
            {
                var point = _dataPoints[i];
                float x = _origin.X + point.X * _scaleX;
                float y = _origin.Y - point.Y * _scaleY;
                float functionY = _origin.Y - (float)(_slopeCoefficient * point.X + _intercept) * _scaleY;

                // Draw data point
                graphics.DrawEllipse(pointPen, x - _pointSize / 2, y - _pointSize / 2, _pointSize, _pointSize);
                graphics.FillEllipse(new SolidBrush(_pointsColor), x - _pointSize / 2 + 1, y - _pointSize / 2 + 1, _pointSize - 2, _pointSize - 2);

                // Draw deviation line
                using (var deviationPen = new Pen(_deviationColor, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                {
                    graphics.DrawLine(deviationPen, x, y, x, functionY);
                }

                if (_showPointInfo)
                {
                    bool shouldShowText = _dataPoints.Count <= 15 || i % 5 == 0;

                    if (!shouldShowText)
                        continue;

                    float functionValue = (float)(_slopeCoefficient * point.X + _intercept);
                    string infoText = $"({point.X:F1}, {point.Y})\nf({point.X:F1}) = {functionValue}";

                    SizeF textSize = graphics.MeasureString(infoText, _axisFont);

                    float textX = x + _pointSize + 5;
                    float textY = y - textSize.Height / 2;

                    graphics.FillRectangle(Brushes.White, textX - 2, textY - 2, textSize.Width + 4, textSize.Height + 4);
                    graphics.DrawRectangle(Pens.LightGray, textX - 2, textY - 2, textSize.Width + 4, textSize.Height + 4);

                    graphics.DrawString(infoText, _axisFont, Brushes.Black, textX, textY);
                }
            }
        }
    }

    // Public properties for customization
    public Color FunctionColor
    {
        get => _functionColor;
        set { _functionColor = value; Invalidate(); }
    }

    public Color PointsColor
    {
        get => _pointsColor;
        set { _pointsColor = value; Invalidate(); }
    }

    public Color GridColor
    {
        get => _gridColor;
        set { _gridColor = value; Invalidate(); }
    }

    public Color DeviationColor
    {
        get => _deviationColor;
        set { _deviationColor = value; Invalidate(); }
    }

    public int PointSize
    {
        get => _pointSize;
        set { _pointSize = value; Invalidate(); }
    }

    public int AxisStepX
    {
        get => _axisStepX;
        set { _axisStepX = value; Invalidate(); }
    }

    public int AxisStepY
    {
        get => _axisStepY;
        set { _axisStepY = value; Invalidate(); }
    }

    public bool ShowPointInfo
    {
        get => _showPointInfo;
        set { _showPointInfo = value; Invalidate(); }
    }

    public Font AxisFont
    {
        get => _axisFont;
        set { _axisFont = value; Invalidate(); }
    }

    public float YPadding
    {
        get => _yPadding;
        set { _yPadding = value; CalculateScales(); Invalidate(); }
    }
}