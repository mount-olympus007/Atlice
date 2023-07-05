using Atlice.Domain.Abstract;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using QRCoder;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System.Net.Mail;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs.Models;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3.Accounts;
using Nethereum.Web3.Accounts.Managed;
using Nethereum.Web3;
using Nethereum.HdWallet;
using Nethereum.Signer;
using NBitcoin;

namespace Atlice.Domain.Concrete
{
    public class Services:IServices
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        private readonly IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBlobService _blobService;
        private readonly IDataRepository _dataRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EFDbContext _dbContext;

        public Services(EFDbContext context, UserManager<ApplicationUser> userManager, IDataRepository dataRepository, IBlobService blobService, IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IHttpContextAccessor contextAccessor)
        {
            _dbContext = context;
            _userManager = userManager;
            _dataRepository = dataRepository;
            _blobService = blobService;
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _contextAccessor = contextAccessor;
        }
        private IView FindView(ActionContext actionContext, string viewName)
        {
            var getViewResult = _razorViewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
            if (getViewResult.Success)
            {
                return getViewResult.View;
            }

            var findViewResult = _razorViewEngine.FindView(actionContext, viewName, isMainPage: true);
            if (findViewResult.Success)
            {
                return findViewResult.View;
            }

            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

            throw new InvalidOperationException(errorMessage);
        }
        public async Task<string> RenderToString(string viewName, object model)
        {
            if(_contextAccessor.HttpContext is not null)
            {
                var actionContext = new ActionContext(_contextAccessor.HttpContext, _contextAccessor.HttpContext.GetRouteData(), new ActionDescriptor());

                await using var sw = new StringWriter();
                var viewResult = FindView(actionContext, viewName);

                if (viewResult == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.RenderAsync(viewContext);
                return sw.ToString();
            }
            else
            {
                return "No View Found. HttpContext is null.";
            }


        }


        public byte[] BitmapToBytesCode(Bitmap image)
        {
            using MemoryStream stream = new();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
        public Bitmap GenerateQR(int width, int height, string text)
        {
            QRCodeGenerator qrGenerator = new ();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new(qrCodeData);

            string webRootPath = _hostingEnvironment.WebRootPath;
            string path = Path.Combine(webRootPath, "icons/atliceQR.png");
            Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(path));

            return qrCodeImage;
        }
        public async Task<Task> SendEmailAsync(string email, string subject, string message)
        {

           
            var pm = new PreMailer.Net.PreMailer(message).MoveCssInline();

            SmtpClient client = new("smtp-mail.outlook.com")
            {
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };
            System.Net.NetworkCredential credentials =
                new(Configuration["EmailSettings:Sender"], Configuration["EmailSettings:Password"]);
            client.EnableSsl = true;
            client.Credentials = credentials;

            MailAddress sender = new("atlicetap@atlice.com", "Atlice Tap");
            MailAddress recipient = new(email);
            try
            {
                var mail = new MailMessage(sender, recipient)
                {
                    Subject = subject,
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
            ApplicationUser? u = _dataRepository.Users.FirstOrDefault(x => x.Email == email);
            if(u != null)
            {
                await _dataRepository.SaveAdminNote(new AdminNote(u.Id,email, "Sent Email: " + subject));
            }
            return Task.CompletedTask;
        }
        public Task SendTextAsync(string phone, string message)
        {
            try
            {

                var accountSid = Configuration["TextSettings:twilsid"];
                var authToken = Configuration["TextSettings:twiltok"];
                TwilioClient.Init(accountSid, authToken);
                var messageOptions = new CreateMessageOptions(new PhoneNumber("+1" + phone))
                {
                    From = new PhoneNumber("+12152615226"),
                    Body = message
                };
                var mess = MessageResource.Create(messageOptions);

            }
            catch
            {

                Task.FromResult(0);
            }
            return Task.CompletedTask;
        }
        public async Task<string> UploadPhotoToCloud(string fileName, Stream stream)
        {
            byte[] buffer = new byte[512];
            stream.Read(buffer, 0, 512);
            string content = Encoding.UTF8.GetString(buffer);
            if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
            {
                return "Invalid Photo";
            }
            try
            {
                using var bitmap = new Bitmap(stream);
            }
            catch (Exception)
            {
                return "Invalid Photo";
            }
            stream.Position = 0;
            BlobContainerClient containerClient = await _blobService.GetBlobContainer();
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(stream, overwrite: true);
            return blobClient.Uri.AbsoluteUri.ToString().Replace("jpeg", "jpg");
        }
        public async Task<string> UploadPhotoStreamToCloud(byte[] incomingbytes, string fileName)
        {
            int ImageMinimumBytes = 512;
            if (incomingbytes.Length < ImageMinimumBytes)
            {
                return "Invalid Photo";
            }
            byte[] buffer = new byte[512];
            Stream stream = new MemoryStream(incomingbytes);
            stream.Read(buffer, 0, 512);
            string content = System.Text.Encoding.UTF8.GetString(buffer);
            if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
            {
                return "Invalid Photo";
            }
            try
            {
                using var bitmap = new Bitmap(stream);
            }
            catch (Exception)
            {
                return "Invalid Photo";
            }
            BlobContainerClient containerClient = await _blobService.GetBlobContainer();
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            MemoryStream ms = new(incomingbytes);
            //Image img = Image.FromStream(ms);
            //await blobClient.UploadAsync(RotateImage(img));
            
            await blobClient.UploadAsync(ms,overwrite:true);
            return blobClient.Uri.AbsoluteUri.ToString().Replace("jpeg", "jpg");
        }
        public async Task<string> BackUpToCloud(byte[] incomingbytes, string fileName)
        {
            BlobContainerClient containerClient = await _blobService.GetBlobContainer("backup");
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
            MemoryStream ms = new(incomingbytes);

            await blobClient.UploadAsync(ms);
            return "Success";
        }
        public async Task<ApplicationUser> DeleteAccount(Guid accountId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(accountId.ToString());
            if(user is not null)
            {
                List<AtliceTap> taps = _dataRepository.Taps.Where(x => x.UserId == accountId).ToList();
                List<ContactPage> pages = _dataRepository.ContactPages.Where(x => x.UserId == accountId).ToList();
                List<ContactList> contactLists = _dataRepository.ContactLists.Where(x => x.UserId == accountId).ToList();
                YouLoveProfile? ylp = _dataRepository.YouLoveProfiles.FirstOrDefault(x => x.Id == user.YouLoveProfileId);
                List<Order>? orders = _dataRepository.Orders.Where(x => x.UserId == user.Id).ToList();
                RewardTracker? rewardTracker = _dataRepository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                Passport? passport = _dataRepository.Passports.FirstOrDefault(x=>x.UserId== user.Id);
                BackupData bd = new()
                {
                    ApplicationUser = user,
                    AtliceTaps = taps,
                    ContactPages = pages,
                    ContactLists = contactLists,
                    YouLoveProfile = ylp,
                    Orders = orders,
                    RewardTracker = rewardTracker,
                    Passport = passport
                };

                string userJsonString = JsonSerializer.Serialize(bd);
                string result = await BackUpToCloud(Encoding.ASCII.GetBytes(userJsonString), user.PhoneNumber + ".json");

                foreach (var tap in taps)
                {
                    if (tap.TapType == TapType.Virtual)
                    {
                        _dbContext.AtliceTap.Remove(tap);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        tap.UserId = null;
                        tap.CustomName = null;
                        tap.ForwardUrl = null;
                        tap.Note = tap.Note + "Account Deleted on " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ";
                        tap.LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
                        tap.Hits = 0;
                        tap.Bypass = false;
                        tap.BypassURL = null;
                        tap.Locked = true;
                        await _dataRepository.SaveTap(tap);
                    }

                }
                foreach (var page in pages)
                {
                    foreach (var link in page.TapLinks)
                    {
                        _dbContext.TapLink.Remove(link);
                    }
                    _dbContext.ContactPage.Remove(page);
                    await _dbContext.SaveChangesAsync();
                }
                foreach (var list in contactLists)
                {
                    foreach (var contact in list.Contacts)
                    {
                        _dbContext.Contact.Remove(contact);
                    }
                    _dbContext.ContactList.Remove(list);
                    await _dbContext.SaveChangesAsync();
                }
                if (ylp is not null)
                {
                    _dbContext.YouLoveProfile.Remove(ylp);
                }
                if (rewardTracker is not null)
                {
                    _dbContext.RewardTracker.Remove(rewardTracker);
                }
                if(passport is not null)
                {
                    _dbContext.Passport.Remove(passport);
                }
                await _dbContext.SaveChangesAsync();
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Count > 0)
                {
                    await _userManager.RemoveFromRolesAsync(user, roles);
                }
                await _userManager.AddToRoleAsync(user, "Deleted");
                var newroles = await _userManager.GetRolesAsync(user);
                await _userManager.UpdateAsync(user);
                return user;
            }
            return new ApplicationUser();
        }
        public async Task<ApplicationUser> CreateAtliceAccount(ApplicationUser user, string role)
        {
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                YouLoveProfile ylp = await _dataRepository.SaveYouLoveProfile(new YouLoveProfile { Id = Guid.NewGuid(), StateofMind = "Relaxed", SocialMediaUse = "Personal" });
                RewardTracker rt = await _dataRepository.SaveRewardTracker(new RewardTracker { Id = Guid.NewGuid(), UserId = user.Id });

                user.YouLoveProfileId = ylp.Id;
                user.InviteCode = user.Id;
                await _userManager.UpdateAsync(user);

                Passport pp = await _dataRepository.SavePassport(new Passport(user.Id));
                if (role == "Lead")
                {
                    pp.Stamps.Add(await _dataRepository.SaveStamp(new Stamp("Lead Stamp", Platform.Atlice, pp.Id)));
                    await _dataRepository.SavePassport(pp);
                }
                if (role == "Prospect")
                {
                    pp.Stamps.Add(await _dataRepository.SaveStamp(new Stamp("Founder's Stamp", Platform.Atlice, pp.Id)));
                    await _dataRepository.SavePassport(pp);
                }
                if (role == "Tourist")
                {
                    pp.Stamps.Add(await _dataRepository.SaveStamp(new Stamp("Device Stamp", Platform.Atlice, pp.Id)));
                    pp.Stamps.Add(await _dataRepository.SaveStamp(new Stamp("Tourist Stamp", Platform.Atlice, pp.Id)));
                    await _dataRepository.SavePassport(pp);
                }
                if (role == "Citizen")
                {
                    pp.Stamps.Add(await _dataRepository.SaveStamp(new Stamp("Citizen's Stamp", Platform.Atlice, pp.Id)));
                    await _dataRepository.SavePassport(pp);
                }
                return user;
            }
            else
            {
                return new ApplicationUser();
            }
        }

        public async Task<ManagedAccount> LoadExsistingEtherWallet(string password)
        {
            var keyStoreEncryptedJson =
                         @"{""crypto"":{""cipher"":""aes-128-ctr"",""ciphertext"":""b4f42e48903879b16239cd5508bc5278e5d3e02307deccbec25b3f5638b85f91"",""cipherparams"":{""iv"":""dc3f37d304047997aa4ef85f044feb45""},""kdf"":""scrypt"",""mac"":""ada930e08702b89c852759bac80533bd71fc4c1ef502291e802232b74bd0081a"",""kdfparams"":{""n"":65536,""r"":1,""p"":8,""dklen"":32,""salt"":""2c39648840b3a59903352b20386f8c41d5146ab88627eaed7c0f2cc8d5d95bd4""}},""id"":""19883438-6d67-4ab8-84b9-76a846ce544b"",""address"":""12890d2cce102216644c59dae5baed380d84830c"",""version"":3}";
            var account = Account.LoadFromKeyStore(keyStoreEncryptedJson, password);
            var manAccount = new ManagedAccount(account.Address, password);
            return manAccount;
        }

        public async Task<ManagedAccount> CreateEtherWallet(string password)
        {
            Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            var account = new Wallet(mnemo.ToString(), password).GetAccount(0, 1);

            //var chainId = 444444444500;
            //int chainId = Chain.MainNet;
            Console.WriteLine("The account address is: " + account.Address);

            var web3 = new Web3(account);
            var balance = await web3.Eth.GetBalance.SendRequestAsync(account.Address);
            Console.WriteLine("The account balance is: " + balance.Value);
            var manAccount = new ManagedAccount(account.Address, password);
            return manAccount;

        }

        public async Task<ManagedAccount> RecoverAccount(string wordlist, string password)
        {
            string[] list = wordlist.Split(',');
            Wordlist w = new Wordlist(list,' ',"test");
            Mnemonic mnemo = new Mnemonic(w, WordCount.Twelve);
            
            var backupSeed = mnemo.ToString();
            var wallet3 = new Wallet(backupSeed, password);
            var recoveredAccount = wallet3.GetAccount(0);
            var manAccount = new ManagedAccount(recoveredAccount.Address, password);
            return manAccount;
        }

    }
}
