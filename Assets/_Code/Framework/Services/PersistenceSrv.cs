using System;
using Framework;
using Framework.Utils;

namespace Project
{
	public interface IPersistenceSrv : IBasicPersistenceSrv
	{
		bool TryGetValue<T>(string key, out T value) 
			where T : struct;

		bool TryGetValues<T>(string key, out T[] values) 
			where T : struct;
		
		void SetValue<T>(string key, T value) 
			where T : struct;
		
		void SetValues<T>(string key, T[] values)
			where T : struct;
	}
	
	public class Ascii85PersistenceSrv : IPersistenceSrv
	{
		private readonly IBasicPersistenceSrv bps;

		public Ascii85PersistenceSrv(IBasicPersistenceSrv bps)
		{
			this.bps = bps;
		}
		
		public bool HasKey(string key) => this.bps.HasKey(key);
		public int GetInt(string key) => this.bps.GetInt(key);
		public string GetString(string key) => this.bps.GetString(key);
		public bool TryGetInt(string key, out int val) => this.bps.TryGetInt(key, out val);
		public bool TryGetString(string key, out string val) => this.bps.TryGetString(key, out val);
		public void SetInt(string key, int val) => this.bps.SetInt(key, val);
		public void SetString(string key, string val) => this.bps.SetString(key, val);
		public void Save() => this.bps.Save();
		
		public bool TryGetValue<T>(string key, out T value) 
			where T : struct
		{
			if (bps.TryGetString(key, out var str))
			{
				var bytes = Ascii85Converter.Decode(str);
				value = BlittableConverter.ValueFromBytes<T>(bytes);
				return true;
			}

			value = default;
			return false;
		}

		public bool TryGetValues<T>(string key, out T[] values) 
			where T : struct
		{
			if (bps.TryGetString(key, out var str))
			{
				var bytes = Ascii85Converter.Decode(str);
				values = BlittableConverter.ArrayFromBytes<T>(bytes);
				return true;
			}

			values = default;
			return false;
		}

		public void SetValue<T>(string key, T value) 
			where T : struct
		{
			var bytes = BlittableConverter.ValueToBytes<T>(value); 
			var str = Ascii85Converter.Encode(bytes);
			this.bps.SetString(key, str);
		}
		
		public void SetValues<T>(string key, T[] values)
			where T : struct
		{
			var bytes = BlittableConverter.ArrayToBytes<T>(values); 
			var str = Ascii85Converter.Encode(bytes);
			this.bps.SetString(key, str);
		}

	}
}