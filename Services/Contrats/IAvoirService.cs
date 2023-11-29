//using Gestions_des_Titres_de_Transport.Models;
//using Microsoft.AspNetCore.Components.Forms;

//namespace Gestions_des_Titres_de_Transport.Services.Contrats
//{
//	public interface IAvoirService
//	{
//		public Task CreerAvoir(List<IBrowserFile> FichierJoints, CreerAvoirModel Model);
//		public Task MettreAJourAvoir(int IDAvoir, List<IBrowserFile> ListeNouveauFichiersJoints, CreerAvoirModel Model);
//		public Task SupprimerAvoir(int IDAvoir);
//		public Task SupprimerPieceJointe(string NomPieceJointe);
//		public Task ValiderSaisieAvoir(int IDAvoir);
//		public Task EnvoyerAvoirEtapeSuperieure(int IDAvoir);
//		public Task RenvoyerAvoirEtapePrecedente(int IDAvoir, string MotifDeRenvoi);
//		public Task<string> ObtenirMotifRenvoiAvoir(int IDRenvoi);
//		public Task<List<DetailsFacture>> ObtenirAvoirsSAISIE_AVOIR(RechercheModel ModelDeRecherche);
//		public Task<List<DetailsFacture>> ObtenirAvoirsVALIDATION_AVOIR(RechercheModel ModelDeRecherche);
//	}
//}
