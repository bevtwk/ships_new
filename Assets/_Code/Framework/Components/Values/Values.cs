using System;

namespace Framework.Components.Values
{
	public interface IReadonlyValue<T>
	{
		T Value { get; }
	}
	
	public interface IValue<T> : IReadonlyValue<T>
	{
		new T Value { get; set; }
	}
	
	public class ValueObject<T> : IValue<T>, IDisposable
	{
		private T val;
		public T Value
		{
			get => this.val;
			set => this.val = value;
		}

		public ValueObject(in T val)
		{
			this.val = val;
		}
		
		public void Dispose()
		{
			this.val = default;
		}
	}

}
