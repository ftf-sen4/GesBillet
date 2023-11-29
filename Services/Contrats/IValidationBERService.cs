using Gestions_des_Titres_de_Transport.Models;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface IValidationBERService
	{
		public Task RenvoyerBER(int IDBEReglement, string MotifDeRenvoi);
		public Task ValiderBER(int IDBEReglement);
		public Task ValiderBER(List<int> ListeIDBEReglement);
		public Task<List<AffichageBER>> ObtenirListeBER(ValidationBERRechercheModel ModelDeRecherche);
		public Task<AffichageBERAvecFacturesLier> ObtenirDetailsBER(int IDBEReglement);
	}
}
