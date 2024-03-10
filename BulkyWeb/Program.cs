using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options=>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//we removed the parameter as we dont want the verified account registration, hence anyone can register and login

//builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddRazorPages(); // added to get the implementation of the Identity razor pages of signup and ligin

// we need to register the service in 'dependency injection container' as we are asking it in the CategoryController
// the 1st parameter takes interface, and 2nd takes the impementation of the interface, 
//i.e when the categorycontroller asks for implementation of ICategoryCont it knows that it needs to provide CategoryRepository
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();


var app = builder.Build();

// Configure the HTTP request pipeline -> tells how request needs to be processed
if (!app.Environment.IsDevelopment()) // env denotes which env variable we are using in launchSettings, a custom env can also be used
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // configures all the static files under wwwroot, and it can be accessed

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// defining the default route settings of the controller, tells where the request needs to be routed 

app.MapRazorPages(); // to map the razor pages implementation as we are using MVC and the identity implementation is razor pages. We need to have that support
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");


// ex: https:// {domain:portno} / controller / action / id (optional)  -> this is the default url pattern
// if no action is present in the url, the default action (here index) is considered i.e ..localhost:5050/Catogory -> here the action 'index' is set by default
// if nothing is given in the url after the domain, by default the controller and action will be considered 

app.Run();
