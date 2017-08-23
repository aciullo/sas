using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using sas_Futura.Controllers.API;
using sas_Futura.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace sas_Futura
{
    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;

        private UserManager<IdentityUser> _userManager;

        
        public AuthRepository()
        {
            _ctx = new AuthContext();
            //_userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
           

        }
        public async Task<IdentityResult> RegisterUser(DeviceUser userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.usuario
            };

            var result = await _userManager.CreateAsync(user, userModel.pass);

            return result;
        }

        public async Task<DeviceUser> FindUser(string userid, string password)
        {
            DeviceUser user = _ctx.DeviceUser.Where(u => u.usuario == userid && u.pass == password).ToList().FirstOrDefault(); 

            return user;
        }

        public void Dispose()
        {
            _ctx.Dispose();
           // _userManager.Dispose();

        }
    }
}