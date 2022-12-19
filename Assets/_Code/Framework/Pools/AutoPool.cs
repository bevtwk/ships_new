using System;
using System.Collections.Generic;

namespace Framework
{
	public class AutoPool<T> : AutoPoolEx<T>
		where T : class, IResetableEx, new()
	{
		public AutoPool()
			: base(() => new T())
		{ }
	}
	
	public class AutoPoolEx<T> : PoolBase<T>
		where T : class, IResetableEx
	{
		private List<T> items;
		private List<T> freeItems;
		private Func<T> createItem;

		public AutoPoolEx(Func<T> createItem)
		{
			this.items = new List<T>();
			this.freeItems = new List<T>();
			this.createItem = createItem;
		}
		
		public override void Dispose()
		{
			var items = this.items;
			for (int k = 0, kend = items.Count; k < kend; ++k)
			{
				var item = items[k];
				if (!item.IsDisposed) 
					item.Dispose();
			}
			items.Clear();
			
			this.freeItems.Clear();
		}

		protected override T GetFreeItemImpl()
		{
			var freeItems = this.freeItems;
			int freeItemsLastIdx = freeItems.Count - 1;

			T item = null;
			if (freeItemsLastIdx >= 0)
			{
				item = freeItems[freeItemsLastIdx];
				item.Reset();
				freeItems.RemoveAt(freeItemsLastIdx);
			}
			else
			{
				item = this.createItem.Invoke();
				this.items.Add(item);
			}
			
			void AutoRemoveImpl(IDisposableEx d)
			{
				freeItems.Add(d as T);
				item.OnDisposeEvent -= AutoRemoveImpl;
			}

			item.OnDisposeEvent += AutoRemoveImpl;
			
			return item;
		}
	}
}
