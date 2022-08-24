using Merolekando.Models;
using Merolekiando.Models;

namespace Merolekando.Services.Token
{
    public interface IJwtToken
    {
        public string CreateUserToken(User user);
    }
}
