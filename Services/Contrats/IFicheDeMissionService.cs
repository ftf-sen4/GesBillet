using Gestions_des_Titres_de_Transport.Models;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
    public interface IFicheDeMissionService
    {
        public Task<AffichageMission> GetFicheDeMission(string ReferenceFicheMission);
        public Task<bool> VerifierFicheMission(string ReferenceFiche);
	}
}
