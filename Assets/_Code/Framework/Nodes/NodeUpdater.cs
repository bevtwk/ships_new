using System;
using System.Collections.Generic;

namespace Framework
{
	public interface INodeUpdater: IDisposable
	{
		void PushUpdateAction(Action<int> a, int gen);
	}

	public class NodeUpdater : INodeUpdater
	{
		private readonly List< ValueTuple<Action<int>, int> > actions = new List< ValueTuple<Action<int>, int> >();
		
		public void PushUpdateAction(Action<int> a, int gen)
		{
			this.actions.Add(ValueTuple.Create(a, gen));
		}

		public void Update()
		{
			var actions = this.actions;
			for (int k = 0; k < actions.Count; ++k)
			{
				var action = actions[k];
				action.Item1.Invoke(action.Item2);
			}

			actions.Clear();
		}

		public void Dispose()
		{
			actions.Clear();
		}
	}
}
