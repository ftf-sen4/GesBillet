using Gestions_des_Titres_de_Transport.Models;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
    public interface IUtilisateurService
    {

        public Task<int> ObtenirMatriculeUtilisateurConnecte(); 
        public Task<DetailsUtilisateur> ObtenirDetailsUtilisateur(string IdentifiantAgent);
        public Task<List<DetailsAutorisationRole>> ObtenirAutorisationsRole(int MatriculeUtilisateur);
		public Task<List<DetailsAutorisationEtape>> ObtenirAutorisationsEtapeUtilisateur(int MatriculeUtilisateur);
	}
}
