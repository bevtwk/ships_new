using System.Runtime.CompilerServices;
using Project.Components;
using UnityEngine;

namespace Project
{
	[CreateAssetMenu(fileName = "RootCfg", menuName = "Configs/RootCfg", order = 0)]
	public class RootCfg : ScriptableObject
	{
		[SerializeField] 
		private GameCfgReference gameCfgReference;
		public GameCfgReference GameCfgReference => this.gameCfgReference;
	}
}
