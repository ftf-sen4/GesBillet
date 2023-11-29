using Gestions_des_Titres_de_Transport.Constant;
using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.SAPCENTERDBModelsEF;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;
using Microsoft.AspNetCore.Components.Forms;

namespace Gestions_des_Titres_de_Transport.Services.Avoir
{
	public class SaisieAvoirService : ISaisieAvoirService
	{
		private readonly IFichierService _fichierService;
		private readonly IUtilisateurService _agentService;
		private readonly SAPCENTERDBContext _sapCenterDBContext;
		private readonly TitreDeTransportContext _titreDeTransportContext;

		public SaisieAvoirService(
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


		public async Task CreerAvoir(List<IBrowserFile> FichierJoints, CreerAvoirModel Model)
		{
			var Transaction = _titreDeTransportContext.Database.BeginTransaction();
			try
			{
				var EtapeDeSaisie = _titreDeTransportContext.Etape.FirstOrDefault(item => item.Numeroetape == ConstantesEtapes.SAISI_AVOIR);
				if (EtapeDeSaisie is null)
				{
					throw new Exception("L' étape n'a pas été trouvée.");
				}
				var MatAgentSaisie = await _agentService.ObtenirMatriculeUtilisateurConnecte();


				//Vérifier les références des titres
				//var ListeReferencesTitres = 


				var AvoirAEnregistrer = new TitreDeTransportModelsEF.Avoir()
				{
					Dateagenceemissionavoir = Model.DateEmission,
					Datesaisiavoir = DateTime.Now,
					Idagencedevoyageavoir = Model.IDAgenceVoyage,
					Idetapeavoir = EtapeDeSaisie.Idetape,
					Matagentsaiavoir = MatAgentSaisie,
					Numeroagenceavoir = Model.RefAvoir,
					Obseravoir = Model.Observation,
				};


				_titreDeTransportContext.Avoir.Add(AvoirAEnregistrer);
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
