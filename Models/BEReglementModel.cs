using System.ComponentModel.DataAnnotations;

namespace Gestions_des_Titres_de_Transport.Models
{
	public class CreerBEReglementModel
	{
		[Required(ErrorMessage = "Choisissez l'agence de voyage.")]
		public int IDAgenceVoyage { get; set; }
		public string? Observation { get; set; }

		[Required(ErrorMessage = "Choisissez les factures à ajouter au BE de règlement.")]
		public List<int> ListeDesIDFacturesChoisis = new List<int>();
	}

	public class CreerBEReglementModelV2
	{
		[Required(ErrorMessage = "Choisissez l'agence de voyage.")]
		public int IDAgenceVoyage { get; set; }
		public string? Observation { get; set; }
		[Required]
		public string NomSignataire { get; set; }
	}

	public class AffichageBER
	{
		public int IDBEReglement { get; set; }
		public int IDAgenceVoyage { get; set; }
		public string LibelleAgenceVoyage { get; set; }
		public int IDEtape { get; set; }
		public int? IDRenvoi { get; set; }
		public string Observation { get; set; } = string.Empty;
		public DateTime? DateSaisie { get; set; }
		public DateTime? DateValidation { get; set; }
	}

	public class SaisieBERRechercheModel
	{
		public bool? EnCours { get; set; }
		public bool? Valides { get; set; }
		public bool? Rejetes { get; set; }
		public DateTime? DateFin { get; set; }
		public DateTime? DateDebut { get; set; }
		public string? TermeDeRecherche { get; set; }
	}

	public class BERFactureModel
	{
		public string RefFacture { get; set; }
		public DateTime DateEmission { get; set; }
		public List<AffichageTitreAutoCompletion> ListeTitres { get; set; } = new List<AffichageTitreAutoCompletion>();
	}

	public class ValidationBERRechercheModel
	{
		public bool? EnCours { get; set; }
		public bool? Valides { get; set; }
		public DateTime? DateFin { get; set; }
		public DateTime? DateDebut { get; set; }
		public string? TermeDeRecherche { get; set; }
	}

	public class AffichageBERAvecPJ
	{
		public int IDBEReglement;
		public int IDAgenceVoyage;
		public int IDEtape { get; set; }
		public int? IDRenvoi { get; set; }
		public string Observation { get; set; }
		public DateTime? DateSaisie { get; set; }
		public DateTime? DateValidation { get; set; }
	}

	public class AffichageBERAvecFacturesLier
	{
		public int IDBEReglement { get; set; }
		public int IDAgenceVoyage { get; set; }
		public int IDEtape { get; set; }
		public int? IDRenvoi { get; set; }
		public string Observation { get; set; }
		public DateTime? DateSaisie { get; set; }
		public DateTime? DateValidation { get; set; }
		public List<AffichageFactureAvecPJ> ListeFacturesLier { get; set; } = new List<AffichageFactureAvecPJ>();
	}

	public class BERDetails
	{
		public int IDAgenceVoyage { get; set; }
		public string NomSignataire { get; set; }
		public int? MatSignataire { get; set; }
		public List<BERFactureModel> ListeFacture { get; set; }
	}

	public class ImprimerBERModel
	{
		public DateTime DateSaisie { get; set; }
		public decimal MontantTotal { get; set; }
		public string LibelleAgence { get; set; }
		public LigneSignataire Signataire { get; set; }
		public List<LigneFacture> ListeLigneFacture { get; set; }
	}

	public class LigneSignataire
	{
		public int? MatSignataire { get; set; }
		public string FonctionSignataire { get; set; }
		public string NomSignataire { get; set; }
	}

	public class LigneFacture
	{
		public string RefFacture { get; set; }
		public DateTime DateEmissionFacture { get; set; }
		public decimal? MontantTotalFacture { get; set; }
		public List<LigneFactureAgent> ListeAgentsBeneficiaires { get; set; } = new List<LigneFactureAgent>();
	}

	public class LigneFactureAgent
	{
		public string NomBeneficiaire { get; set; }
		public string RefTitreBeneficiaire { get; set; }
		public int MatriculeBeneficiaire { get; set; }
	}

	public class AffichageAgentAutoCompletion
	{
		public int MatAgent { get; set; }
		public string NomAgent { get; set; }
	}

	public class AffichageTitreAutoCompletion
	{
		public int IDTitre { get; set; }
		public string NomBeneficiaire { get; set; }
		public string RefTitre { get; set; }
		public int MatAgent { get; set; }
		public string ClasseVoyage { get; set; }
		public string Routing { get; set; }
	}
}
