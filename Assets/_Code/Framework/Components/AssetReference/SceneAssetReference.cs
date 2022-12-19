using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.Components
{
	[Serializable]
	public class SceneAssetReference : AssetReference<SceneAsset>
	{
#if UNITY_EDITOR
		protected override string GetRuntimePathFromAssetPath(string path)
		{
			var pathToFind = path.Replace("\\", "/");
			
			var scenes = EditorBuildSettings.scenes;
			for (int k = 0, kend = scenes.Length; k < kend; ++k)
			{
				var scene = scenes[k];
				if (0 == string.Compare(scene.path, pathToFind, true))
					return scene.enabled ? pathToFind : string.Empty;
			}
			
			return string.Empty;
		}
#endif
	}
}
