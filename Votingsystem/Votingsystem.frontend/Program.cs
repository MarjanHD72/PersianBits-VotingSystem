using Microsoft.EntityFrameworkCore;
using VotingSystem.Domain;
using Votingsystem.frontend;

var builder = WebApplication.CreateBuilder(args);

// SQLite database — connection string is read from appsettings.json ("ConnectionStrings:Default")
// so the database file path can be changed without recompiling.
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=votingsystem.db";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));

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

// Auto-create / migrate DB + seed demo data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    db.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS Notifications (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            UserId INTEGER NOT NULL,
            ElectionId INTEGER NOT NULL,
            IsRead INTEGER NOT NULL DEFAULT 0,
            CreatedAt TEXT NOT NULL,
            FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
            FOREIGN KEY (ElectionId) REFERENCES Elections(Id) ON DELETE CASCADE
        )
    ");
    await DemoSeeder.SeedAsync(db, app.Environment);
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
// ─── AI Chat Endpoint ───────────────────────────────────────────────
app.MapPost("/api/ai/chat", async (HttpContext context) =>
{
    var request = await context.Request.ReadFromJsonAsync<ChatRequest>();
    var history = request?.History;
   
    if (history == null || history.Count == 0)
    {
        history = new List<Message>
        {
            new Message { Role = "user", Content = "Hello" }
        };
    }
// Build conversation string
    var conversation = string.Join("\n", history.Select(m =>
        m.Role == "user" ? $"User: {m.Content}" : $"Assistant: {m.Content}"));

    var lastUserMessage = history.LastOrDefault(m => m.Role == "user")?.Content ?? "";

   var prompt = $@"You are a helpful assistant for the PersianBits Voting System.                                                                                                     
  Keep answers SHORT (2-3 sentences max).                                                                                                                                            
  ONLY answer questions about how this system works. Politely refuse anything unrelated.                                                                                             
                                                                                                                                                                                     
  Here is how the system works:                                                                                                                                                    
  - Admins register at /Auth/Register and get approved by the super-admin.                                                                                                           
  - Admins create elections/polls from their Dashboard. Poll types: Election (candidates), Rating Scale, Multiple Choice, Ranking, and Text Response.                                
  - Each poll gets a unique Session ID in XXXX-XXXX format (e.g. 7F2A-19KD).                                                                                                         
  - Admins share the Session ID with voters. Voters go to the homepage, enter the Session ID in the join box, and vote.                                                              
  - Admins can Start (activate), Stop (close), or Delete their polls from the Dashboard.                                                                                             
  - Users register at /Auth/Register, log in, and can vote in any running poll they have a Session ID for.                                                                           
  - Each user can only vote once per poll.                                                                                                                                           
  - After voting, users are redirected to a Thank You page. Results are visible to the admin in real time.                                                                           
  - The admin dashboard shows KPI stats: total elections, running count, votes today, total votes, total users, and completion rate — plus charts for vote trends and top elections. 

Conversation so far:
{conversation}


Latest question: {lastUserMessage}
Answer:";

    using var http = new HttpClient();
    http.Timeout = TimeSpan.FromSeconds(60);

    try
    {
        var ollamaResponse = await http.PostAsJsonAsync("http://localhost:11434/api/generate", new
        {
            model = "phi3",
            prompt = prompt,
            stream = false
        });

        if (!ollamaResponse.IsSuccessStatusCode)
            return Results.Problem("Ollama server error: " + ollamaResponse.StatusCode);

        var raw = await ollamaResponse.Content.ReadAsStringAsync();
        var json = System.Text.Json.JsonDocument.Parse(raw);
        var reply = json.RootElement.GetProperty("response").GetString();

        return Results.Ok(new { reply });
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem("Cannot connect to Ollama. Make sure it's running on port 11434. Error: " + ex.Message);
    }
}).RequireAuthorization();
app.Run();
public class ChatRequest
{
    public List<Message> History { get; set; } = new();
}

public class Message
{
    public string Role { get; set; } = "";
    public string Content { get; set; } = "";
}