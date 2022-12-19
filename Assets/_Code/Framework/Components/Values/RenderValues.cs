using System;
using UnityEngine;

namespace Framework.Components.Values
{
	public class MaterialMainColor : IValue<Color>, IDisposable
	{
		private Material material;

		public Color Value
		{
			get => material.color;
			set => material.color = value;
		}

		public MaterialMainColor(Material material)
		{
			this.material = material;
		}

		public void Dispose()
		{
			this.material = null;
		}
	}
	
	public class MaterialColor : IValue<Color>, IDisposable
	{
		private Material material;
		private int propId;

		public Color Value
		{
			get => material.GetColor(this.propId);
			set => material.SetColor(this.propId, value);
		}

		public MaterialColor(Material material, string propName)
		{
			this.material = material;
			this.propId = Shader.PropertyToID(propName);
		}

		public void Dispose()
		{
			this.material = null;
			this.propId = -1;
		}
	}
	
	public class MaterialMainTexOffset : IValue<Vector2>, IDisposable
	{
		private Material material;

		public Vector2 Value
		{
			get => material.mainTextureOffset;
			set => material.mainTextureOffset = value;
		}

		public MaterialMainTexOffset(Material material)
		{
			this.material = material;
		}

		public void Dispose()
		{
			this.material = null;
		}
	}
	
	public class MaterialMainTexScale : IValue<Vector2>, IDisposable
	{
		private Material material;

		public Vector2 Value
		{
			get => material.mainTextureScale;
			set => material.mainTextureScale = value;
		}

		public MaterialMainTexScale(Material material)
		{
			this.material = material;
		}

		public void Dispose()
		{
			this.material = null;
		}
	}
	
	public class MaterialTexOffset : IValue<Vector2>, IDisposable
	{
		private Material material;
		private int propId;
		
		public Vector2 Value
		{
			get => material.GetTextureOffset(this.propId);
			set => material.SetTextureOffset(this.propId, value);
		}

		public MaterialTexOffset(Material material, string propName)
		{
			this.material = material;
			this.propId = Shader.PropertyToID(propName);
		}

		public void Dispose()
		{
			this.material = null;
			this.propId = -1;
		}
	}
	
	public class MaterialTexScale : IValue<Vector2>, IDisposable
	{
		private Material material;
		private int propId;
		
		public Vector2 Value
		{
			get => material.GetTextureScale(this.propId);
			set => material.SetTextureScale(this.propId, value);
		}

		public MaterialTexScale(Material material, string propName)
		{
			this.material = material;
			this.propId = Shader.PropertyToID(propName);
		}

		public void Dispose()
		{
			this.material = null;
			this.propId = -1;
		}
	}
	
	public class MaterialFloatProp : IValue<float>, IDisposable
	{
		private Material material;
		private int propId;
		
		public float Value
		{
			get => material.GetFloat(this.propId);
			set => material.SetFloat(this.propId, value);
		}

		public MaterialFloatProp(Material material, string propName)
		{
			this.material = material;
			this.propId = Shader.PropertyToID(propName);
		}

		public void Dispose()
		{
			this.material = null;
			this.propId = -1;
		}
	}
	
	public class MaterialVec2Prop : IValue<Vector2>, IDisposable
	{
		private Material material;
		private int propId;
		
		public Vector2 Value
		{
			get
			{
				var vec = material.GetVector(this.propId);
				return new Vector2(vec.x, vec.y);
			}
			
			set => material.SetVector(this.propId, new Vector4(value.x, value.y, 0f, 0f));
		}

		public MaterialVec2Prop(Material material, string propName)
		{
			this.material = material;
			this.propId = Shader.PropertyToID(propName);
		}

		public void Dispose()
		{
			this.material = null;
			this.propId = -1;
		}
	}
	
	public class MaterialVec3Prop : IValue<Vector3>, IDisposable
	{
		private Material material;
		private int propId;
		
		public Vector3 Value
		{
			get
			{
				var vec = material.GetVector(this.propId);
				return new Vector3(vec.x, vec.y, vec.z);
			}
			
			set => material.SetVector(this.propId, new Vector4(value.x, value.y, value.z, 0f));
		}

		public MaterialVec3Prop(Material material, string propName)
		{
			this.material = material;
			this.propId = Shader.PropertyToID(propName);
		}

		public void Dispose()
		{
			this.material = null;
			this.propId = -1;
		}
	}
	
	public class MaterialVec4Prop : IValue<Vector4>, IDisposable
	{
		private Material material;
		private int propId;
		
		public Vector4 Value
		{
			get => material.GetVector(this.propId);
			set => material.SetVector(this.propId, value);
		}

		public MaterialVec4Prop(Material material, string propName)
		{
			this.material = material;
			this.propId = Shader.PropertyToID(propName);
		}

		public void Dispose()
		{
			this.material = null;
			this.propId = -1;
		}
	}
	
	public class CameraFoV : IValue<float>, IDisposable
	{
		private Camera camera;
		
		public float Value
		{
			get => camera.fieldOfView;
			set => camera.fieldOfView = value;
		}

		public CameraFoV(Camera camera)
		{
			this.camera = camera;
		}

		public void Dispose()
		{
			this.camera = null;
		}
	}
	
	public class CameraAspect : IValue<float>, IDisposable
	{
		private Camera camera;
		
		public float Value
		{
			get => camera.aspect;
			set => camera.aspect = value;
		}

		public CameraAspect(Camera camera)
		{
			this.camera = camera;
		}

		public void Dispose()
		{
			this.camera = null;
		}
	}
	
	public class CameraOrthoSize : IValue<float>, IDisposable
	{
		private Camera camera;
		
		public float Value
		{
			get => camera.orthographicSize;
			set => camera.orthographicSize = value;
		}

		public CameraOrthoSize(Camera camera)
		{
			this.camera = camera;
		}

		public void Dispose()
		{
			this.camera = null;
		}
	}
	
}
