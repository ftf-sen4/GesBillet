using Microsoft.AspNetCore.Authentication.Cookies;
using Syncfusion.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Gestions_des_Titres_de_Transport.Services;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.OrdreDeMissionDBModelsEF;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Gestions_des_Titres_de_Transport.Services.BonDePassage;
using Gestions_des_Titres_de_Transport.Services.Facture;
using Gestions_des_Titres_de_Transport.Services.Rembours;
using Gestions_des_Titres_de_Transport.Services.BER;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor();

// Add Database connections
builder.Services.AddDbContext<SAPCENTERDBContext>(
	   options => options.UseSqlServer(builder.Configuration.GetConnectionString("SAPCENTERDBConnexion")));

builder.Services.AddDbContext<TitreDeTransportContext>(
       options => options.UseSqlServer(builder.Configuration.GetConnectionString("TitreDeTransportDBConnexionLocal")));

builder.Services.AddDbContext<OrdreDeMissionContext>(
	   options => options.UseSqlServer(builder.Configuration.GetConnectionString("OrdreDeMissionDBConnexion")));


// Adding Services
builder.Services.AddScoped<INotificationUIService, NotificationUIService>();
builder.Services.AddTransient<IUtilisateurService, UtilisateurService>();
builder.Services.AddTransient<IAgenceDeVoyageService, AgenceDeVoyageService>();
builder.Services.AddTransient<ICompagnieService, CompagnieService>();
builder.Services.AddTransient<IFicheDeMissionService, FicheDeMissionService>();
builder.Services.AddTransient<IClasseVoyageService, ClasseVoyageService>();
builder.Services.AddTransient<IFichierService, FichierService>();
builder.Services.AddTransient<ISaisieTitreService, SaisieTitreService>();
builder.Services.AddTransient<IValidationTitreService, ValidationTitreService>();
builder.Services.AddTransient<ISaisieBonService, SaisieBonService>();
builder.Services.AddTransient<IValidationBonService, ValidationBonService>();
builder.Services.AddTransient<ISaisieFactureService, SaisieFactureService>();
builder.Services.AddTransient<IValidationFactureService, ValidationFactureService>();
builder.Services.AddTransient<ISaisieBERService, SaisieBERService>();
builder.Services.AddTransient<IValidationBERService, ValidationBERService>();
builder.Services.AddTransient<IGestionAgenceService, GestionAgenceService>();
builder.Services.AddTransient<IGestionCompagnieService, GestionCompagnieService>();
builder.Services.AddTransient<IGestionClasseService, GestionClasseService>();
builder.Services.AddTransient<ISaisieRembService, SaisieRembService>();
builder.Services.AddTransient<CreerPDFService>();
// Commit Ninja

//builder.Services.AddScoped<IAvoirService, AvoirService>();

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetValue<string>("SyncfusionKey"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapControllers();
app.MapFallbackToPage("/_Host");

app.Run();
