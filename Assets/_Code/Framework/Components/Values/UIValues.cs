using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Components.Values
{
	public class UIScrollRectValue : IValue<Vector2>, IDisposable
	{
		private ScrollRect scrollRect;
		
		public Vector2 Value
		{
			get => scrollRect.normalizedPosition;
			set => scrollRect.normalizedPosition = value;
		}
		
		public UIScrollRectValue(ScrollRect scrollRect)
		{
			this.scrollRect = scrollRect;
		}

		public void Dispose()
		{
			this.scrollRect = null;
		}
	}
	
	public class UIScrollRectHorizontalValue : IValue<float>, IDisposable
	{
		private ScrollRect scrollRect;
		
		public float Value
		{
			get => scrollRect.horizontalNormalizedPosition;
			set => scrollRect.horizontalNormalizedPosition = value;
		}
		
		public UIScrollRectHorizontalValue(ScrollRect scrollRect)
		{
			this.scrollRect = scrollRect;
		}

		public void Dispose()
		{
			this.scrollRect = null;
		}
	}
	
	public class UIScrollRectVerticalValue : IValue<float>, IDisposable
	{
		private ScrollRect scrollRect;
		
		public float Value
		{
			get => scrollRect.verticalNormalizedPosition;
			set => scrollRect.verticalNormalizedPosition = value;
		}
		
		public UIScrollRectVerticalValue(ScrollRect scrollRect)
		{
			this.scrollRect = scrollRect;
		}

		public void Dispose()
		{
			this.scrollRect = null;
		}
	}
	
	public class UIScrollbarValue : IValue<float>, IDisposable
	{
		private Scrollbar scrollbar;
		
		public float Value
		{
			get => scrollbar.value;
			set => scrollbar.value = value;
		}
		
		public UIScrollbarValue(Scrollbar scrollbar)
		{
			this.scrollbar = scrollbar;
		}

		public void Dispose()
		{
			this.scrollbar = null;
		}
	}
	
	public class UISliderValue : IValue<float>, IDisposable
	{
		private Slider slider;
		
		public float Value
		{
			get => slider.value;
			set => slider.value = value;
		}
		
		public UISliderValue(Slider slider)
		{
			this.slider = slider;
		}

		public void Dispose()
		{
			this.slider = null;
		}
	}
	
	
	/*public class UIProgressValue : IValue<float>, IDisposable
	{
		private IUIProgress uiProgress;
		
		public float Value
		{
			get => uiProgress.Progress;
			set => uiProgress.Progress = value;
		}
		
		public UIProgressValue(IUIProgress uiProgress)
		{
			this.uiProgress = uiProgress;
		}

		public void Dispose()
		{
			this.uiProgress = null;
		}
	}*/
	
	public class UIImageFillAmountValue : IValue<float>, IDisposable
	{
		private Image image;
		
		public float Value
		{
			get => image.fillAmount;
			set => image.fillAmount = value;
		}
		
		public UIImageFillAmountValue(Image image)
		{
			this.image = image;
		}

		public void Dispose()
		{
			this.image = null;
		}
	}
	

	public class UIGraphicColor : IValue<Color>, IDisposable
	{
		private Graphic graphic;

		public Color Value
		{
			get => graphic.color;
			set => graphic.color = value;
		}

		public UIGraphicColor(Graphic graphic)
		{
			this.graphic = graphic;
		}

		public void Dispose()
		{
			this.graphic = null;
		}
	}
	
	public class UIGraphicAlpha : IValue<float>, IDisposable
	{
		private Graphic graphic;

		public float Value
		{
			get => graphic.color.a;
			set
			{
				var color = graphic.color;
				color.a = value;
			}
		}

		public UIGraphicAlpha(Graphic graphic)
		{
			this.graphic = graphic;
		}

		public void Dispose()
		{
			this.graphic = null;
		}
	}	
	
	public class CanvasGroupAlpha : IValue<float>, IDisposable
	{
		private CanvasGroup canvasGroup;

		public float Value
		{
			get => canvasGroup.alpha;
			set => canvasGroup.alpha = value;
		}

		public CanvasGroupAlpha(CanvasGroup canvasGroup)
		{
			this.canvasGroup = canvasGroup;
		}

		public void Dispose()
		{
			this.canvasGroup = null;
		}
	}
	
	public class UITextValue : IValue<string>, IDisposable
	{
		private Text text;

		public string Value
		{
			get => text.text;
			set => text.text = value;
		}

		public UITextValue(Text text)
		{
			this.text = text;
		}

		public void Dispose()
		{
			this.text = null;
		}
	}
}
