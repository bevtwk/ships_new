#if UNITY_EDITOR
using UnityEditor;

namespace Framework.Components.Editor
{
	[CustomPropertyDrawer(typeof(SceneAssetReference))]
	public sealed class SceneAssetReferencePropertyDrawer : AssetReferencePropertyDrawer<SceneAsset>
	{ }
}
#endif
