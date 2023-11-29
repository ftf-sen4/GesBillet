using System.ComponentModel.DataAnnotations;

namespace Gestions_des_Titres_de_Transport.Models
{
	public class CreerCompagnieModel
	{
		[Required(ErrorMessage = "Veuillez entrer le nom de libellé de la compagnie.")]
		public string LibelleCompagnie{ get; set; }
	}


	public class AffichageCompagnie
	{
		public int Idcompagnie { get; set; }

		public string Libellecompagnie { get; set; }

		public string NomAgentEnr { get; set; }

		public DateTime Dateagentenrcompagnie { get; set; }
	}

	public class RechercheCompagnieModel
	{
		public string TermeDeRecherche { get; set; }
	}
}
