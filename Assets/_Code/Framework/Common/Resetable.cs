using System;
using UnityEngine;

namespace Framework
{
	public interface IResetable : IBaseDisposable
	{
		int Gen { get; }
		void Reset();
	}
	
	public abstract class Resetable : BaseDisposable, IResetable
	{
		public int Gen { get; private set; }

		protected Resetable()
		{
			this.Gen = 0;
		}

		protected override void OnDispose()
		{
			this.Gen++;
		}
		
		public void Reset()
		{
			if (this.IsDisposed)
			{
				this.IsDisposed = false;
				OnReset();
			}
			else
			{
				Debug.LogError("Trying to Reset not Disposed object!");
			}
		}
		
		protected virtual void OnReset()
		{ }
	}
	
	public interface IResetableEx : IResetable, IDisposableEx
	{ }

	public abstract class ResetableEx : Resetable, IResetableEx
	{
		private Action<IDisposableEx> onDisposeEvent;
		public event Action<IDisposableEx> OnDisposeEvent
		{
			add
			{
				if (!this.IsDisposed)
					this.onDisposeEvent += value;
			}

			remove { this.onDisposeEvent -= value; }
		}

		protected override void OnDispose()
		{
			this.onDisposeEvent?.Invoke(this);
			this.onDisposeEvent = null;

			base.OnDispose();
		}
	}
}
