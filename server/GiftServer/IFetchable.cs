using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// This indicates that the specified object can be fetched
        /// </summary>
        /// <remarks>
        /// Fetching an object is much like serializing it; the only difference is that Fetching it returns an XML Document, not just bytecode
        /// </remarks>
        public interface IFetchable
        {
            XmlDocument Fetch();
        }
    }
}