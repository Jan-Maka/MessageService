using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project.Server.Components;
using Project.Server.Data;
using Project.Server.MessageHub;
using Project.Server.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger
builder.Services.AddSwaggerGen();



// Configure database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add password hasher
builder.Services.AddSingleton<IPasswordHasher, Project.Server.Components.PasswordHasher>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", corsBuilder =>
    {
        corsBuilder.WithOrigins("https://localhost:60676") // Frontend URL
                   .AllowAnyMethod()                   // Allow all HTTP methods (GET, POST, etc.)
                   .AllowAnyHeader()                   // Allow any headers
                   .AllowCredentials();                // Allow cookies and credentials
    });
});

var jwtSecret = builder.Configuration["JWT_SECRET"];

if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JWT_SECRET environment variable is missing.");
}

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "Jan Industries",
            ValidAudience = "People",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "XSRF-TOKEN";    // Cookie name for CSRF token
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Secure only over HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict; // Prevent sending in cross-origin requests
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.HttpOnly = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/user/login";
    });


builder.Services.AddSignalR().AddHubOptions<MessageHub>(options =>
{
    options.EnableDetailedErrors = true;
});


builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IGroupChatService, GroupChatService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailVerificationService, EmailVerificationService>();
builder.Services.AddScoped<IVerificationStore, InMemoryVerificationStore>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Enable CORS
app.UseCors("AllowReactApp");
app.MapHub<MessageHub>("/chat/messages");
app.UseAntiforgery();


// Serve static files (React frontend)
app.UseDefaultFiles();
app.UseStaticFiles();

// Configure routing
app.UseAuthentication();
app.UseRouting();
app.UseMiddleware<CSRFMiddleware>();


app.UseAuthorization();

// Map controllers (API endpoints)
app.MapControllers();


// Fallback for React SPA
app.MapFallbackToFile("index.html");

app.Run();
