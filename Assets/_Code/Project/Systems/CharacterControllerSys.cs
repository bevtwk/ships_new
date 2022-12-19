using System;
using UnityEngine;
using UnityEngine.Assertions;
using Framework.Components.Values;

namespace Framework.Systems
{
	public class CharacterControllerSys : CompositeDisposable, IUpdatableSystem
	{
		private class CharacterMoveAct : Resetable
		{
			public CharacterController characterController;
			public IValue<Vector3> worldDirComp;
			public IReadonlyValue<float> speedComp;

			public void Init(
				CharacterController characterController, 
				IValue<Vector3> worldDirComp,
				IReadonlyValue<float> speedComp)
			{
				this.characterController = characterController;
				this.worldDirComp = worldDirComp;
				this.speedComp = speedComp;
			}

			protected override void OnDispose()
			{
				Init(null, null, null);
				base.OnDispose();
			}

			public void Update(float dt)
			{
				var dir = this.worldDirComp.Value;
				float sl = dir.sqrMagnitude;
				if (sl > 0.001f)
				{
					dir *= dt * (this.speedComp.Value / Mathf.Sqrt(sl));
					this.characterController.Move(dir);
				}
			}
		}

		private readonly EnumerablePool<CharacterMoveAct> moveActPool;

		public CharacterControllerSys()
		{
			this.moveActPool = Add(new EnumerablePool<CharacterMoveAct>());
		}

		public IDisposable StartCharacterMoveAct(
			CharacterController characterController,
			IValue<Vector3> worldDirComp,
			IReadonlyValue<float> speedComp
		)
		{
			Assert.IsFalse(this.IsDisposed);
			
			var slot = this.moveActPool.GetFreeSlot();
			slot.Item.Init(characterController, worldDirComp, speedComp);
			return slot;
		}
		
		public void OnUpdate(float dt)
		{
			Assert.IsFalse(this.IsDisposed);

			foreach (var act in this.moveActPool)
				act.Update(dt);
		}
		
	}
}