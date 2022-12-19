using System;

namespace Framework
{
	public sealed class PoolSlot<T> : IDisposable
		where T : class, IResetable
	{
		private T item;
		private int gen;

		public bool HasItem
		{
			get
			{
				var item = this.item;
				return (item != null) && (item.Gen == this.gen);
			}
		}

		public T Item => HasItem ? this.item : null;

		public PoolSlot(T item)
		{
			this.item = item;
			this.gen = item.Gen;
		}

		public void Dispose()
		{
			var item = this.item;
			if (item != null)
			{
				if (item.Gen == this.gen)
					item.Dispose();

				this.item = null;
			}

			this.gen = -1;
		}
	}

	public abstract class PoolBase<T> : IDisposable
		where T : class, IResetable
	{
		public PoolSlot<T> GetFreeSlot() => new PoolSlot<T>(GetFreeItemImpl());

		protected abstract T GetFreeItemImpl();
		public abstract void Dispose();
	}
}
