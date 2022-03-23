namespace PASMBTCP.SQLite
{
    public static class DatabaseController
    {
        private static readonly ClientTable _clientTable = new();
        private static readonly TagTable _tagTable = new();

        /// <summary>
        /// Gets All Table Names In The Database
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<string>> GetAllTables()
        {
            return await _clientTable.GetAllTables();
        }

        /// <summary>
        /// Removes the Client Table From The Database
        /// </summary>
        /// <returns></returns>
        public static async Task DropTable(string tableName)
        {
            await _clientTable.DropTable(tableName);
        }

    }
}
