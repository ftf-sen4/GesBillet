using Gestions_des_Titres_de_Transport.Models;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface IValidationBonService
	{
		public Task EnvoyerBonEtapeSup(int IDBon);
		public Task EnvoyerBonEtapeSup(List<int> ListeIDBon);
		public Task ValiderBon(int IDBon);
		public Task ValiderBon(List<int> ListeIDBon);
		public Task RenvoyerBon(int IDBon, string Motif);
		public Task<List<AffichageBon>> ObtenirListeBon(ValidationBonRechercheModel ModelDeRecherche);
		public Task<AffichageBonAvecTitresLier> ObtenirDetailsBon(int IDBon);
		public Task<ImprimerBonModel> ObtenirModelBonAImprimer(int IDBon);
	}
}
