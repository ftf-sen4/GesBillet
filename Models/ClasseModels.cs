using System.ComponentModel.DataAnnotations;

namespace Gestions_des_Titres_de_Transport.Models
{
	public class CreerClasseModel
	{
		[Required(ErrorMessage = "Veuillez entrer le nom de libellé de la classe.")]
		public string LibelleClasse { get; set; }
	}


	public class AffichageClasse
	{
		public int Idclasse { get; set; }

		public string Libelleclasse { get; set; }

		public string NomAgentEnr { get; set; }

		public DateTime Dateagentenrclasse { get; set; }
	}

	public class RechercheClasseModel
	{
		public string TermeDeRecherche { get; set; }
	}
}
