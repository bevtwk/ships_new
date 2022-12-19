using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.Components
{
	public abstract class AssetReference<T> : AssetReference
		where T : UnityEngine.Object
	{
		public override Type AssetType => typeof(T);
	}
	
	[Serializable]
	public abstract class AssetReference : ISerializationCallbackReceiver
	{
		public abstract Type AssetType { get; }
		
#if UNITY_EDITOR
		[SerializeField]
		private UnityEngine.Object asset;
		
		[SerializeField]
		private string assetPath = string.Empty;
		public string AssetPath => this.assetPath;
#endif

		[SerializeField]
		private string path;
		public string Path => this.path;
		
		public void OnBeforeSerialize()
		{
#if UNITY_EDITOR
			SyncAsset(false);
#endif
		}

		public void OnAfterDeserialize()
		{
#if UNITY_EDITOR
			void HandleAfterDeserialize()
			{
				EditorApplication.update -= HandleAfterDeserialize;
				SyncAsset(true);
			}

			EditorApplication.update += HandleAfterDeserialize;
#endif
		}
		
#if UNITY_EDITOR
		protected abstract string GetRuntimePathFromAssetPath(string path);
		
		private void SyncAsset(bool canLoad)
		{
			var assetType = this.AssetType;
			if (assetType == null)
				return;
			
			var asset = this.asset;
			if ((asset != null) && assetType.IsInstanceOfType(asset))
			{
				var assetPath = AssetDatabase.GetAssetPath(asset);
				this.assetPath = assetPath;
				this.path = GetRuntimePathFromAssetPath(assetPath);
			}
			else if(canLoad)
			{
				RestoreFromAssetPath(assetType);
			}
		}
		
		private void RestoreFromAssetPath(Type assetType)
		{
			//var asset = string.IsNullOrEmpty(assetPath) ? null : AssetDatabase.LoadAssetAtPath(assetPath, assetType);
			//this.asset = asset;
			//this.path = (asset == null) ? string.Empty : GetRuntimePathFromAssetPath(assetPath);
			
			var assetPath = this.assetPath;
			this.asset = null;
			this.path = string.Empty;
			
			if (string.IsNullOrEmpty(assetPath))
				return;
			
			this.asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);
			if (this.asset != null)
				this.path = GetRuntimePathFromAssetPath(assetPath);
			else
				this.assetPath = string.Empty; // mb don't erase old path here?
		}
#endif
	}
	
}
