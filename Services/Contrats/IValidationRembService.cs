using Gestions_des_Titres_de_Transport.Models;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface IValidationRembService
	{
		public Task EnvoiEtapeSup(int IDRemb);
		public Task EnvoiEtapeSup(List<int> ListeIDRemb);
		public Task ValiderRemb(int IDRemb);
		public Task ValiderRemb(List<int> ListeIDRemb);
		public Task RenvoyerRemb(int IDRemb);
		public Task<List<AffichageRemboursement>> ObtenirListeRemb(SaisieRembRechercheModel ModelDeRecherche);
	}
}
