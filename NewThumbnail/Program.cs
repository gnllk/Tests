using System;
using System.Drawing;
using System.IO;

namespace NewThumbnail
{
    class Program
    {
        private const int ThumbnailImageHeight = 240;//px

        static void Main(string[] args)
        {
            try
            {
                var totalFileCount = 0;
                EachFile(AppDomain.CurrentDomain.BaseDirectory, fileName =>
                {
                    var extension = Path.GetExtension(fileName).ToLowerInvariant();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" &&
                        extension != ".bmp") return;

                    Console.WriteLine(fileName);
                    totalFileCount++;
                    var tempName = Path.GetFileNameWithoutExtension(fileName);
                    var tempPath = Path.GetDirectoryName(fileName);
                    using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                    {
                        using (var image = new Bitmap(stream))
                        {
                            var fullName = Path.Combine(tempPath, $"{tempName}.thumb{extension}");
                            using (var thumb = ImageHelper.ScaleImageWithHeight(image, ThumbnailImageHeight))
                            {
                                ImageHelper.SaveImage(thumb, fullName);
                            }
                        }
                    }
                });
                Console.WriteLine($"Total: {totalFileCount}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Press any key to end.");
            Console.ReadKey();
        }

        private static void EachFile(string path, Action<string> dosomething)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                dosomething?.Invoke(file);
            }
            foreach (var dir in Directory.GetDirectories(path))
            {
                EachFile(dir, dosomething);
            }
        }
    }
}
