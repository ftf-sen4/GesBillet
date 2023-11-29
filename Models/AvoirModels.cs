using System.ComponentModel.DataAnnotations;

namespace Gestions_des_Titres_de_Transport.Models
{
	public class CreerAvoirModel
	{
		[Required(ErrorMessage = "Choisissez l'agence de voyage.")]
		public int IDAgenceVoyage { get; set; }

		[Required(ErrorMessage = "Entrez la référence d'avoir.")]
		public string RefAvoir { get; set; }
		public DateTime DateEmission { get; set; }
		public string? Observation { get; set; }
		public List<LigneRemboursement> Remboursements { get; set; } = new List<LigneRemboursement>();
	}

	public class LigneRemboursement
	{
		[Required(ErrorMessage = "La référence est obligatoire.")]
		public string RefTitre { get; set; }
		[Required(ErrorMessage = "Entrez le montant.")]

		public decimal MontantRemb { get; set; }
	}

	public class AffichageAvoir
	{
		public int IDAvoir { get; set; }
		public string RefAvoir { get; set; }
		public int IDEtape { get; set; }
		public int? IDRenvoi { get; set; }
		public string Observation { get; set; }
		public string LibelleAgence { get; set; }
		public DateTime? DateSaisie { get; set; }
		public DateTime? DateEmission { get; set; }
		public DateTime? DateValidation { get; set; }
	}
	public class AffichageAvoirAvecPJ
	{
		public int IDAvoir;
		public string RefAvoir { get; set; }
		public string LibelleAgence { get; set; }
		public DateTime? DateEmission { get; set; }
		public int IDEtape { get; set; }
		public int? IDRenvoi { get; set; }
		public string Observation { get; set; }
		public DateTime? DateSaisie { get; set; }
		public DateTime? DateValidation { get; set; }
		public List<FichierAfficher> ListePiecesJointes { get; set; } = new List<FichierAfficher>();
	}

	public class SaisieAvoirRechercheModel
	{
		//public bool? EnCours { get; set; }
		//public bool? Valides { get; set; }
		//public bool? Rejetes { get; set; }
		public DateTime? DateFin { get; set; }
		public DateTime? DateDebut { get; set; }
		public string? TermeDeRecherche { get; set; }
	}
}
