using Gestions_des_Titres_de_Transport.Models;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface IGestionCompagnieService
	{
		public Task CreerCompagnie(string LibelleCompagnie);
		public Task SupprimerCompagnie(int IDCompagnie);
		public Task ModifierCompagnie(int IDCompagnie, string NouveauLibelle);
		public Task<List<AffichageCompagnie>> ObtenirListeCompagnie(RechercheCompagnieModel ModelDeRecherche);
		public Task<AffichageCompagnie> ObtenirDetailsCompagnie(int IDCompagnie);
	}
}
