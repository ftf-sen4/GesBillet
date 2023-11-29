using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Microsoft.EntityFrameworkCore;

namespace Gestions_des_Titres_de_Transport.Services
{
	public class GestionCompagnieService : IGestionCompagnieService
	{
		private readonly IUtilisateurService _agentService;
		private readonly SAPCENTERDBContext _sapCenterDBContext;
		private readonly TitreDeTransportContext _titreDeTransportContext;

		public GestionCompagnieService(
			TitreDeTransportContext titreDeTransportContext,
	IUtilisateurService agentService,
	SAPCENTERDBContext sapCenterDBContext
			)
		{
			_agentService = agentService;
			_sapCenterDBContext = sapCenterDBContext;
			_titreDeTransportContext = titreDeTransportContext;
		}

		public async Task CreerCompagnie(string LibelleCompagnie)
		{
			try
			{
				var MatAgenCreation = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var NouvelleCompagnie = new Compagnie()
				{
					Dateagentenrcompagnie = DateTime.Now,
					Libellecompagnie = LibelleCompagnie,
					Matagentenrcomapgnie = MatAgenCreation
				};

				_titreDeTransportContext.Compagnie.Add(NouvelleCompagnie);
				_titreDeTransportContext.SaveChanges();
			}catch(Exception) {
				throw;
			}
		}

		public async Task ModifierCompagnie(int IDCompagnie, string NouveauLibelle)
		{
			try
			{
				var MatAgenCreation = await _agentService.ObtenirMatriculeUtilisateurConnecte();

				var CompagnieAModifier = await _titreDeTransportContext.Compagnie.SingleAsync(cmp => cmp.Idcompagnie == IDCompagnie);

				CompagnieAModifier.Libellecompagnie = NouveauLibelle;

				_titreDeTransportContext.Compagnie.Update(CompagnieAModifier);
				_titreDeTransportContext.SaveChanges();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<AffichageCompagnie> ObtenirDetailsCompagnie(int IDCompagnie)
		{
			try
			{
				var MatAgenCreation = await _agentService.ObtenirMatriculeUtilisateurConnecte();

				var CompagnieARechercher = await _titreDeTransportContext.Compagnie.SingleAsync(cmp => cmp.Idcompagnie == IDCompagnie);

				var DetailsCompagnie = new AffichageCompagnie()
				{
					Libellecompagnie = CompagnieARechercher.Libellecompagnie,
					Dateagentenrcompagnie = CompagnieARechercher.Dateagentenrcompagnie,
					Idcompagnie = CompagnieARechercher.Idcompagnie,
					NomAgentEnr = _sapCenterDBContext.Agentssap.Single(age => age.Matricule == CompagnieARechercher.Matagentenrcomapgnie).NomEtPrenoms
				};

				return DetailsCompagnie;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<AffichageCompagnie>> ObtenirListeCompagnie(RechercheCompagnieModel ModelDeRecherche)
		{
			try
			{
				var ResultatRequete = (from compagnie in _titreDeTransportContext.Compagnie
									   select new AffichageCompagnie()
									   {
										  Dateagentenrcompagnie = compagnie.Dateagentenrcompagnie,
										  Idcompagnie = compagnie.Idcompagnie,
										  Libellecompagnie = compagnie.Libellecompagnie,
										  NomAgentEnr = ""
									   });

			
				if (!string.IsNullOrEmpty(ModelDeRecherche.TermeDeRecherche))
				{
					ResultatRequete = ResultatRequete
						.Where(cmp => EF.Functions.Like(cmp.Libellecompagnie, "%" + ModelDeRecherche.TermeDeRecherche + "%")
										);
				}

				return await ResultatRequete.ToListAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task SupprimerCompagnie(int IDCompagnie)
		{
			try
			{
				var CompagnieASupprimer = await _titreDeTransportContext.Compagnie.SingleAsync(cmp => cmp.Idcompagnie == IDCompagnie);
				_titreDeTransportContext.Compagnie.Remove(CompagnieASupprimer);
				_titreDeTransportContext.SaveChanges();
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
