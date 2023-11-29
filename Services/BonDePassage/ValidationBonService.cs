using Gestions_des_Titres_de_Transport.Constant;
using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Microsoft.EntityFrameworkCore;

namespace Gestions_des_Titres_de_Transport.Services.BonDePassage
{
	public class ValidationBonService : IValidationBonService
	{
		private readonly IFichierService _fichierService;
		private readonly IUtilisateurService _agentService;
		private readonly SAPCENTERDBContext _sapCenterDBContext;
		private readonly TitreDeTransportContext _titreDeTransportContext;

		public ValidationBonService(
			TitreDeTransportContext titreDeTransportContext,
	IFichierService fichierService,
	IUtilisateurService agentService,
	SAPCENTERDBContext sapCenterDBContext
			)
		{
			_agentService = agentService;
			_fichierService = fichierService;
			_sapCenterDBContext = sapCenterDBContext;
			_titreDeTransportContext = titreDeTransportContext;
		}
		public async Task EnvoyerBonEtapeSup(int IDBon)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var BonDePassage = await _titreDeTransportContext
					.Bondepassage.SingleAsync(bonp => bonp.Idbonp == IDBon);

				if (BonDePassage is null)
				{
					throw new Exception("Bon de Passage introuvable");
				}

				var IDEtapeActuelle = BonDePassage.Idetapebonp;
				var EtapeActuelle = await _titreDeTransportContext.Etape.SingleAsync(e => e.Idetape == IDEtapeActuelle);
				var EtapeProchaine = await _titreDeTransportContext.Etape
								.Where(e => EtapeActuelle.Numeroetape < e.Numeroetape && e.Sietapeactive == true)
								.OrderBy(item => item.Numeroetape)
								.FirstOrDefaultAsync();

				if (EtapeProchaine is null)
				{
					throw new Exception("Pas d'étape prochaine.");
				}
				BonDePassage.Idetapebonp = EtapeProchaine.Idetape;

				var ListeTitreDeTransport = _titreDeTransportContext.Titredetransport.Where(titre => titre.Idbonp == BonDePassage.Idbonp);
				foreach (var titreitem in ListeTitreDeTransport)
				{
					titreitem.Idetape = EtapeProchaine.Idetape;
				}

				_titreDeTransportContext.Titredetransport.UpdateRange(ListeTitreDeTransport);
				_titreDeTransportContext.SaveChanges();

				_titreDeTransportContext.Bondepassage.Update(BonDePassage);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}
		public async Task EnvoyerBonEtapeSup(List<int> ListIDBonsDePassage)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var ListeDesBonsDePassageAEnvoyerEnEtapeSuperieure = await _titreDeTransportContext
					.Bondepassage
					.Where(bonp => ListIDBonsDePassage.Contains(bonp.Idbonp)).ToListAsync();
				if (ListeDesBonsDePassageAEnvoyerEnEtapeSuperieure.Count() <= 0)
				{
					throw new Exception("Les Titres de Transport n'ont pas été retrouvés.");
				}

				var IDEtapeActuelle = ListeDesBonsDePassageAEnvoyerEnEtapeSuperieure[0].Idetapebonp;
				var EtapeActuelle = await _titreDeTransportContext.Etape.SingleAsync(e => e.Idetape == IDEtapeActuelle);
				var EtapeProchaine = await _titreDeTransportContext.Etape
								.Where(e => EtapeActuelle.Numeroetape < e.Numeroetape && e.Sietapeactive == true)
								.OrderBy(item => item.Numeroetape)
								.FirstOrDefaultAsync();

				if (EtapeProchaine is null)
				{
					throw new Exception("Pas d'étape prochaine.");
				}

				foreach (var item in ListeDesBonsDePassageAEnvoyerEnEtapeSuperieure)
				{
					item.Idetapebonp = EtapeProchaine.Idetape;

					var ListeTitreDeTransport = _titreDeTransportContext.Titredetransport.Where(titre => titre.Idbonp == item.Idbonp);
					foreach (var titreitem in ListeTitreDeTransport)
					{
						titreitem.Idetape = EtapeProchaine.Idetape;
					}

					_titreDeTransportContext.Titredetransport.UpdateRange(ListeTitreDeTransport);
					_titreDeTransportContext.SaveChanges();
				}

				_titreDeTransportContext.Bondepassage.UpdateRange(ListeDesBonsDePassageAEnvoyerEnEtapeSuperieure);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}
		public async Task<AffichageBonAvecTitresLier> ObtenirDetailsBon(int IDBon)
		{
			try
			{
				var BonDePassage = await _titreDeTransportContext.Bondepassage.SingleAsync(item => item.Idbonp == IDBon);
				if (BonDePassage is null)
				{
					throw new Exception("Le bon de passage est introuvable.");
				}

				var EtapeActuelle = _titreDeTransportContext.Etape.FirstOrDefault(item => item.Numeroetape == ConstantesEtapes.VALID_BON);
				if (EtapeActuelle is null)
				{
					throw new Exception("Etape non trouvée.");
				}

				var Resultats = (from bonp in _titreDeTransportContext.Bondepassage
								 join agence in _titreDeTransportContext.Agencevoyage on bonp.Idagencevoyagebonp equals agence.Idagencevoyage
								 where bonp.Idbonp == IDBon
								 select new AffichageBonAvecTitresLier
								 {
									 IDAgenceVoyage = bonp.Idagencevoyagebonp,
									 Observation = bonp.Obserbonp,
									 ListeTitres = (from titre in _titreDeTransportContext.Titredetransport
													join etape in _titreDeTransportContext.Etape on titre.Idetape equals etape.Idetape
													join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
													join compagnie in _titreDeTransportContext.Compagnie on titre.Idcompagnie equals compagnie.Idcompagnie
													join agence in _titreDeTransportContext.Agencevoyage on titre.Idagencevoyage equals agence.Idagencevoyage
													where titre.Idbonp == IDBon
													select new AffichageTitreAvecPJ
													{
														ListePiecesJointes = (from pj in _titreDeTransportContext.Piecejointe
																			  where pj.Idtitrepiecej == titre.Idtitre
																			  select new FichierAfficher()
																			  {
																				  Nom = pj.Nompiecej,
																				  CheminAbsolu = _fichierService.ObtenirCheminAbsoluFichier(pj.Nompiecej)
																			  }).ToList(),
														IDClasse = titre.Idclasse,
														LibelleClasse = classe.Libelleclasse,
														IDCompagnie = titre.Idcompagnie,
														LibelleCompagnie = compagnie.Libellecompagnie,
														IDAgenceVoyage = titre.Idagencevoyage,
														MontantTitreDeTransport = titre.Montanttitret,
														RefFicheDeMission = titre.Reffichemission,
														RefTitre = titre.Reftitret,
														IDTitre = titre.Idtitre,
													}).ToList()
								 }
				);

				var BonDePassageAModifier = await Resultats.FirstAsync();
				if (BonDePassageAModifier is null)
				{
					throw new Exception("Bon de Passage non trouvé");
				}

				return BonDePassageAModifier;
			}
			catch (Exception)
			{
				throw;
			}
		}
		public async Task<List<AffichageBon>> ObtenirListeBon(ValidationBonRechercheModel ModelDeRecherche)
		{
			try
			{
				var EtapeActuelle = await _titreDeTransportContext.Etape.FirstAsync(item => item.Numeroetape == ConstantesEtapes.VALID_BON);

				var ResultatRequete = (from bonp in _titreDeTransportContext.Bondepassage
									   join agence in _titreDeTransportContext.Agencevoyage on bonp.Idagencevoyagebonp equals agence.Idagencevoyage
									   select new AffichageBon
									   {
										   IDBondePassage = bonp.Idbonp,
										   DateDeSaisie = bonp.Datesaibonp,
										   DateValidation = bonp.Datevalbonp,
										   Observation = bonp.Obserbonp,
										   NombreDeBillets = bonp.Titredetransport.Count(),
										   IDEtape = bonp.Idetapebonp,
										   IDRenvoi = bonp.Idrenvoibonp,
										   LibelleAgenceDeVoyage = agence.Libelleagencevoyage
									   }
				);

				if (ModelDeRecherche.DateDebut.HasValue)
				{
					ResultatRequete = ResultatRequete.Where(bonp => bonp.DateDeSaisie >= ModelDeRecherche.DateDebut.Value);
				}

				if (ModelDeRecherche.DateFin.HasValue)
				{
					ResultatRequete = ResultatRequete.Where(bonp => bonp.DateDeSaisie <= ModelDeRecherche.DateFin.Value);
				}

				if (!string.IsNullOrEmpty(ModelDeRecherche.TermeDeRecherche))
				{
					ResultatRequete = ResultatRequete
						.Where(bonp => EF.Functions.Like(bonp.LibelleAgenceDeVoyage, "%" + ModelDeRecherche.TermeDeRecherche + "%")
										|| EF.Functions.Like(bonp.Observation, "%" + ModelDeRecherche.TermeDeRecherche + "%"));
				}

				if (ModelDeRecherche.EnCours.HasValue && ModelDeRecherche.EnCours == true)
				{
					ResultatRequete = ResultatRequete.Where(bonp => bonp.DateDeSaisie != null && bonp.DateValidation == null && bonp.IDEtape == EtapeActuelle.Idetape);
				}

				if (ModelDeRecherche.Valides.HasValue && ModelDeRecherche.Valides == true)
				{
					ResultatRequete = ResultatRequete.Where(bonp => bonp.DateDeSaisie != null && bonp.DateValidation != null && bonp.IDEtape == EtapeActuelle.Idetape);
				}

				return await ResultatRequete.ToListAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}
		public async Task<ImprimerBonModel> ObtenirModelBonAImprimer(int IDBon)
		{
			try
			{
				var BonDePassage = await _titreDeTransportContext.Bondepassage.SingleAsync(bonp => bonp.Idbonp == IDBon);
				if (BonDePassage == null)
				{
					throw new Exception("Bon de passage introuvable.");
				}

				var Resultats = (from bonp in _titreDeTransportContext.Bondepassage
								 join agence in _titreDeTransportContext.Agencevoyage on bonp.Idagencevoyagebonp equals agence.Idagencevoyage
								 where bonp.Idbonp == IDBon
								 select new ImprimerBonModel
								 {
									 IDBonDePassage = bonp.Idbonp,
									 DateSaisie = bonp.Datesaibonp,
									 LibelleAgence = agence.Libelleagencevoyage,
									 NomSignataire = "Nom Signataire",
									 FonctionSignataire = "Fonction Signataire",
									 NombreDeBillets = bonp.Titredetransport.Count(),
									 ListeLigne = (from titre in _titreDeTransportContext.Titredetransport
												   where titre.Idbonp == bonp.Idbonp
												   join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
												   select new LignePassagerBon
												   {
													   Classe = classe.Libelleclasse,
													   DateDepart = titre.Datedepart,
													   NomPassager = "Nom Passager",
													   Routing = titre.Routing,
													   DateDeRetour = titre.Dateretour,
													   MatBeneficiaire = titre.Matagentbeneficiaire
												   }).ToList()
								 }).ToList().First();

				//var AgentSignataire = await _sapCenterDBContext.Agentssap.SingleAsync(age => age.Matricule == BonDePassage.Matagentvalbonp);

				//Resultats.NomSignataire = AgentSignataire.NomEtPrenoms;
				//Resultats.FonctionSignataire = AgentSignataire.Poste;

				var ListePassagers = Resultats.ListeLigne
					.Select(_ => new LignePassagerBon()
					{
						Classe = _.Classe,
						DateDepart = _.DateDepart,
						DateDeRetour = _.DateDeRetour,
						NomPassager = _sapCenterDBContext.Agentssap.Single(agent => agent.Matricule == _.MatBeneficiaire).NomEtPrenoms,
						Routing = _.Routing,
						MatBeneficiaire = _.MatBeneficiaire
					}).ToList();

				Resultats.ListeLigne = ListePassagers;

				return Resultats;
			}
			catch (Exception)
			{
				throw;
			}
		}
		public async Task RenvoyerBon(int IDBon, string Motif)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatAgentRenvoi = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var BonDePassageARenvoyer = await _titreDeTransportContext.Bondepassage.SingleAsync(item => item.Idbonp == IDBon);

				var IDEtapeActuelle = BonDePassageARenvoyer.Idetapebonp;
				var EtapeActuelle = _titreDeTransportContext.Etape.Single(item => item.Idetape == IDEtapeActuelle);
				var EtapePrecedente = await _titreDeTransportContext.Etape
					.Where(item => EtapeActuelle.Numeroetape > item.Numeroetape && item.Sietapeactive == true)
					.OrderByDescending(item => item.Numeroetape)
					.FirstOrDefaultAsync();

				if (BonDePassageARenvoyer is null)
				{
					throw new Exception("Le Bon de Passage est introuvable.");
				}

				var RenvoiBonDePassage = new Renvoi();

				RenvoiBonDePassage.Motifrenvoi = Motif;
				RenvoiBonDePassage.Daterenvoi = DateTime.Now;
				RenvoiBonDePassage.Matagentrenvoi = MatAgentRenvoi;
				RenvoiBonDePassage.Iddocument = BonDePassageARenvoyer.Idbonp;
				RenvoiBonDePassage.Typedocument = TypeDocument.BON_DE_PASSAGE;

				_titreDeTransportContext.Renvoi.Add(RenvoiBonDePassage);
				_titreDeTransportContext.SaveChanges();

				BonDePassageARenvoyer.Idetapebonp = EtapePrecedente.Idetape;
				BonDePassageARenvoyer.Idrenvoibonp = RenvoiBonDePassage.Idrenvoi;
				BonDePassageARenvoyer.Datevalbonp = null;
				BonDePassageARenvoyer.Matagentvalbonp = null;

				foreach (var item in BonDePassageARenvoyer.Titredetransport)
				{
					item.Idetape = EtapePrecedente.Idetape;
					_titreDeTransportContext.Titredetransport.Update(item);
				}

				_titreDeTransportContext.Bondepassage.Update(BonDePassageARenvoyer);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}
		public async Task ValiderBon(int IDBon)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var BonDePassage = await _titreDeTransportContext.Bondepassage.SingleAsync(bonp => bonp.Idbonp == IDBon);
				if (BonDePassage is null)
				{
					throw new Exception("Le Bon de passage est introuvable.");
				}

				var MatAgentValidation = await _agentService.ObtenirMatriculeUtilisateurConnecte();

				BonDePassage.Datevalbonp = DateTime.Now;
				BonDePassage.Matagentvalbonp = MatAgentValidation;

				_titreDeTransportContext.Bondepassage.Update(BonDePassage);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}
		public async Task ValiderBon(List<int> ListeIDBon)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var ListDesBonsDePassages = await _titreDeTransportContext.Bondepassage.Where(bonp => ListeIDBon.Contains(bonp.Idbonp)).ToListAsync();
				if (ListDesBonsDePassages.Count <= 0)
				{
					throw new Exception("Les Bons de passage sont introuvables.");
				}

				var MatAgentValidation = await _agentService.ObtenirMatriculeUtilisateurConnecte();

				foreach (var item in ListDesBonsDePassages)
				{
					item.Datevalbonp = DateTime.Now;
					item.Matagentvalbonp = MatAgentValidation;
				}

				_titreDeTransportContext.Bondepassage.UpdateRange(ListDesBonsDePassages);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}
	}
}
