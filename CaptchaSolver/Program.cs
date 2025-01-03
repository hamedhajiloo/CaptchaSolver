using CaptchaSolver;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

int maxX = 100;
string outputImagePath = "output.jpg"; // Path to save the output image

var backImageUrl = "https://mlk-public.s3.ir-thr-at1.arvanstorage.ir/spyder-captcha%2Fback.png?versionId=";
var overlayImageUrl = "https://mlk-public.s3.ir-thr-at1.arvanstorage.ir/spyder-captcha%2Foverlay.png?versionId=";

using var client = new HttpClient();

await using Stream backgroundStream = await client.GetStreamAsync(backImageUrl);
using Image<Rgba32> backgroundImage = await Image.LoadAsync<Rgba32>(backgroundStream);

await using Stream overlayStream = await client.GetStreamAsync(overlayImageUrl);
using Image<Rgba32> overlayImage = await Image.LoadAsync<Rgba32>(overlayStream);

var x = 0;


while (x < backgroundImage.Width)
{
    using var bitmap = new Image<Rgba32>(backgroundImage.Width, backgroundImage.Height);
    bitmap.Mutate(ctx =>
    {
        ctx.DrawImage(backgroundImage, new Point(0, 0), 1f);
        ctx.DrawImage(overlayImage, new Point(x, 60), 1f);
    });

    await bitmap.SaveAsync(outputImagePath);
    x += 10;
    var a = ImageAnalyzer.GetBlackPercentage(bitmap);

    Console.WriteLine(a);
    await Task.Delay(TimeSpan.FromSeconds(1));
}


