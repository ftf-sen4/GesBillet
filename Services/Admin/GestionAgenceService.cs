using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Microsoft.EntityFrameworkCore;

namespace Gestions_des_Titres_de_Transport.Services
{
	public class GestionAgenceService : IGestionAgenceService
	{
		private readonly IUtilisateurService _agentService;
		private readonly SAPCENTERDBContext _sapCenterDBContext;
		private readonly TitreDeTransportContext _titreDeTransportContext;

		public GestionAgenceService(
			TitreDeTransportContext titreDeTransportContext,
	IUtilisateurService agentService,
	SAPCENTERDBContext sapCenterDBContext
			)
		{
			_agentService = agentService;
			_sapCenterDBContext = sapCenterDBContext;
			_titreDeTransportContext = titreDeTransportContext;
		}

		public async Task CreerAgence(string LibelleAgence)
		{
			try
			{
				var MatAgenCreation = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var NouvelleAgence = new Agencevoyage()
				{
					Dateagentenragencevoyage = DateTime.Now,
					Libelleagencevoyage = LibelleAgence,
					Matagentenragencevoyage = MatAgenCreation
				};

				_titreDeTransportContext.Agencevoyage.Add(NouvelleAgence);
				_titreDeTransportContext.SaveChanges();
			}catch(Exception) {
				throw;
			}
		}

		public async Task ModifierAgence(int IDAgence, string NouveauLibelle)
		{
			try
			{
				var MatAgenCreation = await _agentService.ObtenirMatriculeUtilisateurConnecte();

				var AgenceAModifier = await _titreDeTransportContext.Agencevoyage.SingleAsync(age => age.Idagencevoyage == IDAgence);

				AgenceAModifier.Libelleagencevoyage = NouveauLibelle;

				_titreDeTransportContext.Agencevoyage.Update(AgenceAModifier);
				_titreDeTransportContext.SaveChanges();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<AffichageAgence> ObtenirDetailsAgence(int IDAgence)
		{
			try
			{
				var MatAgenCreation = await _agentService.ObtenirMatriculeUtilisateurConnecte();

				var AgenceARechercher = await _titreDeTransportContext.Agencevoyage.SingleAsync(age => age.Idagencevoyage == IDAgence);

				var DetailsAgence = new AffichageAgence()
				{
					Libelleagencevoyage = AgenceARechercher.Libelleagencevoyage,
					Dateagentenragencevoyage = AgenceARechercher.Dateagentenragencevoyage,
					Idagencevoyage = AgenceARechercher.Idagencevoyage,
					NomAgentEnr = _sapCenterDBContext.Agentssap.Single(age => age.Matricule == AgenceARechercher.Matagentenragencevoyage).NomEtPrenoms
				};

				return DetailsAgence;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<AffichageAgence>> ObtenirListeAgence(RechercheAgenceModel ModelDeRecherche)
		{
			try
			{
				var ResultatRequete = (from agence in _titreDeTransportContext.Agencevoyage
									   select new AffichageAgence()
									   {
										  Dateagentenragencevoyage = agence.Dateagentenragencevoyage,
										  Idagencevoyage = agence.Idagencevoyage,
										  Libelleagencevoyage = agence.Libelleagencevoyage,
										  NomAgentEnr = ""
									   });

			
				if (!string.IsNullOrEmpty(ModelDeRecherche.TermeDeRecherche))
				{
					ResultatRequete = ResultatRequete
						.Where(age => EF.Functions.Like(age.Libelleagencevoyage, "%" + ModelDeRecherche.TermeDeRecherche + "%")
										);
				}

				return await ResultatRequete.ToListAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task SupprimerAgence(int IDAgence)
		{
			try
			{
				var AgenceASupprimer = await _titreDeTransportContext.Agencevoyage.SingleAsync(age => age.Idagencevoyage == IDAgence);
				_titreDeTransportContext.Agencevoyage.Remove(AgenceASupprimer);
				_titreDeTransportContext.SaveChanges();
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
