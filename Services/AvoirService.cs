//using Gestions_des_Titres_de_Transport.Constant;
//using Gestions_des_Titres_de_Transport.Models;
//using Gestions_des_Titres_de_Transport.OrdreDeMissionModelsEF;
//using Gestions_des_Titres_de_Transport.Services.Contrats;
//using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
//using Microsoft.AspNetCore.Components.Forms;

//namespace Gestions_des_Titres_de_Transport.Services
//{
//	public class AvoirService : IAvoirService
//	{
//		private readonly IUtilisateurService _agentService;
//		private readonly IUploadFichierService _uploadFichierService;
//		private readonly OrdreDeMissionContext _ordreDeMissionContext;
//		private readonly TitreDeTransportContext _titreDeTransportContext;

//		public AvoirService(
//			TitreDeTransportContext titreDeTransportContext,
//			OrdreDeMissionContext ordreDeMissionContext,
//			IUploadFichierService uploadFichierService,
//			IUtilisateurService agentService)
//		{
//			_agentService = agentService;
//			_uploadFichierService = uploadFichierService;
//			_ordreDeMissionContext = ordreDeMissionContext;
//			_titreDeTransportContext = titreDeTransportContext;
//		}
//		public async Task CreerAvoir(List<IBrowserFile> FichierJoints, CreerAvoirModel Model)
//		{
//			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
//			try
//			{
//				var EtapeDeSaisie = _titreDeTransportContext.Etape.FirstOrDefault(item => item.Netape == ConstantesEtapes.SAISI_AVOIR);
//				if (EtapeDeSaisie is null)
//				{
//					throw new Exception("L' étape n'a pas été trouvée.");
//				}

//				var IDAgentSaisie = await _agentService.GetIDAgentConnecte();

//				var NouvelAvoir = new Avoir()
//				{
//					Idagentsaiavoir = IDAgentSaisie,
//					Datesaisiavoir = DateTime.Now,
//					Obseravoir = Model.Observation,
//					Idprestataireavoir = Model.IDPrestataire
//				};

//				var ListeDesTitresDeTransport = _titreDeTransportContext
//						.Titredetransport
//						.Where(titre => Model.ListeDesIDTitresChoisis.Contains(titre.Idtitre));

//				var ListeDesRemboursements  = _titreDeTransportContext
//						.Remboursement
//						.Where(remb => Model.ListeDesIDTitresChoisis.Contains((int)remb.Idtitre));

//				NouvelAvoir.Remboursement = ListeDesRemboursements.ToList();

//				_titreDeTransportContext.Avoir.Add(NouvelAvoir);
//				_titreDeTransportContext.SaveChanges();

//				List<Fichier> ListeDesFichiersEnreg = new List<Fichier>();
//				ListeDesFichiersEnreg = await _uploadFichierService.EnregistrerFichierSurServeur(FichierJoints);

//				List<Piecejointe> ListeDesPiecesJointes = new List<Piecejointe>();
//				foreach (var item in ListeDesFichiersEnreg)
//				{
//					ListeDesPiecesJointes.Add(new Piecejointe()
//					{
//						Idetapepiecej = EtapeDeSaisie.Idetape,
//						Idavoirpiecej = NouvelAvoir.Idetapeavoir,
//						Nompiecej = item.Nom
//					});
//				}

//				_titreDeTransportContext.Piecejointe.AddRange(ListeDesPiecesJointes);
//				_titreDeTransportContext.SaveChanges();
//				Transaction.Commit();
//			}
//			catch (Exception)
//			{
//				Transaction.Rollback();
//				throw;
//			}
//		}

//		public async Task MettreAJourAvoir(int IDAvoir, List<IBrowserFile> ListeNouveauFichiersJoints, CreerAvoirModel Model)
//		{
//			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
//			try
//			{
				
//				Transaction.Commit();
//			}
//			catch (Exception)
//			{
//				Transaction.Rollback();
//				throw;
//			}
//		}

//		public async Task EnvoyerAvoirEtapeSuperieure(int IDAvoir)
//		{
//			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
//			try
//			{

//				Transaction.Commit();
//			}
//			catch (Exception)
//			{
//				Transaction.Rollback();
//				throw;
//			}
//		}

//		public async Task<List<DetailsFacture>> ObtenirAvoirsSAISIE_AVOIR(RechercheModel ModelDeRecherche)
//		{
//			try
//			{
//				return new List<DetailsFacture>();
//			}
//			catch (Exception)
//			{
//				throw;
//			}
//		}

//		public async Task<List<DetailsFacture>> ObtenirAvoirsVALIDATION_AVOIR(RechercheModel ModelDeRecherche)
//		{
//			try
//			{
//				return new List<DetailsFacture>();
//			}
//			catch (Exception)
//			{
//				throw;
//			}
//		}

//		public async Task<string> ObtenirMotifRenvoiAvoir(int IDRenvoi)
//		{
//			try
//			{
//				return string.Empty;
//			}
//			catch (Exception)
//			{
//				throw;
//			}
//		}

//		public async Task RenvoyerAvoirEtapePrecedente(int IDAvoir, string MotifDeRenvoi)
//		{
//			try
//			{

//			}
//			catch (Exception)
//			{
//				throw;
//			}
//		}

//		public async Task SupprimerAvoir(int IDAvoir)
//		{
//			try
//			{

//			}
//			catch (Exception)
//			{
//				throw;
//			}
//		}

//		public async Task SupprimerPieceJointe(string NomPieceJointe)
//		{
//			try
//			{

//			}
//			catch (Exception)
//			{
//				throw;
//			}
//		}

//		public async Task ValiderSaisieAvoir(int IDAvoir)
//		{
//			try
//			{

//			}
//			catch (Exception)
//			{
//				throw;
//			}
//		}
//	}
//}
