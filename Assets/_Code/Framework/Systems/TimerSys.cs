using System;
using UnityEngine;

namespace Framework.Systems
{
	public class TimerSys : CompositeDisposable, IUpdatableSystem
	{
		private class WaitAct : Resetable
		{
			private float timeout;
			private Action onComplete;
			private bool completeOnDispose;

			public void Init(float timeout, Action onComplete, bool completeOnDispose)
			{
				this.timeout = timeout;
				this.onComplete = onComplete;
				this.completeOnDispose = completeOnDispose;
			}
			
			protected override void OnDispose()
			{
				if (this.completeOnDispose)
					this.onComplete.Invoke();
				
				this.timeout = 0f;
				this.onComplete = null;
				this.completeOnDispose = false;
				
				base.OnDispose();
			}

			public void Update(float dt)
			{
				float t = this.timeout - dt;
				this.timeout = t;
				if (t <= 0f)
				{
					if (!this.completeOnDispose)
						this.onComplete.Invoke();

					Dispose();
				}
			}
		}

		private class PulseAct : Resetable
		{
			private float timeout;
			private float period;
			private Action<float> onPeriod;

			public void Init(float period, Action<float> onPeriod)
			{
				this.timeout = period;
				this.period = period;
				this.onPeriod = onPeriod;

				if (period < 0.001f)
				{
					Debug.LogError("TimerSys.PulseAct: period < 0.001");
					Dispose();
				}
			}
			
			protected override void OnDispose()
			{
				this.timeout = 0f;
				this.period = 0f;
				this.onPeriod = null;
				
				base.OnDispose();
			}

			public void Update(float dt)
			{
				float t = this.timeout - dt;
				float period = this.period;
				/*while*/ if (t <= 0f)
				{
					this.onPeriod.Invoke(t);
					t += period;
				}

				this.timeout = t;
			}
		}

		private EnumerablePool<WaitAct> waitActPool;
		private EnumerablePool<PulseAct> pulseActPool;

		public TimerSys()
		{
			this.waitActPool = Add(new EnumerablePool<WaitAct>());
			this.pulseActPool = Add(new EnumerablePool<PulseAct>());
		}

		public IDisposable StartWaitAct(float timeout, Action onComplete, bool completeOnDispose)
		{
			var slot = this.waitActPool.GetFreeSlot();
			slot.Item.Init(timeout, onComplete, completeOnDispose);
			return slot;
		}
		
		public IDisposable StartPulseAct(float period, Action<float> onPeriod)
		{
			var slot = this.pulseActPool.GetFreeSlot();
			slot.Item.Init(period, onPeriod);
			return slot;
		}
		

		public void OnUpdate(float dt)
		{
			var utcTimeNow = DateTime.UtcNow;
			
			foreach (var act in this.waitActPool)
				act.Update(dt);
			
			foreach (var act in this.pulseActPool)
				act.Update(dt);
		}
	}
}
