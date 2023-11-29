using Gestions_des_Titres_de_Transport.Constant;
using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Microsoft.EntityFrameworkCore;

namespace Gestions_des_Titres_de_Transport.Services
{
	public class ValidationTitreService : IValidationTitreService
	{
		private readonly IFichierService _fichierService;
		private readonly IUtilisateurService _agentService;
		private readonly SAPCENTERDBContext _sapCenterDBContext;
		private readonly TitreDeTransportContext _titreDeTransportContext;

		public ValidationTitreService(
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

		public async Task<AffichageRenvoi> ObtenirDetailsRenvoi(int IDRenvoi)
		{
			try
			{
				var Resultat = await _titreDeTransportContext.Renvoi.SingleAsync(item => item.Idrenvoi == IDRenvoi);
				var NomAgentRenvoi = await _sapCenterDBContext.Agentssap.Select(agent => agent.NomEtPrenoms).SingleAsync();

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

		public async Task<List<AffichageTitre>> ObtenirListeTitre(ValiderTitreRechercheModel ModelDeRecherche)
		{
			try
			{
				var EtapeActuelle = await _titreDeTransportContext.Etape.SingleOrDefaultAsync(item => item.Numeroetape == ConstantesEtapes.VALID_TITRE);

				var ResultatRequete = (from titre in _titreDeTransportContext.Titredetransport
									   join etape in _titreDeTransportContext.Etape on titre.Idetape equals etape.Idetape
									   join classe in _titreDeTransportContext.Classevoyage on titre.Idclasse equals classe.Idclasse
									   join compagnie in _titreDeTransportContext.Compagnie on titre.Idcompagnie equals compagnie.Idcompagnie
									   join agence in _titreDeTransportContext.Agencevoyage on titre.Idagencevoyage equals agence.Idagencevoyage
									   select new AffichageTitre
									   {
										   IDTitre = titre.Idtitre,
										   AgenceVoyage = agence.Libelleagencevoyage,
										   ClasseVoyage = classe.Libelleclasse,
										   Compagnie = compagnie.Libellecompagnie,
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
										EF.Functions.Like(titre.ClasseVoyage, "%" + ModelDeRecherche.TermeDeRecherche + "%")
										);
				}

				if (ModelDeRecherche.EnCours.HasValue && ModelDeRecherche.EnCours == true)
				{
					ResultatRequete = ResultatRequete.Where(titre => titre.DateSaisiTitre != null && titre.DateValidTitre == null && titre.IDEtape == EtapeActuelle.Idetape && titre.IDRenvoi == null);
				}

				if (ModelDeRecherche.Valides.HasValue && ModelDeRecherche.Valides == true)
				{
					ResultatRequete = ResultatRequete.Where(titre => titre.DateSaisiTitre != null && titre.DateValidTitre != null && titre.IDEtape == EtapeActuelle.Idetape);
				}

				return await ResultatRequete.ToListAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task RenvoyerTitre(int IDTitre, string Motif)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatriculeAgent = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var TitreDeTransportARenvoyer = await _titreDeTransportContext
						.Titredetransport
						.SingleAsync(item => item.Idtitre == IDTitre);

				if (TitreDeTransportARenvoyer is null)
				{
					throw new Exception("Le Titre De Transport est introuvable.");
				}

				var IDEtapeActuelleDuTitreDeTransport = TitreDeTransportARenvoyer.Idetape;
				var EtapeActuelle = _titreDeTransportContext.Etape.Single(item => item.Idetape == IDEtapeActuelleDuTitreDeTransport);
				var EtapePrecedente = await _titreDeTransportContext.Etape
						.Where(item => EtapeActuelle.Numeroetape > item.Numeroetape && item.Sietapeactive == true)
						.OrderByDescending(item => item.Numeroetape)
						.FirstOrDefaultAsync();

				var RenvoiTitre = new Renvoi();

				RenvoiTitre.Motifrenvoi = Motif;
				RenvoiTitre.Daterenvoi = DateTime.Now;
				RenvoiTitre.Matagentrenvoi = MatriculeAgent;
				RenvoiTitre.Typedocument = TypeDocument.TITRE_DE_TRANSPORT;
				RenvoiTitre.Iddocument = TitreDeTransportARenvoyer.Idtitre;

				_titreDeTransportContext.Renvoi.Add(RenvoiTitre);
				_titreDeTransportContext.SaveChanges();

				TitreDeTransportARenvoyer.Idrenvoititret = RenvoiTitre.Idrenvoi;
				TitreDeTransportARenvoyer.Idetape = EtapePrecedente.Idetape;

				TitreDeTransportARenvoyer.Datevaltitret = null;
				TitreDeTransportARenvoyer.Matagentvaltitret = null;

				_titreDeTransportContext.Titredetransport.Update(TitreDeTransportARenvoyer);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}

		public async Task ValiderTitre(int IDTitre)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var Resultat = await _titreDeTransportContext.Titredetransport.SingleAsync(item => item.Idtitre == IDTitre);


				Resultat.Datevaltitret = DateTime.Now;
				Resultat.Matagentvaltitret = MatAgentSaisie;

				_titreDeTransportContext.Titredetransport.Update(Resultat);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception exception)
			{
				Transaction.Rollback();
				throw;
			}
		}

		public async Task ValiderTitre(List<int> ListeIDTitre)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var Resultat = _titreDeTransportContext.Titredetransport.Where(item => ListeIDTitre.Contains(item.Idtitre));

				foreach (var item in Resultat)
				{
					item.Datevaltitret = DateTime.Now;
					item.Matagentvaltitret = MatAgentSaisie;
				}

				_titreDeTransportContext.Titredetransport.UpdateRange(Resultat);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception exception)
			{
				Transaction.Rollback();
				throw;
			}
		}
	}
}
