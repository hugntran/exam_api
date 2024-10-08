using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DMAWS_T2305M_TranHung.Data;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy access
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        //policy.WithOrigins("http://localhost:3000");
        policy.AllowAnyOrigin();
        //policy.WithMethods("POST");
        policy.AllowAnyMethod();
        policy.AllowAnyHeader();
    });
});

// add AUTH JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        options =>
        {
            string key = builder.Configuration["JWT:Key"];
            string issuerX = builder.Configuration["JWT:Issuer"];
            string audienceX = builder.Configuration["JWT:Audience"];
            int lifeTime = Convert.ToInt32(builder.Configuration["JWT:Lifetime"]);

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = issuerX,
                ValidAudience = audienceX,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        }
    );

// connect db
T2305mApiContext.ConnectionString = builder.Configuration.GetConnectionString("T2305M_API");
builder.Services.AddDbContext<T2305mApiContext>(
    options => options.UseSqlServer(T2305mApiContext.ConnectionString)
    );
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

