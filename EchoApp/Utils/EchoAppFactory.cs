using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils
{
    /*  public class EchoAppFactory
      {
          private readonly IUserLoginService _userLoginService;
          private readonly IUserLoginService _adminLoginService;
      }

       * public class UserLoginServiceFactory : ILoginServiceFactory
  {
      private readonly IUserLoginService _userLoginService;
      private readonly IUserLoginService _adminLoginService;
      public UserLoginServiceFactory([Named("UserLogin")]IUserLoginService userLoginService, [Named("AdminLogin")]IUserLoginService adminLoginService)
      {
          _userLoginService = userLoginService;
          _adminLoginService = adminLoginService;
      }

      public IUserLoginService GetLoginService(string username)
      {
          if (username.Contains("@somecompanyname.com")) 
          {
              return _adminLoginService;
          }
          return _userLoginService;
      }
       */
}
