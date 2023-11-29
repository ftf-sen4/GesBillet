using Microsoft.AspNetCore.Components.Forms;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
    public class Fichier
    {
        public string Nom { get; set; }
        public string CheminRelatif { get; set; }
    }

	public interface IFichierService
    {
		public void SupprimerFichier(string NomFichier);
        public string ObtenirCheminAbsoluFichier(string NomFichier);
        public Task<List<Fichier>> EnregistrerFichierSurServeur(List<IBrowserFile> LiteFichiers);
	}
}
