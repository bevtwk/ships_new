using System;
using UnityEngine;

namespace Framework.Components
{
	public abstract class ResourceReference<T> : ResourceReference
		where T : UnityEngine.Object
	{
		public override Type AssetType => typeof(T);
	}
	
	[Serializable]
	public abstract class ResourceReference : AssetReference
	{
#if UNITY_EDITOR
		private const string RESOURCES_FOLDER_NAME = "/Resources/";
		
		protected override string GetRuntimePathFromAssetPath(string path)
		{
			if (string.IsNullOrEmpty(path))
				return string.Empty;

			int idx = path.LastIndexOf(RESOURCES_FOLDER_NAME, StringComparison.CurrentCultureIgnoreCase);
			if (idx < 0)
				return string.Empty;

			idx += RESOURCES_FOLDER_NAME.Length;
			int extIdx = path.LastIndexOf('.');
			int end = (extIdx > idx) ? extIdx : path.Length;
			
			return path.Substring(idx, end - idx);
		}
#endif
	}
	
	[Serializable]
	public class PrefabResourceReference : ResourceReference<GameObject>
	{ }
	
	[Serializable]
	public class SpriteResourceReference : ResourceReference<Sprite>
	{ }

	[Serializable]
	public class MaterialResourceReference : ResourceReference<Material>
	{ }
	
	[Serializable]
	public class TextureResourceReference : ResourceReference<Texture>
	{ }
	
	[Serializable]
	public class Texture2DResourceReference : ResourceReference<Texture2D>
	{ }
	
}
