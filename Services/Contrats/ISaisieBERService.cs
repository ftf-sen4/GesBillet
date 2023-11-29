using Gestions_des_Titres_de_Transport.Models;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface ISaisieBERService
	{
		public Task CreerBEReglement(CreerBEReglementModel Model);
		public Task CreerBEReglementV2(CreerBEReglementModelV2 Model, List<BERFactureModel> ListeFactures);
		public Task ModifierBER(int IDBEReglement, CreerBEReglementModel Model);
		public Task ModifierBEReglementV2(int IDBEReglement, CreerBEReglementModelV2 Model, List<BERFactureModel> ListeFactures);
		public Task SupprimerBER(int IDBEReglement);
		public Task SupprimerBERV2(int IDBEReglement);
		public Task EnvoyerBEREtapeSup(int IDBEReglement);
		public Task EnvoyerBEREtapeSup(List<int> ListeIDBEReglement);
		public Task<List<AffichageBER>> ObtenirListeBER(SaisieBERRechercheModel ModelDeRecherche);
		public Task<AffichageRenvoi> ObtenirDetailsRenvoi(int IDRenvoi);
		public Task<AffichageBERAvecFacturesLier> ObtenirDetailsBER(int IDBEReglement);
		public Task<BERDetails> ObtenirDetailsBERV2(int IDBEReglement);
		public Task<List<AffichageFactureAvecPJ>> ObtenirFacturesAChoisir(int IDAgenceVoyage);
		public Task<List<AffichageFactureAvecPJ>> ObtenirFacturesAChoisirModifier(int IDAgenceVoyage);
		public Task<ImprimerBERModel> ObtenirModelBERAImprimer(int IDBEReglement);
		public Task<List<AffichageAgentAutoCompletion>> ObtenirAgent();
		public Task<List<AffichageTitreAutoCompletion>> ObtenirTitreAutoCompletion(int IDAgenceVoyage);
		public Task<List<AffichageTitreAutoCompletion>> ObtenirTitreAutoCompletionModifier(int IDBEReglement);
	}
}
