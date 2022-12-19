using UnityEngine;

namespace Framework.Components.Handlers
{
	public interface ITriggerHandler
	{
		void OnTriggerEnter(Collider other);
		void OnTriggerExit(Collider other);
		void OnTriggerStay(Collider other);
	}

	public class TriggerHandlerComp : HandlerComp<ITriggerHandler>
	{
		private void OnTriggerEnter(Collider other)
		{
			foreach (var handler in this.handlers)
				handler.OnTriggerEnter(other);
		}

		private void OnTriggerExit(Collider other)
		{
			foreach (var handler in this.handlers)
				handler.OnTriggerExit(other);
		}

		private void OnTriggerStay(Collider other)
		{
			foreach (var handler in this.handlers)
				handler.OnTriggerStay(other);
		}
	}
}
