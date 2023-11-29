using Microsoft.EntityFrameworkCore;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;

namespace Gestions_des_Titres_de_Transport.Services
{
    public class AgenceDeVoyageService : IAgenceDeVoyageService
    {
        private readonly TitreDeTransportContext _context;

        public AgenceDeVoyageService(TitreDeTransportContext titreDeTransportContext)
        {
            _context = titreDeTransportContext;
        }

        public async Task<List<Agencevoyage>> ObtenirAgencesDeVoyages()
        {
            var resultat = await _context.Agencevoyage.ToListAsync();

            return resultat;
        }
    }
}
