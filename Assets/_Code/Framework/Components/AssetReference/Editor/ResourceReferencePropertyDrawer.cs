#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Framework.Components.Editor
{
	[CustomPropertyDrawer(typeof(PrefabResourceReference))]
	public sealed class PrefabResourceReferencePropertyDrawer : AssetReferencePropertyDrawer<GameObject>
	{ }
	
	[CustomPropertyDrawer(typeof(SpriteResourceReference))]
	public sealed class SpriteResourceReferencePropertyDrawer : AssetReferencePropertyDrawer<Sprite>
	{ }

	[CustomPropertyDrawer(typeof(MaterialResourceReference))]
	public sealed class MaterialResourceReferencePropertyDrawer : AssetReferencePropertyDrawer<Material>
	{ }
	
	[CustomPropertyDrawer(typeof(TextureResourceReference))]
	public sealed class TextureResourceReferencePropertyDrawer : AssetReferencePropertyDrawer<Texture>
	{ }
	
	[CustomPropertyDrawer(typeof(Texture2DResourceReference))]
	public sealed class Texture2DResourceReferencePropertyDrawer : AssetReferencePropertyDrawer<Texture2D>
	{ }
}
#endif
