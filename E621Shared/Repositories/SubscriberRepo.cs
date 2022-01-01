using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;

namespace E621Shared.Repositories
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
                "create table if not exists Subscriptions(Id INTEGER PRIMARY KEY, TelegramId INTEGER, Tag TEXT)";

            using var con = _con.Get();
            con.Execute(query);
        }

        //lets do command query seperation, stuff either modifies the dbase, or returns data, not both.

        //Allows the bot to list all your subscriptions
        public Task<IEnumerable<Subscription>> ListSubscriptionsForTelegramUser(long telegramId)
        {
            string query = "select * from Subscriptions where TelegramId = @telegramId";

            using var con = _con.Get();
            return con.QueryAsync<Subscription>(query, new {telegramId});
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

        public Task<IEnumerable<Subscription>> ListAllSubscriptionsForTag(string tag)
        {
            var query = "SELECT * FROM Subscriptions WHERE Tag = @tag";

            using var con = _con.Get();
            return con.QueryAsync<Subscription>(query, new {tag});
        }

        public Task<IEnumerable<Subscription>> ListAllSubscriptionsForTags(List<string> tags)
        {
            var query =
                "SELECT Id, TelegramId, GROUP_CONCAT(Tag, \", \") AS Tag FROM Subscriptions WHERE Tag IN @tags GROUP BY TelegramId";

            using var con = _con.Get();
            return con.QueryAsync<Subscription>(query, new {tags});
        }

        public Task DeleteSubscription(long telegramId, string tag)
        {
            string query = "DELETE FROM Subscriptions WHERE TelegramId = @telegramId and Tag = @tag";

            using var con = _con.Get();
            return con.ExecuteAsync(query, new {telegramId, tag});
        }
    }
}