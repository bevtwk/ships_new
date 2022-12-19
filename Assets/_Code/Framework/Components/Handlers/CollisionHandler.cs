using UnityEngine;

namespace Framework.Components.Handlers
{
	public interface ICollisionHandler
	{
		void OnCollisionEnter(Collision collision);
		void OnCollisionExit(Collision collision);
	}

	public class CollisionHandlerComp : HandlerComp<ICollisionHandler>
	{
		private void OnCollisionEnter(Collision collision)
		{
			foreach (var handler in this.handlers)
				handler.OnCollisionEnter(collision);
		}

		private void OnCollisionExit(Collision collision)
		{
			foreach (var handler in this.handlers)
				handler.OnCollisionExit(collision);
		}
	}
}
