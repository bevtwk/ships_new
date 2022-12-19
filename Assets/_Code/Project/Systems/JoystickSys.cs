using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

using Framework.Components;
using Framework.Components.Values;
using Framework.Components.Handlers;
using Project.Components;

namespace Framework.Systems
{
	public class JoystickSys : CompositeDisposable, ISystem
	{
		private class IdleAct : Resetable
		{
			private Action onDone;

			public void Init(Action onDone)
			{
				this.onDone = onDone;
			}

			protected override void OnDispose()
			{
				Init(null);
				base.OnDispose();
			}

			public void Done() => this.onDone?.Invoke();
		}
		
		private class DragAct : Resetable
		{
			private IValue<Vector2> screenDirComp;
			private Action onDone;
			
			public void Init(IValue<Vector2> screenDirComp, Action onDone)
			{
				this.screenDirComp = screenDirComp;
				this.onDone = onDone;
			}
			
			protected override void OnDispose()
			{
				Init(null, null);
				base.OnDispose();
			}
			
			public void Done() => this.onDone?.Invoke();
			public void SetScreenDir(Vector2 dir) => this.screenDirComp.Value = dir;
		}

		private class JoystickAct : HandlerBase<JoystickAct, IDragEventsHandler>, 
			IDragEventsHandler
		{
			private Action<JoystickAct> onDispose;
			
			private JoystickView joystickView;
			public int Id => this.joystickView.GetInstanceID();
			
			private Camera uiCamera;
			
			private Vector2 joystickCenter = Vector2.zero;
			private Vector3 startPosition = Vector3.zero;
			private float bgRadius;

			public EnumerablePool<IdleAct> IdleActPool { get; private set; }
			public EnumerablePool<DragAct> DragActPool { get; private set; }

			public JoystickAct(Action<JoystickAct> onDispose)
			{
				this.onDispose = onDispose;
				this.IdleActPool = new EnumerablePool<IdleAct>();
				this.DragActPool = new EnumerablePool<DragAct>();
			}

			public void Init(JoystickView joystickView, Camera uiCamera)
			{
				base.Init(joystickView.DragHandlerComp);

				this.joystickView = joystickView;
				this.uiCamera = uiCamera;

				var background = joystickView.Background;
				this.startPosition = background.localPosition;
				this.bgRadius = (background.rect.width - joystickView.Stick.rect.width) * background.anchorMax.x;
			}

			protected override void OnDispose()
			{
				base.OnDispose();

				this.DragActPool.Dispose();
				this.IdleActPool.Dispose();
				
				this.joystickView = null;
				this.uiCamera = null;
			}

			public void OnBeginDrag(PointerEventData eventData)
			{
				if (this.IsDisposed)
					return;

				var joystickView = this.joystickView;
				
				Vector2 pointerPos = eventData.position;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickView.RectTransform, pointerPos, this.uiCamera,
					out pointerPos);

				joystickView.Background.localPosition = pointerPos;
				this.joystickCenter = pointerPos;

				foreach (var idleAct in this.IdleActPool)
					idleAct.Done();
			}

			public void OnEndDrag(PointerEventData eventData)
			{
				if (this.IsDisposed)
					return;
				
				var joystickView = this.joystickView;
				var startPosition = this.startPosition;
				
				joystickView.Background.localPosition = startPosition;
				joystickView.Stick.anchoredPosition = Vector2.zero;
				this.joystickCenter = startPosition;

				foreach (var dragAct in this.DragActPool)
					dragAct.Done();
			}

			public void OnDrag(PointerEventData eventData)
			{
				if (IsDisposed)
					return;
				
				var joystickView = this.joystickView;

				var pointerPos = eventData.position;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickView.RectTransform, pointerPos, this.uiCamera,
					out pointerPos);

				var dpos = pointerPos - this.joystickCenter;
				var r = this.bgRadius;

				var dir = dpos;

				if (dpos.sqrMagnitude <= (r * r))
				{
					dir *= (1f / r);
				}
				else
				{
					dir.Normalize();
					dpos = dir * r;
				}

				joystickView.Stick.localPosition = dpos;

				foreach (var dragAct in this.DragActPool)
					dragAct.SetScreenDir(dir);
			}
		}
		
		private readonly AutoPoolEx<JoystickAct> _joystickActAutoPool;
		private readonly Dictionary<int, PoolSlot<JoystickAct>> joystickActMap;
		
		public JoystickSys()
		{
			this.joystickActMap = new Dictionary<int, PoolSlot<JoystickAct>>();

			this._joystickActAutoPool = new AutoPoolEx<JoystickAct>(
				() => new JoystickAct(act => this.joystickActMap.Remove(act.Id))
			);

			Add(this._joystickActAutoPool);
			
		}
		
		protected override void OnDispose()
		{
			this.joystickActMap.Clear();

			base.OnDispose();
		}

		// Activities:
		IDisposable StartJoystickAct(JoystickView joystickView, Camera uiCamera)
		{
			Assert.IsFalse(this.IsDisposed);

			int joystickViewId = joystickView.GetInstanceID();
			if (this.joystickActMap.ContainsKey(joystickViewId))
				return new ErrDisposable("StartJoystickAct error");

			var slot = this._joystickActAutoPool.GetFreeSlot();
			slot.Item.Init(joystickView, uiCamera);
			
			this.joystickActMap.Add(joystickViewId, slot);

			return slot;
		}

		IDisposable StartIdleAct(JoystickView joystickView, Action onDone)
		{
			var joystickAct = GetJoystickAct(joystickView);
			if (joystickAct == null)
				return new ErrDisposable("StartIdleAct error");

			var slot = joystickAct.IdleActPool.GetFreeSlot();
			slot.Item.Init(onDone);

			return slot;
		}

		IDisposable StartDragAct(JoystickView joystickView, IValue<Vector2> screenDirComp, Action onDone)
		{
			var joystickAct = GetJoystickAct(joystickView);
			if (joystickAct == null)
				return new ErrDisposable("StartDragAct error");

			var slot = joystickAct.DragActPool.GetFreeSlot();
			slot.Item.Init(screenDirComp, onDone);
			
			return slot;
		}
		
		private JoystickAct GetJoystickAct(JoystickView joystickView)
		{
			Assert.IsFalse(this.IsDisposed);
			
			int joystickViewId = joystickView.GetInstanceID();
			if (this.joystickActMap.TryGetValue(joystickViewId, out var slot))
			{
				return slot.Item;
			}

			return null;
		}
	}
}
