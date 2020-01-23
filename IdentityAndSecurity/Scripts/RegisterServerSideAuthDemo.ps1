Clear-Host

$userName = ""
$password = ""

$appDisplayName = "Server-side Auth Demo"
$replyUrl1 = "https://localhost:3007"
$replyUrl2 = "https://localhost:3007/signin-oidc"
$logoutUrl = "https://localhost:3007/signout-oidc"

$outputFile = "$PSScriptRoot\ServerSideAuthDemo.txt"
$newline = "`r`n"
Write-Host "Writing info to $outputFile"

$securePassword = ConvertTo-SecureString –String $password –AsPlainText -Force

$credential = New-Object –TypeName System.Management.Automation.PSCredential `
                         –ArgumentList $userName, $securePassword

$authResult = Connect-AzureAD -Credential $credential

$tenantId = $authResult.TenantId.ToString()
$tenantDomain = $authResult.TenantDomain
$tenantDisplayName = (Get-AzureADTenantDetail).DisplayName

$userAccountId = $authResult.Account.Id
$user = Get-AzureADUser -ObjectId $userAccountId
$userDisplayName = $user.DisplayName

# create app secret
$newGuid = New-Guid
$appSecret = ([System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes(($newGuid))))+"="
$startDate = Get-Date	
$passwordCredential = New-Object -TypeName Microsoft.Open.AzureAD.Model.PasswordCredential
$passwordCredential.StartDate = $startDate
$passwordCredential.EndDate = $startDate.AddYears(1)
$passwordCredential.KeyId = $newGuid
$passwordCredential.Value = $appSecret 

Write-Host "Registering new app $appDisplayName in $tenantDomain"

# create Azure AD Application
$aadApplication = New-AzureADApplication `
                        -DisplayName $appDisplayName `
                        -PublicClient $false `
                        -AvailableToOtherTenants $false `
                        -ReplyUrls @($replyUrl1, $replyUrl2) `
                        -Homepage $replyUrl `
                        -PasswordCredentials $passwordCredential `
                        -LogoutUrl $logoutUrl

# create applicaiton's service principal 
$appId = $aadApplication.AppId
$appObjectId = $aadApplication.ObjectId
$serviceServicePrincipal = New-AzureADServicePrincipal -AppId $appId

# assign current user as owner
Add-AzureADApplicationOwner -ObjectId $aadApplication.ObjectId -RefObjectId $user.ObjectId

# configure login permissions for Azure Graph API
$requiredResourcesAccess1 = New-Object System.Collections.Generic.List[Microsoft.Open.AzureAD.Model.RequiredResourceAccess]

# configure signin delegated permisssions for the Microsoft Graph API
$requiredAccess1 = New-Object -TypeName "Microsoft.Open.AzureAD.Model.RequiredResourceAccess"
$requiredAccess1.ResourceAppId = "00000003-0000-0000-c000-000000000000"

# openid SignIn - 37f7f235-527c-4136-accd-4a02d197296e
$scopeSignIn = New-Object -TypeName "Microsoft.Open.AzureAD.Model.ResourceAccess" `
                          -ArgumentList "37f7f235-527c-4136-accd-4a02d197296e","Scope"

# Mail.Read - 570282fd-fa5c-430d-a7fd-fc8dc98a9dca
$scopeMailRead = New-Object -TypeName "Microsoft.Open.AzureAD.Model.ResourceAccess" `
                            -ArgumentList "570282fd-fa5c-430d-a7fd-fc8dc98a9dca","Scope"

# Mail.Send - e383f46e-2787-4529-855e-0e479a3ffac0
$scopeMailSend = New-Object -TypeName "Microsoft.Open.AzureAD.Model.ResourceAccess" `
                            -ArgumentList "e383f46e-2787-4529-855e-0e479a3ffac0","Scope"

# Sites.ReadWrite.All - 89fe6a52-be36-487e-b7d8-d061c450a026
$scopeSiteReadWriteAll = New-Object -TypeName "Microsoft.Open.AzureAD.Model.ResourceAccess" `
                                    -ArgumentList "89fe6a52-be36-487e-b7d8-d061c450a026","Scope"

# User.ReadWrite - b4e74841-8e56-480b-be8b-910348b18b4c
$scopeUserReadWrite = New-Object -TypeName "Microsoft.Open.AzureAD.Model.ResourceAccess" `
                                 -ArgumentList "b4e74841-8e56-480b-be8b-910348b18b4c","Scope"

$requiredAccess1.ResourceAccess = $scopeSignIn, $scopeMailRead, $scopeMailSend, $scopeSiteReadWriteAll, $scopeUserReadWrite


Set-AzureADApplication -ObjectId $appObjectId -RequiredResourceAccess @($requiredAccess1)

Out-File -FilePath $outputFile -InputObject "--- Info for $appDisplayName ---"
Out-File -FilePath $outputFile -Append -InputObject "TenantDomain : $tenantDomain"
Out-File -FilePath $outputFile -Append -InputObject "TenantId: $tenantId"
Out-File -FilePath $outputFile -Append -InputObject "ClientId: $appId"
Out-File -FilePath $outputFile -Append -InputObject "ClientSecret: $appSecret"
Out-File -FilePath $outputFile -Append -InputObject ""
Out-File -FilePath $outputFile -Append -InputObject "ReplyUrl1: $replyUrl1"
Out-File -FilePath $outputFile -Append -InputObject "ReplyUrl2: $replyUrl2"
Out-File -FilePath $outputFile -Append -InputObject "LogoutUrl: $logoutUrl"

Notepad $outputFile