namespace Gestions_des_Titres_de_Transport.Models
{
	public class WizardBE
	{
		public WizardEtape EtapeActive { get; set; }
		public List<WizardEtape> ListeEtapes { get; set; }
		public bool BoutonPrecedentActif { get; set; }
		public bool BoutonSuivantActif { get; set; }


		public void EtapeSuivante()
		{
			var TempEtapeSuivante = ListeEtapes.First(item => item.Index<EtapeActive.Index && item.SiActive == true);
			if(TempEtapeSuivante != null)
			{
				EtapeActive = TempEtapeSuivante;
			}
		}

		public void EtapePrecedente()
		{
			var TempEtapePrecedente = ListeEtapes.First(item => item.Index > EtapeActive.Index && item.SiActive == true);
			if (TempEtapePrecedente != null)
			{
				EtapeActive = TempEtapePrecedente;
			}
		}
	}

	public class WizardEtape
	{
		public int Index{ get; set; }
		public bool SiCorrect { get; set; }
		public bool SiActive { get; set; }
	}
}
