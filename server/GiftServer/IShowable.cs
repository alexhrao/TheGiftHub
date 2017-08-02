using System;
using GiftServer.Data;
using GiftServer.Server;
namespace GiftServer
{
    namespace Data
    {
        interface IShowable
        {
            void SaveImage(MultipartParser parser);
            void RemoveImage();
        }
    }
}