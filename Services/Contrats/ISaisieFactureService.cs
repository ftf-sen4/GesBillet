using Gestions_des_Titres_de_Transport.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface ISaisieFactureService
	{
		public Task CreerFacture(List<IBrowserFile> FichierJoints, CreerFactureModel Model);
		public Task ModifierFacture(int IDFacture, List<IBrowserFile> ListeNouveauFichiersJoints, CreerFactureModel Model);
		public Task<List<AffichageFacture>> ObtenirListeFacture(SaisieFactureRechercheModel ModelDeRecherche);
		public Task SupprimerPieceJointe(string NomPieceJointe);
		public Task SupprimerFacture(int IDFacture);
		public Task<AffichageRenvoi> ObtenirDetailsRenvoi(int IDRenvoi);
		public Task EnvoyerFactureEtapeSup(int IDFacture);
		public Task EnvoyerFactureEtapeSup(List<int> ListeIDFactures);
		public Task<AffichageFactureAvecPJ> ObtenirDetailsFacture(int IDFacture);
		public Task<List<AffichageTitreAvecPJ>> ObtenirTitresAChoisir(int IDAgenceVoyage);
		public Task<List<AffichageTitreAvecPJ>> ObtenirTitresAChoisirModifier(int IDAgenceVoyage);
	}
}
