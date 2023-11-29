using Microsoft.AspNetCore.Components.Forms;
using Gestions_des_Titres_de_Transport.Services.Contrats;

namespace Gestions_des_Titres_de_Transport.Services
{
	public class FichierService : IFichierService
	{
		private readonly IConfiguration _config;
		public FichierService(IConfiguration config)
		{
			_config = config;
		}

		public void SupprimerFichier(string NomFichier)
		{
			try
			{
				var CheminAbsolu = Path.Combine(_config.GetValue<string>("FileStorage")!, NomFichier);

				if (!File.Exists(CheminAbsolu))
				{
					throw new Exception("Le Fichier n'existe pas");
				}

				File.Delete(CheminAbsolu);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public string ObtenirCheminAbsoluFichier(string NomFichier)
		{
			var CheminAbsoluDuFichier = Path.Combine(_config.GetValue<string>("FileStorage")!, NomFichier);
			return CheminAbsoluDuFichier;

		}
		public async Task<List<Fichier>> EnregistrerFichierSurServeur(List<IBrowserFile> ListeDeFichiers)
		{
			try
			{
				var listeDeFichiersARenvoyer = new List<Fichier>();
				foreach (var item in ListeDeFichiers)
				{
					var Fichier = new Fichier();

					var RepertoireDeFichiersJoints = Path.Combine(_config.GetValue<string>("FileStorage")!);

					var NouveauNomFichier = Path.ChangeExtension($"{Path.GetFileNameWithoutExtension(item.Name)}_{Path.GetRandomFileName()}", Path.GetExtension(item.Name));
					var CheminAbsolu = Path.Combine(_config.GetValue<string>("FileStorage")!, NouveauNomFichier);
					var CheminRelatif = Path.Combine(NouveauNomFichier);

					if (!Directory.Exists(RepertoireDeFichiersJoints))
					{
						Directory.CreateDirectory(Path.Combine(_config.GetValue<string>("FileStorage")!));
					}

					FileStream filestream = new FileStream(CheminAbsolu, FileMode.Create, FileAccess.Write);
					await item.OpenReadStream(long.MaxValue).CopyToAsync(filestream);
					filestream.Close();

					Fichier.CheminRelatif = CheminRelatif;
					Fichier.Nom = NouveauNomFichier;
					listeDeFichiersARenvoyer.Add(Fichier);
				}
				return listeDeFichiersARenvoyer;
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
