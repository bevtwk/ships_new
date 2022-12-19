using UnityEngine;

namespace Framework.Components
{
	public class RectMonoBehaviour : MonoBehaviour
	{
		public RectTransform RectTransform { get; private set; }

		private void Awake()
		{
			this.RectTransform = this.transform as RectTransform;
			OnAwake();
		}
		
		protected virtual void OnAwake()
		{ }
	}
}
