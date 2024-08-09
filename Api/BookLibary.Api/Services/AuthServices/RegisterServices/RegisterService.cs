using BookLibary.Api.Dtos.UserDto;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

namespace BookLibary.Api.Services.AuthServices.RegisterServices
{
    public class RegisterService : IRegisterService
    {
        private readonly IRegisterRepository<User> _repository;
        private string hashedPassword;


        public RegisterService(IRegisterRepository<User> repository)
        {
            _repository = repository;
        }

        public async Task<string> Register(RegisterDto model)
        {
            var UserName = await _repository.GetByNameAsync(model.UserName);
            var Email =await _repository.GetByNameAsync(model.Email);
            SHA1 sha = new SHA1CryptoServiceProvider();
           


                if (UserName != null) {
                return "Kullanılmış Kullanıcı Adı" ;
            }
            if (Email != null)
            {
                return "Kullanılmış Email";
            }
            if (model.Password.Trim() != model.PasswordRepeat.Trim())
            {
                return "Şifreler Uyuşmuyor";
            }
            

            hashedPassword = Convert.ToBase64String(sha.ComputeHash(Encoding.ASCII.GetBytes(model.Password)));
            var url = "https://avatar.iran.liara.run/public";
            if (model.Gender == GenderType.Female)
            {
                url = $"https://avatar.iran.liara.run/public/girl/?username={model.UserName}";

            }
            else
            {
                url = $"https://avatar.iran.liara.run/public/boy/?username={model.UserName}";

            }


            var user = new User
            {
                UserName = model.UserName,
                FullName = model.FullName,
                Email = model.Email,
                Password = hashedPassword,
                gender = model.Gender,
                avatarUrl = url,
                IsAdmin =false,

            };

            await _repository.InsertOneAsync(user);
            return "x";
        }
    }
}
