using System;
using System.Collections.Generic;
using Framework.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

using Framework.Components.Values;

namespace Framework.Systems
{
	public class DisposableScene : IDisposable
	{
		private Scene scene;
		public DisposableScene(Scene scene)
		{
			this.scene = scene;
		}

		public void Dispose()
		{
			if (this.scene.IsValid() && this.scene.isLoaded)
			{
				SceneManager.UnloadScene(scene);
			}
		}
		
		public TView GetView<TView>() 
			where TView : MonoBehaviour
		{
			if (scene.IsValid() && scene.isLoaded)
			{
				var sceneRoots = scene.GetRootGameObjects();
				for (int k = 0, kend = sceneRoots.Length; k < kend; ++k)
				{
					var root = sceneRoots[k];
					var view = root.GetComponent<TView>();
					if (view != null)
						return view;
				}
			}

			return null;
		}
	}
	
	public class SceneLoaderSys : CompositeDisposable, IUpdatableSystem
	{
		private class SceneLoadingAct : Resetable
		{
			private string sceneName;
			private IValue<float> progressValue;
			private Action<DisposableScene> onLoaded;

			private AsyncOperation asyncOp;
			
			public void Init(
				string sceneName, 
				IValue<float> progressValue, 
				Action<DisposableScene> onLoaded
			)
			{
				this.sceneName = sceneName;
				this.progressValue = progressValue;
				this.onLoaded = onLoaded;

				this.asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
				this.asyncOp.allowSceneActivation = true;
			}

			protected override void OnDispose()
			{
				this.sceneName = null;
				this.progressValue = null;
				this.onLoaded = null;
				
				base.OnDispose();
			}

			public void Loaded(Scene scene)
			{
				if (!IsDisposed)
				{
					SceneManager.SetActiveScene(scene);
					
					this.onLoaded?.Invoke(new DisposableScene(scene));
					Dispose();
				}
			}
			
			public void Update()
			{
				var progressValue = this.progressValue;
				var asyncOp = this.asyncOp;
				
				if(progressValue != null && asyncOp != null)
				{
					progressValue.Value = asyncOp.isDone ? 1f : asyncOp.progress;
				}
			}
		}
		
		
		private EnumerablePool<SceneLoadingAct> sceneLoadingActPool;
		
		private Dictionary<string, PoolSlot<SceneLoadingAct>> loadingCache;
		
		public SceneLoaderSys()
		{
			this.loadingCache = new Dictionary<string, PoolSlot<SceneLoadingAct>>();
			
			this.sceneLoadingActPool = Add(new EnumerablePool<SceneLoadingAct>());
			
			SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
		}

		protected override void OnDispose()
		{
			base.OnDispose();

			this.loadingCache.Clear();
			SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
		}

		public IDisposable StartSceneLoadingAct(string sceneName, IValue<float> progressValue, Action<DisposableScene> onLoaded)
		{
			var slot = this.sceneLoadingActPool.GetFreeSlot();
			this.loadingCache.Add(sceneName, slot);
			
			slot.Item.Init(sceneName, progressValue, onLoaded);

			return slot;
		}
		
		public IDisposable StartSceneLoadingAct(SceneAssetReference sceneAssetReference, IValue<float> progressValue, Action<DisposableScene> onLoaded)
		{
			return StartSceneLoadingAct(sceneAssetReference.Path, progressValue, onLoaded);
		}
		
		public void OnUpdate(float dt)
		{
			foreach (var act in this.sceneLoadingActPool)
				act.Update();
		}
		
		private void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (scene.isLoaded)
			{
				string sceneName = scene.name;
				if (this.loadingCache.TryGetValue(sceneName, out var slot))
				{
					this.loadingCache.Remove(sceneName);

					if (slot.HasItem)
						slot.Item.Loaded(scene);
				}
			}
		}
		
	}
}
