using System.ComponentModel.DataAnnotations;

namespace Gestions_des_Titres_de_Transport.Models
{
	public class CreerTitreDeTransportModel
	{
		[Required(ErrorMessage = "La référence de la fiche de mission est obligatoire")]
		public string RefFicheDeMission { get; set; }

		[Required(ErrorMessage = "La référence est obligatoire")]
		public string RefTitre { get; set; }

		public int? IDCompagnie { get; set; }

		public int? IDAgenceVoyage { get; set; }

		public int? IDClasse { get; set; }

		[Required(ErrorMessage = "Entrez le montant du billet")]
		public decimal MontantTitreDeTransport { get; set; }

		public string Observation { get; set; }
	}

	public class CreerPlusieursTitreModel
	{
		[Required(ErrorMessage = "Les références de fiches sont obligatoires")]
		public string ListeRefFicheDeMission { get; set; }
	}

	public class AffichageTitreAvecPJ
	{
		public string RefFicheDeMission { get; set; }
		public string RefTitre { get; set; }
		public decimal? MontantTitreDeTransport { get; set; }
		public int? IDCompagnie { get; set; }
		public string LibelleCompagnie { get; set; } = string.Empty;
		public int? IDAgenceVoyage { get; set; }
		public string LibelleAgenceVoyage{ get; set; } = string.Empty;
		public int? IDClasse { get; set; }
		public string LibelleClasse { get; set; } = string.Empty;
		public int IDTitre { get; set; }
		public int? IDRenvoi { get; set; }
		public List<FichierAfficher> ListePiecesJointes { get; set; } = new List<FichierAfficher>();
		public string? Observation { get; set; }
	}
	public class SaisieTitreRechercheModel
	{
		public bool? EnCours { get; set; }
		public bool? Valides { get; set; }
		public bool? Rejetes { get; set; }
		public bool? NonValides { get; set; }
		public DateTime? DateFin { get; set; }
		public DateTime? DateDebut { get; set; }
		public string? TermeDeRecherche { get; set; }
	}
	public class ValiderTitreRechercheModel
	{
		public bool? EnCours { get; set; }
		public bool? Valides { get; set; }
		public DateTime? DateFin { get; set; }
		public DateTime? DateDebut { get; set; }
		public string? TermeDeRecherche { get; set; }
	}
	public class AffichageTitre
	{
		public int IDEtape { get; set; }
		public int IDTitre { get; set; }
		public string ClasseVoyage { get; set; }
		public string AgenceVoyage { get; set; }
		public string Compagnie { get; set; }
		public string NomAgentBeneficiaire { get; set; }
		public string RefFicheMission { get; set; }
		public string Routing { get; set; }
		public DateTime DateSaisiTitre { get; set; }
		public DateTime? DateValidTitre { get; set; }
		public string RefTitre { get; set; }
		public int? IDRenvoi { get; set; }
		public decimal Montant { get; set; }
		public string Observation { get; set; }
	}
}
