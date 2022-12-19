using System;
using System.Collections.Generic;
using Framework.Components;
using UnityEngine;

using Framework.Components.Values;
using UnityEngine.Assertions;

namespace Framework.Systems
{
	public class SingleResDisposable : BaseDisposable
	{
		private UnityEngine.Object asset;
		public UnityEngine.Object Asset => this.asset;

		private Action<UnityEngine.Object> releaseResCb;
		
		public SingleResDisposable(UnityEngine.Object asset, Action<UnityEngine.Object> releaseResCb)
		{
			this.asset = asset;
			this.releaseResCb = releaseResCb;
		}

		protected override void OnDispose()
		{
			var asset = this.asset;
			this.asset = null;
			
			var releaseResCb = this.releaseResCb;
			this.releaseResCb = null;

			if (asset != null && releaseResCb != null)
				releaseResCb.Invoke(asset);
		}
	}
	
	public class ResListDisposable : BaseDisposable
	{
		private UnityEngine.Object[] assets;
		public UnityEngine.Object[] Assets => this.assets;
		
		private Action<UnityEngine.Object> releaseResCb;

		public ResListDisposable(UnityEngine.Object[] assets, Action<UnityEngine.Object> releaseResCb)
		{
			this.assets = assets;
			this.releaseResCb = releaseResCb;
		}

		protected override void OnDispose()
		{
			var assets = this.assets;
			this.assets = null;

			var releaseResCb = this.releaseResCb;
			this.releaseResCb = null;
			
			if (assets != null && releaseResCb != null)
			{
				for (int k = 0, kend = assets.Length; k < kend; ++k)
				{
					var asset = assets[k];
					if (asset != null)
						releaseResCb.Invoke(asset);
				}
			}
		}
	}
	
	public class ResourceLoaderSys : CompositeDisposable, IUpdatableSystem
	{
		private class SingleResLoadingAct : Resetable
		{
			private ResourceLoaderSys sys;
			public ResourceLoaderSys Sys => this.sys;

			private ResourceReference resourceRef;
			public ResourceReference ResourceRef => this.resourceRef;

			private IValue<float> progressValue;
			private Action<SingleResDisposable> onLoaded;

			private SingleResRequest singleResRequest;
			
			public void Init(ResourceLoaderSys sys, ResourceReference resourceRef, IValue<float> progressValue, Action<SingleResDisposable> onLoaded)
			{
				this.sys = sys;
				this.resourceRef = resourceRef;
				this.progressValue = progressValue;
				this.onLoaded = onLoaded;
				
				this.singleResRequest = new SingleResRequest(this);
			}
			
			protected override void OnDispose()
			{
				var singleResRequest = this.singleResRequest;
				this.singleResRequest = null;
				singleResRequest?.Dispose();
				
				this.sys = null;
				this.resourceRef = null;
				this.progressValue = null;
				this.onLoaded = null;

				base.OnDispose();
			}

			public void Update(float dt)
			{
				var progressValue = this.progressValue;
				if (progressValue != null)
					progressValue.Value = this.singleResRequest.GetProgress();
			}
			
			public void OnLoaded()
			{}
		}
		
		private class ResListLoadingAct : Resetable
		{
			public void Init(ResourceLoaderSys sys, List<ResourceReference> resourceRefList,
				IValue<float> progressValue, Action<ResListDisposable> onLoaded)
			{ }
			
			protected override void OnDispose()
			{
				
				base.OnDispose();
			}

			public void Update(float dt)
			{ }
		}
		
		private abstract class LoadingRequest : BaseDisposable
		{
			protected abstract void OnAssetLoaded(UnityEngine.Object asset);

			public abstract float GetProgress();
		}
		
		private class SingleResRequest : LoadingRequest
		{
			private SingleResLoadingAct act;
			
			public SingleResRequest(SingleResLoadingAct act)
			{
				
			}
			
			protected override void OnDispose()
			{
				
				base.OnDispose();
			}
			
			protected override void OnAssetLoaded(UnityEngine.Object asset)
			{ }
			
			public override float GetProgress()
			{
				return 1f;
			}
		}
		
		private class ListResEntryRequest : LoadingRequest
		{
			private ResListLoadingAct act;
			private int idx;
			
			public ListResEntryRequest(ResListLoadingAct act, int idx)
			{
				
			}
			
			protected override void OnAssetLoaded(UnityEngine.Object asset)
			{ }
			
			public override float GetProgress()
			{
				return 1f;
			}
		}
		
		
		private struct LoadedResource
		{
			public UnityEngine.Object Asset;
			public string Path;
			public int RefCount;
		}

		private Dictionary<int, LoadedResource> loadedResources;
		private Dictionary<string, int> loadedPathToAssetId;

		private Dictionary<string, List<LoadingRequest>> loadingRequests;
		private Dictionary<string, SingleResDisposable> singleResDisposableCache;
		
		private EnumerablePool<SingleResLoadingAct> singleResLoadingActPool;
		private EnumerablePool<ResListLoadingAct> resListLoadingActActPool;

		private Action<UnityEngine.Object> releaseResCb;

		
		public ResourceLoaderSys()
		{
			this.singleResLoadingActPool = Add(new EnumerablePool<SingleResLoadingAct>());
			this.resListLoadingActActPool = Add(new EnumerablePool<ResListLoadingAct>());
			
			
			//UnityEngine.Object obj;
			//int id = obj.GetInstanceID();

			this.releaseResCb = ReleaseRes;
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			
		}

		public IDisposable StartResLoadingAct(ResourceReference resourceRef, IValue<float> progressValue, Action<SingleResDisposable> onLoaded)
		{
			var slot = this.singleResLoadingActPool.GetFreeSlot();
			slot.Item.Init(this, resourceRef, progressValue, onLoaded);
			return slot;
		}
		
		public IDisposable StartResListLoadingAct(List<ResourceReference> resourceRefList, IValue<float> progressValue, Action<ResListDisposable> onLoaded)
		{
			var slot = this.resListLoadingActActPool.GetFreeSlot();
			slot.Item.Init(this, resourceRefList, progressValue, onLoaded);
			return slot;
		}
		
		/*private void LoadRes(ResourceReference resourceRef, LoadingRequest loadingRequest)
		{
			var resourceRequest = Resources.LoadAsync(resourceRef.Path, resourceRef.AssetType);
				
			void ResourceLoaded(AsyncOperation ao)
			{
				ao.completed -= ResourceLoaded;
				
				//ResourceLoadedImpl(gen, ao, onLoaded);
			}
			
			resourceRequest.completed += ResourceLoaded;
		}*/
		
		private void ReleaseRes(UnityEngine.Object asset)
		{
			
		}
		
		public void OnUpdate(float dt)
		{ }
	}
}
