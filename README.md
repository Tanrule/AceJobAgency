# AceJobAgency

1. Google Recaptcha

step 1: https://codeburst.io/implement-recaptcha-in-asp-net-core-and-razor-pages-eed8ae720933
step 2: https://www.nuget.org/packages/AspNetCore.ReCaptcha/

appsetting change section: 

"ReCaptcha": {
    "SiteKey": "",
    "SecretKey": "",
    "Version": ""
  }

2. Google Sign In create credentials 

https://developers.google.com/workspace/guides/create-credentials#api-key

appsetting change section: 

"Google": {
    "ClientId": "",
    "ClientSecret": ""
  }
  
  
3. Integrate gmail to send system emails 

step 1: enable IMAP
https://support.google.com/googlecloud/answer/10636934?hl=en

step 2: Create google app password
https://support.google.com/accounts/answer/185833?hl=en

appsetting change section:

"MailKit": {
    "Username": "",      ----Email
    "Password": "",      ----App password
    "From": {
      "Name": ""
    }
  }
