using Gestions_des_Titres_de_Transport.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface ISaisieTitreService
	{
		public Task CreerTitre(List<IBrowserFile> ListeDeFichiersJoints, CreerTitreDeTransportModel TitreModel, AffichageMission DetailsFicheDeMission);
		public Task ModifierTitre(int IDTitreDeTransport, List<IBrowserFile> ListeNouveauFichiersJoints, CreerTitreDeTransportModel TitreModel, AffichageMission DetailsFicheDeMission);
        public Task SupprimerTitre(int IDTitreDeTransport);
		public Task SupprimerPieceJointeTitre(string NomPieceJointe);
		public Task SupprimerTitre(List<int> ListIDTitreDeTransport);
		public Task EnvoyerTitreEtapeSup(int IDTitreDeTransport);
		public Task EnvoyerTitreEtapeSup(List<int> ListeIDTitresDeTransport);
		public Task<List<AffichageTitre>> ObtenirListeTitre(SaisieTitreRechercheModel ModelDeRecherche);
		public Task<AffichageRenvoi> ObtenirDetailsRenvoi(int IDRenvoi);
		public Task<AffichageTitreAvecPJ> ObtenirDetailsTitre(int IDTitreDeTransport);
		public Task CreerPlusieursTitres(string ListeRefFicheMission);
	}
}
