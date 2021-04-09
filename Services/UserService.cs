using jwt_authentication_service.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace jwt_authentication_service.Services
{
    public interface IUserService
    {
        AuthenticatedUser Authenticate(User user);
        Boolean VerifyTokenValidity(string token);
        List<User> Get();
        User Get(string id);
        User Create(User user);
        void Update(string id, User userIn);
        void Remove(string id);
    }

    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly AppSettings _appSettings;

        public UserService(IUserDatabaseSettings settings, IOptions<AppSettings> appSettings)
        {
            var user = new MongoClient(settings.ConnectionString);
            var database = user.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
            _appSettings = appSettings.Value;
        }

        public AuthenticatedUser Authenticate(User userIn)
        {
            User user = _users.Find(user => user.username == userIn.username && user.password == userIn.password).FirstOrDefault();

            if (user == null) return null;

            String token = GenerateJwtToken(user);

            return new AuthenticatedUser(user, token);
        }

        public Boolean VerifyTokenValidity(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // On met clockskew à zero pour que les tokens expirent au moment de leurs temps d'expiration, et pas 5min plus tard ( ce qui est la valeur par défaut )
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private String GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id) }),
                // Le token expire au bout d'un jour
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public List<User> Get() =>
            _users.Find(client => true).ToList();

        public User Get(string id) =>
            _users.Find<User>(user => user.Id == id).FirstOrDefault();

        public User Create(User user)
        {
            _users.InsertOne(user);

            return user;
        }

        public void Update(string id, User userIn) =>
            _users.ReplaceOne(user => user.Id == id, userIn);

        public void Remove(string id) =>
            _users.DeleteOne(user => user.Id == id);

    }
}
