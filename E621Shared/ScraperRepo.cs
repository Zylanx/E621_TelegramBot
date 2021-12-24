using System.Threading.Tasks;
using Dapper;

namespace E621Shared
{
    public class ScraperRepo
    {
        private readonly ConnectionProvider _con;

        public SubscriberRepo(ConnectionProvider provider)
        {
            _con = provider;
            Init();
        }

        private void Init()
        {
            //real crap intro subscription table.
            string query = "create table if not exists Scraper(LastPolledId INTEGER)";
            string query2 = "insert into Scraper (LastPolledId) values (0)";

            using (var con = _con.Get())
            {
                con.Execute(query);
                con.Execute(query2);
            }
        }

        //lets do command query seperation, stuff either modifies the dbase, or returns data, not both.

        //Allows the bot to list all your subscriptions
        public Task<int> GetLastPolledId()
        {
            string query = "select LastPolledId from Scraper";
            using (var con = _con.Get())
            {
                return con.QueryAsync<int>(query).First();
            }
        }

        //Allows the bot to list all your subscriptions
        public Task<int> UpdateLastPolledId()
        {
            string query = "select LastPolledId from Scraper";
            using (var con = _con.Get())
            {
                return con.QueryAsync<int>(query).First();
            }
        }
    }
}