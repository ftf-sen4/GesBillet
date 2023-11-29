using Gestions_des_Titres_de_Transport.Models;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface IGestionAgenceService
	{
		public Task CreerAgence(string LibelleAgence);
		public Task SupprimerAgence(int IDAgence);
		public Task ModifierAgence(int IDAgence, string NouveauLibelle);
		public Task<List<AffichageAgence>> ObtenirListeAgence(RechercheAgenceModel ModelDeRecherche);
		public Task<AffichageAgence> ObtenirDetailsAgence(int IDAgence);
	}

}
