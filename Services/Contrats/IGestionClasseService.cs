using Gestions_des_Titres_de_Transport.Models;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface IGestionClasseService
	{
		public Task CreerClasse(string LibelleClasse);
		public Task SupprimerClasse(int IDClasse);
		public Task ModifierClasse(int IDClasse, string NouveauLibelle);
		public Task<List<AffichageClasse>> ObtenirListeClasse(RechercheClasseModel ModelDeRecherche);
		public Task<AffichageClasse> ObtenirDetailsClasse(int IDClasse);
	}
}
