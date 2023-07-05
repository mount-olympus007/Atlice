using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Web;
using Twilio.Types;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDataRepository _dataRepository;

        public Worker(ILogger<Worker> logger, IDataRepository dataRepository)
        {
            _dataRepository= dataRepository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool runOnce = false;
            bool runTwice = false;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                if (DateTime.Now.Hour == 12 && runOnce == false)
                {
                    await SendTexts();
                    runOnce = true;
                }

                if (DateTime.Now.Hour == 18 && runTwice == false)
                {
                    await SendTexts();
                    runTwice = true;
                }
                if (DateTime.Now.Hour == 23 && runOnce == true && runTwice == true)
                {
                    runOnce = false;
                    runTwice = false;
                }

                await Task.Delay(10000, stoppingToken);
            }
        }
        public async Task SendTexts()
        {
            foreach (var user in _dataRepository.Users.ToList())
            {
                if (!string.IsNullOrEmpty(user.PhoneNumber))
                {
                    IdentityUserRole<Guid>? role = _dataRepository.UserRoles.FirstOrDefault(x => x.UserId == user.Id);
                    if (role != null)
                    {
                        if (role.RoleId == Guid.Parse("87edb365-1b60-46aa-93b3-ebc548d18c31") || role.RoleId == Guid.Parse("74034857-7758-4035-ace4-a5d62f0ec952") || role.RoleId == Guid.Parse("43b4454c-1e0f-487f-b982-9737ac26a579"))
                        {
                            RewardTracker? rt = _dataRepository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                            if (rt is not null)
                            {
                                if (!rt.EligibilityForm)
                                {
                                    var days = DateTime.Now - user.Created;
                                    if (days.GetValueOrDefault().Days == 2 || days.GetValueOrDefault().Days == 5 || days.GetValueOrDefault().Days == 8 || days.GetValueOrDefault().Days == 9)
                                    {
                                        TwilioClient.Init("ACb1d1152c21f932cfe6889a08235d4289", "56c53a0f2e041625afe93ae710d2d603");
                                        var messageOptions = new CreateMessageOptions(
                                            new PhoneNumber("+1" + user.PhoneNumber))
                                        {
                                            From = new PhoneNumber("+12152615226"),
                                            Body = "Hey " + user.FirstName + "! You have " + (10 - days.GetValueOrDefault().Days).ToString() + " days left to complete Atlice Tap Enrollemnt and Onboarding before lock out. Click the following link to get started. " + "https://atlice.com/betaask/welcome" + " \n\n Add Atlice Tap to your contacts with this link: https://atlice.com/cards?email=atlicetap@atlice.com&contactid=0"
                                        };
                                        var mess = MessageResource.Create(messageOptions);


                                        await _dataRepository.SaveAdminNote(new AdminNote(user.Id, user.UserName, "Verify Phone Text sent from settings"));
                                        continue;
                                    }
                                }
                                if (!rt.OnboardingStep2)
                                {
                                    var days = DateTime.Now - user.Created;
                                    if (days.GetValueOrDefault().Days == 2 || days.GetValueOrDefault().Days == 5 || days.GetValueOrDefault().Days == 8 || days.GetValueOrDefault().Days == 9)
                                    {
                                        TwilioClient.Init("ACb1d1152c21f932cfe6889a08235d4289", "56c53a0f2e041625afe93ae710d2d603");
                                        var messageOptions = new CreateMessageOptions(
                                            new PhoneNumber("+1" + user.PhoneNumber))
                                        {
                                            From = new PhoneNumber("+12152615226"),
                                            Body = "Hey " + user.FirstName + "! You have " + (10 - days.GetValueOrDefault().Days).ToString() + " days left to complete Atlice Tap Enrollemnt and Onboarding before lock out. Click the following link to get started. " + "https://atlice.com/Identity/Account/login?tapid=0" + " \n\n Add Atlice Tap to your contacts with this link: https://atlice.com/cards?email=atlicetap@atlice.com&contactid=0"

                                        };
                                        var mess = MessageResource.Create(messageOptions);

                                        //await _dataRepository.SaveAdminNote(new AdminNote(user.Id, user.UserName, "Verify Phone Text sent from settings"));
                                        continue;
                                    }
                                }
                            }
                        }

                    }
                }

            }

            foreach (var o in _dataRepository.Orders.Where(x => x.Status == OrderStatus.Shipped))
            {
                var span = DateTime.Now - o.OrderShipped;
                if (span.Days >= 10)
                {
                    TwilioClient.Init("ACb1d1152c21f932cfe6889a08235d4289", "56c53a0f2e041625afe93ae710d2d603");
                    var messageOptions = new CreateMessageOptions(
                        new PhoneNumber("+1" + o.Phone))
                    {
                        From = new PhoneNumber("+12152615226"),
                        Body = o.Name.Split(" ")[0] + ", It’s time to get activated! Go to this link to set up your devices and connect them to a contact page. https://atlice.com/tap/" + o.Taps.First().SNumber[..8] + " \n\n Go to step-by-step activation guide here. https://atlice.com/tap/setup"

                    };
                    var mess = MessageResource.Create(messageOptions);

                    WebClient webClient = new WebClient();
                    Uri i = new Uri("https://atlicemedia.blob.core.windows.net/atlicemails/system_message.html");
                    var response = webClient.DownloadString(i);
                    response = response.Replace("[username]", o.Name.Split(' ')[0]);
                    response = response.Replace("[tapid]", o.Taps.First().SNumber[..8]);
                    var pm = new PreMailer.Net.PreMailer(response).MoveCssInline();

                    SmtpClient client = new("smtp-mail.outlook.com")
                    {
                        Port = 587,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false
                    };
                    System.Net.NetworkCredential credentials =
                        new("atlicetap@atlice.com", "Fundations3");
                    client.EnableSsl = true;
                    client.Credentials = credentials;

                    MailAddress sender = new("atlicetap@atlice.com", "Atlice Tap");
                    MailAddress recipient = new(o.Email);
                    try
                    {
                        var mail = new MailMessage(sender, recipient)
                        {
                            Subject = "Atlice Tap System Message",
                            Body = HttpUtility.HtmlDecode(pm.Html),
                            IsBodyHtml = true
                        };
                        client.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
            }
        }
    }
}