using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.OrdreDeMissionDBModelsEF;
using Microsoft.EntityFrameworkCore;

namespace Gestions_des_Titres_de_Transport.Services
{
	public class FicheDeMissionService : IFicheDeMissionService
	{
		private readonly OrdreDeMissionContext _ordreDeMissionContext;
		public FicheDeMissionService(OrdreDeMissionContext ordreDeMissionContext)
		{
			_ordreDeMissionContext = ordreDeMissionContext;
		}
		public async Task<AffichageMission> GetFicheDeMission(string ReferenceFicheMission)
		{
			try
			{
				if (ReferenceFicheMission == null || ReferenceFicheMission.Length < 1)
				{
					throw new Exception("Entrez une référence de fiche de Mission correcte.");
				}

				List<Routing3> ListeDesRouting3DeTypeAvion = await _ordreDeMissionContext.Routing3
					.Where(item => item.RefFiche == ReferenceFicheMission && EF.Functions.Like(item.MoyenTransport, "%avion%"))
					.ToListAsync();

				if (ListeDesRouting3DeTypeAvion.Count <= 1)
				{
					throw new Exception("Fiche de mission introuvable.");
				}

				var DetailsFicheMission = await _ordreDeMissionContext.FicheDeMissionR.SingleAsync(item => item.RefFiche == ReferenceFicheMission);

				if (DetailsFicheMission.Etape <= 20)
				{
					throw new Exception("La Fiche de Mission n'a pas encore été validée.");
				}

				var Routing1Mision = await _ordreDeMissionContext.Routing1.SingleOrDefaultAsync(item => item.RefFiche == ReferenceFicheMission);
				var Routing2Mision = await _ordreDeMissionContext.Routing2.SingleOrDefaultAsync(item => item.RefFiche == ReferenceFicheMission);
				var FicheMission = await _ordreDeMissionContext.FicheDeMission.SingleOrDefaultAsync(item => item.RefFiche == ReferenceFicheMission);

				var detailsMission = new AffichageMission()
				{
					IdAgentBeneficiaire = FicheMission?.Idagent,
					NomAgentBeneficiaire = DetailsFicheMission.NomEtPrénoms,
					ObjetMission = DetailsFicheMission.Objet,
					DateDeDepartVoyage = Routing1Mision?.DateDepart,
					DateRetourVoyage = Routing1Mision?.DateRetour,
					NomCompagnie = ListeDesRouting3DeTypeAvion[0].Transporteur,
					Classe = ListeDesRouting3DeTypeAvion[0].DetailTransporteur,
					MontantDuBillet = Routing1Mision?.MontantBillet,
					Routing = Routing1Mision?.Routing
				};
				return detailsMission;
			}
			catch (Microsoft.Data.SqlClient.SqlException exception)
			{
				throw new Exception(exception.Message);
			}

			catch (Exception)
			{
				throw;
			}

		}

		public async Task<bool> VerifierFicheMission(string ReferenceFiche)
		{
			try
			{

				List<Routing3> ListeDesRouting3DeTypeAvion = await _ordreDeMissionContext.Routing3
					.Where(item => item.RefFiche == ReferenceFiche && EF.Functions.Like(item.MoyenTransport, "%avion%"))
					.ToListAsync();

				if (ListeDesRouting3DeTypeAvion.Count <= 1)
				{
					return false;
				}

				var DetailsFicheMission = await _ordreDeMissionContext.FicheDeMissionR.SingleAsync(item => item.RefFiche == ReferenceFiche);

				if (DetailsFicheMission.Etape <= 20)
				{
					return false;
				}

				return true;
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
