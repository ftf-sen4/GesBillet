using Gestions_des_Titres_de_Transport.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface ISaisieAvoirService
	{
		public Task CreerAvoir(List<IBrowserFile> FichierJoints, CreerAvoirModel Model);
	}
}
