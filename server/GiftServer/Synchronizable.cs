using System;

namespace GiftServer
{
    namespace Data
    {
        interface ISynchronizable
        {
            bool Create();
            bool Update();
            bool Delete();
        }
    }
}