using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Framework
{
	public class CompositeDisposable : BaseDisposable
	{
		private List<IDisposable> disposables;
		
		protected override void OnDispose()
		{
			var disposables = this.disposables;
			
			if (disposables != null)
			{
				int disposablesCount = disposables.Count;
				if (disposablesCount > 0)
				{
					for (int k = (disposablesCount - 1); k >= 0; --k)
					{
						var d = disposables[k];
						d.Dispose();
					}

					disposables.Clear();
				}
			}

			base.OnDispose();
		}

		public TDisposable Add<TDisposable>(TDisposable d)
			where TDisposable : class, IDisposable
		{
			if (d == null)
				return null;

			if (IsDisposed)
				return null;

			var disposables = this.disposables;
			if (disposables == null)
			{
				disposables = new List<IDisposable>();
				this.disposables = disposables;

				disposables.Add(d);
			}
			else if (!disposables.Contains(d))
			{
				disposables.Add(d);
			}

			return d;
		}
	}

	public class CompositeDisposableEx : CompositeDisposable
	{
		private List<IDisposable> autoDisposables;
		
		protected override void OnDispose()
		{
			var autoDisposables = this.autoDisposables;
			if (autoDisposables != null)
			{
				int autoDisposablesCount = autoDisposables.Count;
				if (autoDisposablesCount > 0)
				{
					for (int k = (autoDisposablesCount - 1); k >= 0; --k)
					{
						var d = autoDisposables[k];
						d.Dispose();
					}

					Assert.IsTrue(0 == autoDisposables.Count);
					autoDisposables.Clear();
				}
			}
			
			base.OnDispose();
		}

		private bool RemoveAuto(IDisposable d)
		{
			var autoDisposables = this.autoDisposables;
			if (autoDisposables == null)
				return false;

			return autoDisposables.Remove(d);
		}
		
		public TDisposable AddAuto<TDisposable>(TDisposable d)
			where TDisposable : class, IDisposableEx
		{
			if (d == null)
				return null;
		
			if (IsDisposed)
				return null;
		
			var autoDisposables = this.autoDisposables;
			if (autoDisposables == null)
			{
				autoDisposables = new List<IDisposable>();
				this.autoDisposables = autoDisposables;

				autoDisposables.Add(d);
			}
			else if (!autoDisposables.Contains(d))
			{
				autoDisposables.Add(d);
			}

			void AutoRemoveImpl(IDisposableEx param)
			{
				RemoveAuto(param);
				param.OnDisposeEvent -= AutoRemoveImpl;
			}
			
			d.OnDisposeEvent += AutoRemoveImpl;
			return d;
		}
	}
}
