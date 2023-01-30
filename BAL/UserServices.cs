using System.Security.Claims;

namespace JWT.BAL
{
    public class UserServices : IUserServices
    {
        private readonly IHttpContextAccessor _httpcontext;

        public UserServices(IHttpContextAccessor httpcontext)
        {
            _httpcontext = httpcontext;
        }
        //Authorization claims of a user
        public string GetName()
        {
            var res=string.Empty;
            if(_httpcontext.HttpContext!=null)
            {
                res=_httpcontext.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }
            return(res);

        }
    }
}