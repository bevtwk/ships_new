using System;
using UnityEngine;

namespace Framework.Components.Values
{
	public class TransformPos : IValue<Vector3>, IDisposable
	{
		private Transform transform;
		public Vector3 Value
		{
			get => transform.position;
			set => transform.position = value;
		}
		
		public TransformPos(Transform transform)
		{
			this.transform = transform;
		}

		public void Dispose()
		{
			transform = null;
		}
	}
	
	public class TransformLocalPos : IValue<Vector3>, IDisposable
	{
		private Transform transform;
		public Vector3 Value
		{
			get => transform.localPosition;
			set => transform.localPosition = value;
		}
		
		public TransformLocalPos(Transform transform)
		{
			this.transform = transform;
		}

		public void Dispose()
		{
			transform = null;
		} 
	}
	
	public class TransformRot : IValue<Quaternion>, IDisposable
	{
		private Transform transform;
		public Quaternion Value
		{
			get => transform.rotation;
			set => transform.rotation = value;
		}
		
		public TransformRot(Transform transform)
		{
			this.transform = transform;
		}

		public void Dispose()
		{
			transform = null;
		}
	}
	
	public class TransformLocalRot : IValue<Quaternion>, IDisposable
	{
		private Transform transform;
		public Quaternion Value
		{
			get => transform.localRotation;
			set => transform.localRotation = value;
		}
		
		public TransformLocalRot(Transform transform)
		{
			this.transform = transform;
		}

		public void Dispose()
		{
			transform = null;
		}
	}
	
	public class TransformRotAxisAngle : IValue<Vector4>, IDisposable
	{
		private Transform transform;
		public Vector4 Value
		{
			get
			{
				transform.rotation.ToAngleAxis(out float angle, out Vector3 axis);
				return new Vector4(axis.x, axis.y, axis.z, angle);
			}
			
			set => transform.rotation = Quaternion.AngleAxis(value.w, new Vector3(value.x, value.y, value.z));
		}

		public TransformRotAxisAngle(Transform transform)
		{
			this.transform = transform;
		}

		public void Dispose()
		{
			transform = null;
		}
	}
	
	public class TransformLocalRotAxisAngle : IValue<Vector4>, IDisposable
	{
		private Transform transform;
		public Vector4 Value
		{
			get
			{
				transform.localRotation.ToAngleAxis(out float angle, out Vector3 axis);
				return new Vector4(axis.x, axis.y, axis.z, angle);
			}
			
			set => transform.localRotation = Quaternion.AngleAxis(value.w, new Vector3(value.x, value.y, value.z));
		}

		public TransformLocalRotAxisAngle(Transform transform)
		{
			this.transform = transform;
		}

		public void Dispose()
		{
			transform = null;
		}
	}
	
	public class TransformRotEuler : IValue<Vector3>, IDisposable
	{
		private Transform transform;
		public Vector3 Value
		{
			get => transform.rotation.eulerAngles;
			set => transform.rotation = Quaternion.Euler(value);
		}
		
		public TransformRotEuler(Transform transform)
		{
			this.transform = transform;
		}

		public void Dispose()
		{
			transform = null;
		}
	}
	
	public class TransformLocalRotEuler : IValue<Vector3>, IDisposable
	{
		private Transform transform;
		public Vector3 Value
		{
			get => transform.localRotation.eulerAngles;
			set => transform.localRotation = Quaternion.Euler(value);
		}
		
		public TransformLocalRotEuler(Transform transform)
		{
			this.transform = transform;
		}

		public void Dispose()
		{
			transform = null;
		}
	}
	
	public class TransformLocalScale : IValue<Vector3>, IDisposable
	{
		private Transform transform;
		public Vector3 Value
		{
			get => transform.localScale;
			set => transform.localScale = value;
		}
		
		public TransformLocalScale(Transform transform)
		{
			this.transform = transform;
		}

		public void Dispose()
		{
			transform = null;
		}
	}
}
