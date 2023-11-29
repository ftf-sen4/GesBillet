using Gestions_des_Titres_de_Transport.Constant;
using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Microsoft.EntityFrameworkCore;

namespace Gestions_des_Titres_de_Transport.Services.BER
{
	public class ValidationBERService : IValidationBERService
	{
		private readonly IFichierService _fichierService;
		private readonly IUtilisateurService _agentService;
		private readonly SAPCENTERDBContext _sapCenterDBContext;
		private readonly TitreDeTransportContext _titreDeTransportContext;

		public ValidationBERService(
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
		public async Task<AffichageBERAvecFacturesLier> ObtenirDetailsBER(int IDBEReglement)
		{
			try
			{
				var BEReglement = await _titreDeTransportContext.Bereglement.SingleAsync(item => item.Idbereglem == IDBEReglement);
				if (BEReglement is null)
				{
					throw new Exception("Le bon de passage est introuvable.");
				}

				var EtapeActuelle = _titreDeTransportContext.Etape.FirstOrDefault(item => item.Numeroetape == ConstantesEtapes.SAISI_BEREGLEMENT);
				if (EtapeActuelle is null)
				{
					throw new Exception("Etape non trouvée.");
				}
				var Resultats = await (from ber in _titreDeTransportContext.Bereglement
									   join agence in _titreDeTransportContext.Agencevoyage on ber.Idagencevoyageber equals agence.Idagencevoyage
									   where ber.Idbereglem == IDBEReglement
									   select new AffichageBERAvecFacturesLier
									   {
										   IDBEReglement = ber.Idbereglem,
										   DateSaisie = ber.Datesaiber,
										   DateValidation = ber.Datevalber,
										   IDEtape = ber.Idetapeber,
										   IDRenvoi = ber.Idrenvoiber,
										   IDAgenceVoyage = ber.Idagencevoyageber,
										   ListeFacturesLier = (from facture in _titreDeTransportContext.Facture
																join agence in _titreDeTransportContext.Agencevoyage on facture.Idagencevoyagefacture equals agence.Idagencevoyage
																where facture.Idbereglem == IDBEReglement
																select new AffichageFactureAvecPJ()
																{
																	IDAgenceVoyage = facture.Idagencevoyagefacture,
																	DateSaisie = facture.Datesaisifacture,
																	DateValidation = facture.Datevalfacture,
																	DateEmission = facture.Dateagenceemissionfacture,
																	IDEtape = facture.Idetapefacture,
																	IDFacture = facture.Idfacture,
																	IDRenvoi = facture.Idrenvoifacture,
																	LibelleAgence = agence.Libelleagencevoyage,
																	ListePiecesJointes = (from piecej in _titreDeTransportContext.Piecejointe
																						  where piecej.Idfacturepiecej == facture.Idfacture
																						  select new FichierAfficher()
																						  {
																							  CheminAbsolu = _fichierService.ObtenirCheminAbsoluFichier(piecej.Nompiecej),
																							  Nom = piecej.Nompiecej
																						  }).ToList(),
																	ListeTitres = new List<AffichageTitreAvecPJ>()
																}).ToList()
									   }).ToListAsync();

				return Resultats.First();
			}
			catch (Exception)
			{
				throw;
			}
		}
		public async Task<List<AffichageBER>> ObtenirListeBER(ValidationBERRechercheModel ModelDeRecherche)
		{
			try
			{
				var EtapeActuelle = _titreDeTransportContext.Etape.First(item => item.Numeroetape == ConstantesEtapes.VALID_BEREGLEMENT);

				var ResultatRequete = (from ber in _titreDeTransportContext.Bereglement
									   join agence in _titreDeTransportContext.Agencevoyage on ber.Idagencevoyageber equals agence.Idagencevoyage
									   join etape in _titreDeTransportContext.Etape on ber.Idetapeber equals etape.Idetape
									   select new AffichageBER
									   {
										   DateSaisie = ber.Datesaiber,
										   DateValidation = ber.Datevalber,
										   Observation = ber.Obserber,
										   IDBEReglement = ber.Idbereglem,
										   IDEtape = ber.Idetapeber,
										   IDRenvoi = ber.Idrenvoiber,
										   LibelleAgenceVoyage = agence.Libelleagencevoyage
									   }
				); ;

				if (ModelDeRecherche.DateDebut.HasValue)
				{
					ResultatRequete = ResultatRequete.Where(ber => ber.DateSaisie >= ModelDeRecherche.DateDebut.Value);
				}

				if (ModelDeRecherche.DateFin.HasValue)
				{
					ResultatRequete = ResultatRequete.Where(ber => ber.DateSaisie <= ModelDeRecherche.DateFin.Value);
				}

				if (!string.IsNullOrEmpty(ModelDeRecherche.TermeDeRecherche))
				{
					ResultatRequete = ResultatRequete
						.Where(ber => EF.Functions.Like(ber.IDBEReglement.ToString(), "%" + ModelDeRecherche.TermeDeRecherche + "%") ||
										 EF.Functions.Like(ber.LibelleAgenceVoyage.ToString(), "%" + ModelDeRecherche.TermeDeRecherche + "%"));
				}

				if (ModelDeRecherche.EnCours.HasValue && ModelDeRecherche.EnCours == true)
				{
					ResultatRequete = ResultatRequete.Where(ber => ber.DateSaisie != null && ber.DateValidation == null && ber.IDEtape == EtapeActuelle.Idetape && ber.IDRenvoi == null);
				}

				if (ModelDeRecherche.Valides.HasValue && ModelDeRecherche.Valides == true)
				{
					ResultatRequete = ResultatRequete.Where(ber => ber.DateSaisie != null && ber.DateValidation != null && ber.IDEtape == EtapeActuelle.Idetape);
				}

				return await ResultatRequete.ToListAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}
		public async Task RenvoyerBER(int IDBEReglement, string MotifDeRenvoi)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatAgentRenvoi = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var BEReglementARenvoyer = await _titreDeTransportContext.Bereglement.SingleAsync(ber => ber.Idbereglem == IDBEReglement);
				if (BEReglementARenvoyer is null)
				{
					throw new Exception("Le BE de règlement est introuvable.");
				}
				var IDEtapeActuelle = BEReglementARenvoyer.Idetapeber;
				var EtapeActuelle = _titreDeTransportContext.Etape.Single(item => item.Idetape == IDEtapeActuelle);
				var EtapePrecedente = await _titreDeTransportContext.Etape
					.Where(item => EtapeActuelle.Numeroetape > item.Numeroetape && item.Sietapeactive == true)
					.OrderByDescending(item => item.Numeroetape)
					.FirstOrDefaultAsync();

				if (EtapePrecedente is null)
				{
					throw new Exception("Pas d'étape précédente.");
				}

				var ListeFacturesLier = _titreDeTransportContext.Facture.Where(fact => fact.Idbereglem == IDBEReglement);
				foreach (var fact in ListeFacturesLier)
				{
					fact.Idetapefacture = EtapePrecedente.Idetape;
					var Titres = fact.Titredetransport;

					foreach (var titre in Titres)
					{
						titre.Idetape = EtapePrecedente.Idetape;
						_titreDeTransportContext.Titredetransport.Update(titre);
					}

					_titreDeTransportContext.Facture.Update(fact);
				}


				var RenvoiBER = new Renvoi();

				RenvoiBER.Motifrenvoi = MotifDeRenvoi;
				RenvoiBER.Daterenvoi = DateTime.Now;
				RenvoiBER.Matagentrenvoi = MatAgentRenvoi;
				RenvoiBER.Iddocument = BEReglementARenvoyer.Idbereglem;
				RenvoiBER.Typedocument = TypeDocument.BE_REGLEMENT;

				_titreDeTransportContext.Renvoi.Add(RenvoiBER);
				_titreDeTransportContext.SaveChanges();

				BEReglementARenvoyer.Idetapeber = EtapePrecedente.Idetape;
				BEReglementARenvoyer.Idrenvoiber = RenvoiBER.Idrenvoi;
				
				_titreDeTransportContext.Bereglement.Update(BEReglementARenvoyer);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}
		public async Task ValiderBER(int IDBEReglement)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatAgentValidation = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var BEReglementAValider = await _titreDeTransportContext.Bereglement.SingleAsync(ber => ber.Idbereglem == IDBEReglement);

				BEReglementAValider.Datevalber = DateTime.Now;
				BEReglementAValider.Matagentvalber = MatAgentValidation;

				_titreDeTransportContext.Bereglement.Update(BEReglementAValider);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}
		public async Task ValiderBER(List<int> ListeIDBEReglement)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatAgentValidation = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var ListeBEReglementAValider = _titreDeTransportContext.Bereglement.Where(ber => ListeIDBEReglement.Contains(ber.Idbereglem));


                foreach (var ber in ListeBEReglementAValider)
                {
					ber.Datevalber = DateTime.Now;
					ber.Matagentvalber = MatAgentValidation;
					_titreDeTransportContext.Bereglement.Update(ber);
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
	}
}
