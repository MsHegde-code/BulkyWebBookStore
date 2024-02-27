using BulkyWeb.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options=>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




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

app.UseAuthorization();

// defining the default route settings of the controller, tells where the request needs to be routed 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// ex: https:// {domain:portno} / controller / action / id (optional)  -> this is the default url pattern
// if no action is present in the url, the default action (here index) is considered i.e ..localhost:5050/Catogory -> here the action 'index' is set by default
// if nothing is given in the url after the domain, by default the controller and action will be considered 

app.Run();
