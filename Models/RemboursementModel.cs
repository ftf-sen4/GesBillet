namespace Gestions_des_Titres_de_Transport.Models
{
	public class CreerRemboursementModel
	{
		public string RefTitre { get; set; }
		public string RoutingNonUtiliser { get; set; }
		public string? Observation { get; set; }
	}

	public class SaisieRembRechercheModel
	{
		public bool? EnCours { get; set; }
		public bool? Valides { get; set; }
		public DateTime? DateFin { get; set; }
		public DateTime? DateDebut { get; set; }
		public string? TermeDeRecherche { get; set; }
	}

	public class AffichageRemboursement
	{
		public int IDTitre { get; set; }
		public string RefTitre { get; set; }
		public string RoutingInitial { get; set; }
		public string RoutingNonUtiliser { get; set; }
		public int IDRemb { get; set; }
		public int IDEtape { get; set; }
		public int? IDRenvoi { get; set; }
		public string? Observation { get; set; }
		public DateTime? DateSaisie { get; set; }
		public DateTime? DateValidation { get; set; }
	}
}
