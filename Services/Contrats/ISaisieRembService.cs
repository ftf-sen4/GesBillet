using Gestions_des_Titres_de_Transport.Models;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface ISaisieRembService
	{
		public Task CreerRemboursement(CreerRemboursementModel Model);
		public Task ModifierRemboursement(int IDRemb, CreerRemboursementModel Model);
		public Task SupprimerRemboursement(int IDRemb);
		public Task<AffichageRemboursement> ObtenirDetailsRemb(int IDRemb);
		public Task<AffichageRenvoi> ObtenirDetailsRenvoi(int IDRenvoi);
		public Task EnvoiEtapeSup(int IDRemb);
		public Task EnvoiEtapeSup(List<int> ListeIDRemb);
		public Task<List<AffichageRemboursement>> ObtenirListeRemb(SaisieRembRechercheModel ModelDeRecherche);
		public Task<string> VerifierRefTitre(string RefTitre);
	}
}
