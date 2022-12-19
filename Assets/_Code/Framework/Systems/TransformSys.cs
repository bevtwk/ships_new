using System;
using UnityEngine;
using UnityEngine.Assertions;
using Framework.Components.Values;

namespace Framework.Systems
{
	public class TransformSys : CompositeDisposable, IUpdatableSystem
	{
		private const float FLOAT_EPS = 0.0001f;
		
		private class LookAtAct : Resetable
		{
			private IValue<Quaternion> rotation;
			private IReadonlyValue<Vector3> pos;
			private IReadonlyValue<Vector3> targetPos;
			private float angleSpeed;
			
			public void Init(IValue<Quaternion> rotation, IReadonlyValue<Vector3> pos, IReadonlyValue<Vector3> targetPos, float angleSpeed)
			{
				this.rotation = rotation;
				this.pos = pos;
				this.targetPos = targetPos;
				this.angleSpeed = angleSpeed;
			}

			protected override void OnDispose()
			{
				Init(null, null, null, 0f);
				base.OnDispose();
			}

			public void Update(float dt)
			{
				var dpos = this.targetPos.Value - this.pos.Value;
				if (dpos.sqrMagnitude > FLOAT_EPS)
				{
					var toRotation = Quaternion.LookRotation(dpos.normalized);

					var a = this.angleSpeed;
					if (a > 0f)
					{
						a *= dt;
						
						var rotation = this.rotation;
						var fromRotation = rotation.Value;
						rotation.Value = Quaternion.RotateTowards(fromRotation, toRotation, a);
					}
					else
					{
						this.rotation.Value = toRotation;
					}
				}
			}
		}
		
		private class LookDirAct : Resetable
		{
			private IValue<Quaternion> rotation;
			private IReadonlyValue<Vector3> dir;
			private float angleSpeed;
			
			public void Init(IValue<Quaternion> rotation, IReadonlyValue<Vector3> dir, float angleSpeed)
			{
				this.rotation = rotation;
				this.dir = dir;
				this.angleSpeed = angleSpeed;
			}

			protected override void OnDispose()
			{
				Init(null, null, 0f);
				base.OnDispose();
			}

			public void Update(float dt)
			{
				var dirVal = this.dir.Value;
				float dirLenSq = dirVal.sqrMagnitude;
				
				if (dirLenSq < FLOAT_EPS)
					return;

				dirVal *= 1f / Mathf.Sqrt(dirLenSq);
				var toRotation = Quaternion.LookRotation(dirVal);

				var a = this.angleSpeed;
				if (a > FLOAT_EPS)
				{
					a *= dt;
					
					var rotation = this.rotation;
					var fromRotation = rotation.Value;
					rotation.Value = Quaternion.RotateTowards(fromRotation, toRotation, a);
				}
				else
				{
					rotation.Value = toRotation;
				}
			}
		}
		
		private class MoveAct : Resetable
		{
			private IValue<Vector3> pos;
			private IReadonlyValue<Vector3> dir;
			private IReadonlyValue<float> speed;
			
			public void Init(IValue<Vector3> pos, IReadonlyValue<Vector3> dir, IReadonlyValue<float> speed)
			{
				this.pos = pos;
				this.dir = dir;
				this.speed = speed;
			}

			protected override void OnDispose()
			{
				Init(null, null, null);
				base.OnDispose();
			}

			public void Update(float dt)
			{
				var dirVal = this.dir.Value;
				float dirLenSq = dirVal.sqrMagnitude;
				
				float s = this.speed.Value;
				
				if ( (dirLenSq < FLOAT_EPS) || (s < FLOAT_EPS))
					return;

				dirVal *= (s * dt) / Mathf.Sqrt(dirLenSq);
				this.pos.Value += dirVal;
			}
		}
		
		private class MoveToAct : Resetable
		{
			private IValue<Vector3> pos;
			private IReadonlyValue<Vector3> targetPos;
			private IReadonlyValue<float> speed;
			private float doneRangeSq;
			private Action onComplete;
			
			public void Init(IValue<Vector3> pos, IReadonlyValue<Vector3> targetPos, IReadonlyValue<float> speed, float doneRange, Action onComplete)
			{
				this.pos = pos;
				this.targetPos = targetPos;
				this.speed = speed;

				doneRange = (doneRange < FLOAT_EPS) ? FLOAT_EPS : doneRange;
				this.doneRangeSq = doneRange * doneRange;
				
				this.onComplete = onComplete;
			}

			protected override void OnDispose()
			{
				Init(null, null, null, FLOAT_EPS, null);
				base.OnDispose();
			}

			public void Update(float dt)
			{
				var posValue = this.pos.Value;
				var targetPosVal = this.targetPos.Value;
				var dpos = targetPosVal - posValue;
				float dposLenSq = dpos.sqrMagnitude;
				float doneRangeSq = this.doneRangeSq;

				if (doneRangeSq < dposLenSq)
				{
					float s = this.speed.Value;
					if (s < FLOAT_EPS)
						return;
					
					s *= dt;
					
					if ((s * s) >= dposLenSq)
					{
						this.pos.Value = targetPosVal;
					}
					else
					{
						float dposLen = Mathf.Sqrt(dposLenSq);
						this.pos.Value = posValue + (dpos * (s / dposLen));

						dposLen -= s;
						dposLenSq = dposLen * dposLen;
						if (doneRangeSq > dposLenSq)
							return;
					}
				}
				
				this.onComplete?.Invoke();
				Dispose();
			}
		}

		private readonly EnumerablePool<LookAtAct> lookAtActPool;
		private readonly EnumerablePool<LookDirAct> lookDirActPool;
		private readonly EnumerablePool<MoveAct> moveActPool;
		private readonly EnumerablePool<MoveToAct> moveToActPool;

		public TransformSys()
		{
			this.lookAtActPool = Add(new EnumerablePool<LookAtAct>());
			this.lookDirActPool = Add(new EnumerablePool<LookDirAct>());
			this.moveActPool = Add(new EnumerablePool<MoveAct>());
			this.moveToActPool = Add(new EnumerablePool<MoveToAct>());
		}

		public IDisposable StartLookAtAct(IValue<Quaternion> rot, IReadonlyValue<Vector3> pos, IReadonlyValue<Vector3> targetPos, float angleSpeed = -1f)
		{
			Assert.IsFalse(this.IsDisposed);
			
			var slot = this.lookAtActPool.GetFreeSlot();
			slot.Item.Init(rot, pos, targetPos, angleSpeed);
			return slot;
		}

		public IDisposable StartLookDirAct(IValue<Quaternion> rot, IReadonlyValue<Vector3> dir, float angleSpeed = -1f)
		{
			Assert.IsFalse(this.IsDisposed);

			var slot = this.lookDirActPool.GetFreeSlot();
			slot.Item.Init(rot, dir, angleSpeed);
			return slot;
		}
		
		public IDisposable StartMoveAct(IValue<Vector3> pos, IReadonlyValue<Vector3> dir, IReadonlyValue<float> speed)
		{
			Assert.IsFalse(this.IsDisposed);

			var slot = this.moveActPool.GetFreeSlot();
			slot.Item.Init(pos, dir, speed);
			return slot;
		}
		
		public IDisposable StartMoveToAct(IValue<Vector3> pos, IReadonlyValue<Vector3> targetPos, IReadonlyValue<float> speed, float doneRange = FLOAT_EPS, Action onComplete = null)
		{
			Assert.IsFalse(this.IsDisposed);

			var slot = this.moveToActPool.GetFreeSlot();
			slot.Item.Init(pos, targetPos, speed, doneRange, onComplete);
			return slot;
		}

		public void OnUpdate(float dt)
		{
			Assert.IsFalse(this.IsDisposed);

			foreach (var act in this.lookAtActPool)
				act.Update(dt);
			
			foreach (var act in this.lookDirActPool)
				act.Update(dt);
			
			foreach (var act in this.moveActPool)
				act.Update(dt);
			
			foreach (var act in this.moveToActPool)
				act.Update(dt);
		}
	}
}
