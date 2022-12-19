using UnityEngine;

namespace Framework.Components.Handlers
{
	public interface IAnimationEventHandler
	{
		void OnAnimationEvent(AnimationEvent ae);
	}

	public class AnimationEventHandlerComp : HandlerComp<IAnimationEventHandler>
	{
		private void OnAnimationEvent(AnimationEvent ae)
		{
			foreach (var handler in this.handlers)
				handler.OnAnimationEvent(ae);
		}
	}
}
