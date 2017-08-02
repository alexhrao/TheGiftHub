using System.IO;
using GiftServer.Server;
using System.Drawing;
using System.Drawing.Imaging;
using GiftServer.Properties;

namespace GiftServer
{
    namespace Data
    {
        public class ImageProcessor
        {
            private readonly MultipartParser _parser;
            public readonly byte[] Data;

            public ImageProcessor(MultipartParser parser)
            {
                _parser = parser;
                // Switch data format:
                switch (Resources.ImageFormat.ToLower())
                {
                    case ".jpeg":
                    case ".jpg":
                        using (MemoryStream original = new MemoryStream(parser.FileContents))
                        using (MemoryStream output = new MemoryStream())
                        using (Image jpg = Image.FromStream(original))
                        {
                            jpg.Save(output, ImageFormat.Jpeg);
                            Data = output.ToArray();
                        }
                        return;
                    case ".png":
                        using (MemoryStream original = new MemoryStream(parser.FileContents))
                        using (MemoryStream output = new MemoryStream())
                        using (Image png = Image.FromStream(original))
                        {
                            png.Save(output, ImageFormat.Png);
                            Data = output.ToArray();
                        }
                        return;
                    case ".bmp":
                        using (MemoryStream original = new MemoryStream(parser.FileContents))
                        using (MemoryStream output = new MemoryStream())
                        using (Image bmp = Image.FromStream(original))
                        {
                            bmp.Save(output, ImageFormat.Bmp);
                            Data = output.ToArray();
                        }
                        return;
                    case ".gif":
                        using (MemoryStream original = new MemoryStream(parser.FileContents))
                        using (MemoryStream output = new MemoryStream())
                        using (Image gif = Image.FromStream(original))
                        {
                            gif.Save(output, ImageFormat.Gif);
                            Data = output.ToArray();
                        }
                        return;
                    default:
                        throw new InvalidDataException("Unknown type " + Resources.ImageFormat);
                }
            }
        }
    }
}
