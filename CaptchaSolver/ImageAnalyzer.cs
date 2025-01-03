using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CaptchaSolver
{
    public static class ImageAnalyzer
    {
        public static int BeamSize { get; set; } = 50;
        public static int GridSize { get; set; } = 2;
        public static Rgba32 EmptyPlaceColor = new Rgba32(0, 0, 0, 255); // Black color
        public static int Height { get; set; }
        public static int Width { get; set; }
        public static Image<Rgba32> Image { get; set; }

        public static int GetBlackPercentage(Image<Rgba32> image)
        {
            Image = image;
            Height = image.Height;
            Width = image.Width;

            var colors = new List<Rgba32>();

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

            var avgColor = new Rgba32(
                (byte)colors.Average(c => c.R),
                (byte)colors.Average(c => c.G),
                (byte)colors.Average(c => c.B),
                255
            );

            var distance = GetColorDistance(avgColor, EmptyPlaceColor);
            return distance;
        }

        public static Point RealPoint(Point point)
        {
            return new Point(point.X, Height - point.Y);
        }

        public static int GetColorDistance(Rgba32 left, Rgba32 right)
        {
            return (Math.Abs(left.R - right.R) + Math.Abs(left.G - right.G) + Math.Abs(left.B - right.B)) / 3;
        }

        public static Rgba32 PickBeamColor(Point location, int radius)
        {
            var colors = new List<Rgba32>
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

            return new Rgba32(
                (byte)colors.Average(c => c.R),
                (byte)colors.Average(c => c.G),
                (byte)colors.Average(c => c.B),
                255
            );
        }

        public static Rgba32 PickPixelColor(Point location)
        {
            return Image[location.X, location.Y];
        }
    }
}
