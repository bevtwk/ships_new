using System;
using UnityEngine;

namespace Framework
{
	public interface IBaseDisposable : IDisposable
	{
		bool IsDisposed { get; }
	}
	
	public interface IDisposableEx : IDisposable
	{
		event Action<IDisposableEx> OnDisposeEvent;
	}
	
	public abstract class BaseDisposable : IBaseDisposable
	{
		public bool IsDisposed { get; protected set; }
		
		protected BaseDisposable()
		{
			this.IsDisposed = false;
		}
		
		public void Dispose()
		{
			if (!this.IsDisposed)
			{
				this.IsDisposed = true;
				OnDispose();
			}
		}
		
		protected virtual void OnDispose()
		{ }
	}
	
	public abstract class BaseDisposableEx : BaseDisposable, IDisposableEx
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

	public class ErrDisposable : IDisposable
	{
		private string error;

		public ErrDisposable(string error)
		{
			this.error = error;
			Debug.LogError(error);
		}
		
		public void Dispose() { }
	}
}
