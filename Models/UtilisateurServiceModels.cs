using System.ComponentModel.DataAnnotations;

namespace Gestions_des_Titres_de_Transport.Models
{
	public class ParamConnexUtilisateur
	{
		[Required]
		public string Identifiant { get; set; }

		[Required]
		public string MotDePasse { get; set; }
	}
	public class DetailsUtilisateur
	{
		public int Matricule { get; set; }
		public string NomEtPrenoms { get; set; }
		public string Domaine { get; set; }
		public string Departement { get; set; }
		public string Poste { get; set; }
		public string Mail { get;set; }
	}
	public class DetailsAutorisationEtape
	{
		public int IDEtape { get; set; }
		public int NumeroEtape { get; set; }
		public string LibelleEtape { get; set; }
	}
	public class DetailsAutorisationRole
	{
		public int IDRole { get; set; }
		public int NumeroRole { get; set; }
		public string SlugRole { get; set; }
		public string LibelleRole { get; set; }
	}
}
