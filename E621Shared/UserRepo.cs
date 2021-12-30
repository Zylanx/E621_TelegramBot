using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace E621Shared
{
    public class User
    {
        public string? UserId { get; set; }

        public string? ChatId { get; set; }
        // public UserState? State { get; set; }
    }

    public class UserRepo
    {
        private readonly ConnectionProvider _con;

        public UserRepo(ConnectionProvider provider)
        {
            _con = provider;
            Init();
        }

        private void Init()
        {
            string query =
                "create table if not exists Users(Id INTEGER PRIMARY KEY, UserId TEXT, ChatId TEXT)";

            using var con = _con.Get();
            con.Execute(query);
        }

        //lets do command query seperation, stuff either modifies the dbase, or returns data, not both.

        public Task<int> CreateUser(User user)
        {
            string query =
                "INSERT INTO Users (UserId, ChatId) VALUES (@UserId, @ChatId)";

            using var con = _con.Get();
            return con.ExecuteAsync(query, user);
        }

        public async Task<User?> GetUser(string userId)
        {
            string query = "SELECT * FROM Users WHERE UserId = @userId";

            using var con = _con.Get();
            return (await con.QueryAsync<User>(query)).FirstOrDefault();
        }

        // TODO: Maybe make it an async enumerable?
        public Task<IEnumerable<User>> ListAllUsers()
        {
            string query = "select * from Users";

            using var con = _con.Get();
            return con.QueryAsync<User>(query);
        }
    }
}