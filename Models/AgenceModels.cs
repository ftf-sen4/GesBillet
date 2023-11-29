using System.ComponentModel.DataAnnotations;

namespace Gestions_des_Titres_de_Transport.Models
{

	public class CreerAgenceModel
	{
		[Required(ErrorMessage = "Veuillez entrer le nom de libellé de l'agence.")]
		public string LibelleAgence { get; set; }
	}


	public class AffichageAgence
	{
		public int Idagencevoyage { get; set; }

		public string Libelleagencevoyage { get; set; }

		public string NomAgentEnr { get; set; }

		public DateTime Dateagentenragencevoyage { get; set; }
	}

	public class RechercheAgenceModel
	{
		public string TermeDeRecherche { get; set; }
	}
}
