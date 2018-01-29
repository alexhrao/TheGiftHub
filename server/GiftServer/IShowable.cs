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
        public interface IShowable
        {
            /// <summary>
            /// Save the image contained in the byte array and associate it with this object
            /// </summary>
            /// <param name="contents">The image to save - any format is fine</param>
            void SaveImage(byte[] contents);
            /// <summary>
            /// Delete any images associated with this object
            /// </summary>
            void RemoveImage();
            /// <summary>
            /// Get the image associated with this object
            /// </summary>
            /// <returns>A web-ready URI for this image, to be embedded in HTML</returns>
            string GetImage();
        }
    }
}