﻿using System;
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
                var image = Image.FromFile("01.jpg");
                Console.WriteLine($"Original With: {image.Width}; Original Height: {image.Height}");

                var thumbnail = ScaleImage(image, 80);
                Console.WriteLine($"Thumb With: {thumbnail.Width}; Thumb Height: {thumbnail.Height}");

                SaveImage(thumbnail, "01.thumb.jpg");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }

        private static Image ScaleImage(Image image, int targetImageWidth = 100)
        {
            if (image == null)
                throw new ArgumentException($"Invalid {nameof(image)}");

            const int max64K = 1920 * 8;
            if (targetImageWidth < 1 || targetImageWidth > max64K)
                throw new ArgumentOutOfRangeException(nameof(targetImageWidth),
                    $"The range of useful values for the targetImageWidth is from 1 to {max64K}");

            var targetHeight = (int)Math.Round((double)image.Height / image.Width * targetImageWidth, 0);
            var target = new Bitmap(targetImageWidth, targetHeight);

            var graph = Graphics.FromImage(target);
            graph.DrawImage(image,
                new Rectangle(0, 0, targetImageWidth, targetHeight),
                new Rectangle(0, 0, image.Width, image.Height),
                GraphicsUnit.Pixel);

            return target;
        }

        private static void SaveImage(Image image, string fileName, long quality = 80)
        {
            if (image == null)
                throw new ArgumentException($"Invalid {nameof(image)}");

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException($"Invalid {nameof(fileName)}");

            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException(nameof(quality),
                    "The range of useful values for the quality category is from 0 to 100");

            var mimeType = GetImageMimeTypeFromFileName(fileName);
            var encoderInfo = ImageCodecInfo.GetImageEncoders().First(a => a.MimeType == mimeType);
            var encoderParam = new EncoderParameters { Param = new[] { new EncoderParameter(Encoder.Quality, quality) } };

            image.Save(fileName, encoderInfo, encoderParam);
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
                case ".jpg": return "image/jpeg";
                case ".bmp": return "image/jpeg";
                case ".png": return "image/jpeg";
                case ".gif": return "image/jpeg";
                default: throw new NotSupportedException($"Not support \"{extension}\"");
            }
        }
    }
}