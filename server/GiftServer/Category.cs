using System;

namespace GiftServer
{
    namespace Data
    {
        public class Category : ISynchronizable
        {
            public long Id = -1;
            public Category(long id)
            {

            }
            public bool Create()
            {
                return false;
            }
            public bool Update()
            {
                return false;
            }
            public bool Delete()
            {
                return false;
            }
        }
    }
}