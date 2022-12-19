using System;
using UnityEngine;

using Framework.Components.Values;
using Framework.Utils;

namespace Framework.Systems
{
	public class TweenSys : CompositeDisposable, IUpdatableSystem
	{
		[Serializable]
		public enum Easing
		{
			Linear,
			Quadratic,
			Cubic,
			Quartic,
			Quintic,
			Sinusoidal,
			Exponential,
			Circular,
			Elastic,
			Back,
			Bounce
		}

		[Serializable]
		public enum EasingDir
		{
			In, Out, InOut
		}

		private abstract class TweenAct<T> : Resetable
		{
			protected IValue<T> target;
			protected T fromVal;
			protected T toVal;
			private Action onComplete;
			private bool completeOnDispose;

			private Func<float, float> easingFunc;
			
			private float invDuration;
			private float timeout;

			public void Init(
				IValue<T> target, T fromVal, T toVal, 
				float duration, Func<float, float> easingFunc,
				Action onComplete, bool completeOnDispose)
			{
				if (target != null)
					target.Value = fromVal;
				
				this.target = target;
				this.fromVal = fromVal;
				this.toVal = toVal;
				this.easingFunc = easingFunc;
				this.onComplete = onComplete;
				this.completeOnDispose = completeOnDispose;

				if (duration > 0.001f)
				{
					this.invDuration = 1f / duration;
					this.timeout = duration;
				}
				else
				{
					this.invDuration = 0f;
					this.timeout = 0f;
				}
				
			}
			
			protected override void OnDispose()
			{
				if (this.completeOnDispose)
					this.onComplete.Invoke();
				
				Init(null, default, default, 0f, null, null, false);
				
				base.OnDispose();
			}

			public void Update(float dt)
			{
				float t = this.timeout - dt;
				this.timeout = t;
				if (t <= 0f)
				{
					this.target.Value = toVal;
					
					if (!this.completeOnDispose)
						this.onComplete?.Invoke();

					Dispose();
					return;
				}

				float coef = 1f - (t * this.invDuration);

				var easingFunc = this.easingFunc;
				if (easingFunc != null)
					coef = this.easingFunc.Invoke(coef);

				LerpImpl(coef);
			}

			protected abstract void LerpImpl(float coef);
		}

		private class FloatTweenAct : TweenAct<float>
		{
			protected override void LerpImpl(float coef)
			{
				this.target.Value = this.fromVal * (1f - coef) + (this.toVal * coef);
			}
		}
		
		private class Vector2TweenAct : TweenAct<Vector2>
		{
			protected override void LerpImpl(float coef)
			{
				this.target.Value = this.fromVal * (1f - coef) + (this.toVal * coef);
			}
		}
		
		private class Vector3TweenAct : TweenAct<Vector3>
		{
			protected override void LerpImpl(float coef)
			{
				this.target.Value = this.fromVal * (1f - coef) + (this.toVal * coef);
			}
		}
		
		private class Vector4TweenAct : TweenAct<Vector4>
		{
			protected override void LerpImpl(float coef)
			{
				this.target.Value = this.fromVal * (1f - coef) + (this.toVal * coef);
			}
		}
		
		private class QuaternionTweenAct : TweenAct<Quaternion>
		{
			protected override void LerpImpl(float coef)
			{
				this.target.Value = Quaternion.SlerpUnclamped(this.fromVal, this.toVal, coef);
			}
		}
		
		private class ColorTweenAct : TweenAct<Color>
		{
			protected override void LerpImpl(float coef)
			{
				this.target.Value = Color.LerpUnclamped(this.fromVal, this.toVal, coef);
			}
		}
		
		private class RectTweenAct : TweenAct<Rect>
		{
			protected override void LerpImpl(float coef)
			{
				var fromVal = this.fromVal;
				var toVal = this.toVal;
				float coef0 = 1f - coef;

				this.target.Value = new Rect(
					fromVal.x * coef0 + toVal.x * coef,
					fromVal.y * coef0 + toVal.y * coef,
					fromVal.width * coef0 + toVal.width * coef,
					fromVal.height * coef0 + toVal.height * coef
				);
			}
		}
		
		private EnumerablePool<FloatTweenAct> floatTweenActPool;
		private EnumerablePool<Vector2TweenAct> vec2TweenActPool;
		private EnumerablePool<Vector3TweenAct> vec3TweenActPool;
		private EnumerablePool<Vector4TweenAct> vec4TweenActPool;
		private EnumerablePool<QuaternionTweenAct> quatTweenActPool;
		private EnumerablePool<ColorTweenAct> colorTweenActPool;
		private EnumerablePool<RectTweenAct> rectTweenActPool;

		public TweenSys()
		{
			this.floatTweenActPool = Add(new EnumerablePool<FloatTweenAct>());
			this.vec2TweenActPool = Add(new EnumerablePool<Vector2TweenAct>());
			this.vec3TweenActPool = Add(new EnumerablePool<Vector3TweenAct>());
			this.vec4TweenActPool = Add(new EnumerablePool<Vector4TweenAct>());
			this.quatTweenActPool = Add(new EnumerablePool<QuaternionTweenAct>());
			this.colorTweenActPool = Add(new EnumerablePool<ColorTweenAct>());
			this.rectTweenActPool = Add(new EnumerablePool<RectTweenAct>());
		}

		public IDisposable StartTweenAct(
			IValue<float> target, float fromVal, float toVal, 
			float duration, 
			Easing easing = Easing.Linear, EasingDir easingDir = EasingDir.In,
			Action onComplete = null, bool completeOnDispose = false
		)
		{
			var slot = this.floatTweenActPool.GetFreeSlot();
			var easingFunc = GetEasingFunc(easing, easingDir);
			slot.Item.Init(target, fromVal, toVal, duration, easingFunc, onComplete, completeOnDispose);
			
			return slot;
		}
		
		public IDisposable StartTweenAct(
			IValue<Vector2> target, Vector2 fromVal, Vector2 toVal, 
			float duration, 
			Easing easing = Easing.Linear, EasingDir easingDir = EasingDir.In,
			Action onComplete = null, bool completeOnDispose = false
		)
		{
			var slot = this.vec2TweenActPool.GetFreeSlot();
			var easingFunc = GetEasingFunc(easing, easingDir);
			slot.Item.Init(target, fromVal, toVal, duration, easingFunc, onComplete, completeOnDispose);
			
			return slot;
		}

		public IDisposable StartTweenAct(
			IValue<Vector3> target, Vector3 fromVal, Vector3 toVal, 
			float duration, 
			Easing easing = Easing.Linear, EasingDir easingDir = EasingDir.In,
			Action onComplete = null, bool completeOnDispose = false
		)
		{
			var slot = this.vec3TweenActPool.GetFreeSlot();
			var easingFunc = GetEasingFunc(easing, easingDir);
			slot.Item.Init(target, fromVal, toVal, duration, easingFunc, onComplete, completeOnDispose);
			
			return slot;
		}
		
		public IDisposable StartTweenAct(
			IValue<Vector4> target, Vector4 fromVal, Vector4 toVal, 
			float duration,
			Easing easing = Easing.Linear, EasingDir easingDir = EasingDir.In,
			Action onComplete = null, bool completeOnDispose = false
		)
		{
			var slot = this.vec4TweenActPool.GetFreeSlot();
			var easingFunc = GetEasingFunc(easing, easingDir);
			slot.Item.Init(target, fromVal, toVal, duration, easingFunc, onComplete, completeOnDispose);
			
			return slot;
		}
		
		public IDisposable StartTweenAct(
			IValue<Quaternion> target, Quaternion fromVal, Quaternion toVal, 
			float duration,
			Easing easing = Easing.Linear, EasingDir easingDir = EasingDir.In,
			Action onComplete = null, bool completeOnDispose = false
		)
		{
			var slot = this.quatTweenActPool.GetFreeSlot();
			var easingFunc = GetEasingFunc(easing, easingDir);
			slot.Item.Init(target, fromVal, toVal, duration, easingFunc, onComplete, completeOnDispose);
			
			return slot;
		}
		
		public IDisposable StartTweenAct(
			IValue<Color> target, Color fromVal, Color toVal, 
			float duration,
			Easing easing = Easing.Linear, EasingDir easingDir = EasingDir.In,
			Action onComplete = null, bool completeOnDispose = false
		)
		{
			var slot = this.colorTweenActPool.GetFreeSlot();
			var easingFunc = GetEasingFunc(easing, easingDir);
			slot.Item.Init(target, fromVal, toVal, duration, easingFunc, onComplete, completeOnDispose);
			
			return slot;
		}
		
		public IDisposable StartTweenAct(
			IValue<Rect> target, Rect fromVal, Rect toVal, 
			float duration,
			Easing easing = Easing.Linear, EasingDir easingDir = EasingDir.In,
			Action onComplete = null, bool completeOnDispose = false
		)
		{
			var slot = this.rectTweenActPool.GetFreeSlot();
			var easingFunc = GetEasingFunc(easing, easingDir);
			slot.Item.Init(target, fromVal, toVal, duration, easingFunc, onComplete, completeOnDispose);
			
			return slot;
		}

		public void OnUpdate(float dt)
		{
			foreach (var act in this.floatTweenActPool)
				act.Update(dt);
			
			foreach (var act in this.vec2TweenActPool)
				act.Update(dt);
			
			foreach (var act in this.vec3TweenActPool)
				act.Update(dt);

			foreach (var act in this.vec4TweenActPool)
				act.Update(dt);
			
			foreach (var act in this.quatTweenActPool)
				act.Update(dt);
			
			foreach (var act in this.colorTweenActPool)
				act.Update(dt);
			
			foreach (var act in this.rectTweenActPool)
				act.Update(dt);
		}
		
		private Func<float, float> GetEasingFunc(Easing easing, EasingDir easingDir)
		{
			if (easing == Easing.Linear)
				return null;

			if (easingDir == EasingDir.In)
			{
				switch (easing)
				{ 
					case Easing.Quadratic:
						return EasingFuncs.Quadratic.In;
					case Easing.Cubic:
						return EasingFuncs.Cubic.In;
					case Easing.Quartic:
						return EasingFuncs.Quartic.In;
					case Easing.Quintic:
						return EasingFuncs.Quintic.In;
					case Easing.Sinusoidal:
						return EasingFuncs.Sinusoidal.In;
					case Easing.Exponential:
						return EasingFuncs.Exponential.In;
					case Easing.Circular:
						return EasingFuncs.Circular.In;
					case Easing.Elastic:
						return EasingFuncs.Elastic.In;
					case Easing.Back:
						return EasingFuncs.Back.In;
					case Easing.Bounce:
						return EasingFuncs.Bounce.In;
				}
			}
			else if (easingDir == EasingDir.Out)
			{
				switch (easing)
				{ 
					case Easing.Quadratic:
						return EasingFuncs.Quadratic.Out;
					case Easing.Cubic:
						return EasingFuncs.Cubic.Out;
					case Easing.Quartic:
						return EasingFuncs.Quartic.Out;
					case Easing.Quintic:
						return EasingFuncs.Quintic.Out;
					case Easing.Sinusoidal:
						return EasingFuncs.Sinusoidal.Out;
					case Easing.Exponential:
						return EasingFuncs.Exponential.Out;
					case Easing.Circular:
						return EasingFuncs.Circular.Out;
					case Easing.Elastic:
						return EasingFuncs.Elastic.Out;
					case Easing.Back:
						return EasingFuncs.Back.Out;
					case Easing.Bounce:
						return EasingFuncs.Bounce.Out;
				}
				
			}
			else if (easingDir == EasingDir.InOut)
			{
				switch (easing)
				{ 
					case Easing.Quadratic:
						return EasingFuncs.Quadratic.InOut;
					case Easing.Cubic:
						return EasingFuncs.Cubic.InOut;
					case Easing.Quartic:
						return EasingFuncs.Quartic.InOut;
					case Easing.Quintic:
						return EasingFuncs.Quintic.InOut;
					case Easing.Sinusoidal:
						return EasingFuncs.Sinusoidal.InOut;
					case Easing.Exponential:
						return EasingFuncs.Exponential.InOut;
					case Easing.Circular:
						return EasingFuncs.Circular.InOut;
					case Easing.Elastic:
						return EasingFuncs.Elastic.InOut;
					case Easing.Back:
						return EasingFuncs.Back.InOut;
					case Easing.Bounce:
						return EasingFuncs.Bounce.InOut;
				}
			}

			Debug.LogError("TweenSys: Invalid Easing Func!");
			return null;
		}

	}
}
