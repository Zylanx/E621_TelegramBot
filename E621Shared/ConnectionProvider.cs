using System.Data;
using System.Data.SQLite;

namespace E621Shared
{
    public class ConnectionProvider
    {
        public IDbConnection Get()
        {
            //Dapper opens the connection for you if there is need for that.
            return
                new SQLiteConnection(
                    "Data Source=mydb.db;Version=3;"); //todo, make this configurable, if there is ever need
        }
    }
}