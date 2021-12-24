using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace E621Shared
{
    public class ConnectionProvider
    {
        public IDbConnection Get()
        {
            //Dapper opens the connection for you if there is need for that.
            return
                SQLiteConnection(
                    "Data Source=mydb.db;Version=3;"); //todo, make this configurable, if there is ever need
        }
    }

    public class Subscription
    {
        public string TelegramId { get; set; }
        public string Tag { get; set; }
    }

    public class SubscriberRepo
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
            string query =
                "create table if not exists Subscriptions(Id INTEGER PRIMARY KEY, TelegramId TEXT, Tag TEXT)";

            using var con = _con.Get();
            con.Execute(query);
        }

        //lets do command query seperation, stuff either modifies the dbase, or returns data, not both.

        //Allows the bot to list all your subscriptions
        public Task<List<Subscription>> ListSubscriptionsForTelegramUser(string telegramId)
        {
            string query = "select * from subscriptions where TelegramId = @telegramId";

            using var con = _con.Get();
            return con.QueryAsync<Subscription>(query, new {telegramId}).ToList();
        }

        public Task<int> CreateSubscription(Subscription subscription)
        {
            string query =
                "insert into subscription (TelegramId, Tag) values (@TelegramId, @Tag)"; //case sensitive match? not sure.

            using var con = _con.Get();
            return con.ExecuteAsync(query, subscription);
        }

        public Task<IEnumerable<Subscription>> ListAllSubscriptions()
        {
            string query = "select * from subscriptions";

            using var con = _con.Get();
            return con.QueryAsync<Subscription>(query);
        }

        public Task DeleteSubscription(int id, string telegramId) //Must pass user doing the delete request
        {
            string query = "Delete from subscriptions where Id = @id and TelegramId = @telegramId";

            using var con = _con.Get();
            return con.ExecuteAsync(query, new {id, telegramId});
        }
    }
}