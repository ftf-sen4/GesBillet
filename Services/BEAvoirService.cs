//using Gestions_des_Titres_de_Transport.Constant;
//using Gestions_des_Titres_de_Transport.Models;
//using Gestions_des_Titres_de_Transport.OrdreDeMissionModelsEF;
//using Gestions_des_Titres_de_Transport.Services.Contrats;
//using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
//using Microsoft.EntityFrameworkCore;

//namespace Gestions_des_Titres_de_Transport.Services
//{
//	public class BEAvoirService: IBEAvoirService
//	{
//		private readonly IUtilisateurService _agentService;
//		private readonly IUploadFichierService _uploadFichierService;
//		private readonly OrdreDeMissionContext _ordreDeMissionContext;
//		private readonly TitreDeTransportContext _titreDeTransportContext;
//		public BEAvoirService(TitreDeTransportContext titreDeTransportContext,
//			OrdreDeMissionContext ordreDeMissionContext,
//			IUploadFichierService uploadFichierService,
//			IUtilisateurService agentService) {
//			_agentService = agentService;
//			_uploadFichierService = uploadFichierService;
//			_ordreDeMissionContext = ordreDeMissionContext;
//			_titreDeTransportContext = titreDeTransportContext;
//		}

//		public async Task CreerBEAvoir(CreerBEAvoirModel Model)
//		{
//			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
//			try
//			{
//				var NouveauBEAvoir = new Beavoir();
//				var IDAgentSaisie = await _agentService.GetIDAgentConnecte();
//				var EtapeSaisieBEReglement = _titreDeTransportContext.Etape.Single(facture => facture.Netape == ConstantesEtapes.SAISI_BEAVOIR);

//				var ListeDesAvoirsChoisis = _titreDeTransportContext.Avoir.Where(avoir => Model.ListeDesIDAvoirsChoisis.Contains(avoir.Idavoir));
				
//				foreach (var avoir in ListeDesAvoirsChoisis)
//				{
//					avoir.Idavoir = EtapeSaisieBEReglement.Idetape;
//					NouveauBEAvoir.Avoir.Add(avoir);
//				}

//				NouveauBEAvoir.Datesaisibeavoir = DateTime.Now;
//				NouveauBEAvoir.Obserbeavoir = Model.Observation;
//				NouveauBEAvoir.Idetapebeavoir = EtapeSaisieBEReglement.Idetape;
//				NouveauBEAvoir.Idetapebeavoir = IDAgentSaisie;

//				await _titreDeTransportContext.Beavoir.AddAsync(NouveauBEAvoir);
//				Transaction.Commit();
//			}
//			catch (Exception)
//			{
//				Transaction.Rollback();
//				throw;
//			}
//		}

//		public async Task MettreAJourBEAvoir(int IDBEAvoir, CreerBEAvoirModel Model)
//		{
//			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
//			try
//			{
//				var NouveauBEAvoir = new Beavoir();

//				var BEAvoirAMettreAJour = await _titreDeTransportContext.Beavoir.SingleAsync(bea => bea.Idbeavoir == IDBEAvoir);
//				if(BEAvoirAMettreAJour is null)
//				{
//					throw new Exception("BE Avoir introuvable.");
//				}

//				var IDAgentSaisie = await _agentService.GetIDAgentConnecte();
//				var ListeDesAvoirsChoisis = _titreDeTransportContext.Avoir.Where(avoir => Model.ListeDesIDAvoirsChoisis.Contains(avoir.Idavoir));
//				BEAvoirAMettreAJour.Avoir.Clear();

//				foreach (var avoir in ListeDesAvoirsChoisis)
//				{
//					BEAvoirAMettreAJour.Avoir.Add(avoir);
//				}

//				BEAvoirAMettreAJour.Datesaisibeavoir = DateTime.Now;
//				BEAvoirAMettreAJour.Obserbeavoir = Model.Observation;
//				BEAvoirAMettreAJour.Idagentsaibeavoir = IDAgentSaisie;

//				_titreDeTransportContext.Beavoir.Update(BEAvoirAMettreAJour);
//				Transaction.Commit();
//			}
//			catch (Exception)
//			{
//				Transaction.Rollback();
//				throw;
//			}
//		}

//		public async Task SupprimerBEAvoir(int IDBEAvoir)
//		{
//			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
//			try
//			{
//				var BEAvoirASupprimer = await _titreDeTransportContext.Beavoir.SingleAsync(bea => bea.Idbeavoir == IDBEAvoir);
//				if (BEAvoirASupprimer is null)
//				{
//					throw new Exception("BE Avoir introuvable.");
//				}
//				_titreDeTransportContext.Beavoir.Remove(BEAvoirASupprimer);
//				await _titreDeTransportContext.SaveChangesAsync();
//				Transaction.Commit();
//			}
//			catch (Exception)
//			{
//				Transaction.Rollback();
//				throw;
//			}
//		}

//		public Task EnvoyerBEAvoirEtapeSuperieure(int IDBEAvoir)
//		{
//			throw new NotImplementedException();
//		}

//		public Task<List<DetailsBEAvoir>> ObtenirBEAvoirSAISI_BEAvoir(RechercheModel ModelDeRecherche)
//		{
//			throw new NotImplementedException();
//		}

//		public Task<List<DetailsBEAvoir>> ObtenirBEAvoirVALIDATION_BEAvoir(RechercheModel ModelDeRecherche)
//		{
//			throw new NotImplementedException();
//		}

//		public async Task<string> ObtenirMotifRenvoiBEAvoir(int IDRenvoi)
//		{
//			try
//			{
//				var Resultat = await _titreDeTransportContext
//										.Renvoi
//										.SingleAsync(item => item.Idrenvoi == IDRenvoi && item.Typedocument == TypeDocument.BE_AVOIR);
//				return Resultat.Motif;
//			}
//			catch (Exception)
//			{
//				throw;
//			}
//		}

//		public Task RenvoyerBEAvoirEtapePrecedente(int IDBEAvoir, string MotifDeRenvoi)
//		{
//			throw new NotImplementedException();
//		}

//		public Task ValiderSaisieBEAvoir(int IDBEAvoir)
//		{
//			throw new NotImplementedException();
//		}
//	}
//}
