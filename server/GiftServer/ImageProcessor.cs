using System.IO;
using GiftServer.Server;
using System.Drawing;
using System.Drawing.Imaging;
using GiftServer.Properties;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// Process an image and get it's final form
        /// </summary>
        public class ImageProcessor
        {
            /// <summary>
            /// The data (the output)
            /// </summary>
            public readonly byte[] Data;
            /// <summary>
            /// Create a new image processor with the given contents
            /// </summary>
            /// <param name="contents">A byte array representing an image - any format is fine</param>
            public ImageProcessor(byte[] contents)
            {
                // Switch data format:
                switch (Constants.ImageFormat.ToLower())
                {
                    case ".jpeg":
                    case ".jpg":
                        using (MemoryStream original = new MemoryStream(contents))
                        using (MemoryStream output = new MemoryStream())
                        using (Image jpg = Image.FromStream(original))
                        {
                            jpg.Save(output, ImageFormat.Jpeg);
                            Data = output.ToArray();
                        }
                        return;
                    case ".png":
                        using (MemoryStream original = new MemoryStream(contents))
                        using (MemoryStream output = new MemoryStream())
                        using (Image png = Image.FromStream(original))
                        {
                            png.Save(output, ImageFormat.Png);
                            Data = output.ToArray();
                        }
                        return;
                    case ".bmp":
                        using (MemoryStream original = new MemoryStream(contents))
                        using (MemoryStream output = new MemoryStream())
                        using (Image bmp = Image.FromStream(original))
                        {
                            bmp.Save(output, ImageFormat.Bmp);
                            Data = output.ToArray();
                        }
                        return;
                    case ".gif":
                        using (MemoryStream original = new MemoryStream(contents))
                        using (MemoryStream output = new MemoryStream())
                        using (Image gif = Image.FromStream(original))
                        {
                            gif.Save(output, ImageFormat.Gif);
                            Data = output.ToArray();
                        }
                        return;
                    default:
                        throw new InvalidDataException("Unknown type " + Constants.ImageFormat);
                }
            }
        }
    }
}
