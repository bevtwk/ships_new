using UnityEngine;

using Framework.Components;
using Framework.Components.Handlers;

namespace Project.Components
{
	public class JoystickView : RectMonoBehaviour
	{
		public RectTransform Background;
		public RectTransform Stick;
		public DragHandlerComp DragHandlerComp;
	}
}
