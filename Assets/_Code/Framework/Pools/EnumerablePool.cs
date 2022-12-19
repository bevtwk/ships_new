using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Framework
{
	public class EnumerablePool<T> : EnumerablePoolEx<T>
		where T : class, IResetable, new()
	{
		public EnumerablePool()
			: base(() => new T())
		{ }
	}

	public class EnumerablePoolEx<T> : PoolBase<T>
		where T : class, IResetable
	{
		private List<T> items;
		private int usedCount;
		private Func<T> createItem;

		public EnumerablePoolEx(Func<T> createItem)
		{
			this.items = new List<T>();
			this.usedCount = 0;
			this.createItem = createItem;
		}

		public override void Dispose()
		{
			var items = this.items;
			for (int k = 0, kend = this.usedCount; k < kend; ++k)
				items[k].Dispose();

			this.usedCount = 0;
			items.Clear();
		}

		protected override T GetFreeItemImpl()
		{
			var items = this.items;
			int k = this.usedCount;
			this.usedCount++;

			T item = null; 
			if (k < items.Count)
			{
				item = items[k];
				item.Reset();
			}
			else
			{
				Assert.IsTrue(k == items.Count);

				item = this.createItem.Invoke();
				this.items.Add(item);
			}

			return item;
		}
		
		public Enumerator GetEnumerator() => new Enumerator(this);

		public struct Enumerator : IDisposable
		{
			private EnumerablePoolEx<T> pool;
			private int idx;
			private int lastUsedIdx;
			
			public T Current => this.pool.items[this.lastUsedIdx];
			
			public Enumerator(EnumerablePoolEx<T> pool)
			{
				this.pool = pool;
				this.idx = -1;
				this.lastUsedIdx = -1;
			}
			
			public void Dispose()
			{
				this.pool = null;
				this.idx = -1;
				this.lastUsedIdx = -1;
			}
			
			public bool MoveNext()
			{
				var pool = this.pool;
				
				int usedCount = pool.usedCount;
				if (usedCount <= 0)
					return false;

				int idx = this.idx;
				if (idx == usedCount)
					return false;
				
				var items = pool.items;
				int lastUsedIdx = this.lastUsedIdx;

				if (idx < usedCount)
				{
					while(true)
					{
						++idx;

						if (idx == usedCount)
						{
							lastUsedIdx++;
							this.pool.usedCount = lastUsedIdx;
							
							this.idx = idx;
							this.lastUsedIdx = lastUsedIdx;
							
							return false;
						}

						var item = items[idx];
						if (!item.IsDisposed)
						{
							lastUsedIdx++;

							if (lastUsedIdx != idx)
							{
								items[idx] = items[lastUsedIdx];
								items[lastUsedIdx] = item;
							}

							break;
						}
					}
				}

				this.idx = idx;
				this.lastUsedIdx = lastUsedIdx;

				return true;
			}

			public void Reset()
			{
				this.idx = -1;
				this.lastUsedIdx = -1;
			}
		}
	}
}
