using Gestions_des_Titres_de_Transport.Constant;
using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Microsoft.EntityFrameworkCore;

namespace Gestions_des_Titres_de_Transport.Services.Rembours
{
	public class SaisieRembService : ISaisieRembService
	{
		private readonly IFichierService _fichierService;
		private readonly IUtilisateurService _agentService;
		private readonly SAPCENTERDBContext _sapCenterDBContext;
		private readonly TitreDeTransportContext _titreDeTransportContext;

		public SaisieRembService(
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
		public async Task CreerRemboursement(CreerRemboursementModel Model)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var EtapeSaisieRemb = await _titreDeTransportContext.Etape.SingleAsync(item => item.Numeroetape == ConstantesEtapes.SAISI_REMBOURSEMENT);
				var TitreDeTransportAMettreAjour = await _titreDeTransportContext.Titredetransport.SingleAsync(titre => titre.Reftitret == Model.RefTitre);

				var NouveauRemboursement = new Remboursement()
				{
					Datesairemb = DateTime.Now,
					Matagentsairemb = MatAgentSaisie,
					Idetaperemb = EtapeSaisieRemb.Idetape,
					Sirembourse = false,
					Siremboursable = true,
					Observationremb = Model.Observation,
					Routingnonutiliser = Model.RoutingNonUtiliser
				};

				_titreDeTransportContext.Remboursement.Add(NouveauRemboursement);
				_titreDeTransportContext.SaveChanges();

				TitreDeTransportAMettreAjour.Idremb = NouveauRemboursement.Idremb;
				_titreDeTransportContext.Titredetransport.Update(TitreDeTransportAMettreAjour);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}

		public Task EnvoiEtapeSup(int IDRemb)
		{
			throw new NotImplementedException();
		}

		public async Task EnvoiEtapeSup(List<int> ListeIDRemb)
		{
			throw new NotImplementedException();
		}

		public async Task ModifierRemboursement(int IDRemb, CreerRemboursementModel Model)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var MatAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var EtapeSaisieRemb = await _titreDeTransportContext.Etape.SingleAsync(item => item.Numeroetape == ConstantesEtapes.SAISI_REMBOURSEMENT);
				var RembAModifier = await _titreDeTransportContext.Remboursement.SingleAsync(remb => remb.Idremb == IDRemb);
				var AncienTitreDeTransportLier = await _titreDeTransportContext.Titredetransport.SingleAsync(titre => titre.Idremb == IDRemb);
				var NouveauTitreDeTransportLier = await _titreDeTransportContext.Titredetransport.SingleAsync(titre => titre.Reftitret == Model.RefTitre);

				AncienTitreDeTransportLier.Idremb = null;
				_titreDeTransportContext.Titredetransport.Update(AncienTitreDeTransportLier);
				_titreDeTransportContext.SaveChanges();

				RembAModifier.Datesairemb = DateTime.Now;
				RembAModifier.Matagentsairemb = MatAgentSaisie;
				RembAModifier.Idetaperemb = EtapeSaisieRemb.Idetape;
				RembAModifier.Sirembourse = false;
				RembAModifier.Siremboursable = true;
				RembAModifier.Observationremb = Model.Observation;
				RembAModifier.Routingnonutiliser = Model.RoutingNonUtiliser;

				_titreDeTransportContext.Remboursement.Update(RembAModifier);

				NouveauTitreDeTransportLier.Idremb = IDRemb;
				_titreDeTransportContext.Titredetransport.Update(NouveauTitreDeTransportLier);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
		}

		public async Task<AffichageRemboursement> ObtenirDetailsRemb(int IDRemb)
		{
			try
			{
				var Resultat = (from titre in _titreDeTransportContext.Titredetransport
								join remb in _titreDeTransportContext.Remboursement on titre.Idremb equals remb.Idremb
								select new AffichageRemboursement
								{
									DateSaisie = remb.Datesairemb,
									Observation = remb.Observationremb,
									IDEtape = remb.Idetaperemb,
									IDRemb = remb.Idremb,
									IDTitre = titre.Idtitre,
									RefTitre = titre.Reftitret,
									RoutingInitial = titre.Routing,
									RoutingNonUtiliser = remb.Routingnonutiliser
								});

				return await Resultat.FirstAsync();
			}
			catch(Exception)
			{
				throw;
			}
		}

		public Task<AffichageRenvoi> ObtenirDetailsRenvoi(int IDRenvoi)
		{
			throw new NotImplementedException();
		}

		public async Task<List<AffichageRemboursement>> ObtenirListeRemb(SaisieRembRechercheModel ModelDeRecherche)
		{
			try
			{
				var EtapeActuelle = _titreDeTransportContext.Etape.Single(item => item.Numeroetape == ConstantesEtapes.SAISI_BEREGLEMENT);
				//var EtapeValidation = _titreDeTransportContext.Etape.First(item => item.Numeroetape == ConstantesEtapes.VALID_BEREGLEMENT);

				var ResultatRequete = (from remb in _titreDeTransportContext.Remboursement
									   join titre in _titreDeTransportContext.Titredetransport on remb.Idremb equals titre.Idremb
									   select new AffichageRemboursement
									   {
										   DateSaisie = remb.Datesairemb,
										   Observation = remb.Observationremb,
										   IDRemb = remb.Idremb,
										   IDTitre = titre.Idtitre,
										   RoutingInitial = titre.Routing,
										   RoutingNonUtiliser = remb.Routingnonutiliser,
										   RefTitre = titre.Reftitret
									   });

				if (ModelDeRecherche.DateDebut.HasValue)
				{
					ResultatRequete = ResultatRequete.Where(remb => remb.DateSaisie >= ModelDeRecherche.DateDebut.Value);
				}

				if (ModelDeRecherche.DateFin.HasValue)
				{
					ResultatRequete = ResultatRequete.Where(remb => remb.DateSaisie <= ModelDeRecherche.DateFin.Value);
				}

				if (!string.IsNullOrEmpty(ModelDeRecherche.TermeDeRecherche))
				{
					ResultatRequete = ResultatRequete
						.Where(remb => EF.Functions.Like(remb.RefTitre.ToString(), "%" + ModelDeRecherche.TermeDeRecherche + "%") ||
										EF.Functions.Like(remb.RoutingInitial.ToString(), "%" + ModelDeRecherche.TermeDeRecherche + "%") ||
										 EF.Functions.Like(remb.RoutingNonUtiliser.ToString(), "%" + ModelDeRecherche.TermeDeRecherche + "%") ||
										 EF.Functions.Like(remb.Observation, "%" + ModelDeRecherche.TermeDeRecherche + "%")
										 );
				}

				//if (ModelDeRecherche.EnCours.HasValue && ModelDeRecherche.EnCours == true)
				//{
				//	ResultatRequete = ResultatRequete.Where(remb => remb.DateSaisie != null && remb.DateValidation == null && remb.IDEtape == EtapeActuelle.Idetape && remb.IDRenvoi == null);
				//}

				//if (ModelDeRecherche.Valides.HasValue && ModelDeRecherche.Valides == true)
				//{
				//	ResultatRequete = ResultatRequete.Where(remb => remb.DateSaisie != null && remb.DateValidation != null && remb.IDEtape == EtapeValidation.Idetape);
				//}

				return await ResultatRequete.ToListAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

        public async Task SupprimerRemboursement(int IDRemb)
        {
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var RemboursementActif = await _titreDeTransportContext.Remboursement.SingleAsync(remb => remb.Idremb == IDRemb);
				var TitreLierAuRembActif = await _titreDeTransportContext.Titredetransport.SingleAsync(titre => titre.Idremb == IDRemb);

				TitreLierAuRembActif.Idremb = null;
				_titreDeTransportContext.Titredetransport.Update(TitreLierAuRembActif);
				_titreDeTransportContext.SaveChanges();


				_titreDeTransportContext.Remboursement.Remove(RemboursementActif);
				_titreDeTransportContext.SaveChanges();
				Transaction.Commit();
			}
			catch (Exception)
			{
				Transaction.Rollback();
				throw;
			}
        }
		public async Task<string> VerifierRefTitre(string RefTitre)
		{
			try
			{
				var TitreARechercher = await _titreDeTransportContext.Titredetransport.SingleAsync(titre => titre.Reftitret == RefTitre);
				if (TitreARechercher is null)
				{
					throw new Exception("Aucun titre ne porte cette référence. Rééssayez.");
				}

				return TitreARechercher.Routing;
			} 
			catch (Exception)
			{
				throw;
			}
		}
	}
}
