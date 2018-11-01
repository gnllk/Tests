using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ImageCompress
{
    internal class Program
    {
        private static void Main()
        {
            try
            {
                using (var image = Image.FromFile("01.jpg"))
                {
                    Console.WriteLine($"Original Width: {image.Width}; Original Height: {image.Height}");

                    using (var thumbnail1 = ScaleImageWithWidth(image, 80))
                    {
                        Console.WriteLine($"Thumb Width: {thumbnail1.Width}; Thumb Height: {thumbnail1.Height}");
                        SaveImage(thumbnail1, "01.thumb.jpg");
                        SaveImage(thumbnail1, "01.thumb.bmp");
                        SaveImage(thumbnail1, "01.thumb.png");
                        SaveImage(thumbnail1, "01.thumb.gif");
                    }

                    using (var thumbnail2 = ScaleImageWithHeight(image, 80))
                    {
                        Console.WriteLine($"Thumb Width: {thumbnail2.Width}; Thumb Height: {thumbnail2.Height}");
                        SaveImage(thumbnail2, "01.thumb2.jpg");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }

        private static Image ScaleImageWithWidth(Image image, int targetImageWidth = 100)
        {
            if (image == null)
                throw new ArgumentException($"Invalid {nameof(image)}");

            var targetImageHeight = (int)Math.Round((double)image.Height / image.Width * targetImageWidth, 0);

            return ScaleImage(image, targetImageWidth, targetImageHeight);
        }

        private static Image ScaleImageWithHeight(Image image, int targetImageHeight = 100)
        {
            if (image == null)
                throw new ArgumentException($"Invalid {nameof(image)}");

            var targetImageWidth = (int)Math.Round((double)image.Width / image.Height * targetImageHeight, 0);

            return ScaleImage(image, targetImageWidth, targetImageHeight);
        }

        private static Image ScaleImage(Image image, int targetImageWidth, int targetImageHeight)
        {
            if (image == null)
                throw new ArgumentException($"Invalid {nameof(image)}");

            const int maxW64K = 1920 * 8;
            if (targetImageWidth < 1 || targetImageWidth > maxW64K)
                throw new ArgumentOutOfRangeException(nameof(targetImageWidth),
                    $"The range of useful values for the targetImageWidth is from 1 to {maxW64K}");

            const int maxH64K = 1080 * 8;
            if (targetImageHeight < 1 || targetImageHeight > maxH64K)
                throw new ArgumentOutOfRangeException(nameof(targetImageHeight),
                    $"The range of useful values for the targetImageWidth is from 1 to {maxH64K}");

            var target = new Bitmap(targetImageWidth, targetImageHeight);

            using (var graph = Graphics.FromImage(target))
            {
                graph.DrawImage(image,
                    new Rectangle(0, 0, targetImageWidth, targetImageHeight),
                    new Rectangle(0, 0, image.Width, image.Height),
                    GraphicsUnit.Pixel);
            }

            return target;
        }

        private static void SaveImage(Image image, string fileName, long quality = 80)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException($"Invalid {nameof(fileName)}");

            var mimeType = GetImageMimeTypeFromFileName(fileName);
            using (var file = File.Create(fileName))
            {
                SaveImage(image, file, mimeType, quality);
            }
        }

        private static void SaveImage(Image image, Stream stream, string mimeType, long quality = 80)
        {
            if (image == null)
                throw new ArgumentException($"Invalid {nameof(image)}");

            if (null == stream || !stream.CanWrite)
                throw new ArgumentException($"Invalid {nameof(stream)}");

            if (string.IsNullOrWhiteSpace(mimeType))
                throw new ArgumentException($"Invalid {nameof(mimeType)}");

            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException(nameof(quality),
                    "The range of useful values for the quality category is from 0 to 100");

            var encoderInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(a => a.MimeType == mimeType);
            if (encoderInfo == null)
                throw new NotSupportedException($"Not support MimeType of \"{mimeType}\"");

            var encoderParam = new EncoderParameters
            {
                Param = new[]
                {
                    new EncoderParameter(Encoder.Quality, quality),
                    //new EncoderParameter(Encoder.ColorDepth, 8L)//does not work for png
                }
            };

            image.Save(stream, encoderInfo, encoderParam);
        }

        private static string GetImageMimeTypeFromFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException($"Invalid {nameof(fileName)}");

            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException($"Cannot get extension from \"{fileName}\"");

            switch (extension.ToLowerInvariant())
            {
                case ".jpeg":
                case ".jpg": return "image/jpeg";
                case ".bmp": return "image/bmp";
                case ".png": return "image/png";
                case ".gif": return "image/gif";
                default: throw new NotSupportedException($"Not support image type of \"{extension}\"");
            }
        }
    }
}
