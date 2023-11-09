using Microsoft.AspNetCore.Identity;

namespace CodePulse.API.Repositories.Interface
{
    public interface ITokenRepository
    {
        //To create token against user and also asign token against role
        string CreateJwtToken(IdentityUser user, List<string> roles); 
    }
}
