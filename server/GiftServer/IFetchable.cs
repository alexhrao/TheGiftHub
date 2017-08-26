using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        public interface IFetchable
        {
            XmlDocument Fetch();
        }
    }
}