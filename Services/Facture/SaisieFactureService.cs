using Gestions_des_Titres_de_Transport.Constant;
using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace Gestions_des_Titres_de_Transport.Services.Facture
{
	public class SaisieFactureService : ISaisieFactureService
	{
		private readonly IFichierService _fichierService;
		private readonly IUtilisateurService _agentService;
		private readonly SAPCENTERDBContext _sapCenterDBContext;
		private readonly TitreDeTransportContext _titreDeTransportContext;

		public SaisieFactureService(
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
		public async Task CreerFacture(List<IBrowserFile> FichierJoints, CreerFactureModel Model)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var EtapeDeSaisie = _titreDeTransportContext.Etape.FirstOrDefault(item => item.Numeroetape == ConstantesEtapes.SAISI_FACTURE);
				if (EtapeDeSaisie is null)
				{
					throw new Exception("L' étape n'a pas été trouvée.");
				}

				var MatAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();

				var NouvelleFacture = new TitreDeTransportModelsEF.Facture()
				{
					Idagencevoyagefacture = Model.IDAgenceDeVoyage,
					Matagentsaifacture = MatAgentSaisie,
					Datesaisifacture = DateTime.Now,
					Obserfacture = Model.Observation,
					Dateagenceemissionfacture = Model.DateEmission,
					Idetapefacture = EtapeDeSaisie.Idetape,
					Numerofactureagence = Model.RefFacture
				};
				_titreDeTransportContext.Facture.Add(NouvelleFacture);
				_titreDeTransportContext.SaveChanges();

				var ListeDesTitresDeTransport = _titreDeTransportContext
						.Titredetransport
						.Where(titre => Model.ListeDesIDTitresChoisis.Contains(titre.Idtitre));

                foreach (var titre in ListeDesTitresDeTransport)
                {
					titre.Idfacture = NouvelleFacture.Idfacture;
                }

				_titreDeTransportContext.Titredetransport.UpdateRange(ListeDesTitresDeTransport);
				_titreDeTransportContext.SaveChanges();

				List<Fichier> ListeDesFichiersEnreg = new List<Fichier>();
				ListeDesFichiersEnreg = await _fichierService.EnregistrerFichierSurServeur(FichierJoints);

				List<Piecejointe> ListeDesPiecesJointes = new List<Piecejointe>();
				foreach (var item in ListeDesFichiersEnreg)
				{
					ListeDesPiecesJointes.Add(new Piecejointe()
					{
						Idetapepiecej = EtapeDeSaisie.Idetape,
						Idfacturepiecej = NouvelleFacture.Idfacture,
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

		public async Task EnvoyerFactureEtapeSup(int IDFacture)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var FactureAEnvoyerEnEtapeSuperieure = _titreDeTransportContext.Facture.SingleOrDefault(fact => fact.Idfacture == IDFacture);
				if (FactureAEnvoyerEnEtapeSuperieure is null)
				{
					throw new Exception("La facture est introuvable.");
				}

				var IDEtapeActuelle = FactureAEnvoyerEnEtapeSuperieure.Idetapefacture;
				var EtapeActuelle = await _titreDeTransportContext.Etape.SingleAsync(item => item.Idetape == IDEtapeActuelle);
				var EtapeProchaine = await _titreDeTransportContext.Etape
												.Where(e => EtapeActuelle.Numeroetape < e.Numeroetape && e.Sietapeactive == true)
												.OrderBy(item => item.Numeroetape)
												.FirstOrDefaultAsync();
				if (EtapeProchaine is null)
				{
					throw new Exception("Pas d'étape prochaine.");
				}


				var ListeDesTitresLiersFacture = FactureAEnvoyerEnEtapeSuperieure.Titredetransport;

				foreach (var item in ListeDesTitresLiersFacture)
				{
					item.Idetape = EtapeProchaine.Idetape;
				}

				_titreDeTransportContext.Titredetransport.UpdateRange(ListeDesTitresLiersFacture);

				FactureAEnvoyerEnEtapeSuperieure.Idetapefacture = EtapeProchaine.Idetape;
				_titreDeTransportContext.Facture.Update(FactureAEnvoyerEnEtapeSuperieure);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}

		public async Task EnvoyerFactureEtapeSup(List<int> ListeIDFactures)
		{

			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var ListeDesFactures = await _titreDeTransportContext.Facture.Where(fact => ListeIDFactures.Contains(fact.Idfacture)).ToListAsync();
				if (ListeDesFactures.Count < 0)
				{
					throw new Exception("Les factures sont introuvables.");
				}

				var IDEtapeActuelle = ListeDesFactures[0].Idetapefacture;
				var EtapeActuelle = await _titreDeTransportContext.Etape.SingleAsync(item => item.Idetape == IDEtapeActuelle);
				var EtapeProchaine = await _titreDeTransportContext.Etape
												.Where(e => EtapeActuelle.Numeroetape < e.Numeroetape && e.Sietapeactive == true)
												.OrderBy(item => item.Numeroetape)
												.FirstOrDefaultAsync();
				if (EtapeProchaine is null)
				{
					throw new Exception("Pas d'étape prochaine.");
				}

                foreach (var item in ListeDesFactures)
                {
					item.Idetapefacture = EtapeProchaine.Idetape;

					var ListeTitresTransport = item.Titredetransport;

					foreach (var titre in ListeTitresTransport)
					{
						titre.Idetape = EtapeProchaine.Idetape;
					}
					_titreDeTransportContext.Titredetransport.UpdateRange(ListeTitresTransport);
					_titreDeTransportContext.Facture.Update(item);
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

		public async Task ModifierFacture(int IDFacture, List<IBrowserFile> ListeNouveauFichiersJoints, CreerFactureModel Model)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var FactureAModifier = await _titreDeTransportContext.Facture.SingleAsync(fact => fact.Idfacture == IDFacture);
				var EtapeDeSaisie = _titreDeTransportContext.Etape.FirstOrDefault(item => item.Numeroetape == ConstantesEtapes.SAISI_FACTURE);
				if (EtapeDeSaisie is null)
				{
					throw new Exception("L' étape n'a pas été trouvée.");
				}

				var MatAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();

				FactureAModifier.Obserfacture = Model.Observation;
				FactureAModifier.Datesaisifacture = DateTime.Now;
				FactureAModifier.Matagentsaifacture = MatAgentSaisie;
				FactureAModifier.Dateagenceemissionfacture = Model.DateEmission;
				FactureAModifier.Numerofactureagence = Model.RefFacture;

				var ListeAnciensTitresLier = _titreDeTransportContext.Titredetransport.Where(titre => titre.Idfacture == FactureAModifier.Idfacture);

                foreach (var item in ListeAnciensTitresLier)
                {
					item.Idfacture = null;
                }
				_titreDeTransportContext.Titredetransport.UpdateRange(ListeAnciensTitresLier);

                var ListeNouveauxTitresLier = _titreDeTransportContext
						.Titredetransport
						.Where(titre => Model.ListeDesIDTitresChoisis.Contains(titre.Idtitre));

				foreach (var titre in ListeNouveauxTitresLier)
				{
					titre.Idfacture = FactureAModifier.Idfacture;
				}
				_titreDeTransportContext.Titredetransport.UpdateRange(ListeAnciensTitresLier);


				FactureAModifier.Idrenvoifacture = null;
				_titreDeTransportContext.Facture.Update(FactureAModifier);
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
							Idetapepiecej = EtapeDeSaisie.Idetape,
							Idfacturepiecej = FactureAModifier.Idfacture,
							Nompiecej = item.Nom,
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

		public async Task<AffichageFactureAvecPJ> ObtenirDetailsFacture(int IDFacture)
		{
			try
			{
				var EtapeDeSaisie = await _titreDeTransportContext.Etape.FirstOrDefaultAsync(item => item.Numeroetape == ConstantesEtapes.SAISI_FACTURE);
				if (EtapeDeSaisie is null)
				{
					throw new Exception("Etape non trouvée.");
				}

				var Resultat = await (from facture in _titreDeTransportContext.Facture
								join agence in _titreDeTransportContext.Agencevoyage on facture.Idagencevoyagefacture equals agence.Idagencevoyage
								where facture.Idfacture == IDFacture
								select new AffichageFactureAvecPJ()
								{
									IDAgenceVoyage = facture.Idagencevoyagefacture,
									DateSaisie = facture.Datesaisifacture,
									RefFacture = facture.Numerofactureagence,
									DateValidation = facture.Datevalfacture,
									DateEmission = facture.Dateagenceemissionfacture,
									IDEtape = facture.Idetapefacture,
									IDFacture = facture.Idfacture,
									IDRenvoi = facture.Idrenvoifacture,
									LibelleAgence = agence.Libelleagencevoyage,
									ListePiecesJointes = (from piecej in _titreDeTransportContext.Piecejointe
														  where piecej.Idfacturepiecej == IDFacture
														  select new FichierAfficher()
														  {
															  CheminAbsolu= _fichierService.ObtenirCheminAbsoluFichier(piecej.Nompiecej),
															  Nom = piecej.Nompiecej
														  }).ToList(),
									ListeTitres = (from titre in _titreDeTransportContext.Titredetransport
												   join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
												   join compagnie in _titreDeTransportContext.Compagnie on titre.Idcompagnie equals compagnie.Idcompagnie
												   join agence2 in _titreDeTransportContext.Agencevoyage on titre.Idagencevoyage equals agence2.Idagencevoyage
												   where titre.Idfacture == IDFacture
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
												   }).ToList()
								}).FirstAsync();

				return Resultat;
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
					NomAgentRenvoi = NomAgentRenvoi,
				};

				return DetailsDuRenvoi;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<AffichageFacture>> ObtenirListeFacture(SaisieFactureRechercheModel ModelDeRecherche)
		{
			try
			{
				var EtapeActuelle = _titreDeTransportContext.Etape.First(item => item.Numeroetape == ConstantesEtapes.SAISI_FACTURE);
				var EtapeValidation = _titreDeTransportContext.Etape.First(item => item.Numeroetape == ConstantesEtapes.VALID_FACTURE);

				var ResultatRequete = (from facture in _titreDeTransportContext.Facture
									   join agence in _titreDeTransportContext.Agencevoyage on facture.Idagencevoyagefacture equals agence.Idagencevoyage
									   select new AffichageFacture
									   {
										   DateSaisie = facture.Datesaisifacture,
										   DateValidation = facture.Datevalfacture,
										   IDEtape = facture.Idetapefacture,
										   LibelleAgence = agence.Libelleagencevoyage,
										   NombreDeBillets = facture.Titredetransport.Count(),
										   IDFacture = facture.Idfacture,
										   IDRenvoi = facture.Idrenvoifacture,
										   Observation = facture.Obserfacture
									   }
				);

				if (ModelDeRecherche.DateDebut.HasValue)
				{
					ResultatRequete = ResultatRequete.Where(fact => fact.DateSaisie >= ModelDeRecherche.DateDebut.Value);
				}

				if (ModelDeRecherche.DateFin.HasValue)
				{
					ResultatRequete = ResultatRequete.Where(fact => fact.DateSaisie <= ModelDeRecherche.DateFin.Value);
				}

				if (!string.IsNullOrEmpty(ModelDeRecherche.TermeDeRecherche))
				{
					ResultatRequete = ResultatRequete
						.Where(fact => EF.Functions.Like(fact.LibelleAgence, "%" + ModelDeRecherche.TermeDeRecherche + "%")
										|| EF.Functions.Like(fact.Observation, "%" + ModelDeRecherche.TermeDeRecherche + "%"));
				}

				if (ModelDeRecherche.EnCours.HasValue && ModelDeRecherche.EnCours == true)
				{
					ResultatRequete = ResultatRequete.Where(fact => fact.DateSaisie != null && fact.DateValidation == null && fact.IDEtape == EtapeActuelle.Idetape && fact.IDRenvoi == null);
				}

				if (ModelDeRecherche.Valides.HasValue && ModelDeRecherche.Valides == true)
				{
					ResultatRequete = ResultatRequete.Where(fact => fact.DateSaisie != null && fact.DateValidation != null && fact.IDEtape == EtapeValidation.Idetape);
				}

				if (ModelDeRecherche.Rejetes.HasValue && ModelDeRecherche.Rejetes == true)
				{
					ResultatRequete = ResultatRequete.Where(fact => fact.DateValidation == null && fact.IDRenvoi != null && fact.IDEtape == EtapeActuelle.Idetape);
				}

				return await ResultatRequete.ToListAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task SupprimerFacture(int IDFacture)
		{
			var transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var EtapeDeSaisie = _titreDeTransportContext.Etape.FirstOrDefault(item => item.Numeroetape == ConstantesEtapes.SAISI_FACTURE);
				if (EtapeDeSaisie == null)
				{
					throw new Exception("L' étape n'a pas été trouvée.");
				}

				var FactureASupprimer = await _titreDeTransportContext
					.Facture
					.SingleAsync(item => item.Idfacture == IDFacture);

				if (FactureASupprimer is null)
				{
					throw new Exception("La Facture est introuvable.");
				}

				var ListeDesPiecesJointes = _titreDeTransportContext.Piecejointe.Where(item => item.Idfacturepiecej == IDFacture);

				if (ListeDesPiecesJointes.Count() is not 0)
				{
					foreach (var item in ListeDesPiecesJointes)
					{
						_titreDeTransportContext.Piecejointe.Remove(item);
					}
				}

				_titreDeTransportContext.Facture.Remove(FactureASupprimer);
				_titreDeTransportContext.SaveChanges();
				transaction.Commit();
			}
			catch (Exception)
			{
				transaction.Rollback();
				throw;
			}
		}

		public async Task SupprimerPieceJointe(string NomPieceJointe)
		{
			try
			{
				var PieceJointeASupprimer = await _titreDeTransportContext.Piecejointe.SingleAsync(item => item.Nompiecej == NomPieceJointe);
				if (PieceJointeASupprimer is null)
				{
					throw new Exception("Piece Jointe Introuvable");
				}

				//TODO FAUT IL SUPPRIMER LES FICHIERS SUR LE SERVEUR
				//_uploadFichierService.SupprimerFichier(PieceJointeASupprimer.Nompiecej);

				_titreDeTransportContext.Piecejointe.Remove(PieceJointeASupprimer);
				await _titreDeTransportContext.SaveChangesAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<AffichageTitreAvecPJ>> ObtenirTitresAChoisir(int IDAgenceVoyage)
		{
			try
			{
				var EtapeActuelle = _titreDeTransportContext.Etape.First(item => item.Numeroetape == ConstantesEtapes.SAISI_FACTURE);

				var ResultatRequete = (from titre in _titreDeTransportContext.Titredetransport
									   join etape in _titreDeTransportContext.Etape on titre.Idetape equals etape.Idetape
									   join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
									   join compagnie in _titreDeTransportContext.Compagnie on titre.Idcompagnie equals compagnie.Idcompagnie
									   join agence in _titreDeTransportContext.Agencevoyage on titre.Idagencevoyage equals agence.Idagencevoyage
									   where titre.Idagencevoyage == IDAgenceVoyage
									   where titre.Datevaltitret != null
									   where titre.Matagentvaltitret != null
									   where etape.Numeroetape <= EtapeActuelle.Numeroetape 
									   where titre.Idfacture == null
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

		public async Task<List<AffichageTitreAvecPJ>> ObtenirTitresAChoisirModifier(int IDAgenceVoyage)
		{
			try
			{
				var EtapeActuelle = _titreDeTransportContext.Etape.First(item => item.Numeroetape == ConstantesEtapes.SAISI_FACTURE);

				var ResultatRequete = (from titre in _titreDeTransportContext.Titredetransport
									   join etape in _titreDeTransportContext.Etape on titre.Idetape equals etape.Idetape
									   join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
									   join compagnie in _titreDeTransportContext.Compagnie on titre.Idcompagnie equals compagnie.Idcompagnie
									   join agence in _titreDeTransportContext.Agencevoyage on titre.Idagencevoyage equals agence.Idagencevoyage
									   where titre.Idagencevoyage == IDAgenceVoyage
									   where titre.Datevaltitret != null
									   where titre.Matagentvaltitret != null
									   where etape.Numeroetape <= EtapeActuelle.Numeroetape
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
	}
}
