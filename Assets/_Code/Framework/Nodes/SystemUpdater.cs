using System;
using System.Collections.Generic;

namespace Framework
{
	public interface ISystem : IBaseDisposable
	{ }

	public interface IUpdatableSystem : ISystem
	{
		void OnUpdate(float dt);
	}

	public interface ISystemUpdater: IDisposable
	{
		void Add(IUpdatableSystem srv);
	}

	public sealed class SystemUpdater : ISystemUpdater
	{
		private readonly List<IUpdatableSystem> updatableServices = new List<IUpdatableSystem>();

		public void Add(IUpdatableSystem sys)
		{
			if (sys != null && !sys.IsDisposed)
				this.updatableServices.Add(sys);
		}

		public void Update(float dt)
		{
			var updatableServices = this.updatableServices;

			int i = 0;
			for (int k = 0; k < updatableServices.Count; ++k)
			{
				var srv = updatableServices[k];
				if (!srv.IsDisposed)
				{
					srv.OnUpdate(dt);

					if (i < k)
					{
						updatableServices[i] = srv;
						updatableServices[k] = null;
					}

					++i;
				}
			}

			if (i < updatableServices.Count) 
				updatableServices.RemoveRange(i, updatableServices.Count - i);
		}

		public void Dispose()
		{
			this.updatableServices.Clear();
		}
	}
}
