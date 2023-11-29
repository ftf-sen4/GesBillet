using System.ComponentModel.DataAnnotations;

namespace Gestions_des_Titres_de_Transport.Models
{
	public class CreerBEAvoirModel
	{
		[Required(ErrorMessage = "Choisissez l'agence de voyage.")]
		public int IDAgenceVoyage { get; set; }

		public string? Observation;

		[Required(ErrorMessage = "Choisissez les factures à ajouter au BE de règlement.")]
		public List<int> ListeDesIDAvoirsChoisis = new List<int>();
	}

	public class AffichageBEA
	{
		public int IDBEAvoir;
		public int IDEtape { get; set; }
		public int? IDRenvoi { get; set; }
		public string Observation { get; set; }
		public DateTime? DateSaisie { get; set; }
		public DateTime? DateValidation { get; set; }
	}
}
