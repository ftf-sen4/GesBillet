using Microsoft.EntityFrameworkCore;
using Gestions_des_Titres_de_Transport.Services.Contrats;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;

namespace Gestions_des_Titres_de_Transport.Services
{
    public class CompagnieService : ICompagnieService
    {
        private readonly TitreDeTransportContext _context;

        public CompagnieService(TitreDeTransportContext titreDeTransportContext)
        {
            _context = titreDeTransportContext;
        }

        public async Task<List<Compagnie>> ObtenirCompagnies()
        {
            var compagnies = await _context.Compagnie.ToListAsync();
            return compagnies;
        }
    }
}
