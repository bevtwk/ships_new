using System;
using UnityEngine;

namespace Framework.Components.Values
{
	public class RectTransformRect : IReadonlyValue<Rect>, IDisposable
	{
		private RectTransform rectTransform;

		public Rect Value
		{
			get => rectTransform.rect;
		}

		public RectTransformRect(RectTransform rectTransform)
		{
			this.rectTransform = rectTransform;
		}

		public void Dispose()
		{
			rectTransform = null;
		}
	}
	
	public class RectTransformPivot : IValue<Vector2>, IDisposable
	{
		private RectTransform rectTransform;

		public Vector2 Value
		{
			get => rectTransform.pivot;
			set => rectTransform.pivot = value;
		}

		public RectTransformPivot(RectTransform rectTransform)
		{
			this.rectTransform = rectTransform;
		}

		public void Dispose()
		{
			rectTransform = null;
		}
	}
	
	public class RectTransformAnchorMin : IValue<Vector2>, IDisposable
	{
		private RectTransform rectTransform;

		public Vector2 Value
		{
			get => rectTransform.anchorMin;
			set => rectTransform.anchorMin = value;
		}

		public RectTransformAnchorMin(RectTransform rectTransform)
		{
			this.rectTransform = rectTransform;
		}

		public void Dispose()
		{
			rectTransform = null;
		}
	}
	
	public class RectTransformAnchorMax : IValue<Vector2>, IDisposable
	{
		private RectTransform rectTransform;

		public Vector2 Value
		{
			get => rectTransform.anchorMax;
			set => rectTransform.anchorMax = value;
		}

		public RectTransformAnchorMax(RectTransform rectTransform)
		{
			this.rectTransform = rectTransform;
		}

		public void Dispose()
		{
			rectTransform = null;
		}
	}
	
	public class RectTransformAnchoredPos : IValue<Vector2>, IDisposable
	{
		private RectTransform rectTransform;

		public Vector2 Value
		{
			get => rectTransform.anchoredPosition;
			set => rectTransform.anchoredPosition = value;
		}

		public RectTransformAnchoredPos(RectTransform rectTransform)
		{
			this.rectTransform = rectTransform;
		}

		public void Dispose()
		{
			rectTransform = null;
		}
	}
	
	public class RectTransformSizeDelta : IValue<Vector2>, IDisposable
	{
		private RectTransform rectTransform;

		public Vector2 Value
		{
			get => rectTransform.sizeDelta;
			set => rectTransform.sizeDelta = value;
		}

		public RectTransformSizeDelta(RectTransform rectTransform)
		{
			this.rectTransform = rectTransform;
		}

		public void Dispose()
		{
			rectTransform = null;
		}
	}
}
