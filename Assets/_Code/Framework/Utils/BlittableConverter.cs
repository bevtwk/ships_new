using System;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;

namespace Framework.Utils
{
	public static class BlittableConverter
	{
		public static byte[] ValueToBytes<T>(T value) 
			where T : struct
		{
			int structSize = Marshal.SizeOf<T>();
			byte[] byteArray = new byte[structSize];

			IntPtr ptr = Marshal.AllocHGlobal(structSize);
			Marshal.StructureToPtr(value, ptr, true);
			Marshal.Copy(ptr, byteArray, 0, structSize);
			Marshal.FreeHGlobal(ptr);
			return byteArray;
		}

		public static T ValueFromBytes<T>(byte[] byteArray) 
			where T : struct
		{
			int structSize = Marshal.SizeOf<T>();
			Assert.IsTrue(byteArray.Length == structSize);
			
			IntPtr ptr = Marshal.AllocHGlobal(structSize);
			Marshal.Copy(byteArray, 0, ptr, structSize);
			T value = Marshal.PtrToStructure<T>(ptr);
			Marshal.FreeHGlobal(ptr);

			return value;
		}

		public static byte[] ArrayToBytes<T>(T[] array)
			where T : struct
		{
			if (array == null || array.Length == 0)
				return Array.Empty<byte>();
			
			var structSize = Marshal.SizeOf<T>();
			int arrayLength = array.Length;
			var byteSize = structSize * arrayLength;
			
			var byteArray = new byte[byteSize];

			var ptr = Marshal.AllocHGlobal(structSize);
			int byteOffset = 0;
			for (int k = 0; k < arrayLength; ++k)
			{
				Marshal.StructureToPtr(array[k], ptr, true);
				Marshal.Copy(ptr, byteArray, byteOffset, structSize);
				byteOffset += structSize;
			}
			Marshal.FreeHGlobal(ptr);
			
			return byteArray;
		}
		
		public static T[] ArrayFromBytes<T>(byte[] byteArray)
			where T : struct
		{
			if (byteArray == null || byteArray.Length == 0)
				return Array.Empty<T>();
			
			var structSize = Marshal.SizeOf<T>();
			var byteSize = byteArray.Length;
			int arrayLength = byteSize / structSize;
			
			Assert.IsTrue(0 == byteSize % structSize);
			
			var array = new T[arrayLength];
			
			var ptr = Marshal.AllocHGlobal(structSize);
			int byteOffset = 0;
			for (int k = 0; k < arrayLength; ++k)
			{
				Marshal.Copy(byteArray, byteOffset, ptr, structSize);
				array[k] = Marshal.PtrToStructure<T>(ptr);
				byteOffset += structSize;
			}
			Marshal.FreeHGlobal(ptr);
			return array;
		}
		
	}
}
