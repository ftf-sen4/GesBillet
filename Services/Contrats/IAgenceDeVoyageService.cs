using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;


namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface IAgenceDeVoyageService
	{
		public Task<List<Agencevoyage>> ObtenirAgencesDeVoyages();
	}
}
