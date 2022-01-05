using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace E621Shared.Repositories
{
    public class ScraperRepo
    {
        private readonly ConnectionProvider _con;

        public ScraperRepo(ConnectionProvider provider)
        {
            _con = provider;
            Init();
        }

        private void Init()
        {
            string query = "create table if not exists Scraper(LastPolledId INTEGER)";
            //insert a null if row doesn't exist already.
            string query2 = "INSERT INTO Scraper (LastPolledId) SELECT NULL WHERE NOT EXISTS (SELECT * FROM Scraper)";
            var con = _con.Get();
            con.Execute(query);
            con.Execute(query2);
        }

        //lets do command query seperation, stuff either modifies the dbase, or returns data, not both.

        public async Task<int?> GetLastPolledId()
        {
            string query = "select LastPolledId from Scraper";

            var con = _con.Get();
            return (await con.QueryAsync<int?>(query)).First();
        }

        public Task<int> UpdateLastPolledId(int id)
        {
            string query = "update Scraper set LastPolledId = @id";
            var con = _con.Get();
            return con.ExecuteAsync(query, new { id });
        }
    }
}