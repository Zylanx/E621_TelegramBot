using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;

namespace E621Shared.Repositories
{
    public class User
    {
        [ExplicitKey]
        public long UserId { get; set; }

        public long ChatId { get; set; }
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
                "create table if not exists Users(UserId INTEGER PRIMARY KEY, ChatId INTEGER)";

            using var con = _con.Get();
            con.Execute(query);
        }

        //lets do command query seperation, stuff either modifies the dbase, or returns data, not both.

        public Task<int> CreateUser(User user)
        {
            using var con = _con.Get();
            return con.InsertAsync(user);
        }

        public Task<User?> GetUser(long userId)
        {
            using var con = _con.Get();
            return con.GetAsync<User?>(userId);
        }

        // TODO: Maybe make it an async enumerable?
        public Task<IEnumerable<User>> ListAllUsers()
        {
            using var con = _con.Get();
            return con.GetAllAsync<User>();
        }
    }
}