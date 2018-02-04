namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// Indicates that this class can "synchronize" with the database - it can be created, updated, and deleted from the Database
        /// </summary>
        public interface ISynchronizable
        {
            /// <summary>
            /// The ID for this Synchronizable object
            /// </summary>
            ulong ID
            {
                get;
            }
            /// <summary>
            /// Create a record of this in the database
            /// </summary>
            void Create();
            /// <summary>
            /// Update a record of this in the database
            /// </summary>
            void Update();
            /// <summary>
            /// Delete the record of this in the database
            /// </summary>
            void Delete();
        }
    }
}