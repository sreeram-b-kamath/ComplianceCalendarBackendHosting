using ComplianceCalendar.Data;
using ComplianceCalendar.Endpoints;
using ComplianceCalendar.MappingConfig;
using ComplianceCalendar.Repositories;
using ComplianceCalendar.Repository;
using ComplianceCalendar.Repository.IRepository;
using ComplianceCalendar.Services;
using ComplianceCalendar.Services.EmailService;
using ComplianceCalendar.Services.EMailService;
using ComplianceCalendar.Services.Scheduler;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Serilog;



var builder = WebApplication.CreateBuilder(args);



// Configure Serilog
Log.Logger = new LoggerConfiguration()
 &nbsp;&nbsp; .ReadFrom.Configuration(builder.Configuration)
 &nbsp;&nbsp; .CreateLogger();



builder.Host.UseSerilog();



// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddTransient<IEmailSender, EmailSender>();



// Register repositories
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IFilingsRepository, FilingsRepository>();
builder.Services.AddScoped<IFilingSchedulerRepository, FilingSchedulerRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IEmailRepository, EMailRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUploadFilingsRepository, UploadFilingsRepository>();



// Register services
builder.Services.AddScoped<IPendingFilingsService, PendingFilingsService>();
builder.Services.AddScoped<IDocNotUploadedService, DocNotUploadedService>();
builder.Services.AddScoped<IFilingMailService, FilingMailService>();
builder.Services.AddScoped<IFilingsService, FilingsService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();



// Register jobs
builder.Services.AddTransient<FilingJob>();
builder.Services.AddTransient<UpdateFilingStatusJob>();
builder.Services.AddTransient<PendingFilingsJob>();
builder.Services.AddTransient<DocNotUploadedJob>();



// Load environment variables from .env file
DotNetEnv.Env.Load();



// Get email settings from .env file
var emailUsername = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
var emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
var emailHost = Environment.GetEnvironmentVariable("EMAIL_HOST");
var emailPort = 587; // Default port for SMTP



// Configure email settings using Options pattern
builder.Services.Configure<EmailSettings>(options =>
{
 &nbsp;&nbsp; options.SenderEmail = emailUsername;
 &nbsp;&nbsp; options.SenderPassword = emailPassword;
 &nbsp;&nbsp; options.SmtpServer = emailHost;
 &nbsp;&nbsp; options.SmtpPort = emailPort;
});



// Get connection string from .env file
var connectionString = Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION_STRING");



// Configure DbContexts
builder.Services.AddDbContext<APIContext>(options =>
{
 &nbsp;&nbsp; options.UseNpgsql(connectionString);
});



builder.Services.AddDbContext<AdminContext>(options =>
{
 &nbsp;&nbsp; options.UseNpgsql(connectionString);
 &nbsp;&nbsp; options.EnableSensitiveDataLogging();
 &nbsp;&nbsp; options.LogTo(Console.WriteLine, LogLevel.Information);
});



builder.Services.AddDbContext<ManageUserContext>(options =>
{
 &nbsp;&nbsp; options.UseNpgsql(connectionString);
 &nbsp;&nbsp; options.EnableSensitiveDataLogging();
 &nbsp;&nbsp; options.LogTo(Console.WriteLine, LogLevel.Information);
});



// Add Quartz services
builder.Services.AddSingleton<IJobFactory, SchedulerJobFactory>();
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();



// Define job schedules
builder.Services.AddSingleton(new JobSchedule(
 &nbsp;&nbsp; jobType: typeof(FilingJob),
 &nbsp;&nbsp; cronExpression: "0 30 16 * * ?"));



builder.Services.AddSingleton(new JobSchedule(
 &nbsp;&nbsp; jobType: typeof(UpdateFilingStatusJob),
 &nbsp;&nbsp; cronExpression: "0 00 00 * * ?"));



builder.Services.AddSingleton(new JobSchedule(
 &nbsp;&nbsp; jobType: typeof(PendingFilingsJob),
 &nbsp;&nbsp; cronExpression: "0 28 17 * * ?"));



builder.Services.AddSingleton(new JobSchedule(
 &nbsp;&nbsp; jobType: typeof(DocNotUploadedJob),
 &nbsp;&nbsp; cronExpression: "0 34 16 * * ?"));



builder.Services.AddHostedService<QuartzHostedService>();



// Configure CORS policy to allow requests from Vercel
builder.Services.AddCors(options =>
{
 &nbsp;&nbsp; options.AddPolicy("AllowSpecificOrigin",
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; builder => builder
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; .WithOrigins("https://compliance-calendar-hosting-kkph.vercel.app") // Replace with your actual Vercel URL
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; .AllowAnyMethod()
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; .AllowAnyHeader());
});



var app = builder.Build();



// Apply CORS policy
app.UseCors("AllowSpecificOrigin");



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
 &nbsp;&nbsp; app.UseSwagger();
 &nbsp;&nbsp; app.UseSwaggerUI();
}



// Map endpoints
app.MapDepartmentEndpoints();
app.MapFilingEndpoints();
app.MapAuthenticationEndpoints();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();



app.Run();
