using GiftServer.Server;
namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// An Interface indicating the class has an associated image and allows manipulations of said image
        /// </summary>
        /// <remarks>
        /// IShowable indicates that the class is "showable" to the user - For example, both the User and the Gift classes have associated images, 
        /// so they must implement this interface
        /// </remarks>
        interface IShowable
        {
            void SaveImage(MultipartParser parser);
            void RemoveImage();
            string GetImage();
        }
    }
}