using System;
using UnityEngine;
using UnityEngine.Assertions;

using Framework.Components.Values;

namespace Framework.Systems
{
	public class RectTransformSys : CompositeDisposable, IUpdatableSystem
	{
		private class UIRectFollowAct : Resetable
		{
			private IReadonlyValue<Vector3> targetWorldPos;
			private RectTransform uiRectTransform;
			private RectTransform uiParentRectTransform;
			private Camera mainCamera;
			private Camera uiCamera;

			public void Init(
				IReadonlyValue<Vector3> targetWorldPos,
				RectTransform uiRectTransform,
				Camera mainCamera,
				Camera uiCamera
			)
			{
				this.targetWorldPos = targetWorldPos;
				this.uiRectTransform = uiRectTransform;
				this.mainCamera = mainCamera;
				this.uiCamera = uiCamera;
				this.uiParentRectTransform = uiRectTransform.parent as RectTransform;
			}

			protected override void OnDispose()
			{
				Init(null, null, null, null);
				base.OnDispose();
			}

			public void Update(float dt)
			{
				var uiPosition = RectTransformUtility.WorldToScreenPoint(this.mainCamera, this.targetWorldPos.Value);
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.uiParentRectTransform, uiPosition, this.uiCamera, out var finalPosition))
					this.uiRectTransform.localPosition = finalPosition;
			}
		}

		private readonly EnumerablePool<UIRectFollowAct> uiRectFollowActPool;

		public RectTransformSys()
		{
			this.uiRectFollowActPool = Add(new EnumerablePool<UIRectFollowAct>());
		}
		
		IDisposable StartUIRectFollowAct(
			IReadonlyValue<Vector3> targetWorldPos,
			RectTransform uiRectTransform,
			Camera mainCamera,
			Camera uiCamera
		)
		{
			Assert.IsFalse(this.IsDisposed);

			var slot = this.uiRectFollowActPool.GetFreeSlot();
			slot.Item.Init(targetWorldPos, uiRectTransform, mainCamera, uiCamera);
			return slot;
		}

		public void OnUpdate(float dt)
		{
			Assert.IsFalse(this.IsDisposed);

			foreach (var act in this.uiRectFollowActPool)
				act.Update(dt);
		}
		
	}
}
