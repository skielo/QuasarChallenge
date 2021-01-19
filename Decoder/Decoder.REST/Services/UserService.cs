using Decoder.REST.Entities;
using Decoder.REST.Helpers;
using Decoder.REST.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Decoder.REST.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }

    public class UserService : IUserService
    {
        //For the sake of this demo excersise the user name is hsolo and the pass is chewbacca
        private List<User> _users;

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _users = new List<User>
            {
                new User { Id = 1, FirstName = "Han", LastName = "Solo", Username = "hsolo", Password = _appSettings.UserPass }
            };
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password.Base64Encode());

            if (user == null) return null;

            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public IEnumerable<User> GetAll()
        {
            return _users;
        }

        public User GetById(int id)
        {
            return _users.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// This method generates a token that last for 7 days
        /// </summary>
        /// <param name="user">User logged in</param>
        /// <returns>The required token</returns>
        private string generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
