using Microsoft.EntityFrameworkCore;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;

namespace Gestions_des_Titres_de_Transport.Services
{
    public class ClasseVoyageService : IClasseVoyageService
    {
        private readonly TitreDeTransportContext _context;

        public ClasseVoyageService(TitreDeTransportContext titreDeTransportContext)
        {
            _context = titreDeTransportContext;
        }

        public async Task<List<Classevoyage>> ObtenirClassesVoyage()
        {
            var resultat = await _context.Classevoyage.ToListAsync();

            return resultat;
        }
    }
}
