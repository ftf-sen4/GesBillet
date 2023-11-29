using System.ComponentModel.DataAnnotations;

namespace Gestions_des_Titres_de_Transport.Models
{
	public class RenvoiModel
	{
		[Required(ErrorMessage = "Entrez le Motif de renvoi."), MinLength(4, ErrorMessage = "4 Charactères minimum")]
		public string MotifDeRenvoi { get; set; }
	}

	public class AffichageRenvoi
	{
		public string Motifrenvoi { get; set; }
		public DateTime Daterenvoi { get; set; }
		public string NomAgentRenvoi { get; set; }
	}

	public class FichierAfficher
	{
		public string Nom { get; set; }
		public string CheminAbsolu { get; set; }
	}

	public class AffichageAgent
	{
		public int MatAgent { get; set; }
		public string Nom { get; set; }
		public string Poste { get; set; }
		public string Domaine { get; set; }
		public string Departement { get; set; }
	}
}
