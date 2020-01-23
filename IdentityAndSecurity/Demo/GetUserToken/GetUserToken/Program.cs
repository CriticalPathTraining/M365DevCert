using Microsoft.Identity.Client;
using System;
using System.Linq;
using Diagnostics = System.Diagnostics;
using System.Security;
using System.Threading.Tasks;
using msal = Microsoft.Identity.Client;
using Microsoft.Graph;

class Program {

 
  static string GetAccessTokenWithDeviceCode(string[] scopes) {

    // device code authentication requires tenant-specific authority URL
    var appPublic = PublicClientApplicationBuilder.Create(AppSettings.clientId)
                      .WithAuthority(AppSettings.tenantSpecificAuthority)
                      .Build();

    // this method call will block until you have logged in using the generated device code
    var authResult = appPublic.AcquireTokenWithDeviceCode(scopes, deviceCodeCallbackParams => {
      // retrieve device code and verification URL from deviceCodeCallbackParams 
      string deviceCode = deviceCodeCallbackParams.UserCode;
      string verificationUrl = deviceCodeCallbackParams.VerificationUrl;
      Console.WriteLine();
      Console.WriteLine("When prompted by the browser, copy-and-paste the following device code: " + deviceCode);
      Console.WriteLine();
      Console.WriteLine("Opening Browser at " + verificationUrl);
      Diagnostics.Process.Start("chrome.exe", verificationUrl);
      Console.WriteLine();
      Console.WriteLine("This console app will now block until you enter the device code and log in");
      // return task result
      return Task.FromResult(0);
    }).ExecuteAsync().Result;


    return authResult.AccessToken;

  }


  static void Main() {

    //GetUserInfo();
    GetOrgInfo();

  }

  // static IAuthenticationProvider authProvider = new UserInteractiveAuthProvider();
  // static IAuthenticationProvider authProvider = new UserDirectPasswordAuthProvider();
  // static IAuthenticationProvider authProvider = new UserDeviceCodeAuthProvider();
  //static IAuthenticationProvider authProvider = new AppOnlyAuthProvider();

  static GraphServiceClient graphServiceClient = new GraphServiceClient(authProvider);

  static void GetUserInfo() {

    // call across Internet and wait for response
    var user = graphServiceClient.Me.Request().GetAsync().Result;

    Console.WriteLine("Current user info obtained with .NET Client");
    Console.WriteLine("-------------------------------------------");
    Console.WriteLine("Display Name: " + user.DisplayName);
    Console.WriteLine("First Name: " + user.GivenName);
    Console.WriteLine("Last Name: " + user.Surname);
    Console.WriteLine("User Principal Name: " + user.UserPrincipalName);
    Console.WriteLine();
    Console.WriteLine();
  }

  static void GetOrgInfo() {

    var org = graphServiceClient.Organization.Request().GetAsync().Result.FirstOrDefault<Organization>();

    Console.WriteLine("Current organizational info obtained with .NET Client");
    Console.WriteLine("-------------------------------------------");
    Console.WriteLine("ID: " + org.Id);
    Console.WriteLine("Display Name: " + org.DisplayName);
    Console.WriteLine("Tenant Domain: " + org.VerifiedDomains.FirstOrDefault<VerifiedDomain>().Name);
    Console.WriteLine("Country Letter Code: " + org.CountryLetterCode);
    Console.WriteLine("Technical Email: " + org.TechnicalNotificationMails.FirstOrDefault());
    Console.WriteLine();
    Console.WriteLine();
  }
}