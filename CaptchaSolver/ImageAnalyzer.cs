using System.Drawing;
using System.Drawing.Drawing2D;

namespace CaptchaSolver
{
    public static class ImageAnalyzer
    {
        public static int BeamSize { get; set; } = 50;

        public static int GridSize { get; set; } = 2;

        public static Color EmptyPlaceColor = Color.Black;

        public static int Height { get; set; }
        public static int Width { get; set; }
        public static Bitmap Image { get; set; }

        public static int GetBlackPercentage(Bitmap image)
        {
            Image = image;
            Height = image.Height;
            Width = image.Width;

            var colors = new List<Color>();

            for (var x = BeamSize + 1; x < Width - BeamSize - 1; x += GridSize)
            {
                for (var y = BeamSize + 1; y < Height - BeamSize - 1; y += GridSize)
                {
                    var location = new Point(x, y);
                    var real = RealPoint(location);
                    var color = PickBeamColor(real, BeamSize);
                    colors.Add(color);
                }
            }

            var avgColor = Color.FromArgb((int)colors.Average(c => c.R), (int)colors.Average(c => c.G), (int)colors.Average(c => c.B)); ;

            var distance = GetColorDistance(avgColor, EmptyPlaceColor);

            return distance;
        }

        public static Point RealPoint(Point point)
        {
            return new Point(point.X, Height - point.Y);
        }

        public static int GetColorDistance(Color left, Color right)
        {
            return (Math.Abs(left.R - right.R) + Math.Abs(left.G - right.G) + Math.Abs(left.B - right.B)) / 3;
        }
        public static Color PickBeamColor(Point location, int radius)
        {
            var colors = new List<Color>
            {
                PickPixelColor(location)
            };

            for (var i = GridSize; i < radius; i += GridSize)
            {
                for (int j = GridSize; j < radius; j += GridSize)
                {
                    colors.Add(PickPixelColor(new Point(location.X + i, location.Y + j)));
                    colors.Add(PickPixelColor(new Point(location.X + i, location.Y - j)));
                    colors.Add(PickPixelColor(new Point(location.X - i, location.Y + j)));
                    colors.Add(PickPixelColor(new Point(location.X - i, location.Y - j)));
                }
            }
            return Color.FromArgb((int)colors.Average(c => c.R), (int)colors.Average(c => c.G), (int)colors.Average(c => c.B));
        }

        public static Color PickPixelColor(Point location)
        {
            //var realPoint = RealPoint(location);
            var pixel = Image.GetPixel(location.X, location.Y);
            return pixel;
        }
    }
}
