using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shippo;
using System.Collections;

namespace Atlice.WebUI.Pages.BetaAsk
{
    [Authorize(Roles = "Citizen,Tourist,Adminis")]

    public class OnboardingModel : PageModel
    {
        private readonly IDataRepository repository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IServices services;
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public OnboardingModel(UserManager<ApplicationUser> manager, IDataRepository dataRepository, IServices _services)
        {
            services = _services;
            userManager = manager;
            repository = dataRepository;
        }
        public async Task<IActionResult> OnGet()
        {
            
            ApplicationUser? user = await userManager.GetUserAsync(User);
            if(user is not null)
            {
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                if (r == null || r.Credentials == false || r.EligibilityForm == false)
                {
                    return RedirectToPage("/BetaAsk/eligibility_form");
                }
                if (r.PlacedOrder == true)
                {
                    return RedirectToPage("/BetaAsk/opt_in_final");
                }
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAllAboard(string shipName, string add1, string add2, string city, string state, string zip, string GetTagSelection, string CardType, string FullName, string Company, string Role, byte[]? logourl)
        {
            ApplicationUser? user = await userManager.GetUserAsync(User);
            
            if (user != null && user.PhoneNumber is not null && user.Email is not null)
            {
                user.Taps = new List<AtliceTap>(user.Taps);
                Domain.Entities.Order order = new(user.FirstName + " " + user.LastName, shipName, add1, city, state, zip, user.PhoneNumber, user.Email, user.Id, user.Id.ToString())
                {
                    NameOnCard = FullName,
                    Company = Company,
                    Role = Role,
                    ShipAddressLine2 = add2
                };
                if (CardType == "Legacy")
                {
                    if (logourl == null)
                    {
                        order.LogoUrl = "../icons/LGYCSTLGCD1.png";
                    }
                    else
                    {
                        order.LogoUrl = await services.UploadPhotoStreamToCloud(logourl, user.Id + "logo.jpg");

                    }
                    order.Comments = "Name on Card: " + FullName + ", Company: " + Company + ", Role/Position: " + Role;
                    AtliceTap? card = repository.Taps.FirstOrDefault(x => x.UserId == null && x.Note == null && x.Sku == SKU.LGYCSTLGCD1);
                    if (card != null)
                    {
                        card.LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
                        card.Note = card.Note + "Assigned on: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ";
                        card = await repository.SaveTap(card);
                        order.Taps.Add(card);
                    }
                    else
                    {
                        card = new AtliceTap
                        {
                            UserId = null,
                            Locked = true,
                            TapType = TapType.Card,
                            Note = "AutoGenerated, No Inventory; Assigned on: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ",
                            Sku = SKU.LGYCSTLGCD1
                        };
                        card = await repository.SaveTap(card);
                        order.Taps.Add(card);

                    }

                }
                else
                {
                    AtliceTap? card = repository.Taps.FirstOrDefault(x => x.UserId == null && x.Note == null && x.Sku == SKU.CLSCDWHT && x.TapType == TapType.Card);
                    if (card != null)
                    {
                        card.LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
                        card.Note = card.Note + "Assigned on: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ";
                        card = await repository.SaveTap(card);
                        order.Taps.Add(card);
                    }
                    else
                    {
                        card = new AtliceTap
                        {
                            
                            Locked = true,
                            TapType = TapType.Card,
                            Note = "AutoGenerated, No Inventory; Assigned on: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ",
                            Sku = SKU.CLSCDWHT
                        };
                        card = await repository.SaveTap(card);
                        order.Taps.Add(card);
                    }

                }
                if (GetTagSelection == "Yes")
                {
                    AtliceTap? tag = repository.Taps.FirstOrDefault(x => x.UserId == null && x.Note == null && x.Sku == SKU.CLSTG25GRY1);
                    if (tag != null)
                    {
                        tag.Note = tag.Note + "Assigned on: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ";
                        tag.LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
                        tag = await repository.SaveTap(tag);
                        order.Taps.Add(tag);
                    }
                    else
                    {
                        tag = new AtliceTap
                        {
                            Locked = true,
                            TapType = TapType.Tag,
                            Note = "AutoGenerated, No Inventory; Assigned on: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ",
                            Sku = SKU.CLSTG25GRY1
                        };
                        order.Taps.Add(tag);
                    }
                }
                order = await repository.SaveOrder(order);

                

                await userManager.UpdateAsync(user);

                TempData["confirm"] = "Your order has been recieved!";

                await repository.SaveAdminNote(new AdminNote(user.Id, user.Email, "New Beta Ask Order; "));
               
                var em = await services.RenderToString("/pages/shared/emails/orderconfirmationemail.cshtml", order);

                if(order.Email is not null && order.Phone is not null)
                {
                    await services.SendEmailAsync(order.Email, "Atlice Tap Beta Ask Order Confirmation", em);
                    await services.SendTextAsync(order.Phone, "You have recieved a confirmation of your order. Please check your junk mail.");

                }
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                if (r != null)
                {
                    r.PlacedOrder = true;
                    await repository.SaveRewardTracker(r);
                    await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Placed an order, email and text sent", EventType.User, "Onboarding", false));


                    return RedirectToPage("/BetaAsk/opt_in_final");
                }
                
            }
            return Page();
        }

    }
}