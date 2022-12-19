using System;
using System.IO;
using System.Text;

namespace Framework.Utils
{
	public static class Ascii85Converter
	{
		public static string Encode(byte[] bytes)
		{
			if (bytes == null)
				throw new ArgumentNullException("bytes");

			StringBuilder sb = new StringBuilder(bytes.Length * 5 / 4);

			int count = 0;
			uint value = 0;
			foreach (byte b in bytes)
			{
				value |= ((uint) b) << (24 - (count * 8));
				count++;

				if (count == 4)
				{
					if (value == 0)
						sb.Append('z');
					else
						EncodeValue(sb, value, 0);
					count = 0;
					value = 0;
				}
			}

			if (count > 0)
				EncodeValue(sb, value, 4 - count);

			return sb.ToString();
		}
		
		public static byte[] Decode(string encoded)
		{
			if (encoded == null)
				throw new ArgumentNullException("encoded");

			using (MemoryStream stream = new MemoryStream(encoded.Length * 4 / 5))
			{
				// walk the input string
				int count = 0;
				uint value = 0;
				foreach (char ch in encoded)
				{
					if (ch == 'z' && count == 0)
					{
						DecodeValue(stream, value, 0);
					}
					else if (ch < c_firstCharacter || ch > c_lastCharacter)
					{
						throw new FormatException($"Invalid character '{ch}' in Ascii85 block.");
					}
					else
					{
						try
						{
							checked { value += (uint) (s_powersOf85[count] * (ch - c_firstCharacter)); }
						}
						catch (OverflowException ex)
						{
							throw new FormatException("The current group of characters decodes to a value greater than UInt32.MaxValue.", ex);
						}
					
						count++;

						if (count == 5)
						{
							DecodeValue(stream, value, 0);
							count = 0;
							value = 0;
						}
					}
				}

				if (count == 1)
				{
					throw new FormatException("The final Ascii85 block must contain more than one character.");
				}
				else if (count > 1)
				{
					// decode any remaining characters
					for (int padding = count; padding < 5; padding++)
					{
						try
						{
							checked { value += 84 * s_powersOf85[padding]; }
						}
						catch (OverflowException ex)
						{
							throw new FormatException("The current group of characters decodes to a value greater than UInt32.MaxValue.", ex);
						}
					}
					DecodeValue(stream, value, 5 - count);
				}

				return stream.ToArray();
			}
		}

		private static void EncodeValue(StringBuilder sb, uint value, int paddingBytes)
		{
			char[] encoded = new char[5];

			for (int index = 4; index >= 0; index--)
			{
				encoded[index] = (char) ((value % 85) + c_firstCharacter);
				value /= 85;
			}

			if (paddingBytes != 0)
				Array.Resize(ref encoded, 5 - paddingBytes);

			sb.Append(encoded);
		}

		private static void DecodeValue(Stream stream, uint value, int paddingChars)
		{
			stream.WriteByte((byte) (value >> 24));
			if (paddingChars == 3)
				return;
			stream.WriteByte((byte) ((value >> 16) & 0xFF));
			if (paddingChars == 2)
				return;
			stream.WriteByte(((byte) ((value >> 8) & 0xFF)));
			if (paddingChars == 1)
				return;
			stream.WriteByte((byte) (value & 0xFF));
		}

		// the first and last characters used in the Ascii85 encoding character set
		const char c_firstCharacter = '!';
		const char c_lastCharacter = 'u';

		static readonly uint[] s_powersOf85 = new uint[] { 85u * 85u * 85u * 85u, 85u * 85u * 85u, 85u * 85u, 85u, 1 };
	}
}