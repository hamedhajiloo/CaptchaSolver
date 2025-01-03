using System.Drawing;
using System.Net;

public class ShapeDetector
{
    public async Task<List<Point>> FindDarkShapesFromUrl(string imageUrl)
    {
        using var client = new HttpClient();
        await using Stream stream = await client.GetStreamAsync(imageUrl);
        using Bitmap image = new Bitmap(stream);
        return FindDarkShapes(image);
    }

    public List<Point> FindDarkShapes(Bitmap image)
    {
        var darkPoints = new List<Point>();
        int threshold = 127;

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                Color pixel = image.GetPixel(x, y);
                int gray = (pixel.R + pixel.G + pixel.B) / 3;

                if (gray < threshold)
                {
                    darkPoints.Add(new Point(x, y));
                }
            }
        }

        var shapes = GroupIntoShapes(darkPoints);

        return shapes.OrderByDescending(s => s.Count)
                    .Take(2)
                    .Select(shape => new Point(
                        shape.Min(p => p.X),
                        shape.Max(p => p.Y)))
                    .ToList();
    }

    private List<List<Point>> GroupIntoShapes(List<Point> points)
    {
        var shapes = new List<List<Point>>();
        var visited = new HashSet<Point>();

        foreach (var point in points)
        {
            if (!visited.Contains(point))
            {
                var shape = new List<Point>();
                FloodFill(point, points, visited, shape);
                shapes.Add(shape);
            }
        }

        return shapes;
    }

    private void FloodFill(Point start, List<Point> points, HashSet<Point> visited, List<Point> shape)
    {
        if (visited.Contains(start)) return;

        visited.Add(start);
        shape.Add(start);

        var neighbors = points.Where(p =>
            Math.Abs(p.X - start.X) <= 1 &&
            Math.Abs(p.Y - start.Y) <= 1 &&
            !visited.Contains(p));

        foreach (var neighbor in neighbors)
        {
            FloodFill(neighbor, points, visited, shape);
        }
    }
}