﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BOS.StarterCode.Helpers
{
    public class EmailHelper : IEmailSender
    {
        private string _fromEmail;
        private string _apiKey;

        public EmailHelper(string fromEmail, string apiKey)
        {
            _fromEmail = fromEmail;
            _apiKey = apiKey;
        }
        
        public async Task SendEmail(string toEmailAddress, Dictionary<string, string> parameters)
        {
            try
            {
                var subject = parameters["Subject"];
                var htmlContent = FetchTemplate(parameters);
                await SendEmailAsync(toEmailAddress, subject, htmlContent);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(_apiKey);
            var msg = MailHelper.CreateSingleEmail(new EmailAddress(_fromEmail, "BOS Team"), new EmailAddress(email), subject, "", htmlMessage);
            var response = await client.SendEmailAsync(msg);
        }
        private string FetchTemplate(Dictionary<string, string> parameters)
        {
            string fileName = string.Empty;
            String template = string.Empty;
            switch (parameters["Action"])
            {
                case "Registration":
                    fileName = "register.html";
                    break;
                case "ForgotPassword":
                    fileName = "forgotpassword.html";
                    break;
                default:
                    fileName = "";
                    break;
            }
            string filePath = Directory.GetCurrentDirectory();

            using (StreamReader sr = new StreamReader(filePath + "/Helpers/Email/Templates/" + fileName))
            {
                // Read the stream to a string, and write the string to the console.
                template = sr.ReadToEnd();
                Console.WriteLine(template);
            }
            UpdateTemplateValues(ref parameters, ref template);
            return template;
        }

        private void UpdateTemplateValues(ref Dictionary<string, string> parameters, ref string template)
        {
            template = template.Replace("{ApplicationLink}", parameters["ApplicationURL"]);
            template = template.Replace("{FromAddress}", parameters["From"]);
            switch (parameters["Action"])
            {
                case "Registration":
                    template = template.Replace("{Name}", parameters["Name"]);
                    break;
                case "ForgotPassword":
                    template = template.Replace("{Password}", parameters["Password"]);
                    break;
                default:
                    break;
            }
        }
    }
}
