namespace Gestions_des_Titres_de_Transport.Services
{
	public class ChargementService
	{
		public event Action<bool>? OnCharger;
		
		public void CacherChargement()
		{
			OnCharger?.Invoke(false);
		}

		public void MontrerChargement()
		{
			OnCharger?.Invoke(true);
		}
	}
}
