namespace Gestions_des_Titres_de_Transport.Models
{
	public class AffichageMission
	{
		public string? ObjetMission { get; set; }
		public int? IdAgentBeneficiaire { get; set; }
		public string? NomAgentBeneficiaire { get; set; }
		public string? NomCompagnie { get; set; }
		public string? Routing { get; set; }
		public string? Classe { get; set; }
		public DateTime? DateDeDepartVoyage { get; set; }
		public DateTime? DateRetourVoyage { get; set; }
		public int? MontantDuBillet { get; set; }
	}
}
