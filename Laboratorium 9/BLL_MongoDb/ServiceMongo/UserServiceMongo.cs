using BLL.DTOModels.UserDTOs;
using BLL.ServiceInterfaces;
using MongoDB.Driver;
using System.Threading.Tasks;
using static BLL_MongoDb.ModelMongo.UserMongo;

namespace BLL_MongoDb.ServiceMongo
{
    public class UserServiceMongo : IUserService
    {
        private readonly IMongoCollection<MongoUser> _users;
        private static string? _sessionLogin = null;

        public UserServiceMongo(IMongoDatabase database)
        {
            _users = database.GetCollection<MongoUser>("users");
        }

        public async Task<bool> Login(UserLoginRequestDTO dto)
        {
            var user = await GetUserByLoginAndPassword(dto.Login, dto.Password);

            if (user != null)
            {
                _sessionLogin = user.Login;
                return true;
            }

            return false;
        }

        public Task Logout()
        {
            _sessionLogin = null;
            return Task.CompletedTask;
        }

        private async Task<MongoUser?> GetUserByLoginAndPassword(string login, string password)
        {
            var filter = Builders<MongoUser>.Filter.Eq(u => u.Login, login) &
                         Builders<MongoUser>.Filter.Eq(u => u.Password, password) &
                         Builders<MongoUser>.Filter.Eq(u => u.IsActive, true);

            return await _users.Find(filter).FirstOrDefaultAsync();
        }
    }
}
