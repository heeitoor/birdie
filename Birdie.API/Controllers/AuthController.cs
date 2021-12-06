using System.Threading.Tasks;
using Birdie.API.Common;
using Birdie.Service.Business;
using Birdie.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Birdie.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserBusiness userBusiness;
        private readonly IJwtGenerator jwtGenerator;

        public AuthController(ILogger<AuthController> logger, IUserBusiness userBusiness, IJwtGenerator jwtGenerator)
        {
            this._logger = logger;
            this.userBusiness = userBusiness;
            this.jwtGenerator = jwtGenerator;
        }

        [HttpPost("signup")]
        public Task<bool> SignUp([FromBody] UserSignUpModel model)
        {
            return userBusiness.Create(model);
        }

        [HttpPost("login")]
        public async Task<TokenResponse> Login([FromBody] UserLoginModel model)
        {
            int id = await userBusiness.Authenticate(model);
            return jwtGenerator.GetToken(id, model.UserName);
        }
    }
}
