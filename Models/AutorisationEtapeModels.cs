namespace Gestions_des_Titres_de_Transport.Models
{
	public class AutorisationEtapeModels
	{
		
	}
	public class CreerAutoEtapeModel
	{
		public int MatAgent { get; set; }
		public DateTime DateExpiration { get; set; }
	}

	public class AffichageAutoEtapes
	{
		public int IDAutoEtape { get; set; }

		public int IDEtape { get; set; }

		public string LibelleEtape { get; set; }

		public bool? Simodification { get; set; }

		public bool? Siconsul { get; set; }

		public bool? Sisuppression { get; set; }

		public int Matagentautoetape { get; set; }

		public string NomAgentAutoEtape { get; set; }

		public DateTime Dateexpirationautoetape { get; set; }

		public DateTime Dateattributionautoetape { get; set; }
	}
}
