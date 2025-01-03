using CaptchaSolver;
using System.Drawing;

int maxX = 100;
string outputImagePath = "output.jpg"; // Path to save the output image

var backImageUrl = "https://mlk-public.s3.ir-thr-at1.arvanstorage.ir/spyder-captcha%2Fback.png?versionId=";
var overlayImageUrl = "https://mlk-public.s3.ir-thr-at1.arvanstorage.ir/spyder-captcha%2Foverlay.png?versionId=";

using var client = new HttpClient();

await using Stream bacckgrounStream = await client.GetStreamAsync(backImageUrl);
using Bitmap backgroundImage = new Bitmap(bacckgrounStream);

await using Stream overlayStream = await client.GetStreamAsync(overlayImageUrl);
using Bitmap overlayImage = new Bitmap(overlayStream);


var x = 0;

while (true)
{
    using var bitmap = new Bitmap(backgroundImage.Width, backgroundImage.Height);
    using var graphics = Graphics.FromImage(bitmap);

    graphics.DrawImage(backgroundImage, 0, 0, backgroundImage.Width, backgroundImage.Height);
    graphics.DrawImage(overlayImage, new Point(x, 60));

    bitmap.Save(outputImagePath);
    x += 10;
    var a = ImageAnalyzer.GetBlackPercentage(bitmap);

    Console.WriteLine(a);
    await Task.Delay(TimeSpan.FromSeconds(1));
}

