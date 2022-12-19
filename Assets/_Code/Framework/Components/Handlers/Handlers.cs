using System.Collections.Generic;
using UnityEngine;

namespace Framework.Components.Handlers
{
	public abstract class HandlerBase<T, TInterface> : ResetableEx
		where T : HandlerBase<T, TInterface>, TInterface
		where TInterface : class
	{
		private HandlerComp<TInterface> handlerComp;

		protected HandlerBase()
		{ }

		protected void Init(HandlerComp<TInterface> handlerComp)
		{
			this.handlerComp = handlerComp;
			handlerComp.AddHandler(this as TInterface);
		}

		protected override void OnDispose()
		{
			var handlerComp = this.handlerComp;
			if (handlerComp != null)
			{
				handlerComp.RemoveHandler(this as TInterface);
				this.handlerComp = null;
			}

			base.OnDispose();
		}
	}

	public abstract class HandlerComp<TInterface> : MonoBehaviour
	{
		protected readonly List<TInterface> handlers = new List<TInterface>();

		public void AddHandler(TInterface handler)
		{
			this.handlers.Add(handler);
		}
		
		public void RemoveHandler(TInterface handler)
		{
			this.handlers.Remove(handler);
		}
		
		private void OnDestroy()
		{
			this.handlers.Clear();
		}
	}
}
