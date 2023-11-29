using Gestions_des_Titres_de_Transport.Constant;
using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Microsoft.EntityFrameworkCore;

namespace Gestions_des_Titres_de_Transport.Services.BonDePassage
{
	public class SaisieBonService : ISaisieBonService
	{
		private readonly IFichierService _fichierService;
		private readonly IUtilisateurService _agentService;
		private readonly SAPCENTERDBContext _sapCenterDBContext;
		private readonly TitreDeTransportContext _titreDeTransportContext;

		public SaisieBonService(
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
		public async Task CreerBon(CreerBonDePassageModel Model)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var NouveauBonDePassage = new Bondepassage();
				var MatriculeAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var EtapeSaisieBonPassage = _titreDeTransportContext.Etape.Single(item => item.Numeroetape == ConstantesEtapes.SAISI_BON);

				var ListeDesTitresDeTransportChoisis = _titreDeTransportContext.Titredetransport.Where(item => Model.ListeDesIDTitresChoisis.Contains(item.Idtitre));

				foreach (var Titre in ListeDesTitresDeTransportChoisis)
				{
					//Titre.Idetape = EtapeSaisieBonPassage.Idetape;
					NouveauBonDePassage.Titredetransport.Add(Titre);
				}

				NouveauBonDePassage.Obserbonp = Model.Observation;
				NouveauBonDePassage.Datesaibonp = DateTime.Now;
				NouveauBonDePassage.Matagentsaibonp = MatriculeAgentSaisie;
				NouveauBonDePassage.Idagencevoyagebonp = Model.IDAgenceVoyage;
				NouveauBonDePassage.Idetapebonp = EtapeSaisieBonPassage.Idetape;

				_titreDeTransportContext.Bondepassage.Add(NouveauBonDePassage);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
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
					throw new Exception("Les Bons de passage n'ont pas été retrouvés.");
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

		public async Task ModifierBon(int IDBon, CreerBonDePassageModel Model)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatriculeAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var BonDePassageAMettreAJour = _titreDeTransportContext.Bondepassage.Single(item => item.Idbonp == IDBon);
				var NouveauxTitres = _titreDeTransportContext.Titredetransport.Where(item => Model.ListeDesIDTitresChoisis.Contains(item.Idtitre));
				var AncienTitres = _titreDeTransportContext.Titredetransport.Where(item => item.Idbonp == IDBon);

				foreach(var titre in AncienTitres)
				{
					titre.Idbonp = null;
				}
				_titreDeTransportContext.Titredetransport.UpdateRange(AncienTitres);

				foreach (var titre in NouveauxTitres)
				{
					titre.Idbonp = IDBon;
				}
				_titreDeTransportContext.Titredetransport.UpdateRange(NouveauxTitres);

				BonDePassageAMettreAJour.Datesaibonp = DateTime.Now;
				BonDePassageAMettreAJour.Obserbonp = Model.Observation;
				BonDePassageAMettreAJour.Matagentsaibonp = MatriculeAgentSaisie;
				BonDePassageAMettreAJour.Idagencevoyagebonp = Model.IDAgenceVoyage;

				BonDePassageAMettreAJour.Idrenvoibonp = null;
				BonDePassageAMettreAJour.Matagentvalbonp = null;

				_titreDeTransportContext.Bondepassage.Update(BonDePassageAMettreAJour);
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

				var EtapeActuelle = _titreDeTransportContext.Etape.FirstOrDefault(item => item.Numeroetape == ConstantesEtapes.SAISI_BON);
				if (EtapeActuelle is null)
				{
					throw new Exception("Etape non trouvée.");
				}

				var Resultats = (from bonp in _titreDeTransportContext.Bondepassage
								 join agence in _titreDeTransportContext.Agencevoyage on bonp.Idagencevoyagebonp equals agence.Idagencevoyage
								 where bonp.Idbonp == IDBon
								 select new AffichageBonAvecTitresLier
								 {
									 IDRenvoi = bonp.Idrenvoibonp,
									 IDAgenceVoyage = bonp.Idagencevoyagebonp,
									 Observation = bonp.Obserbonp,
									 ListeTitres = (from titre in _titreDeTransportContext.Titredetransport
													join etape in _titreDeTransportContext.Etape on titre.Idetape equals etape.Idetape
													join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
													join compagnie in _titreDeTransportContext.Compagnie on titre.Idcompagnie equals compagnie.Idcompagnie
													join agence in _titreDeTransportContext.Agencevoyage on titre.Idagencevoyage equals agence.Idagencevoyage
													where titre.Idagencevoyage == bonp.Idagencevoyagebonp
													where titre.Idbonp == bonp.Idbonp
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
														IDTitre = titre.Idtitre
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

		public async Task<AffichageRenvoi> ObtenirDetailsRenvoi(int IDRenvoi)
		{
			try
			{
				var Resultat = await _titreDeTransportContext.Renvoi.SingleAsync(item => item.Idrenvoi == IDRenvoi);
				var NomAgentRenvoi = await _sapCenterDBContext.Agentssap.Where(agent => agent.Matricule == Resultat.Matagentrenvoi).Select(agent => agent.NomEtPrenoms).SingleAsync();

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

		public async Task<List<AffichageBon>> ObtenirListeBon(SaisieBonRechercheModel ModelDeRecherche)
		{
			try
			{
				var EtapeActuelle = await _titreDeTransportContext.Etape.FirstAsync(item => item.Numeroetape == ConstantesEtapes.SAISI_BON);

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
					ResultatRequete = ResultatRequete.Where(bonp => bonp.DateDeSaisie != null && bonp.DateValidation == null && bonp.IDEtape == EtapeActuelle.Idetape && bonp.IDRenvoi == null);
				}

				if (ModelDeRecherche.Valides.HasValue && ModelDeRecherche.Valides == true)
				{
					ResultatRequete = ResultatRequete.Where(bonp => bonp.DateDeSaisie != null && bonp.DateValidation != null);
				}

				if (ModelDeRecherche.Rejetes.HasValue && ModelDeRecherche.Rejetes == true)
				{
					ResultatRequete = ResultatRequete.Where(bonp => bonp.DateValidation == null && bonp.IDRenvoi != null && bonp.IDEtape == EtapeActuelle.Idetape);
				}

				return await ResultatRequete.ToListAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<AffichageTitreAvecPJ>> ObtenirListeTitreAChoisir(int IDAgenceVoyage)
		{
			try
			{
				var EtapeActuelle = _titreDeTransportContext.Etape.First(item => item.Numeroetape  == ConstantesEtapes.SAISI_BON);

				var ResultatRequete = (from titre in _titreDeTransportContext.Titredetransport
									   join etape in _titreDeTransportContext.Etape on titre.Idetape equals etape.Idetape
									   join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
									   join compagnie in _titreDeTransportContext.Compagnie on titre.Idcompagnie equals compagnie.Idcompagnie
									   join agence in _titreDeTransportContext.Agencevoyage on titre.Idagencevoyage equals agence.Idagencevoyage
									   where titre.Idagencevoyage == IDAgenceVoyage
									   where titre.Idetape == EtapeActuelle.Idetape
									   where titre.Idbonp == null
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
										   IDCompagnie = titre.Idcompagnie,
										   IDAgenceVoyage = titre.Idagencevoyage,
										   MontantTitreDeTransport = titre.Montanttitret,
										   RefFicheDeMission = titre.Reffichemission,
										   RefTitre = titre.Reftitret,
										   IDTitre = titre.Idtitre,
										   LibelleClasse = classe.Libelleclasse,
										   LibelleCompagnie = compagnie.Libellecompagnie
									   });

				return await ResultatRequete.ToListAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<AffichageTitreAvecPJ>> ObtenirListeTitreAChoisirModifier(int IDBonDePassage)
		{
			try
			{
				var EtapeActuelle = await _titreDeTransportContext.Etape.SingleAsync(item => item.Numeroetape == ConstantesEtapes.SAISI_BON);

				var BonDePassageEnCours = await _titreDeTransportContext.Bondepassage.SingleAsync(_ => _.Idbonp == IDBonDePassage);


				var ListeTitresDuBonActif = (from titre in _titreDeTransportContext.Titredetransport
									  join etape in _titreDeTransportContext.Etape on titre.Idetape equals etape.Idetape
									  join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
									  join compagnie in _titreDeTransportContext.Compagnie on titre.Idcompagnie equals compagnie.Idcompagnie
									  join agence in _titreDeTransportContext.Agencevoyage on titre.Idagencevoyage equals agence.Idagencevoyage
									  where titre.Idbonp == IDBonDePassage
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
										  IDCompagnie = titre.Idcompagnie,
										  IDAgenceVoyage = titre.Idagencevoyage,
										  MontantTitreDeTransport = titre.Montanttitret,
										  RefFicheDeMission = titre.Reffichemission,
										  RefTitre = titre.Reftitret,
										  IDTitre = titre.Idtitre,
										  LibelleClasse = classe.Libelleclasse,
										  LibelleCompagnie = compagnie.Libellecompagnie
									  }).ToList();




				var ResultatRequete = (from titre in _titreDeTransportContext.Titredetransport
									   join etape in _titreDeTransportContext.Etape on titre.Idetape equals etape.Idetape
									   join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
									   join compagnie in _titreDeTransportContext.Compagnie on titre.Idcompagnie equals compagnie.Idcompagnie
									   join agence in _titreDeTransportContext.Agencevoyage on titre.Idagencevoyage equals agence.Idagencevoyage
									   where titre.Idagencevoyage == BonDePassageEnCours.Idagencevoyagebonp && titre.Idetape == EtapeActuelle.Idetape
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
										   IDCompagnie = titre.Idcompagnie,
										   IDAgenceVoyage = titre.Idagencevoyage,
										   MontantTitreDeTransport = titre.Montanttitret,
										   RefFicheDeMission = titre.Reffichemission,
										   RefTitre = titre.Reftitret,
										   IDTitre = titre.Idtitre,
										   LibelleClasse = classe.Libelleclasse,
										   LibelleCompagnie = compagnie.Libellecompagnie
									   }).ToList();

				ResultatRequete.AddRange(ListeTitresDuBonActif);

				return ResultatRequete;
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

		public async Task SupprimerBon(int IDBon)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var BonDePassageASupprimer = await _titreDeTransportContext.Bondepassage.SingleAsync(bonp => bonp.Idbonp == IDBon);
				if (BonDePassageASupprimer is null)
				{
					throw new Exception("Bon de passage introuvable.");
				}

				var ListeTitresLier = _titreDeTransportContext.Titredetransport.Where(titre => titre.Idbonp == IDBon);
				foreach (var item in ListeTitresLier)
				{
					item.Idbonp = null;
				}
				_titreDeTransportContext.Titredetransport.UpdateRange(ListeTitresLier);
				_titreDeTransportContext.SaveChanges();

				_titreDeTransportContext.Bondepassage.Remove(BonDePassageASupprimer);
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
