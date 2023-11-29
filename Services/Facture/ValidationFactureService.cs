using Gestions_des_Titres_de_Transport.Constant;
using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Microsoft.EntityFrameworkCore;

namespace Gestions_des_Titres_de_Transport.Services.Facture
{
	public class ValidationFactureService : IValidationFactureService
	{
		private readonly IFichierService _fichierService;
		private readonly IUtilisateurService _agentService;
		private readonly SAPCENTERDBContext _sapCenterDBContext;
		private readonly TitreDeTransportContext _titreDeTransportContext;

		public ValidationFactureService(
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
		public async Task<AffichageFactureAvecPJ> ObtenirDetailsFacture(int IDFacture)
		{
			try
			{
				var Resultat = await (from facture in _titreDeTransportContext.Facture
									  join agence in _titreDeTransportContext.Agencevoyage on facture.Idagencevoyagefacture equals agence.Idagencevoyage
									  where facture.Idfacture == IDFacture
									  select new AffichageFactureAvecPJ()
									  {
										  IDAgenceVoyage = facture.Idagencevoyagefacture,
										  DateSaisie = facture.Datesaisifacture,
										  DateValidation = facture.Datevalfacture,
										  IDEtape = facture.Idetapefacture,
										  IDFacture = facture.Idfacture,
										  IDRenvoi = facture.Idrenvoifacture,
										  RefFacture = facture.Numerofactureagence,
										  LibelleAgence = agence.Libelleagencevoyage,
										  ListePiecesJointes = (from piecej in _titreDeTransportContext.Piecejointe
																where piecej.Idfacturepiecej == facture.Idfacture
																select new FichierAfficher()
																{
																	CheminAbsolu = _fichierService.ObtenirCheminAbsoluFichier(piecej.Nompiecej),
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
		public async Task<List<AffichageFacture>> ObtenirListeFacture(
			ValidationFactureRechercheModel ModelDeRecherche)
		{
			try
			{
				var EtapeActuelle = _titreDeTransportContext.Etape.First(item => item.Numeroetape == ConstantesEtapes.VALID_FACTURE);

				var ResultatRequete = (from facture in _titreDeTransportContext.Facture
									   join etape in _titreDeTransportContext.Etape on facture.Idetapefacture equals etape.Idetape
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
					ResultatRequete = ResultatRequete.Where(fact => fact.DateSaisie != null && fact.DateValidation == null && fact.IDEtape == EtapeActuelle.Idetape);
				}

				if (ModelDeRecherche.Valides.HasValue && ModelDeRecherche.Valides == true)
				{
					ResultatRequete = ResultatRequete.Where(fact => fact.DateSaisie != null && fact.DateValidation != null && fact.IDEtape == EtapeActuelle.Idetape);
				}

				return await ResultatRequete.ToListAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task RenvoyerFacture(int IDFacture, string MotifDeRenvoi)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatAgentRenvoi = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var FactureARenvoyer = await _titreDeTransportContext.Facture.SingleAsync(item => item.Idfacture == IDFacture);

				var IDEtapeActuelle = FactureARenvoyer.Idetapefacture;
				var EtapeActuelle = await _titreDeTransportContext.Etape.SingleAsync(item => item.Idetape == IDEtapeActuelle);
				var EtapePrecedente = await _titreDeTransportContext.Etape
	.Where(item => EtapeActuelle.Numeroetape > item.Numeroetape && item.Sietapeactive == true)
	.OrderByDescending(item => item.Numeroetape)
	.FirstOrDefaultAsync();

				if (FactureARenvoyer is null)
				{
					throw new Exception("La facture est introuvable.");
				}

				var RenvoiFacture = new Renvoi();

				RenvoiFacture.Motifrenvoi = MotifDeRenvoi;
				RenvoiFacture.Daterenvoi = DateTime.Now;
				RenvoiFacture.Matagentrenvoi = MatAgentRenvoi;
				RenvoiFacture.Iddocument = FactureARenvoyer.Idfacture;
				RenvoiFacture.Typedocument = TypeDocument.FACTURE;

				_titreDeTransportContext.Renvoi.Add(RenvoiFacture);
				_titreDeTransportContext.SaveChanges();

				FactureARenvoyer.Idetapefacture = EtapePrecedente.Idetape;
				FactureARenvoyer.Idrenvoifacture = RenvoiFacture.Idrenvoi;


				var ListeTitre = _titreDeTransportContext.Titredetransport.Where(titre => titre.Idfacture == FactureARenvoyer.Idfacture);
				foreach (var item in ListeTitre)
				{
					item.Idetape = EtapePrecedente.Idetape;
				}

				_titreDeTransportContext.Titredetransport.UpdateRange(ListeTitre);
				_titreDeTransportContext.Facture.Update(FactureARenvoyer);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}

		public async Task ValiderFacture(int IDFacture)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatAgentValider = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var FactureAValider = _titreDeTransportContext.Facture.Single(item => item.Idfacture == IDFacture);

				FactureAValider.Datevalfacture = DateTime.Now;
				FactureAValider.Matagentvalfacture = MatAgentValider;

				_titreDeTransportContext.Facture.Update(FactureAValider);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}

		public async Task ValiderFacture(List<int> ListeIDFacture)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatAgentValider = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var ListeFacturesAValider = _titreDeTransportContext.Facture.Where(fact => ListeIDFacture.Contains(fact.Idfacture));

                foreach (var item in ListeFacturesAValider)
                {
					item.Datevalfacture = DateTime.Now;
					item.Matagentvalfacture = MatAgentValider;
                }

				_titreDeTransportContext.Facture.UpdateRange(ListeFacturesAValider);
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
