using System;
using UnityEngine;

namespace Framework
{
	public abstract class Node : ResetableEx
	{
		public class Scope : CompositeDisposableEx, IResetable
		{
			public int Gen { get; private set; }

			public Scope()
			{
				this.Gen = 0;
			}

			protected override void OnDispose()
			{
				this.Gen++;

				base.OnDispose();
			}
			
			public void Reset()
			{
				if (this.IsDisposed)
				{
					this.IsDisposed = false;
				}
				else
				{
					Debug.LogError("Trying to Reset not Disposed object!");
				}
			}
		}
		
		private readonly INodeUpdater nodeUpdater;
		private readonly ISystemUpdater systemUpdater;
		
		private Container container;
		private bool ownContainer;
		
		protected readonly Scope scope;
		private readonly Scope disposables;
		
		private readonly Action<int> applyNextScope;
		private Action initNextScope;
		private int priority;
		
		private Node(INodeUpdater nodeUpdater, ISystemUpdater systemUpdater, Container container, bool ownContainer)
		{
			this.nodeUpdater = nodeUpdater;
			this.systemUpdater = systemUpdater;
			
			this.container = container;
			this.ownContainer = ownContainer;
			
			this.scope = new Scope();
			this.disposables = new Scope();

			this.applyNextScope = this.ApplyNextScope;
			
			this.initNextScope = null;
			this.priority = -1;
		}
		
		protected Node(INodeUpdater nodeUpdater, ISystemUpdater systemUpdater) 
			: this(nodeUpdater, systemUpdater, new Container(), true)
		{ }
		
		protected Node(Node parentNode, bool ownContainer = false) 
			: this(parentNode.nodeUpdater, parentNode.systemUpdater, 
				ownContainer ? new Container(parentNode.container) : parentNode.container,
				ownContainer)
		{ }
		
		protected override void OnDispose()
		{
			this.scope.Dispose();
			this.disposables.Dispose();

			if (ownContainer)
				this.container.Dispose();
			
			this.initNextScope = null;
			this.priority = -1;
			
			base.OnDispose();
		}

		protected override void OnReset()
		{
			this.scope.Reset();
			this.disposables.Reset();

			if (ownContainer)
				this.container.Reset();
			
			base.OnReset();
		}

		private void ApplyNextScope(int gen)
		{
			if (this.IsDisposed || this.Gen != gen)
				return;
			
			var initNextScope = this.initNextScope;
			this.initNextScope = null;

			this.scope.Dispose();
			this.scope.Reset();
			this.priority = -1;
			
			if (initNextScope == null)
			{
				UnityEngine.Debug.LogError($"Node.ApplyNextScope(): createNextScope == null !");
#if UNITY_EDITOR
				UnityEngine.Debug.Break();
#endif
				Dispose();
			}
			else
			{
				initNextScope.Invoke();
			}
		}

		public void NextScope(Action scopeInit, int priority)
		{
			if (this.IsDisposed)
				return;

			int prevPriority = this.priority;
			if ((prevPriority < 0) || (priority > prevPriority))
			{
				if (this.initNextScope == null)
					this.nodeUpdater.PushUpdateAction(this.applyNextScope, this.Gen);

				this.initNextScope = scopeInit;
				this.priority = priority;
			}
		}
		
		#region NextScope Callbacks
		
		protected void ScopeEv(out Action cb, Action scopeInit, int priority = 0)
		{
			cb = () => NextScope(scopeInit, priority);
		}

		protected void ScopeEv<T>(out Action<T> cb, Action<T> scopeInit, int priority = 0)
		{
			cb = (param) => NextScope(() => scopeInit(param), priority);
		}
		
		protected void ScopeEv<T1, T2>(out Action<T1, T2> cb, Action<T1, T2> scopeInit, int priority = 0)
		{
			cb = (param1, param2) => NextScope(() => scopeInit(param1, param2), priority);
		}
		
		/*protected Action<T1, T2, T3> ScopeEv<T1, T2, T3>(Action<T1, T2, T3> scopeInit, int priority = 0)
		{
			return (param1, param2, param3) => NextScope(() => scopeInit(param1, param2, param3), priority);
		}
		
		protected Action<T1, T2, T3, T4> ScopeEv<T1, T2, T3, T4>(Action<T1, T2, T3, T4> scopeInit, int priority = 0)
		{
			return (param1, param2, param3, param4) => NextScope(() => scopeInit(param1, param2, param3, param4), priority);
		}*/
		#endregion

		#region Node members registration
		protected TSys AddSystem<TSys>(TSys sys, bool register = true)
			where TSys : class, ISystem
		{
			if (this.IsDisposed)
			{
				UnityEngine.Debug.LogError($"Node.AddSystem(): trying to add System to disposed Node");
				return sys;
			}
			
			var updatableSys = sys as IUpdatableSystem;
			if (updatableSys != null)
				this.systemUpdater.Add(updatableSys);

			var disposables = this.disposables;
			
			if (register)
				disposables.Add(this.container.Register(sys));
			
			return disposables.Add(sys);
		}

		protected TSys GetSystem<TSys>()
			where TSys : class, ISystem
		{
			return this.container.Resolve<TSys>();
		}
		
		protected TData AddData<TData>(TData data, bool register = true)
			where TData: class, IDisposable
		{
			if (this.IsDisposed)
			{
				UnityEngine.Debug.LogError($"Node.AddData(): trying to add Data to disposed Node");
				return data;
			}
			
			var disposables = this.disposables;
			
			if (register)
				disposables.Add(this.container.Register(data));
			
			return disposables.Add(data);
		}
		
		protected TData GetData<TData>()
			where TData : class, IDisposable
		{
			return this.container.Resolve<TData>();
		}
		
		protected T Add<T>(T d)
			where T : class, IDisposable
		{
			return this.disposables.Add(d);
		}
		#endregion
	}
	
}
