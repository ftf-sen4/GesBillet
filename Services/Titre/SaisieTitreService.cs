using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.OrdreDeMissionDBModelsEF;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Microsoft.AspNetCore.Components.Forms;
using Gestions_des_Titres_de_Transport.Constant;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Data;

namespace Gestions_des_Titres_de_Transport.Services
{
	public class SaisieTitreService : ISaisieTitreService
	{
		private readonly IFichierService _fichierService;
		private readonly IUtilisateurService _agentService;
		private readonly SAPCENTERDBContext _sapCenterDBContext;
		private readonly TitreDeTransportContext _titreDeTransportContext;
		private readonly IFicheDeMissionService _ficheDeMissionService;

		public SaisieTitreService(
			TitreDeTransportContext titreDeTransportContext,
	IFichierService fichierService,
	IUtilisateurService agentService,
	SAPCENTERDBContext sapCenterDBContext,
	IFicheDeMissionService ficheDeMissionService
			)
		{
			_agentService = agentService;
			_fichierService = fichierService;
			_sapCenterDBContext = sapCenterDBContext;
			_titreDeTransportContext = titreDeTransportContext;
			_ficheDeMissionService = ficheDeMissionService;
		}

		public async Task CreerPlusieursTitres(string RefsFicheMission)
		{
			try
			{

				var EtapeDeSaisie = _titreDeTransportContext.Etape.FirstOrDefault(item => item.Numeroetape == ConstantesEtapes.SAISI_TITRE);
				if (EtapeDeSaisie is null)
				{
					throw new Exception("L' étape n'a pas été trouvée.");
				}

				var MatAgentConnecter = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var ListeDesRefFiche = RefsFicheMission.Split(",").Select(refFic=> refFic.Trim());

				var ListeDesTitres = new List<Titredetransport>();
                foreach (var referenceFiche in ListeDesRefFiche)
                {
					var DetailsFiche = await _ficheDeMissionService.GetFicheDeMission(referenceFiche);


					var MotCle = $"%{DetailsFiche?.NomAgentBeneficiaire.Trim()}%";
					var AgentBeneficiaire = await _sapCenterDBContext.Agentssap.SingleAsync(item => EF.Functions.Like(item.NomEtPrenoms, MotCle));


					var NouveauTitre = new Titredetransport()
					{
						Datesaititret = DateTime.Now,
						Montanttitret = (decimal)DetailsFiche.MontantDuBillet,
						Matagentsaititret = MatAgentConnecter,
						Reffichemission = referenceFiche,
						Reftitret = "",
						Datedepart = (DateTime)DetailsFiche.DateDeDepartVoyage,
						Dateretour = (DateTime)DetailsFiche.DateRetourVoyage,
						Routing = DetailsFiche.Routing,
						Idetape = EtapeDeSaisie.Idetape,
						Matagentbeneficiaire = AgentBeneficiaire.Matricule
					};

					ListeDesTitres.Add(NouveauTitre);
                }

				await _titreDeTransportContext.Titredetransport.AddRangeAsync(ListeDesTitres);
				await _titreDeTransportContext.SaveChangesAsync();
            }
			catch (Exception)
			{
				throw;
			}
		}

		public async Task CreerTitre(List<IBrowserFile> ListeFichiersJoints,
			CreerTitreDeTransportModel Model,
			AffichageMission DetailsFicheDeMission)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var TitreDeTransportExistant = _titreDeTransportContext.Titredetransport.FirstOrDefault(item => item.Reftitret == Model.RefTitre);
				if (TitreDeTransportExistant is not null)
				{
					throw new Exception("Le titre de transport existe déjà");
				}

				var EtapeDeSaisie = _titreDeTransportContext.Etape.FirstOrDefault(item => item.Numeroetape == ConstantesEtapes.SAISI_TITRE);
				if (EtapeDeSaisie is null)
				{
					throw new Exception("L' étape n'a pas été trouvée.");
				}

				var MatAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var MotCle = $"%{DetailsFicheDeMission?.NomAgentBeneficiaire.Trim()}%";
				var AgentBeneficiaire = await _sapCenterDBContext.Agentssap.SingleAsync(item => EF.Functions.Like(item.NomEtPrenoms, MotCle));


				var nouveauTitreDeTransport = new Titredetransport
				{
					Idclasse = Model.IDClasse,
					Idcompagnie = Model.IDCompagnie,
					Idagencevoyage = Model.IDAgenceVoyage,
					Reffichemission = Model.RefFicheDeMission,
					Reftitret = Model.RefTitre,
					Montanttitret = Model.MontantTitreDeTransport,
					Datesaititret = DateTime.Now,
					Idetape = EtapeDeSaisie.Idetape,
					Matagentsaititret = MatAgentSaisie,
					Obstitret = Model.Observation,
					Routing = DetailsFicheDeMission.Routing,
					Datedepart = (DateTime)DetailsFicheDeMission?.DateDeDepartVoyage,
					Dateretour = (DateTime)DetailsFicheDeMission?.DateRetourVoyage,
					Matagentbeneficiaire = AgentBeneficiaire.Matricule,
				};

				_titreDeTransportContext.Add(nouveauTitreDeTransport);
				_titreDeTransportContext.SaveChanges();

				List<Fichier> ListeDesFichiersEnreg = new List<Fichier>();
				ListeDesFichiersEnreg = await _fichierService.EnregistrerFichierSurServeur(ListeFichiersJoints);


				List<Piecejointe> ListeDesPiecesJointes = new List<Piecejointe>();
				foreach (var item in ListeDesFichiersEnreg)
				{
					ListeDesPiecesJointes.Add(new Piecejointe()
					{
						Idetapepiecej = EtapeDeSaisie.Idetape,
						Idtitrepiecej = nouveauTitreDeTransport.Idtitre,
						Nompiecej = item.Nom,
						Dateenrpiecej = DateTime.Now
					});
				}

				_titreDeTransportContext.Piecejointe.AddRange(ListeDesPiecesJointes);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}
		public async Task EnvoyerTitreEtapeSup(int IDTitreDeTransport)
		{

			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var TitreDeTransport = await _titreDeTransportContext.Titredetransport.SingleAsync(titre => titre.Idtitre == IDTitreDeTransport);
				if (TitreDeTransport is null)
				{
					throw new Exception("Le Titre de Transport est introuvable.");
				}

				var IDEtapeActuelle = TitreDeTransport.Idetape;
				var EtapeActuelle = _titreDeTransportContext.Etape.Single(item => item.Idetape == IDEtapeActuelle);
				var EtapeProchaine = await _titreDeTransportContext.Etape
												.Where(e => EtapeActuelle.Numeroetape < e.Numeroetape && e.Sietapeactive == true)
												.OrderBy(item => item.Numeroetape)
												.FirstOrDefaultAsync();

				if (EtapeProchaine is null)
				{
					throw new Exception("Pas d'étape prochaine.");
				}

				TitreDeTransport.Idetape = EtapeProchaine.Idetape;

				_titreDeTransportContext.Titredetransport.Update(TitreDeTransport);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}
		public async Task EnvoyerTitreEtapeSup(List<int> ListeIDTitresDeTransport)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var ListeDesTitresDeTransport = await _titreDeTransportContext.Titredetransport.Where(titre => ListeIDTitresDeTransport.Contains(titre.Idtitre)).ToListAsync();
				if (ListeDesTitresDeTransport.Count <= 0)
				{
					throw new Exception("Les Titres de Transport n'ont pas été retrouvés.");
				}

				var IDEtapeActuelle = ListeDesTitresDeTransport[0].Idetape;
				var EtapeActuelle = _titreDeTransportContext.Etape.Single(item => item.Idetape == IDEtapeActuelle);
				var EtapeProchaine = await _titreDeTransportContext.Etape
												.Where(e => EtapeActuelle.Numeroetape < e.Numeroetape && e.Sietapeactive == true)
												.OrderBy(item => item.Numeroetape)
												.FirstOrDefaultAsync();

				if (EtapeProchaine is null)
				{
					throw new Exception("Pas d'étape prochaine.");
				}

				foreach (var item in ListeDesTitresDeTransport)
				{
					item.Idetape = EtapeProchaine.Idetape;
					_titreDeTransportContext.Titredetransport.Update(item);
				}

				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}
		public async Task ModifierTitre(int IDTitreDeTransport, List<IBrowserFile> ListeNouveauFichiersJoints, CreerTitreDeTransportModel TitreModel, AffichageMission DetailsFicheDeMission)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var TitreDeTransportAMettreAjour = _titreDeTransportContext.Titredetransport.FirstOrDefault(item => item.Idtitre == IDTitreDeTransport);
				if (TitreDeTransportAMettreAjour is null)
				{
					throw new Exception("Le titre de transport n'existe pas.");
				}

				var MatriculeAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var MotCle = $"%{DetailsFicheDeMission?.NomAgentBeneficiaire.Trim()}%";

				var AgentBeneficiaire = await _sapCenterDBContext.Agentssap.SingleAsync(item => EF.Functions.Like(item.NomEtPrenoms, MotCle));

				TitreDeTransportAMettreAjour.Idclasse = TitreModel.IDClasse;
				TitreDeTransportAMettreAjour.Idcompagnie = TitreModel.IDCompagnie;
				TitreDeTransportAMettreAjour.Idagencevoyage = TitreModel.IDAgenceVoyage;
				TitreDeTransportAMettreAjour.Reftitret = TitreModel.RefTitre;
				TitreDeTransportAMettreAjour.Reffichemission = TitreModel.RefFicheDeMission;
				TitreDeTransportAMettreAjour.Montanttitret = TitreModel.MontantTitreDeTransport;
				TitreDeTransportAMettreAjour.Datesaititret = DateTime.Now;
				TitreDeTransportAMettreAjour.Matagentbeneficiaire = AgentBeneficiaire.Matricule;
				TitreDeTransportAMettreAjour.Matagentsaititret = MatriculeAgentSaisie;
				TitreDeTransportAMettreAjour.Obstitret = TitreModel.Observation;
				TitreDeTransportAMettreAjour.Routing = DetailsFicheDeMission.Routing;
				TitreDeTransportAMettreAjour.Datedepart = (DateTime)DetailsFicheDeMission.DateDeDepartVoyage;
				TitreDeTransportAMettreAjour.Dateretour = (DateTime)DetailsFicheDeMission.DateRetourVoyage;

				var SiTitreRefExiste = _titreDeTransportContext.Titredetransport.Where(titre => titre.Reftitret == TitreModel.RefTitre).ToList();
				if(SiTitreRefExiste.Count > 0)
				{
					throw new Exception("La référence de ce titre existe déjà.");
				}

				TitreDeTransportAMettreAjour.Idrenvoititret = null;

				_titreDeTransportContext.Titredetransport.Update(TitreDeTransportAMettreAjour);
				_titreDeTransportContext.SaveChanges();

				if (ListeNouveauFichiersJoints.Count() is not 0)
				{
					List<Fichier> ListeDesFichiersEnreg = new List<Fichier>();
					ListeDesFichiersEnreg = await _fichierService.EnregistrerFichierSurServeur(ListeNouveauFichiersJoints);

					List<Piecejointe> ListeDesPiecesJointes = new List<Piecejointe>();
					foreach (var item in ListeDesFichiersEnreg)
					{
						ListeDesPiecesJointes.Add(new Piecejointe()
						{
							Nompiecej = item.Nom,
							Idtitrepiecej = TitreDeTransportAMettreAjour.Idtitre,
							Idetapepiecej = TitreDeTransportAMettreAjour.Idetape,
							Dateenrpiecej = DateTime.Now
						});
					}

					_titreDeTransportContext.Piecejointe.AddRange(ListeDesPiecesJointes);
				}

				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}
		public async Task<AffichageRenvoi> ObtenirDetailsRenvoi(int IDRenvoi)
		{
			try
			{
				var Resultat = await _titreDeTransportContext.Renvoi.SingleAsync(item => item.Idrenvoi == IDRenvoi);
				var NomAgentRenvoi = await _sapCenterDBContext.Agentssap.Where(agent=> agent.Matricule == Resultat.Matagentrenvoi).Select(agent => agent.NomEtPrenoms).SingleAsync();

				var DetailsDuRenvoi = new AffichageRenvoi
				{
					Daterenvoi = Resultat.Daterenvoi,
					Motifrenvoi = Resultat.Motifrenvoi,
					NomAgentRenvoi = NomAgentRenvoi
				};

				return DetailsDuRenvoi;
			}
			catch (Exception)
			{
				throw;
			}
		}
		public async Task<AffichageTitreAvecPJ> ObtenirDetailsTitre(int IDTitreDeTransport)
		{
			try
			{
				var EtapeDeSaisie = await _titreDeTransportContext.Etape.FirstOrDefaultAsync(item => item.Numeroetape == ConstantesEtapes.SAISI_TITRE);
				if (EtapeDeSaisie is null)
				{
					throw new Exception("Etape non trouvée.");
				}

				var Resultats = (from titre in _titreDeTransportContext.Titredetransport
								 join etape in _titreDeTransportContext.Etape on titre.Idetape equals etape.Idetape
								 where titre.Idtitre == IDTitreDeTransport
								 select new AffichageTitreAvecPJ
								 {
									 RefFicheDeMission = titre.Reffichemission,
									 RefTitre = titre.Reftitret,
									 IDClasse = titre.Idclasse,
									 IDCompagnie = titre.Idcompagnie,
									 MontantTitreDeTransport = titre.Montanttitret,
									 Observation = titre.Obstitret,
									 IDAgenceVoyage = titre.Idagencevoyage,
									 IDRenvoi = titre.Idrenvoititret
								 });

				var TitreAModifier = await Resultats.FirstAsync();
				if (TitreAModifier is null)
				{
					throw new Exception("Titre De Transport introuvable.");
				}

				var ListePiecesJointes = await _titreDeTransportContext
					.Piecejointe
					.Where(item => item.Idetapepiecej == EtapeDeSaisie.Idetape && item.Idtitrepiecej == IDTitreDeTransport)
					.Select(item => item.Nompiecej)
					.Select(item => new FichierAfficher()
					{
						Nom = item,
						CheminAbsolu = _fichierService.ObtenirCheminAbsoluFichier(item)
					})
					.ToListAsync();

				TitreAModifier.ListePiecesJointes?.AddRange(ListePiecesJointes);

				return TitreAModifier;
			}
			catch (Exception)
			{
				throw;
			}
		}
		public async Task<List<AffichageTitre>> ObtenirListeTitre(SaisieTitreRechercheModel ModelDeRecherche)
		{
			try
			{ 
				var EtapeActuelle = await _titreDeTransportContext.Etape.SingleOrDefaultAsync(item => item.Numeroetape == ConstantesEtapes.SAISI_TITRE);
				var EtapeValidation = await _titreDeTransportContext.Etape.SingleOrDefaultAsync(item => item.Numeroetape == ConstantesEtapes.VALID_TITRE);

				var ResultatRequete = (from titre in _titreDeTransportContext.Titredetransport
									   join etape in _titreDeTransportContext.Etape on titre.Idetape equals etape.Idetape
									   join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
									   into jointureGroup
									   from jointure in jointureGroup.DefaultIfEmpty()
									   join compagnie in _titreDeTransportContext.Compagnie on titre.Idcompagnie equals compagnie.Idcompagnie
									   into jointureGroup2
									   from jointure2 in jointureGroup2.DefaultIfEmpty()
									   join agence in _titreDeTransportContext.Agencevoyage on titre.Idagencevoyage equals agence.Idagencevoyage
									   into jointureGroup3 from jointure3 in jointureGroup3.DefaultIfEmpty()
									   select new AffichageTitre
									   {
										   IDTitre = titre.Idtitre,
										   AgenceVoyage = jointure3.Libelleagencevoyage,
										   ClasseVoyage = jointure.Libelleclasse,
										   Compagnie = jointure2.Libellecompagnie,
										   Routing = titre.Routing,
										   RefFicheMission = titre.Reffichemission,
										   IDRenvoi = titre.Idrenvoititret,
										   Montant = titre.Montanttitret,
										   DateSaisiTitre = titre.Datesaititret,
										   DateValidTitre = titre.Datevaltitret,
										   RefTitre = titre.Reftitret,
										   IDEtape = titre.Idetape
									   }
				);



				if (ModelDeRecherche.DateDebut.HasValue)
				{
					ResultatRequete = ResultatRequete.Where(titre => titre.DateSaisiTitre >= ModelDeRecherche.DateDebut.Value);
				}

				if (ModelDeRecherche.DateFin.HasValue)
				{
					ResultatRequete = ResultatRequete.Where(titre => titre.DateSaisiTitre <= ModelDeRecherche.DateFin.Value);
				}

				if (!string.IsNullOrEmpty(ModelDeRecherche.TermeDeRecherche))
				{
					ResultatRequete = ResultatRequete
						.Where(titre => EF.Functions.Like(titre.RefTitre, "%" + ModelDeRecherche.TermeDeRecherche + "%") ||
										EF.Functions.Like(titre.RefFicheMission, "%" + ModelDeRecherche.TermeDeRecherche + "%") ||
										EF.Functions.Like(titre.Compagnie, "%" + ModelDeRecherche.TermeDeRecherche + "%") ||
										EF.Functions.Like(titre.AgenceVoyage, "%" + ModelDeRecherche.TermeDeRecherche + "%") ||
										EF.Functions.Like(titre.ClasseVoyage, "%" + ModelDeRecherche.TermeDeRecherche + "%") ||
										EF.Functions.Like(titre.Routing, "%" + ModelDeRecherche.TermeDeRecherche + "%")
										)
						;
				}

				if (ModelDeRecherche.NonValides.HasValue && ModelDeRecherche.NonValides == true)
				{
					ResultatRequete = ResultatRequete.Where(titre => titre.DateValidTitre == null && titre.IDEtape == EtapeActuelle.Idetape && titre.IDRenvoi == null && (titre.ClasseVoyage == null || titre.Compagnie == null || titre.AgenceVoyage == null || titre.RefTitre == ""));
				}

				if (ModelDeRecherche.EnCours.HasValue && ModelDeRecherche.EnCours == true)
				{
					ResultatRequete = ResultatRequete.Where(titre => titre.DateValidTitre == null && titre.IDEtape == EtapeActuelle.Idetape && titre.IDRenvoi == null && !(titre.ClasseVoyage == null || titre.Compagnie == null || titre.AgenceVoyage == null || titre.RefTitre == ""));
				}

				if (ModelDeRecherche.Valides.HasValue && ModelDeRecherche.Valides == true)
				{
					ResultatRequete = ResultatRequete.Where(titre => titre.DateSaisiTitre != null && titre.DateValidTitre != null);
				}

				if (ModelDeRecherche.Rejetes.HasValue && ModelDeRecherche.Rejetes == true)
				{
					ResultatRequete = ResultatRequete.Where(titre => titre.DateValidTitre == null && titre.DateValidTitre == null && titre.IDEtape == EtapeActuelle.Idetape && titre.IDRenvoi != null);
				}

				return await ResultatRequete.OrderByDescending(titre => titre.DateSaisiTitre).ToListAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}
		public async Task SupprimerPieceJointeTitre(string NomPieceJointe)
		{
			try
			{
				var PieceJointeASupprimer = await _titreDeTransportContext.Piecejointe.SingleAsync(item => item.Nompiecej == NomPieceJointe);
				if (PieceJointeASupprimer is null)
				{
					throw new Exception("Piece Jointe Introuvable");
				}

				//_uploadFichierService.SupprimerFichier(PieceJointeASupprimer.Nompiecej);

				_titreDeTransportContext.Piecejointe.Remove(PieceJointeASupprimer);
				await _titreDeTransportContext.SaveChangesAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}
		public async Task SupprimerTitre(int IDTitreDeTransport)
		{
			var transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var TitreDeTransportASupprimer = await _titreDeTransportContext
					.Titredetransport
					.SingleAsync(item => item.Idtitre == IDTitreDeTransport);

				if (TitreDeTransportASupprimer is null)
				{
					throw new Exception("Le Titre De Transport est introuvable.");
				}

				var ListeDesPiecesJointes = await _titreDeTransportContext.Piecejointe.Where(item => item.Idtitrepiecej == IDTitreDeTransport).ToListAsync();
				if (ListeDesPiecesJointes.Count is not 0)
				{
					foreach (var item in ListeDesPiecesJointes)
					{
						_titreDeTransportContext.Piecejointe.Remove(item);
					}
				}

				_titreDeTransportContext.Titredetransport.Remove(TitreDeTransportASupprimer);
				_titreDeTransportContext.SaveChanges();
				transaction.Commit();
			}
			catch (Exception)
			{
				transaction.Rollback();
				throw;
			}
		}
		public async Task SupprimerTitre(List<int> ListIDTitreDeTransport)
		{
			var transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var ListeDesTitresDeTransport = await _titreDeTransportContext.Titredetransport
					.Where(titre => ListIDTitreDeTransport.Contains(titre.Idtitre))
					.ToListAsync();

				var IDEtapeActuelle = ListeDesTitresDeTransport[0].Idetape;

				foreach (var titre in ListeDesTitresDeTransport)
				{
					var ListeDesPiecesJointes = await _titreDeTransportContext.Piecejointe.Where(pj => titre.Idtitre == pj.Idtitrepiecej).ToListAsync();
					if (ListeDesPiecesJointes.Count is not 0)
					{
						foreach (var pj in ListeDesPiecesJointes)
						{
							_titreDeTransportContext.Piecejointe.Remove(pj);
						}
					}
					_titreDeTransportContext.Titredetransport.Remove(titre);
				}

				_titreDeTransportContext.SaveChanges();
				transaction.Commit();
			}
			catch (Exception)
			{
				transaction.Rollback();
				throw;
			}
		}
	}
}
