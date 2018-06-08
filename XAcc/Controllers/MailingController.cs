using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace XAcc.Controllers
{
    public class MailingController : Controller
    {
        public IActionResult Index()
        {
            return Json("This action has no view");
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> SendMail()
        {
            if(await SendMailAsyncWithMailjet() == true)
            {
                return Json("Ok, Send completed.");
            }
            else
            {
                return Json("Oh!, There's some error.");
            }
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> SendMailWithAttachment(List<IFormFile> files)
        {
            if (files == null)
                return Json("file not selected.");


            if(await SendMailAsyncWithMailjet(files) == true)
            {
                return Json("Ok, Send completed.");
            }
            else
            {
                return Json("Oh!, There's some error.");
            }

        }

        public static async Task<bool> SendMailAsyncWithMailjet(List<IFormFile> attachFile = null)
        {
            MailjetClient client = new MailjetClient("API_KEY_IN_MAILJET", "API_SECRET_IN_MAILJET")
            {
                Version = ApiVersion.V3_1,
            };

            JArray attach_files = new JArray();
            if(attachFile != null)
            {
                foreach (var f in attachFile)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        f.CopyTo(ms);

                        attach_files.Add(new JObject
                        {
                            {"ContentType", f.ContentType },
                            {"FileName", f.FileName },
                            {"Base64Content", Convert.ToBase64String(ms.ToArray()) }
                        });
                    }
                }
            }
            

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
               .Property(Send.Messages, new JArray {
                    new JObject
                    {
                        {
                            "From", new JObject
                            {
                                //{"Email", "email_in_mailjet"},
                                //{"Name", "label_of_email_in_mailjet"}
                            }
                        },
                        {
                            "To", new JArray
                            {
                                new JObject
                                {
                                    {"Email", "test36@gmail.com"},
                                    {"Name", "Wee"}
                                },
                                //new JObject
                                //{
                                //    {"Email", "test@hotmail.com" },
                                //    {"Name", "WeeHot" }
                                //},
                                //new JObject
                                //{
                                //    {"Email", "test.again@gmail.com" },
                                //    {"Name", "Win" }
                                //}
                            }
                        },
                        {
                            "Subject",
                            "Test sending email by MailJet ... ทดสอบส่งเมล์ด้วย MailJet"
                        },
                        {
                            "TextPart",
                            "ลองส่งอีเมล์ด้วย MailJet"
                        },
                        {
                            "HTMLPart",
                            "<h3>Hello World</h3><br />It'a amazing."
                        },
                        {
                            "Attachments", attach_files
                            //new JArray
                            //{
                            //    new JObject
                            //    {
                            //        {"ContentType", "text/plain" },
                            //        {"FileName", "file_test.txt" },
                            //        {"Base64Content", Convert.ToBase64String(Encoding.ASCII.GetBytes("Test content in text file")) }
                            //    }
                            //}
                        }
                    }
                });

            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());
                return true;
            }
            else
            {
                Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
                Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
                Console.WriteLine(response.GetData());
                Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                return false;
            }
        }

        static async Task SendMailAsyncWithSendgrid()
        {
            //var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var client = new SendGridClient("Sendgrid_Api_Key");
            var subject = "Test sending mail with SendGrid - ทดสอบ";
            var plainTextContent = "ลองส่งเมล์ด้วย SendGrid";
            var htmlContent = "<strong>It's so easy..</strong>";

            var msg = new SendGridMessage();
            msg.Personalizations = new List<Personalization>
            {
                new Personalization
                {
                    Tos = new List<EmailAddress>
                    {
                        new EmailAddress("test@gmail.com", "Wee")
                    },
                    Ccs = new List<EmailAddress>
                    {
                        new EmailAddress("test@hotmail.com", "WeeHot")
                    },
                    Bccs = new List<EmailAddress>
                    {
                        new EmailAddress("test.again@gmail.com", "Win")
                    }
                }
            };
            msg.From = new EmailAddress("from@gmail.com", "Sender");
            msg.Subject = subject;
            msg.PlainTextContent = plainTextContent;
            msg.HtmlContent = htmlContent;

            var response = await client.SendEmailAsync(msg);
        }
    }
}