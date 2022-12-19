using System;
using Framework;
using Framework.Systems;
using UnityEngine;

namespace Project
{
	public class RootNode : Node
	{
		public readonly RootCfg RootCfg;
		public readonly RootView RootView;

		private ResourceLoaderSys resourceLoaderSys;
		
		private readonly Action bootEv;
		//private readonly Action<DisposableResource<GameCfg>> gameEv;
		
		public RootNode(
			INodeUpdater nodeUpdater, ISystemUpdater systemUpdater, 
			RootCfg rootCfg, RootView rootView
		) 
		: base(nodeUpdater, systemUpdater)
		{
			this.RootCfg = rootCfg;
			this.RootView = rootView;

			this.resourceLoaderSys = this.AddSystem(new ResourceLoaderSys());
			
			ScopeEv(out this.bootEv, BootScope, 0);
			//ScopeEv(out this.gameEv, GameScope, 0);
			
			this.bootEv.Invoke();
		}
		
		private void BootScope()
		{
			Debug.LogError("BootScope");
			
			var s = this.scope;
			//s.Add(this.resourceLoaderSys.StartResourceLoadingAct(RootCfg.GameCfgReference, null, gameEv));
			
		}
		
		/*private void GameScope(DisposableResource<GameCfg> dRes)
		{
			Debug.LogError("GameScope");
			
			var s = this.scope;
			s.Add(dRes);
			
			var gameCfg = dRes.Asset;
			Debug.LogError($"GameScope ({gameCfg.test})");
		}*/
	}
}