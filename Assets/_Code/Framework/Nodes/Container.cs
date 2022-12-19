using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Framework
{
	public class Container : Resetable
	{
		private class Registration : IDisposable
		{
			private Container cont;
			private Type type;
			
			public Registration(Container cont, Type type)
			{
				this.cont = cont;
				this.type = type;
			}

			public void Dispose()
			{
				var cont = this.cont;
				var type = this.type;
				
				if (cont != null && type != null)
				{
					var registeredObjects = cont.registeredObjects;
					if (registeredObjects.TryGetValue(type, out var d))
					{
						registeredObjects.Remove(type);
					}
				
					this.cont = null;
					this.type = null;
				}
			}
		}
		
		private readonly Container parent;
		private readonly Dictionary<Type, IDisposable> registeredObjects;

		public Container(Container parent = null)
		{
			this.parent = parent;
			this.registeredObjects = new Dictionary<Type, IDisposable>();
		}

		protected override void OnDispose()
		{
			this.registeredObjects.Clear();
			
			base.OnDispose();
		}

		public IDisposable Register<T>(T value)
			where T: class, IDisposable
		{
			Assert.IsFalse(IsDisposed);
			
			var type = typeof(T);
			registeredObjects.Add(type, value);

			return new Registration(this, type);
		}

		public T Resolve<T>()
			where T: class, IDisposable	
		{
			Assert.IsFalse(IsDisposed);

			var type = typeof(T);
			
			Container cont = this;
			T val = null;
			while (cont != null)
			{
				var contRegisteredObjects = cont.registeredObjects;
				if (contRegisteredObjects.TryGetValue(type, out var d))
				{
					val = (T)d;
					break;
				}

				cont = cont.parent;
			}

			Assert.IsNotNull(val);
			return val;
		}
		

	}
	
	public class NamedContainer : BaseDisposable
	{
		private class Registration : IDisposable
		{
			private NamedContainer cont;
			private string name;
			private Type type;

			public Registration(NamedContainer cont, string name, Type type)
			{
				this.cont = cont;
				this.name = name;
				this.type = type;
			}

			public void Dispose()
			{
				var cont = this.cont;
				var type = this.type;
				var name = this.name;
				
				if (cont != null && type != null)
				{
					var registeredWithNameObjects = cont.registeredWithNameObjects;
					if (registeredWithNameObjects.TryGetValue(type, out var dict))
					{
						if (dict.TryGetValue(name, out var d))
							dict.Remove(name);
					}
					
					this.cont = null;
					this.type = null;
				}
			}
		}
		
		private readonly NamedContainer parent;
		private readonly Dictionary<Type, Dictionary<string, IDisposable>> registeredWithNameObjects;

		public NamedContainer(NamedContainer parent = null)
		{
			this.registeredWithNameObjects = new Dictionary<Type, Dictionary<string, IDisposable>>();
		}
		
		protected override void OnDispose()
		{
			foreach (var dictPair in this.registeredWithNameObjects)
			{
				var dict = dictPair.Value;
				dict.Clear();
			}

			this.registeredWithNameObjects.Clear();
		}
		
		public IDisposable Register<T>(string name, T value)
			where T: class, IDisposable
		{
			Assert.IsFalse(IsDisposed);
			
			var type = typeof(T);
			var registeredWithNameObjects = this.registeredWithNameObjects;
			
			Dictionary<string, IDisposable> dict = null;
			if (!registeredWithNameObjects.TryGetValue(type, out dict))
			{
				dict = new Dictionary<string, IDisposable>();
				registeredWithNameObjects.Add(type, dict);
			}

			dict.Add(name, value);

			return new Registration(this, name, type);
		}

		public T Resolve<T>(string name)
			where T: class, IDisposable	
		{
			Assert.IsFalse(IsDisposed);
			
			var type = typeof(T);
			
			NamedContainer cont = this;
			T val = null;
			while (cont != null)
			{
				var contRegisteredWithNameObjects = cont.registeredWithNameObjects;
				if (contRegisteredWithNameObjects.TryGetValue(type, out var dict))
				{
					if (dict.TryGetValue(name, out var d))
					{
						val = (T)d;
						break;
					}
				}

				cont = cont.parent;
			}

			Assert.IsNotNull(val);
			return val;
		}
	}
}
