using System.DirectoryServices;
using System.Security.Claims;
using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.Services;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestions_des_Titres_de_Transport.Controllers
{

    public class LoginController : Controller
	{
		private readonly IUtilisateurService _utilisateurService;
		public LoginController(IUtilisateurService utilisateurService)
		{
			_utilisateurService = utilisateurService;
		}

		[HttpPost("/connexion")]
		public async Task<IActionResult> Login(ParamConnexUtilisateur paramConnexUtilisateur)
		{
            try
            {
				var MotDePasse = paramConnexUtilisateur.MotDePasse;
				var Identifiant = paramConnexUtilisateur.Identifiant;

				if (!ModelState.IsValid)
				{
					return LocalRedirect("/connexion");
				}

				//LDAP ACTIVE DIRECTORY AUTHENTICATION
				//DirectoryEntry entree = new DirectoryEntry("LDAP://192.168.0.1", username: $"BOADSIEGE\\{paramConnexUtilisateur.Identifiant}", password: paramConnexUtilisateur.MotDePasse);
				//DirectorySearcher recherche = new DirectorySearcher(entree);
				//SearchResult resultat = recherche.FindOne();

				//if (resultat == null)
				//{
				//                return LocalRedirect("/connexion");
				//            }

				var DetailsDeUtilisateur = await _utilisateurService.ObtenirDetailsUtilisateur(Identifiant);
				if (DetailsDeUtilisateur.NomEtPrenoms == string.Empty)
				{
                    return LocalRedirect("/connexion");
                }

				var Revendications = new List<Claim>();
                var AutorisationsDEtapes = await _utilisateurService.ObtenirAutorisationsEtapeUtilisateur(DetailsDeUtilisateur.Matricule!);

				Revendications.Add(new Claim(ClaimTypes.Email, DetailsDeUtilisateur.Mail));
				Revendications.Add(new Claim(ClaimTypes.Name, DetailsDeUtilisateur.NomEtPrenoms));
				Revendications.Add(new Claim(ClaimTypes.Sid, DetailsDeUtilisateur.Matricule.ToString()));

				foreach (var autorisation in AutorisationsDEtapes)
                {
					Revendications.Add(new Claim(ClaimTypes.Role, autorisation.NumeroEtape.ToString()));
                }


				var AutorisationsDeRoles = await _utilisateurService.ObtenirAutorisationsRole(DetailsDeUtilisateur.Matricule!);

				foreach (var autorisation in AutorisationsDeRoles)
				{
					Revendications.Add(new Claim(ClaimTypes.Role, autorisation.SlugRole));
				}

				var Identite = new ClaimsIdentity(Revendications, CookieAuthenticationDefaults.AuthenticationScheme);
				var Utilisateur = new ClaimsPrincipal(Identite);

				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, Utilisateur);
				return LocalRedirect("/");
			}
			catch (Exception)
            {
                return LocalRedirect("/connexion");
			}
		}

		[HttpGet("/deconnection")]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return LocalRedirect("/connexion");
		}
	}
}
