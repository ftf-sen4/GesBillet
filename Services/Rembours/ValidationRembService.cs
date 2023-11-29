using Gestions_des_Titres_de_Transport.Models;
using Gestions_des_Titres_de_Transport.Services.Contrats;

namespace Gestions_des_Titres_de_Transport.Services.Rembours
{
	public class ValidationRembService : IValidationRembService
	{
		public Task EnvoiEtapeSup(int IDRemb)
		{
			throw new NotImplementedException();
		}

		public Task EnvoiEtapeSup(List<int> ListeIDRemb)
		{
			throw new NotImplementedException();
		}

		public Task<List<AffichageRemboursement>> ObtenirListeRemb(SaisieRembRechercheModel ModelDeRecherche)
		{
			throw new NotImplementedException();
		}

		public Task RenvoyerRemb(int IDRemb)
		{
			throw new NotImplementedException();
		}

		public Task ValiderRemb(int IDRemb)
		{
			throw new NotImplementedException();
		}

		public Task ValiderRemb(List<int> ListeIDRemb)
		{
			throw new NotImplementedException();
		}
	}
}
