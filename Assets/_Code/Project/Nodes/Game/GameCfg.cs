using System.Collections.Generic;
using Framework.Components;
using UnityEngine;

namespace Project
{
	[CreateAssetMenu(fileName = "GameCfg", menuName = "Configs/GameCfg", order = 1)]
	public class GameCfg : ScriptableObject
	{
		public string test;

		public MaterialResourceReference[] materials;
		
		public SceneAssetReference[] scenes; 
		
		public SceneAssetReference mainScene; 
		
	}
}