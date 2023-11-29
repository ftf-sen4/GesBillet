using Gestions_des_Titres_de_Transport.Models;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface ISaisieBonService
	{
		public Task CreerBon(CreerBonDePassageModel Model);
		public Task ModifierBon(int IDBon, CreerBonDePassageModel Model);
		public Task SupprimerBon(int IDBon);
		public Task EnvoyerBonEtapeSup(int IDBon);
		public Task EnvoyerBonEtapeSup(List<int> ListIDBonsDePassage);
		public Task<List<AffichageBon>> ObtenirListeBon(SaisieBonRechercheModel ModelDeRecherche);
		public Task<AffichageRenvoi> ObtenirDetailsRenvoi(int IDRenvoi);
		public Task<List<AffichageTitreAvecPJ>> ObtenirListeTitreAChoisir(int IDAgenceVoyage);
		public Task<List<AffichageTitreAvecPJ>> ObtenirListeTitreAChoisirModifier(int IDBonDePassage);
		public Task<ImprimerBonModel> ObtenirModelBonAImprimer(int IDBon);
		public Task<AffichageBonAvecTitresLier> ObtenirDetailsBon(int IDBon);
	}
}
