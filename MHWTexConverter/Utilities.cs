using System;
using System.Linq;

namespace MHWTexConverter
{
	internal class Utils
	{
		public static byte[] StringToByteArray(string hex)
		{
			return Enumerable.Range(0, hex.Length)
							 .Where(x => x % 2 == 0)
							 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
							 .ToArray();
		}

		public static byte[] AsciiStringToByteArray(string origin)
		{
			byte[] ret = new byte[origin.Length];
			for (int i = 0; i < origin.Length; i++)
			{
				ret[i] = Convert.ToByte(origin[i]);
			}

			return ret;
		}

		public static byte[] intToBytesLittle(int value, int repeat = 1)
		{
			byte[] src = new byte[4];
			src[0] = (byte)(value & 0xFF);
			src[1] = (byte)((value >> 8) & 0xFF);
			src[2] = (byte)((value >> 16) & 0xFF);
			src[3] = (byte)((value >> 24) & 0xFF);

			if (repeat > 1)
			{
				byte[] repeatsrc = new byte[4 * repeat];
				for (int i = 0; i < repeat; i++)
				{
					repeatsrc[i * 4] = src[0];
					repeatsrc[i * 4 + 1] = src[1];
					repeatsrc[i * 4 + 2] = src[2];
					repeatsrc[i * 4 + 3] = src[3];
				}

				return repeatsrc;
			}

			return src;
		}

		/*public static byte[] intToBytesBig(int value)
		{
			byte[] src = new byte[4];
			src[0] = (byte)((value >> 24) & 0xFF);
			src[1] = (byte)((value >> 16) & 0xFF);
			src[2] = (byte)((value >> 8) & 0xFF);
			src[3] = (byte)(value & 0xFF);

			return src;
		}*/
	}
}
