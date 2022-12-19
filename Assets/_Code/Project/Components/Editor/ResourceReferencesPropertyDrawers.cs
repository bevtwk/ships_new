#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using Framework.Components;
using Framework.Components.Editor;

namespace Project.Components.Editor
{
	[CustomPropertyDrawer(typeof(GameCfgReference))]
	public sealed class GameCfgReferencePropertyDrawer : AssetReferencePropertyDrawer<GameCfg>
	{ }
	
}

#endif
