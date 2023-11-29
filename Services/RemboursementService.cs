//using Gestions_des_Titres_de_Transport.Constant;
//using Gestions_des_Titres_de_Transport.Models;
//using Gestions_des_Titres_de_Transport.OrdreDeMissionDBModelsEF;
//using Gestions_des_Titres_de_Transport.Services.Contrats;
//using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
//using Microsoft.EntityFrameworkCore;

//namespace Gestions_des_Titres_de_Transport.Services
//{
//	public class RemboursementService : IRemboursementService
//	{
//		private readonly IUtilisateurService _agentService;
//		private readonly IUploadFichierService _uploadFichierService;
//		private readonly OrdreDeMissionContext _ordreDeMissionContext;
//		private readonly TitreDeTransportContext _titreDeTransportContext;

//		public RemboursementService(TitreDeTransportContext titreDeTransportContext,
//			OrdreDeMissionContext ordreDeMissionContext,
//			IUploadFichierService uploadFichierService,
//			IUtilisateurService agentService)
//		{
//			_agentService = agentService;
//			_uploadFichierService = uploadFichierService;
//			_ordreDeMissionContext = ordreDeMissionContext;
//			_titreDeTransportContext = titreDeTransportContext;
//		}
//		public async Task CreerRemboursement(CreerRemboursementModel Model)
//		{
//			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
//			try
//			{
//				var MatAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();
//				var EtapeSaisieRemb = await _titreDeTransportContext.Etape.SingleAsync(item => item.Numeroetape == ConstantesEtapes.SAISI_REMBOURSEMENT);

//				var TitreDeTransport = await _titreDeTransportContext.Titredetransport.SingleAsync(titre => titre.Idtitre == Model.IDTitre);

//				var NouveauRemboursement = new Remboursement()
//				{
//					Datesairemb = DateTime.Now,
//					Matagentsairemb = MatAgentSaisie,
//					Idetaperemb = EtapeSaisieRemb.Idetape,
//					Idtitre = Model.IDTitre,
//					Sirembourse = false,
//					Siremboursable = true,
//					Observationremb = Model.Observation
//				};

//				_titreDeTransportContext.Remboursement.Add(NouveauRemboursement);
//				_titreDeTransportContext.SaveChanges();

//				TitreDeTransport.Idremb = NouveauRemboursement.Idremb;
//				_titreDeTransportContext.Titredetransport.Update(TitreDeTransport);
//				Transaction.Commit();
//			}
//			catch (Exception)
//			{
//				Transaction.Rollback();
//				throw;
//			}
//		}

//		public async Task MettreAJourRemboursement(int IDRemboursement, CreerRemboursementModel Model)
//		{
//			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
//			try
//			{
//				var IDAgentSaisi = await _agentService.GetIDAgentConnecte();
//				var RemboursementAMettreAjour = await _titreDeTransportContext
//						.Remboursement
//						.SingleAsync(remb => remb.Idremb == IDRemboursement);

//				if (RemboursementAMettreAjour is null)
//				{
//					throw new Exception("Remboursement introuvable.");
//				}

//				RemboursementAMettreAjour.Datesairemb = DateTime.Now;
//				RemboursementAMettreAjour.Idagentsairemb = IDAgentSaisi;
//				RemboursementAMettreAjour.Idtitre = Model.IDTitre;

//				Transaction.Commit();
//			}
//			catch (Exception)
//			{
//				Transaction.Rollback();
//				throw;
//			}
//		}

//		public async Task SupprimerRemboursement(int IDRemboursement)
//		{
//			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
//			try
//			{
//				var RemboursementASupprimer = await _titreDeTransportContext.Remboursement.SingleAsync(remb => remb.Idremb == IDRemboursement);

//				_titreDeTransportContext.Remboursement.Remove(RemboursementASupprimer);
//				_titreDeTransportContext.SaveChanges();
//				Transaction.Commit();
//			}
//			catch (Exception)
//			{
//				Transaction.Rollback();
//				throw;
//			}
//		}

//		public async Task EnvoyerRemboursementEnEtapeSuperieure(int IDRemboursement)
//		{
//			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
//			try
//			{
//				var RemboursementAEnvoyerEnEtapeSuperieure = await _titreDeTransportContext.Remboursement.SingleAsync(remb => remb.Idremb == IDRemboursement);
//				if (RemboursementAEnvoyerEnEtapeSuperieure is null)
//				{
//					throw new Exception("Le Remboursement est introuvable.");
//				}

//				var TitreDeTransportLier = await _titreDeTransportContext.Titredetransport.SingleAsync(titre => titre.Idtitre == RemboursementAEnvoyerEnEtapeSuperieure.Idtitre);

//				var IDEtapeActuelle = RemboursementAEnvoyerEnEtapeSuperieure.Idetaperemb;
//				var EtapeActuelle = await _titreDeTransportContext.Etape.SingleAsync(item => item.Idetape == IDEtapeActuelle);
//				var EtapeProchaine = _titreDeTransportContext.Etape.FirstOrDefault(e => e.Netape > EtapeActuelle.Netape && e.Sietapeactif == true);

//				if (EtapeProchaine is null)
//				{
//					throw new Exception("Pas d'étape prochaine.");
//				}

//				TitreDeTransportLier.Idetape = EtapeProchaine.Idetape;
//				_titreDeTransportContext.Titredetransport.Update(TitreDeTransportLier);

//				RemboursementAEnvoyerEnEtapeSuperieure.Idetaperemb = EtapeProchaine.Idetape;
//				_titreDeTransportContext.Remboursement.Update(RemboursementAEnvoyerEnEtapeSuperieure);
//				_titreDeTransportContext.SaveChanges();
//				Transaction.Commit();
//			}
//			catch (Exception)
//			{
//				Transaction.Rollback();
//				throw;
//			}
//		}

//		public async Task<string> ObtenirMotifRenvoiRemboursement(int IDRenvoi)
//		{
//			try
//			{
//				var Resultat = await _titreDeTransportContext
//										.Renvoi
//										.SingleAsync(item => item.Idrenvoi == IDRenvoi && item.Typedocument == TypeDocument.REMBOURSEMENT);
//				return Resultat.Motif;
//			}
//			catch (Exception)
//			{
//				throw;
//			}
//		}

//		public async Task<List<DetailsRemboursement>> ObtenirRembSAISIE_REMBOURSEMENT(RechercheModel ModelDeRecherche)
//		{
//			try
//			{
//				var EtapeActuelle = _titreDeTransportContext.Etape.First(item => item.Netape == ConstantesEtapes.SAISI_REMBOURSEMENT);
//				var EtapeValidation = _titreDeTransportContext.Etape.First(item => item.Netape == ConstantesEtapes.VALID_REMBOURSEMENT);

//				var ResultatRequete = (from remb in _titreDeTransportContext.Remboursement
//									   join etape in _titreDeTransportContext.Etape on remb.Idetaperemb equals etape.Idetape
//									   join titre in _titreDeTransportContext.Titredetransport on remb.Idtitre equals titre.Idtitre
//									   join prestataire in _titreDeTransportContext.Prestataire on titre.Idprestataire equals prestataire.Idprestataire
//									   select new DetailsRemboursement
//									   {
//										   IDRenvoi = remb.Idrenvoiremb,
//										   DateDeSaisie = remb.Datesairemb,
//										   DateValidation = remb.Datevalremb,
//										   IDTitre = remb.Idtitre,
//										   IDRemb = remb.Idremb,
//										   IDEtape = remb.Idetaperemb,
//										   Observation = remb.Observationremb
//									   }
//				);

//				if (ModelDeRecherche.DateDebut.HasValue)
//				{
//					ResultatRequete = ResultatRequete.Where(remb => remb.DateDeSaisie >= ModelDeRecherche.DateDebut.Value);
//				}

//				if (ModelDeRecherche.DateFin.HasValue)
//				{
//					ResultatRequete = ResultatRequete.Where(remb => remb.DateDeSaisie <= ModelDeRecherche.DateFin.Value);
//				}

//				if (!string.IsNullOrEmpty(ModelDeRecherche.TermeDeRecherche))
//				{
//					ResultatRequete = ResultatRequete
//						.Where(remb => EF.Functions.Like(remb.IDRenvoi.ToString(), "%" + ModelDeRecherche.TermeDeRecherche + "%"));
//				}

//				if (ModelDeRecherche.EnCours.HasValue && ModelDeRecherche.EnCours == true)
//				{
//					ResultatRequete = ResultatRequete.Where(remb => remb.DateDeSaisie != null && remb.DateValidation == null && remb.IDEtape == EtapeActuelle.Idetape);
//				}

//				if (ModelDeRecherche.Valides.HasValue && ModelDeRecherche.Valides == true)
//				{
//					ResultatRequete = ResultatRequete.Where(remb => remb.DateDeSaisie != null && remb.DateValidation != null && remb.IDEtape == EtapeValidation.Idetape);
//				}

//				if (ModelDeRecherche.Rejetes.HasValue && ModelDeRecherche.Rejetes == true)
//				{
//					ResultatRequete = ResultatRequete.Where(remb => remb.DateValidation == null && remb.SiRembRenvoyer == true && remb.IDEtape == EtapeActuelle.Idetape);
//				}

//				return await ResultatRequete.ToListAsync();
//			}
//			catch (Exception)
//			{
//				throw;
//			}
//		}

//		public async Task<List<DetailsRemboursement>> OBtenirRembVALIDATION_REMBOURSEMENT(RechercheModel ModelDeRecherche)
//		{
//			try
//			{
//				var EtapeActuelle = _titreDeTransportContext.Etape.First(item => item.Netape == ConstantesEtapes.VALID_REMBOURSEMENT);

//				var ResultatRequete = (from remb in _titreDeTransportContext.Remboursement
//									   join etape in _titreDeTransportContext.Etape on remb.Idetaperemb equals etape.Idetape
//									   join titre in _titreDeTransportContext.Titredetransport on remb.Idtitre equals titre.Idtitre
//									   join prestataire in _titreDeTransportContext.Prestataire on titre.Idprestataire equals prestataire.Idprestataire
//									   select new DetailsRemboursement
//									   {
//										   IDRenvoi = remb.Idrenvoiremb,
//										   DateDeSaisie = remb.Datesairemb,
//										   DateValidation = remb.Datevalremb,
//										   IDTitre = remb.Idtitre,
//										   IDRemb = remb.Idremb,
//										   IDEtape = remb.Idetaperemb,
//										   Observation = remb.Observationremb
//									   }
//				);

//				if (ModelDeRecherche.DateDebut.HasValue)
//				{
//					ResultatRequete = ResultatRequete.Where(remb => remb.DateDeSaisie >= ModelDeRecherche.DateDebut.Value);
//				}

//				if (ModelDeRecherche.DateFin.HasValue)
//				{
//					ResultatRequete = ResultatRequete.Where(remb => remb.DateDeSaisie <= ModelDeRecherche.DateFin.Value);
//				}

//				if (!string.IsNullOrEmpty(ModelDeRecherche.TermeDeRecherche))
//				{
//					ResultatRequete = ResultatRequete
//						.Where(remb => EF.Functions.Like(remb.IDRenvoi.ToString(), "%" + ModelDeRecherche.TermeDeRecherche + "%"));
//				}

//				if (ModelDeRecherche.EnCours.HasValue && ModelDeRecherche.EnCours == true)
//				{
//					ResultatRequete = ResultatRequete.Where(remb => remb.DateDeSaisie != null && remb.DateValidation == null && remb.IDEtape == EtapeActuelle.Idetape);
//				}

//				if (ModelDeRecherche.Valides.HasValue && ModelDeRecherche.Valides == true)
//				{
//					ResultatRequete = ResultatRequete.Where(remb => remb.DateDeSaisie != null && remb.DateValidation != null && remb.IDEtape == EtapeActuelle.Idetape);
//				}

//				return await ResultatRequete.ToListAsync();
//			}
//			catch (Exception)
//			{
//				throw;
//			}
//		}
//		public async Task RenvoyerRemboursementEtapePrecedente(int IDRemboursement, string MotifDeRenvoi)
//		{
//			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
//			try
//			{
//				var IDAgentRenvoi = await _agentService.GetIDAgentConnecte();
//				var RemboursementARenvoyer = _titreDeTransportContext.Remboursement.Single(item => item.Idremb == IDRemboursement);
//				if (RemboursementARenvoyer is null)
//				{
//					throw new Exception("Le Remboursement est introuvable.");
//				}

//				var TitreDeTransportLier = _titreDeTransportContext.Titredetransport.Single(titre => titre.Idtitre == RemboursementARenvoyer.Idtitre);

//				var IDEtapeActuelle = RemboursementARenvoyer.Idetaperemb;
//				var EtapeActuelle = _titreDeTransportContext.Etape.Single(item => item.Idetape == IDEtapeActuelle);
//				var EtapePrecedente = _titreDeTransportContext.Etape.Single(item => item.Netape < EtapeActuelle.Netape && item.Sietapeactif == true);

//				var RenvoiBonDePassage = new Renvoi();
//				RenvoiBonDePassage.Motif = MotifDeRenvoi;
//				RenvoiBonDePassage.Daterenvoi = DateTime.Now;
//				RenvoiBonDePassage.Idagentrenvoi = IDAgentRenvoi;
//				RenvoiBonDePassage.Iddocument = RemboursementARenvoyer.Idremb;
//				RenvoiBonDePassage.Typedocument = TypeDocument.REMBOURSEMENT;

//				_titreDeTransportContext.Renvoi.Add(RenvoiBonDePassage);
//				_titreDeTransportContext.SaveChanges();

//				TitreDeTransportLier.Idetape = EtapePrecedente.Idetape;
//				_titreDeTransportContext.Titredetransport.Update(TitreDeTransportLier);

//				RemboursementARenvoyer.Idetaperemb = EtapePrecedente.Idetape;
//				RemboursementARenvoyer.Idrenvoiremb = RenvoiBonDePassage.Idrenvoi;

//				_titreDeTransportContext.Remboursement.Update(RemboursementARenvoyer);
//				_titreDeTransportContext.SaveChanges();
//				Transaction.Commit();
//			}
//			catch (Exception)
//			{
//				Transaction.Rollback();
//				throw;
//			}
//		}

//		public async Task ValiderSaisiRemboursement(int IDRemboursement)
//		{
//			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
//			try
//			{
//				var IDAgentValidation = await _agentService.GetIDAgentConnecte();
//				var RemboursementAValider = _titreDeTransportContext.Remboursement.Single(remb => remb.Idremb == IDRemboursement);

//				RemboursementAValider.Datevalremb = DateTime.Now;
//				RemboursementAValider.Idagentvalremb = IDAgentValidation;

//				_titreDeTransportContext.Remboursement.Update(RemboursementAValider);
//				Transaction.Commit();
//			}
//			catch (Exception)
//			{
//				Transaction.Rollback();
//				throw;
//			}
//		}
//	}
//}
