using System.IO;
using GiftServer.Server;
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
                string type = parser.ContentType.Substring(parser.ContentType.IndexOf("/") + 1);
                switch (type)
                {
                    case "jpeg":
                    case "jpg":
                        Data = parser.FileContents;
                        return;
                    case "png":
                        Data = parser.FileContents;
                        return;
                    case "bmp":
                        Data = parser.FileContents;
                        return;
                    default:
                        throw new InvalidDataException("Unknown type " + type);
                }
            }
        }
    }
}
