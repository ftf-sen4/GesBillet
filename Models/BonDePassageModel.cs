using System.ComponentModel.DataAnnotations;

namespace Gestions_des_Titres_de_Transport.Models
{
	public class CreerBonDePassageModel
	{
		[Required(ErrorMessage = "Choisissez l'agence de voyage.")]
		public int IDAgenceVoyage { get; set; }
		public string? Observation { get; set; }
		[Required(ErrorMessage = "Choisissez les titres à ajouter au bon de passage."), MinLength(1, ErrorMessage = "Sélectionnez au moins un titre.")]
		public List<int> ListeDesIDTitresChoisis { get; set; } = new List<int>();
	}

	public class AffichageBonAvecTitresLier
	{
		public int IDAgenceVoyage { get; set; }
		public int? IDRenvoi { get; set; }
		public string? Observation { get; set; }
		public List<AffichageTitreAvecPJ> ListeTitres { get; set; } = new List<AffichageTitreAvecPJ>();
	}

	public class SaisieBonRechercheModel
	{
		public bool? EnCours { get; set; }
		public bool? Valides { get; set; }
		public bool? Rejetes { get; set; }
		public bool? NonValides { get; set; }
		public DateTime? DateFin { get; set; }
		public DateTime? DateDebut { get; set; }
		public string? TermeDeRecherche { get; set; }
	}

	public class ValidationBonRechercheModel
	{
		public bool? EnCours { get; set; }
		public bool? Valides { get; set; }
		public bool? Rejetes { get; set; }
		public bool? NonValides { get; set; }
		public DateTime? DateFin { get; set; }
		public DateTime? DateDebut { get; set; }
		public string? TermeDeRecherche { get; set; }
	}

	public class ImprimerBonModel {
		public int IDBonDePassage { get; set; }
		public int NombreDeBillets { get; set; }
		public DateTime? DateSaisie { get; set; }
		public string NomSignataire { get; set; }
		public string FonctionSignataire { get; set; }
		public string LibelleAgence { get; set; }
		public List<LignePassagerBon> ListeLigne { get; set; } = new List<LignePassagerBon>();
	}

	public class LignePassagerBon
	{
		public string NomPassager { get; set; }
		public string Routing { get; set; }
		public string Classe { get; set; }
		public DateTime? DateDepart { get; set; }
		public DateTime? DateDeRetour { get; set; }
		public int MatBeneficiaire { get; set; }
	}
	public class AffichageBon
	{
		public int IDBondePassage { get; set; }
		public int IDEtape { get; set; }
		public int? IDRenvoi { get; set; }
		public string LibelleAgenceDeVoyage { get; set; } = string.Empty;
		public int NombreDeBillets { get; set; }
		public string Observation { get; set; } = string.Empty;
		public DateTime? DateDeSaisie { get; set; }
		public DateTime? DateValidation { get; set; }
	}
}
