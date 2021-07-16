using MSN.ModelContext;
using System;
using Microsoft.AspNetCore.Identity;
using MSN.BlazorServer.Data;
using MSN.Model;
using System.Linq;
using System.Threading.Tasks;

namespace MSN.ViewModel
{
    public class LoginUserModel : IDisposable
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SecurityStatement { get; set; }
        protected MSNContext context { get; set; }
        public LoginUserModel()
        {
            context = new MSNContext();
        }
        public AccountUser LoginUser()
        {
            return context.AccountUser.Where(x => x.Username == UserName && x.Password == Password && x.Visible).FirstOrDefault();
        }
        public async Task<string> ResetUserPassword(MSNBlazorServerContext context, SignInManager<IdentityUser> signInManager)
        {
            string result = string.Empty;
            IdentityUser user = await signInManager.UserManager.FindByNameAsync(UserName);
            SecurityStatement statement = context.SecurityStatements.Where(x => x.Statement == this.SecurityStatement).FirstOrDefault();
            if (user == null || statement == null || user.Id != statement.AspNetUsersID.ToString())
                return string.Empty;
                
            // Security Statement Match
            {
                string newPassword = System.IO.Path.GetRandomFileName().Replace(".", "");
                string token = await signInManager.UserManager.GeneratePasswordResetTokenAsync(user);
                IdentityResult resetResult = await signInManager.UserManager.ResetPasswordAsync(user, token, newPassword);
                if (resetResult.Succeeded)
                    result = newPassword;
            }
            return result;
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
