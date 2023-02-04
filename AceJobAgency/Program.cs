using AceJobAgency.Helpers;
using AceJobAgency.Infra.Contexts;
using AceJobAgency.Infra.Entities.Identity;
using AceJobAgency.Infra.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AspNetCore.ReCaptcha;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("Identity");

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<AgencyContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders()
                ;

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
    options.Password.RequireLowercase = false;
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); //set time here 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization(config =>
{
    config.AddPolicy("Admin",
        policy => policy.RequireRole("Admin"));
    config.AddPolicy("Applicant",
        policy => policy.RequireRole("Applicant"));
});

builder.Services.AddSingleton<FileHelper>();
builder.Services.AddRazorPages();

builder.Services.AddAntiforgery(options =>
{
    // Set Cookie properties using CookieBuilder properties†.
    options.FormFieldName = "acingsec";
    options.HeaderName = "X-CSRF-TOKEN-ACINGSEC";
    options.SuppressXFrameOptionsHeader = false;
});

builder.Services.AddReCaptcha(builder.Configuration.GetSection("ReCaptcha"));

ServiceExtension.AddServices(builder.Services);

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Error?xcode={0}";
    options.SlidingExpiration = true;
});

builder.Services.AddAuthentication()
   .AddGoogle(options =>
   {
       IConfigurationSection googleAuthNSection =
       builder.Configuration.GetSection("Google");
       options.ClientId = googleAuthNSection["ClientId"];
       options.ClientSecret = googleAuthNSection["ClientSecret"];
   });
   //.AddFacebook(options =>
   //{
   //    IConfigurationSection FBAuthNSection =
   //    config.GetSection("Authentication:FB");
   //    options.ClientId = FBAuthNSection["ClientId"];
   //    options.ClientSecret = FBAuthNSection["ClientSecret"];
   //})
   //.AddMicrosoftAccount(microsoftOptions =>
   //{
   //    microsoftOptions.ClientId = config["Authentication:Microsoft:ClientId"];
   //    microsoftOptions.ClientSecret = config["Authentication:Microsoft:ClientSecret"];
   //})
   //.AddTwitter(twitterOptions =>
   //{
   //    twitterOptions.ConsumerKey = config["Authentication:Twitter:ConsumerAPIKey"];
   //    twitterOptions.ConsumerSecret = config["Authentication:Twitter:ConsumerSecret"];
   //    twitterOptions.RetrieveUserDetails = true;
   //});

var app = builder.Build();
app.UseSession();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseStatusCodePagesWithRedirects("/error?xcode={0}");
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
