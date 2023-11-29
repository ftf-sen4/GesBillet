using System.Security.Claims;
using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Gestions_des_Titres_de_Transport.Services
{
    public class UtilisateurService: IUtilisateurService
    {
        private readonly SAPCENTERDBContext _sapCenterDBContext;
        private readonly TitreDeTransportContext _titreDeTransportContext;
		private readonly AuthenticationStateProvider _authenticationStateProvider;
		
        public UtilisateurService(TitreDeTransportContext titreDeTransportContext, 
            AuthenticationStateProvider authenticationStateProvider,
			SAPCENTERDBContext sapCenterDBContext)
        {
            _sapCenterDBContext = sapCenterDBContext;
            _titreDeTransportContext = titreDeTransportContext;
			_authenticationStateProvider = authenticationStateProvider;
		}

		public async Task<int> ObtenirMatriculeUtilisateurConnecte()
		{
			try
			{
				var EtatAuthentification = await _authenticationStateProvider.GetAuthenticationStateAsync();
				var Utilisateur = EtatAuthentification.User;
				var Revendication = Utilisateur.FindFirst(claim => claim.Type == ClaimTypes.Sid);

				if (Revendication == null)
				{
					throw new Exception("Revendication non trouvée.");
				}

				var MatriculeUtilisateur = int.Parse(Revendication.Value);
				if (MatriculeUtilisateur == 0)
				{
					throw new Exception("Impossible de récupérer le matricule de l'utilisateur.");
				}

				return MatriculeUtilisateur;
			}
			catch (Exception)
			{
				throw;
			}
		}
		public async Task<DetailsUtilisateur> ObtenirDetailsUtilisateur(string IdentifiantAgent)
        {
            try
            {
                var DetailsAgent = new DetailsUtilisateur();
                var Agent = await _sapCenterDBContext.Agentssap.SingleAsync(agent => EF.Functions.Like(agent.Mail, $"%{IdentifiantAgent}%"));
                if(Agent is null)
                {
                    throw new Exception("L'utilisateur n'a pas été trouvée.");
                }
				Console.Write(Agent.NomEtPrenoms);

				DetailsAgent.Mail = Agent.Mail;
				DetailsAgent.Poste = Agent.Poste;
                DetailsAgent.Matricule = Agent.Matricule;
                DetailsAgent.Departement = Agent.Libfille;
				DetailsAgent.Domaine = Agent.Libelleparent;
                DetailsAgent.NomEtPrenoms = Agent.NomEtPrenoms;
                return DetailsAgent;
            }
            catch (Exception)
            {
				throw;
			}
        }
		public async Task<List<DetailsAutorisationRole>> ObtenirAutorisationsRole(int MatriculeUtilisateur)
		{
			try
			{
				var AutorisationRoles = await (from autorole in _titreDeTransportContext.Autorisationrole
											   join role in _titreDeTransportContext.Roleutilisateur on autorole.Idroleutilisateur equals role.Idroleutilisateur
											   where autorole.Matagentroleauto == MatriculeUtilisateur
											   select new DetailsAutorisationRole
											   {
												   IDRole = autorole.Idroleauto,
												   SlugRole = role.Slugroleutilisateur,
												   LibelleRole = role.Libelleroleutilisateur
											   }).ToListAsync();
				return AutorisationRoles;
			}
			catch (Exception)
			{
				throw;
			}
		}
		public async Task<List<DetailsAutorisationEtape>> ObtenirAutorisationsEtapeUtilisateur(int MatriculeUtilisateur)
		{
            try
            {
				var AutorisationEtapes = await (from autoetape in _titreDeTransportContext.Autorisationetape
                          join etape in _titreDeTransportContext.Etape on autoetape.Idetape equals etape.Idetape
                          where autoetape.Matagentautoetape == MatriculeUtilisateur
                          select new DetailsAutorisationEtape
                          {
                              IDEtape = autoetape.Idetape,
                              LibelleEtape = etape.Libelleetape,
                              NumeroEtape = etape.Idetape
                          }).ToListAsync();

                return AutorisationEtapes;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
