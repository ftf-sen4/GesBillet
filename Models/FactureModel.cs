using System.ComponentModel.DataAnnotations;
using Gestions_des_Titres_de_Transport.TitreDeTransportModelsEF;

namespace Gestions_des_Titres_de_Transport.Models
{
	public class CreerFactureModel
	{
		[Required(ErrorMessage = "Choisissez l'agence de voyage.")]
		public int IDAgenceDeVoyage { get; set; }

		[Required]
		public DateTime DateEmission { get; set; }

		[Required]
		public string RefFacture { get; set; }

		public string? Observation { get; set; }

		[Required(ErrorMessage = "Choisissez les titres à ajouter à la facture.")]
		public List<int> ListeDesIDTitresChoisis { get; set; } = new List<int>();
	}

	public class SaisieFactureRechercheModel
	{
		public bool? EnCours { get; set; }
		public bool? Valides { get; set; }
		public bool? Rejetes { get; set; }
		public bool? NonValides { get; set; }
		public DateTime? DateFin { get; set; }
		public DateTime? DateDebut { get; set; }
		public string? TermeDeRecherche { get; set; }
	}

	public class ValidationFactureRechercheModel
	{
		public bool? EnCours { get; set; }
		public bool? Valides { get; set; }
		public bool? NonValides { get; set; }
		public DateTime? DateFin { get; set; }
		public DateTime? DateDebut { get; set; }
		public string? TermeDeRecherche { get; set; }
	}

	public class AffichageFacture
	{
		public int IDFacture { get; set; }
		public int IDEtape { get; set; }
		public int? IDRenvoi { get; set; }
		public string RefFacture { get; set; }
		public string LibelleAgence { get; set; }
		public int NombreDeBillets { get; set; }
		public string Observation { get; set; }
		public DateTime DateEmission { get; set; }
		public DateTime? DateSaisie { get; set; }
		public DateTime? DateValidation { get; set; }
	}

	public class AffichageFactureAvecPJ
	{
		public int IDFacture { get; set; }
		public int IDEtape { get; set; }
		public int? IDRenvoi { get; set; }
		public string RefFacture { get; set; }
		public int IDAgenceVoyage { get; set; }
		public string LibelleAgence { get; set; }
		public int NombreDeBillets { get; set; }
		public string Observation { get; set; }
		public DateTime DateEmission { get; set; }
		public DateTime? DateSaisie { get; set; }
		public DateTime? DateValidation { get; set; }
		public List<FichierAfficher> ListePiecesJointes { get; set; } = new List<FichierAfficher>();
		public List<AffichageTitreAvecPJ> ListeTitres { get; set; } = new List<AffichageTitreAvecPJ>();
	}
}
