using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace E621Shared.Repositories
{
    public class GroupChat
    {
        public string? ChatId { get; set; }
        public string? InitiatingUserId { get; set; }
    }

    public class GroupChatRepo
    {
        private readonly ConnectionProvider _con;

        public GroupChatRepo(ConnectionProvider provider)
        {
            _con = provider;
            Init();
        }

        private void Init()
        {
            string query =
                "create table if not exists GroupChat(Id INTEGER PRIMARY KEY, UserId TEXT, ChatId TEXT)";

            var con = _con.Get();
            con.Execute(query);
        }

        //lets do command query seperation, stuff either modifies the dbase, or returns data, not both.

        public Task<int> CreateUser(User user)
        {
            string query =
                "INSERT INTO Users (TelegramId, ChatId) VALUES (@TelegramId, @ChatId)";

            var con = _con.Get();
            return con.ExecuteAsync(query, user);
        }

        public async Task<User?> GetUser(string telegramId)
        {
            string query = "SELECT * FROM Users WHERE TelegramId = @telegramId";

            var con = _con.Get();
            return (await con.QueryAsync<User>(query)).FirstOrDefault();
        }
    }
}