#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Framework.Components.Editor
{
	public class AssetReferencePropertyDrawer<T> : PropertyDrawer
		where T : UnityEngine.Object
	{
		private const string assetPropertyString = "asset";
		private const string assetPathPropertyString = "assetPath";
		private const string runtimePathPropertyString = "path";
		
		protected static readonly float LINE_HEIGHT = EditorGUIUtility.singleLineHeight;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			
				GUI.Box(position, GUIContent.none);
				
				var assetProperty = property.FindPropertyRelative(assetPropertyString);
				var assetPathProperty = property.FindPropertyRelative(assetPathPropertyString);
				var pathProperty = property.FindPropertyRelative(runtimePathPropertyString);
				
				var assetControlID = GUIUtility.GetControlID(FocusType.Passive);
				
				EditorGUI.BeginChangeCheck();
					assetProperty.objectReferenceValue = EditorGUI.ObjectField(
						new Rect(position.x, position.y, position.width, LINE_HEIGHT),
						label, assetProperty.objectReferenceValue, typeof(T), false);
				
				if (EditorGUI.EndChangeCheck())
				{
					if (assetProperty.objectReferenceValue == null)
					{
						pathProperty.stringValue = string.Empty;
						assetPathProperty.stringValue = string.Empty;
					}
				}
				
				if (assetProperty.objectReferenceValue != null)
				{
					var labelWidth = EditorGUIUtility.labelWidth;
					
					position.x += labelWidth;
					position.width -= labelWidth;
					
					position.y += LINE_HEIGHT;
					position.height = LINE_HEIGHT;

					var path = pathProperty.stringValue;
					var assetPath = assetPathProperty.stringValue;

					bool hasPath = string.IsNullOrEmpty(path);
					var iconContent = EditorGUIUtility.IconContent(hasPath ? "d_winbtn_mac_close" : "d_winbtn_mac_max");
					var labelContent = new GUIContent( hasPath ? $"assetPath:'{assetPath}'" : path);

					var labelRect = position;
					labelRect.width = EditorGUIUtility.labelWidth;
					var iconRect = labelRect;
					iconRect.width = iconContent.image.width;
					labelRect.width -= iconRect.width;
					labelRect.x += iconRect.width;
					
					EditorGUI.PrefixLabel(iconRect, assetControlID, iconContent);
					EditorGUI.PrefixLabel(labelRect, assetControlID, labelContent);
				}
				
			EditorGUI.EndProperty();
		}
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var sceneAssetProperty = property.FindPropertyRelative(assetPropertyString);
			var lines = (sceneAssetProperty.objectReferenceValue != null) ? 2f : 1f;
			
			return LINE_HEIGHT * lines;
		}
		
	}
}

#endif
