using MongoDB.Driver;
using MyApp.Model;
using BCrypt.Net;

namespace MyApp.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService()
        {
            var settings = MongoClientSettings.FromConnectionString(
                "mongodb://student:IAmTh3B3st@185.157.245.38:5003");

            var client = new MongoClient(settings);
            var database = client.GetDatabase("BasicFatDB"); // ✅ Ta base
            _users = database.GetCollection<User>("user");    // ✅ Ta collection
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _users.Find(user => true).ToListAsync();
        }

        public async Task AddUserAsync(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.CreatedAt = DateTime.UtcNow;
            await _users.InsertOneAsync(user);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPassword);
        }

        public async Task DeleteUserAsync(string id)
        {
            await _users.DeleteOneAsync(user => user.Id == id);
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
                return null;

            bool valid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            return valid ? user : null;
        }

        public async Task UpdateUserAsync(User user, string newPassword = null)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }

            await _users.ReplaceOneAsync(filter, user);
        }

    }
}
