using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;

namespace E621Shared
{
    public class Subscription
    {
        public long Id { get; set; }
        public long TelegramId { get; set; }
        public string Tag { get; set; } = null!;
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
                "create table if not exists Subscriptions(Id INTEGER PRIMARY KEY, UserId INTEGER, Tag TEXT)";

            using var con = _con.Get();
            con.Execute(query);
        }

        //lets do command query seperation, stuff either modifies the dbase, or returns data, not both.

        //Allows the bot to list all your subscriptions
        public Task<IEnumerable<Subscription>> ListSubscriptionsForTelegramUser(long userId)
        {
            string query = "select * from Subscriptions where UserId = @userId";

            using var con = _con.Get();
            return con.QueryAsync<Subscription>(query, new {userId});
        }

        public Task<int> CreateSubscription(Subscription subscription)
        {
            using var con = _con.Get();
            return con.InsertAsync(subscription);
        }

        public Task<IEnumerable<Subscription>> ListAllSubscriptions()
        {
            using var con = _con.Get();
            return con.GetAllAsync<Subscription>();
        }

        public Task DeleteSubscription(long id, long userId) //Must pass user doing the delete request
        {
            string query = "Delete from Subscriptions where Id = @id and UserId = @userId";

            using var con = _con.Get();
            return con.ExecuteAsync(query, new {id, userId});
        }
    }
}