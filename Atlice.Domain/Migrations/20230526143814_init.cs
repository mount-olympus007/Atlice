using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlice.Domain.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Who = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    What = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    When = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminNote", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConnectedUsers",
                columns: table => new
                {
                    ConnectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Admin = table.Column<bool>(type: "bit", nullable: false),
                    Mobile = table.Column<bool>(type: "bit", nullable: false),
                    DateConnected = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectedUsers", x => x.ConnectionId);
                });

            migrationBuilder.CreateTable(
                name: "ContactList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactPage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Grid = table.Column<bool>(type: "bit", nullable: false),
                    GridPreview = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileLead = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageType = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhonePreview = table.Column<bool>(type: "bit", nullable: false),
                    PhonePublished = table.Column<bool>(type: "bit", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailPreview = table.Column<bool>(type: "bit", nullable: false),
                    EmailPublished = table.Column<bool>(type: "bit", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebsitePreview = table.Column<bool>(type: "bit", nullable: false),
                    WebsitePublished = table.Column<bool>(type: "bit", nullable: false),
                    SmsAlerts = table.Column<bool>(type: "bit", nullable: false),
                    SmsPreview = table.Column<bool>(type: "bit", nullable: false),
                    SubmitContact = table.Column<bool>(type: "bit", nullable: false),
                    SubConpreview = table.Column<bool>(type: "bit", nullable: false),
                    NoteToSelf = table.Column<bool>(type: "bit", nullable: false),
                    NotetoSelfPreview = table.Column<bool>(type: "bit", nullable: false),
                    SaveToContacts = table.Column<bool>(type: "bit", nullable: false),
                    SaveToContactsPreview = table.Column<bool>(type: "bit", nullable: false),
                    VName = table.Column<bool>(type: "bit", nullable: false),
                    VLead = table.Column<bool>(type: "bit", nullable: false),
                    VPhone = table.Column<bool>(type: "bit", nullable: false),
                    VPhonePreview = table.Column<bool>(type: "bit", nullable: false),
                    VEmail = table.Column<bool>(type: "bit", nullable: false),
                    VEmailPreview = table.Column<bool>(type: "bit", nullable: false),
                    VWebsite = table.Column<bool>(type: "bit", nullable: false),
                    VWebsitePreview = table.Column<bool>(type: "bit", nullable: false),
                    ProImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationPreview = table.Column<bool>(type: "bit", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationPublished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ErrorFeedback",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorFeedback", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Who = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    What = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    MethodName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasError = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gift",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    To = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    From = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GiftType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gift", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitude = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Longitude = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GoogleID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WebflowStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipAddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipAddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Last4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChargeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameOnCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerPaid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tracking = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderRecieved = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderShipped = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderDelivered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookMarked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Passport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passport", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RewardTracker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Credentials = table.Column<bool>(type: "bit", nullable: false),
                    EligibilityForm = table.Column<bool>(type: "bit", nullable: false),
                    PlacedOrder = table.Column<bool>(type: "bit", nullable: false),
                    OnboardingStep2 = table.Column<bool>(type: "bit", nullable: false),
                    VerifyStep = table.Column<bool>(type: "bit", nullable: false),
                    Terms = table.Column<bool>(type: "bit", nullable: false),
                    DeviceSelect = table.Column<bool>(type: "bit", nullable: false),
                    SetupContactPage = table.Column<bool>(type: "bit", nullable: false),
                    OnboardingStep7 = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardTracker", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "State",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lng = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormsOfId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VotersWithoutId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YouLoveProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StateofMind = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entrepreneur = table.Column<bool>(type: "bit", nullable: false),
                    Creator = table.Column<bool>(type: "bit", nullable: false),
                    BrandOwner = table.Column<bool>(type: "bit", nullable: false),
                    TeamLeader = table.Column<bool>(type: "bit", nullable: false),
                    IndependentContractor = table.Column<bool>(type: "bit", nullable: false),
                    Employee = table.Column<bool>(type: "bit", nullable: false),
                    Cards = table.Column<bool>(type: "bit", nullable: false),
                    Payments = table.Column<bool>(type: "bit", nullable: false),
                    Paypal = table.Column<bool>(type: "bit", nullable: false),
                    Cashapp = table.Column<bool>(type: "bit", nullable: false),
                    Venmo = table.Column<bool>(type: "bit", nullable: false),
                    Stripe = table.Column<bool>(type: "bit", nullable: false),
                    Apple = table.Column<bool>(type: "bit", nullable: false),
                    Google = table.Column<bool>(type: "bit", nullable: false),
                    Other = table.Column<bool>(type: "bit", nullable: false),
                    SocialMedia = table.Column<bool>(type: "bit", nullable: false),
                    SocialMediaUse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessPage = table.Column<bool>(type: "bit", nullable: false),
                    Zoom = table.Column<bool>(type: "bit", nullable: false),
                    ContentCreator = table.Column<bool>(type: "bit", nullable: false),
                    Write = table.Column<bool>(type: "bit", nullable: false),
                    Video = table.Column<bool>(type: "bit", nullable: false),
                    Photographer = table.Column<bool>(type: "bit", nullable: false),
                    Design = table.Column<bool>(type: "bit", nullable: false),
                    Artwork = table.Column<bool>(type: "bit", nullable: false),
                    Music = table.Column<bool>(type: "bit", nullable: false),
                    Otherjob = table.Column<bool>(type: "bit", nullable: false),
                    ChainAssets = table.Column<bool>(type: "bit", nullable: false),
                    StoreAssets = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YouLoveProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageVisit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContactDownloaded = table.Column<bool>(type: "bit", nullable: false),
                    ContactPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Counter = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageVisit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageVisit_ContactPage_ContactPageId",
                        column: x => x.ContactPageId,
                        principalTable: "ContactPage",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TapLink",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkType = table.Column<int>(type: "int", nullable: false),
                    SocialProviderMainUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VCard = table.Column<bool>(type: "bit", nullable: false),
                    VPreview = table.Column<bool>(type: "bit", nullable: false),
                    ContactPage = table.Column<bool>(type: "bit", nullable: false),
                    ShowPreview = table.Column<bool>(type: "bit", nullable: false),
                    LinkClicks = table.Column<int>(type: "int", nullable: false),
                    SocialProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WalletAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TapLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TapLink_ContactPage_ContactPageId",
                        column: x => x.ContactPageId,
                        principalTable: "ContactPage",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SmsAlerts = table.Column<bool>(type: "bit", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberQuote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AboutMe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MyWebsite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMarried = table.Column<bool>(type: "bit", nullable: false),
                    HaveKids = table.Column<bool>(type: "bit", nullable: false),
                    Bookmarked = table.Column<bool>(type: "bit", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AstroSign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AffiliateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    YouLoveProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoverUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebflowPaymentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Secret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InviteCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TermsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Interests = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateMeet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkedPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApplicationUserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactListId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contact_ContactList_ContactListId",
                        column: x => x.ContactListId,
                        principalTable: "ContactList",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contact_ContactPage_LinkedPageId",
                        column: x => x.LinkedPageId,
                        principalTable: "ContactPage",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contact_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostType = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    EmbedCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SharedLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                    table.ForeignKey(
                        name: "FK_News_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VCards",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Business = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Linklist = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Lead = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VCards_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Stamp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PassportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stamp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stamp_Passport_PassportId",
                        column: x => x.PassportId,
                        principalTable: "Passport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Legislator",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Party = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tenure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Long = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Legislator", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Legislator_State_StateId",
                        column: x => x.StateId,
                        principalTable: "State",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LinkClick",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LinkClickType = table.Column<int>(type: "int", nullable: false),
                    PageVisitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkClick", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinkClick_PageVisit_PageVisitId",
                        column: x => x.PageVisitId,
                        principalTable: "PageVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AtliceTap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TapType = table.Column<int>(type: "int", nullable: false),
                    Sku = table.Column<int>(type: "int", nullable: false),
                    CustomName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ForwardUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastEdited = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hits = table.Column<int>(type: "int", nullable: false),
                    Bypass = table.Column<bool>(type: "bit", nullable: false),
                    BypassURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BypassSocialProvider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BypassImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Locked = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtliceTap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AtliceTap_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AtliceTap_ContactPage_ContactPageId",
                        column: x => x.ContactPageId,
                        principalTable: "ContactPage",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AtliceTap_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AtliceTap_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Badges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Badges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Badges_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatMessage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessage_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatMessage_Chat_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chat",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organization_AspNetUsers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Film",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUploaded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Film", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Film_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Photo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUploaded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Height = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Width = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photo_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LocationId",
                table: "AspNetUsers",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AtliceTap_ApplicationUserId",
                table: "AtliceTap",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AtliceTap_ContactPageId",
                table: "AtliceTap",
                column: "ContactPageId");

            migrationBuilder.CreateIndex(
                name: "IX_AtliceTap_LocationId",
                table: "AtliceTap",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AtliceTap_OrderId",
                table: "AtliceTap",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Badges_ApplicationUserId",
                table: "Badges",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ChatId",
                table: "ChatMessage",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_SenderId",
                table: "ChatMessage",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_ContactListId",
                table: "Contact",
                column: "ContactListId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_LinkedPageId",
                table: "Contact",
                column: "LinkedPageId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_LocationId",
                table: "Contact",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Film_NewsId",
                table: "Film",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_Legislator_StateId",
                table: "Legislator",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkClick_PageVisitId",
                table: "LinkClick",
                column: "PageVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_News_LocationId",
                table: "News",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ApplicationUserId",
                table: "Notifications",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_ManagerId",
                table: "Organization",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_PageVisit_ContactPageId",
                table: "PageVisit",
                column: "ContactPageId");

            migrationBuilder.CreateIndex(
                name: "IX_Photo_NewsId",
                table: "Photo",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_Stamp_PassportId",
                table: "Stamp",
                column: "PassportId");

            migrationBuilder.CreateIndex(
                name: "IX_TapLink_ContactPageId",
                table: "TapLink",
                column: "ContactPageId");

            migrationBuilder.CreateIndex(
                name: "IX_VCards_LocationId",
                table: "VCards",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminNote");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AtliceTap");

            migrationBuilder.DropTable(
                name: "Badges");

            migrationBuilder.DropTable(
                name: "ChatMessage");

            migrationBuilder.DropTable(
                name: "ConnectedUsers");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "ErrorFeedback");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Film");

            migrationBuilder.DropTable(
                name: "Gift");

            migrationBuilder.DropTable(
                name: "Legislator");

            migrationBuilder.DropTable(
                name: "LinkClick");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "Photo");

            migrationBuilder.DropTable(
                name: "RewardTracker");

            migrationBuilder.DropTable(
                name: "Stamp");

            migrationBuilder.DropTable(
                name: "TapLink");

            migrationBuilder.DropTable(
                name: "VCards");

            migrationBuilder.DropTable(
                name: "YouLoveProfile");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Chat");

            migrationBuilder.DropTable(
                name: "ContactList");

            migrationBuilder.DropTable(
                name: "State");

            migrationBuilder.DropTable(
                name: "PageVisit");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "Passport");

            migrationBuilder.DropTable(
                name: "ContactPage");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
