using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Microsoft.EntityFrameworkCore;

namespace Gestions_des_Titres_de_Transport.Services
{
	public class GestionClasseService : IGestionClasseService
	{
		private readonly IUtilisateurService _agentService;
		private readonly SAPCENTERDBContext _sapCenterDBContext;
		private readonly TitreDeTransportContext _titreDeTransportContext;

		public GestionClasseService(
			TitreDeTransportContext titreDeTransportContext,
	IUtilisateurService agentService,
	SAPCENTERDBContext sapCenterDBContext
			)
		{
			_agentService = agentService;
			_sapCenterDBContext = sapCenterDBContext;
			_titreDeTransportContext = titreDeTransportContext;
		}

		public async Task CreerClasse(string LibelleClasse)
		{
			try
			{
				var MatAgenCreation = await _agentService.ObtenirMatriculeUtilisateurConnecte();
				var NouvelleClasse = new Classevoyage()
				{
					Dateagentenrclasse = DateTime.Now,
					Libelleclasse = LibelleClasse,
					Matagentenrclasse = MatAgenCreation
				};

				_titreDeTransportContext.Classevoyage.Add(NouvelleClasse);
				_titreDeTransportContext.SaveChanges();
			}catch(Exception) {
				throw;
			}
		}

		public async Task ModifierClasse(int IDClasse, string NouveauLibelle)
		{
			try
			{
				var MatAgenCreation = await _agentService.ObtenirMatriculeUtilisateurConnecte();

				var ClasseAModifier = await _titreDeTransportContext.Classevoyage.SingleAsync(age => age.Idclasse == IDClasse);

				ClasseAModifier.Libelleclasse = NouveauLibelle;

				_titreDeTransportContext.Classevoyage.Update(ClasseAModifier);
				_titreDeTransportContext.SaveChanges();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<AffichageClasse> ObtenirDetailsClasse(int IDClasse)
		{
			try
			{
				var MatAgenCreation = await _agentService.ObtenirMatriculeUtilisateurConnecte();

				var ClasseARechercher = await _titreDeTransportContext.Classevoyage.SingleAsync(age => age.Idclasse == IDClasse);

				var DetailsClasse = new AffichageClasse()
				{
					Libelleclasse = ClasseARechercher.Libelleclasse,
					Dateagentenrclasse = ClasseARechercher.Dateagentenrclasse,
					Idclasse = ClasseARechercher.Idclasse,
					NomAgentEnr = _sapCenterDBContext.Agentssap.Single(age => age.Matricule == ClasseARechercher.Matagentenrclasse).NomEtPrenoms
				};

				return DetailsClasse;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<AffichageClasse>> ObtenirListeClasse(RechercheClasseModel ModelDeRecherche)
		{
			try
			{
				var ResultatRequete = (from agence in _titreDeTransportContext.Classevoyage
									   select new AffichageClasse()
									   {
										  Dateagentenrclasse = agence.Dateagentenrclasse,
										  Idclasse = agence.Idclasse,
										  Libelleclasse = agence.Libelleclasse,
										  NomAgentEnr = ""
									   });

			
				if (!string.IsNullOrEmpty(ModelDeRecherche.TermeDeRecherche))
				{
					ResultatRequete = ResultatRequete
						.Where(clas => EF.Functions.Like(clas.Libelleclasse, "%" + ModelDeRecherche.TermeDeRecherche + "%")
										);
				}

				return await ResultatRequete.ToListAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task SupprimerClasse(int IDClasse)
		{
			try
			{
				var ClasseASupprimer = await _titreDeTransportContext.Classevoyage.SingleAsync(age => age.Idclasse == IDClasse);
				_titreDeTransportContext.Classevoyage.Remove(ClasseASupprimer);
				_titreDeTransportContext.SaveChanges();
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
