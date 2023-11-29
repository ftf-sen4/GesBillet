namespace Gestions_des_Titres_de_Transport.Services.Contrats
{
	public interface INotificationUIService
	{
		public event Action<bool>? OnCharger;
		public event Action<NotificationData>? OnAfficher;
		public void CacherChargement();
		public void MontrerChargement();
		public void AfficherMessage(NotificationData notification);
	}
}
