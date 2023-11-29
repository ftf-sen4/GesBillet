using System.ComponentModel;
using Gestions_des_Titres_de_Transport.Services.Contrats;

namespace Gestions_des_Titres_de_Transport.Services
{
	public enum TypeDeNotification
	{
		Succes,
		Info,
		Attention,
		Erreur
	}
	public class NotificationData
	{
		public string? Message { get; set; }
		public TypeDeNotification TypeMessage { get; set; }
	}

	public class NotificationUIService : INotificationUIService
	{
		public event Action<bool>? OnCharger;
		public event Action<NotificationData>? OnAfficher;

		public void AfficherMessage(NotificationData notification)
		{
			OnAfficher?.Invoke(notification);
		}

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
