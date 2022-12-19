using System;
using Framework;
using UnityEngine;

namespace Project
{
	public interface IBasicPersistenceSrv
	{
		bool HasKey(string key);
		
		int GetInt(string key);
		string GetString(string key);
		
		bool TryGetInt(string key, out int val);
		bool TryGetString(string key, out string val);
		
		void SetInt(string key, int val);
		void SetString(string key, string val);

		void Save();
	}
	
	public class PlayerPrefsPersistenceSrv : IBasicPersistenceSrv
	{
		public bool HasKey(string key) => PlayerPrefs.HasKey(key);
		
		public int GetInt(string key) => PlayerPrefs.GetInt(key);
		public string GetString(string key) => PlayerPrefs.GetString(key);
		
		public bool TryGetInt(string key, out int val)
		{
			bool ret = PlayerPrefs.HasKey(key);
			val = ret ? PlayerPrefs.GetInt(key) : default;
			return ret;
		}
		
		public bool TryGetString(string key, out string val)
		{
			bool ret = PlayerPrefs.HasKey(key);
			val = ret ? PlayerPrefs.GetString(key) : default;
			return ret;
		}
		
		public void SetInt(string key, int val) => PlayerPrefs.SetInt(key, val);
		public void SetString(string key, string val) => PlayerPrefs.SetString(key, val);

		public void Save() => PlayerPrefs.Save();
	}
}
