using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace E621Shared
{
    public class Subscription
    {
        public string? TelegramId { get; set; }
        public string? Tag { get; set; }
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
            string query =
                "create table if not exists Subscriptions(Id INTEGER PRIMARY KEY, UserId TEXT, Tag TEXT)";

            using var con = _con.Get();
            con.Execute(query);
        }

        //lets do command query seperation, stuff either modifies the dbase, or returns data, not both.

        //Allows the bot to list all your subscriptions
        public Task<IEnumerable<Subscription>> ListSubscriptionsForTelegramUser(string userId)
        {
            string query = "select * from subscriptions where UserId = @userId";

            using var con = _con.Get();
            return con.QueryAsync<Subscription>(query, new {userId});
        }

        public Task<int> CreateSubscription(Subscription subscription)
        {
            string query =
                "insert into subscription (UserId, Tag) values (@UserId, @Tag)";

            using var con = _con.Get();
            return con.ExecuteAsync(query, subscription);
        }

        public Task<IEnumerable<Subscription>> ListAllSubscriptions()
        {
            string query = "select * from subscriptions";

            using var con = _con.Get();
            return con.QueryAsync<Subscription>(query);
        }

        public Task DeleteSubscription(int id, string userId) //Must pass user doing the delete request
        {
            string query = "Delete from subscriptions where Id = @id and UserId = @userId";

            using var con = _con.Get();
            return con.ExecuteAsync(query, new {id, userId});
        }
    }
}