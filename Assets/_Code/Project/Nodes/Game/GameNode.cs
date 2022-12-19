using System;
using Framework;
using UnityEngine;

namespace Project
{
	public class GameNode : Node
	{
		public readonly GameCfg GameCfg;
		
		private readonly Action loadMainMenuEv;
		private readonly Action<GameCfg> loadJourneyEv;
		
		
		public GameNode(RootNode rootNode, GameCfg gameCfg)
		: base(rootNode)
		{
			this.GameCfg = gameCfg;
			
			
		}
	}
	
}