using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Twilio.Types;
using Twilio;
using System;
using System.Collections.Generic;
using Twilio.Rest.Api.V2010.Account;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace SendSMS
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("sendSMS")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            // Create connection to azure key vault 
            var KeyVaultUI = "https://akv-os-ccs.vault.azure.net/";
            var client = new SecretClient(new Uri(KeyVaultUI), new DefaultAzureCredential());

            // Retrieve the twilio SID
            KeyVaultSecret sid = client.GetSecret("TWILIOSID");
            var accountSid = sid.Value;

            // Retrieve the twilio token for auth
            KeyVaultSecret token = client.GetSecret("TWILIOAUTHTOKEN");
            var authToken = token.Value;

            // And to not expose Leos Phone Number to strangers we also retrieve that
            KeyVaultSecret leoNummer = client.GetSecret("LeoPhoneNumber");
            var phoneNumber = leoNummer.Value;

            // Connect to the Twilio service
            TwilioClient.Init(accountSid, authToken);
            var messageOptions = new CreateMessageOptions(
              new PhoneNumber(phoneNumber));

            // Send from assigned source message
            messageOptions.From = new PhoneNumber("+18706148721");
            messageOptions.Body = "Flo du Specht!";
            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);
            
            // Log results
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Message sent to Leo!");
        }
    }
}
