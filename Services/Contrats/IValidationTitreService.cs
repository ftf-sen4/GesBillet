using Gestions_des_Titres_de_Transport.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface IValidationTitreService
	{
		public Task EnvoyerTitreEtapeSup(int IDTitreDeTransport);
		public Task EnvoyerTitreEtapeSup(List<int> ListeIDTitresDeTransport);
		public Task<List<AffichageTitre>> ObtenirListeTitre(ValiderTitreRechercheModel ModelDeRecherche);
		public Task<AffichageRenvoi> ObtenirDetailsRenvoi(int IDRenvoi);
		public Task<AffichageTitreAvecPJ> ObtenirDetailsTitre(int IDTitreDeTransport);
		public Task RenvoyerTitre(int IDTitre, string Motif);
		public Task ValiderTitre(int IDTitre);
		public Task ValiderTitre(List<int> ListeIDTitre);
	}
}
