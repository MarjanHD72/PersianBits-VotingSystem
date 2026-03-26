using Microsoft.EntityFrameworkCore;
using VotingSystem.Domain;

var builder = WebApplication.CreateBuilder(args);

// SQLite database
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=votingsystem.db"));

// Email service
var emailService = new EmailService
{
    SmtpHost = builder.Configuration["Email:SmtpHost"] ?? "smtp.gmail.com",
    SmtpPort = int.Parse(builder.Configuration["Email:SmtpPort"] ?? "587"),
    SmtpUser = builder.Configuration["Email:SmtpUser"] ?? "",
    SmtpPass = builder.Configuration["Email:SmtpPass"] ?? "",
    FromEmail = builder.Configuration["Email:FromEmail"] ?? "",
    AdminEmail = builder.Configuration["Email:AdminEmail"] ?? ""
};
builder.Services.AddSingleton(emailService);

// Cookie authentication
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", o =>
    {
        o.LoginPath = "/Auth/Login";
        o.AccessDeniedPath = "/Auth/Login";
        o.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddAuthorization();
builder.Services.AddRazorPages();

var app = builder.Build();

// Auto-create / migrate DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Create uploads folder
var uploadsPath = Path.Combine(app.Environment.WebRootPath, "uploads");
Directory.CreateDirectory(uploadsPath);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();

app.Run();
