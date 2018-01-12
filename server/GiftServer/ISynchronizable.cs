using System;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// Indicates that this class can "synchronize" with the database - it can be created, updated, and deleted from the Database
        /// </summary>
        interface ISynchronizable
        {
            bool Create();
            bool Update();
            bool Delete();
        }
    }
}