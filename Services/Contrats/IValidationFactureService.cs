using Gestions_des_Titres_de_Transport.Models;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface IValidationFactureService
	{
		public Task RenvoyerFacture(int IDFacture, string MotifDeRenvoi);
		public Task ValiderFacture(int IDFacture);
		public Task ValiderFacture(List<int> ListeIDFacture);
		public Task EnvoyerFactureEtapeSup(int IDFacture);
		public Task EnvoyerFactureEtapeSup(List<int> ListeIDFacture);
		public Task<AffichageFactureAvecPJ> ObtenirDetailsFacture(int IDFacture);
		public Task<List<AffichageFacture>> ObtenirListeFacture(ValidationFactureRechercheModel ModelDeRecherche);
	}
}
