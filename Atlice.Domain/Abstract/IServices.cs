using Atlice.Domain.Entities;
using Nethereum.Web3.Accounts.Managed;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlice.Domain.Abstract
{
    public interface IServices
    {
        Task<string> RenderToString(string viewName, object model);
        byte[] BitmapToBytesCode(Bitmap image);
        Bitmap GenerateQR(int width, int height, string text);
        Task<Task> SendEmailAsync(string email, string subject, string message);
        Task SendTextAsync(string phone, string message);
        Task<string> UploadPhotoStreamToCloud(byte[] incomingbytes, string fileName);
        Task<string> UploadPhotoToCloud(string fileName, Stream stream);
        Task<string> BackUpToCloud(byte[] incomingbytes, string fileName);
        Task<ApplicationUser> DeleteAccount(Guid accountId);
        Task<ApplicationUser> CreateAtliceAccount(ApplicationUser user, string role);
        Task<ManagedAccount> LoadExsistingEtherWallet(string password);

    }
}
