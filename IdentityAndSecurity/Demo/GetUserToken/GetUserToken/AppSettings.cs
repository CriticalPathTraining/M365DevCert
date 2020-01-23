using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class AppSettings {

  // update the following four constants with the values from your envirionment
  public const string clientId = "f78c070d-d551-4998-82f2-89b7da7270dd";
  public const string tenantName = "M365DevCert.onMicrosoft.com";
  public const string redirectUri = "https://localhost/app1234";

  public const string appOnlyClientId = "a713dbea-d0a9-44c4-a901-8b53da52a2ee";
  public const string appOnlyClientSecret = "8=F1@x09LzY[=bC2b]gvhFcEm4gO3o@_";

  // generic v2 endpoint references "organizations" instead of "common"
  public const string tenantCommonAuthority = "https://login.microsoftonline.com/organizations";
  public const string tenantSpecificAuthority = "https://login.microsoftonline.com/" + tenantName;

  // Microsoft Graph API Root URL  
  public const string urlMicrosoftGraphApiRoot = "https://graph.microsoft.com/";

  public static readonly string[] scopesForMicrosoftGraph = new string[] {"user.read"};

  public const string userName = "tedp@M365DevCert.onMicrosoft.com";
  public const string userPassword =                                                                                                      "pass@word1";

}
