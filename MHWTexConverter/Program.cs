using System;
using System.IO;
using System.Collections.Generic;

namespace MHWTexConverter
{
	internal class Program
	{
		const int MagicNumberTex = 0x00584554;
		const int MagicNumberDds = 0x20534444;
		const string WMagicNumberDds = "444453207C00000007100A00";
		const string WMagicNumberTex = "5445580010000000000000000000000002000000";
		const string CompressOption = "08104000";
		const string DX10FixedFlags = "03000000000000000100000000000000";
		const string TexFixedUnk = "01000000000000000000000000000000FFFFFFFF0000000000000000";

		public enum MHW_TEX_FORMAT
		{
			DXGI_FORMAT_UNKNOWN				= 0,
			DXGI_FORMAT_R8G8B8A8_UNORM		= 7,
			DXGI_FORMAT_R8G8B8A8_UNORM_SRGB	= 9, // LUTs
			DXGI_FORMAT_R8G8_UNORM			= 19,
			DXGI_FORMAT_BC1_UNORM			= 22,
			DXGI_FORMAT_BC1_UNORM_SRGB		= 23,
			DXGI_FORMAT_BC4_UNORM			= 24,
			DXGI_FORMAT_BC5_UNORM			= 26,
			DXGI_FORMAT_BC6H_UF16			= 28,
			DXGI_FORMAT_BC7_UNORM			= 30,
			DXGI_FORMAT_BC7_UNORM_SRGB		= 31
		}

		// Value References
		// https://learn.microsoft.com/en-us/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format
		// https://microsoft.github.io/DirectX-Specs/d3d/SamplerFeedback.html#api
		public enum DXGI_FORMAT
		{
			DXGI_FORMAT_UNKNOWN									= 0,
			DXGI_FORMAT_R32G32B32A32_TYPELESS					= 1,
			DXGI_FORMAT_R32G32B32A32_FLOAT						= 2,
			DXGI_FORMAT_R32G32B32A32_UINT						= 3,
			DXGI_FORMAT_R32G32B32A32_SINT						= 4,
			DXGI_FORMAT_R32G32B32_TYPELESS						= 5,
			DXGI_FORMAT_R32G32B32_FLOAT							= 6,
			DXGI_FORMAT_R32G32B32_UINT							= 7,
			DXGI_FORMAT_R32G32B32_SINT							= 8,
			DXGI_FORMAT_R16G16B16A16_TYPELESS					= 9,
			DXGI_FORMAT_R16G16B16A16_FLOAT						= 10,
			DXGI_FORMAT_R16G16B16A16_UNORM						= 11,
			DXGI_FORMAT_R16G16B16A16_UINT						= 12,
			DXGI_FORMAT_R16G16B16A16_SNORM						= 13,
			DXGI_FORMAT_R16G16B16A16_SINT						= 14,
			DXGI_FORMAT_R32G32_TYPELESS							= 15,
			DXGI_FORMAT_R32G32_FLOAT							= 16,
			DXGI_FORMAT_R32G32_UINT								= 17,
			DXGI_FORMAT_R32G32_SINT								= 18,
			DXGI_FORMAT_R32G8X24_TYPELESS						= 19,
			DXGI_FORMAT_D32_FLOAT_S8X24_UINT					= 20,
			DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS				= 21,
			DXGI_FORMAT_X32_TYPELESS_G8X24_UINT					= 22,
			DXGI_FORMAT_R10G10B10A2_TYPELESS					= 23,
			DXGI_FORMAT_R10G10B10A2_UNORM						= 24,
			DXGI_FORMAT_R10G10B10A2_UINT						= 25,
			DXGI_FORMAT_R11G11B10_FLOAT							= 26,
			DXGI_FORMAT_R8G8B8A8_TYPELESS						= 27,
			DXGI_FORMAT_R8G8B8A8_UNORM							= 28,
			DXGI_FORMAT_R8G8B8A8_UNORM_SRGB						= 29,
			DXGI_FORMAT_R8G8B8A8_UINT							= 30,
			DXGI_FORMAT_R8G8B8A8_SNORM							= 31,
			DXGI_FORMAT_R8G8B8A8_SINT							= 32,
			DXGI_FORMAT_R16G16_TYPELESS							= 33,
			DXGI_FORMAT_R16G16_FLOAT							= 34,
			DXGI_FORMAT_R16G16_UNORM							= 35,
			DXGI_FORMAT_R16G16_UINT								= 36,
			DXGI_FORMAT_R16G16_SNORM							= 37,
			DXGI_FORMAT_R16G16_SINT								= 38,
			DXGI_FORMAT_R32_TYPELESS							= 39,
			DXGI_FORMAT_D32_FLOAT								= 40,
			DXGI_FORMAT_R32_FLOAT								= 41,
			DXGI_FORMAT_R32_UINT								= 42,
			DXGI_FORMAT_R32_SINT								= 43,
			DXGI_FORMAT_R24G8_TYPELESS							= 44,
			DXGI_FORMAT_D24_UNORM_S8_UINT						= 45,
			DXGI_FORMAT_R24_UNORM_X8_TYPELESS					= 46,
			DXGI_FORMAT_X24_TYPELESS_G8_UINT					= 47,
			DXGI_FORMAT_R8G8_TYPELESS							= 48,
			DXGI_FORMAT_R8G8_UNORM								= 49,
			DXGI_FORMAT_R8G8_UINT								= 50,
			DXGI_FORMAT_R8G8_SNORM								= 51,
			DXGI_FORMAT_R8G8_SINT								= 52,
			DXGI_FORMAT_R16_TYPELESS							= 53,
			DXGI_FORMAT_R16_FLOAT								= 54,
			DXGI_FORMAT_D16_UNORM								= 55,
			DXGI_FORMAT_R16_UNORM								= 56,
			DXGI_FORMAT_R16_UINT								= 57,
			DXGI_FORMAT_R16_SNORM								= 58,
			DXGI_FORMAT_R16_SINT								= 59,
			DXGI_FORMAT_R8_TYPELESS								= 60,
			DXGI_FORMAT_R8_UNORM								= 61,
			DXGI_FORMAT_R8_UINT									= 62,
			DXGI_FORMAT_R8_SNORM								= 63,
			DXGI_FORMAT_R8_SINT									= 64,
			DXGI_FORMAT_A8_UNORM								= 65,
			DXGI_FORMAT_R1_UNORM								= 66,
			DXGI_FORMAT_R9G9B9E5_SHAREDEXP						= 67,
			DXGI_FORMAT_R8G8_B8G8_UNORM							= 68,
			DXGI_FORMAT_G8R8_G8B8_UNORM							= 69,
			DXGI_FORMAT_BC1_TYPELESS							= 70,
			DXGI_FORMAT_BC1_UNORM								= 71,
			DXGI_FORMAT_BC1_UNORM_SRGB							= 72,
			DXGI_FORMAT_BC2_TYPELESS							= 73,
			DXGI_FORMAT_BC2_UNORM								= 74,
			DXGI_FORMAT_BC2_UNORM_SRGB							= 75,
			DXGI_FORMAT_BC3_TYPELESS							= 76,
			DXGI_FORMAT_BC3_UNORM								= 77,
			DXGI_FORMAT_BC3_UNORM_SRGB							= 78,
			DXGI_FORMAT_BC4_TYPELESS							= 79,
			DXGI_FORMAT_BC4_UNORM								= 80,
			DXGI_FORMAT_BC4_SNORM								= 81,
			DXGI_FORMAT_BC5_TYPELESS							= 82,
			DXGI_FORMAT_BC5_UNORM								= 83,
			DXGI_FORMAT_BC5_SNORM								= 84,
			DXGI_FORMAT_B5G6R5_UNORM							= 85,
			DXGI_FORMAT_B5G5R5A1_UNORM							= 86,
			DXGI_FORMAT_B8G8R8A8_UNORM							= 87,
			DXGI_FORMAT_B8G8R8X8_UNORM							= 88,
			DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM				= 89,
			DXGI_FORMAT_B8G8R8A8_TYPELESS						= 90,
			DXGI_FORMAT_B8G8R8A8_UNORM_SRGB						= 91,
			DXGI_FORMAT_B8G8R8X8_TYPELESS						= 92,
			DXGI_FORMAT_B8G8R8X8_UNORM_SRGB						= 93,
			DXGI_FORMAT_BC6H_TYPELESS							= 94,
			DXGI_FORMAT_BC6H_UF16								= 95,
			DXGI_FORMAT_BC6H_SF16								= 96,
			DXGI_FORMAT_BC7_TYPELESS							= 97,
			DXGI_FORMAT_BC7_UNORM								= 98,
			DXGI_FORMAT_BC7_UNORM_SRGB							= 99,
			DXGI_FORMAT_AYUV									= 100,
			DXGI_FORMAT_Y410									= 101,
			DXGI_FORMAT_Y416									= 102,
			DXGI_FORMAT_NV12									= 103,
			DXGI_FORMAT_P010									= 104,
			DXGI_FORMAT_P016									= 105,
			DXGI_FORMAT_420_OPAQUE								= 106,
			DXGI_FORMAT_YUY2									= 107,
			DXGI_FORMAT_Y210									= 108,
			DXGI_FORMAT_Y216									= 109,
			DXGI_FORMAT_NV11									= 110,
			DXGI_FORMAT_AI44									= 111,
			DXGI_FORMAT_IA44									= 112,
			DXGI_FORMAT_P8										= 113,
			DXGI_FORMAT_A8P8									= 114,
			DXGI_FORMAT_B4G4R4A4_UNORM							= 115,
			DXGI_FORMAT_P208									= 130,
			DXGI_FORMAT_V208									= 131,
			DXGI_FORMAT_V408									= 132,
			DXGI_FORMAT_SAMPLER_FEEDBACK_MIN_MIP_OPAQUE			= 189,
			DXGI_FORMAT_SAMPLER_FEEDBACK_MIP_REGION_USED_OPAQUE	= 190,
			DXGI_FORMAT_FORCE_UINT								= -1 // 0xffffffff
		}

		public readonly static Dictionary<MHW_TEX_FORMAT, string> FormatTagMap = new Dictionary<MHW_TEX_FORMAT, string>()
		{
			{ MHW_TEX_FORMAT.DXGI_FORMAT_UNKNOWN, "UNKN_" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, "R8G8B8A8_" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB, "SR8G8B8A8_" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_R8G8_UNORM, "R8G8_" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_BC1_UNORM,"DXT1L_" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB,"BC1S_" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_BC4_UNORM, "BC4_" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_BC5_UNORM, "BC5_" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_BC6H_UF16, "BC6_" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_BC7_UNORM, "BC7L_" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB, "BC7S_" }
		};

		public readonly static Dictionary<MHW_TEX_FORMAT, string> FormatMagicMap = new Dictionary<MHW_TEX_FORMAT, string>()
		{
			{ MHW_TEX_FORMAT.DXGI_FORMAT_UNKNOWN, "UNKN" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, "DX10" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB, "DX10" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_R8G8_UNORM, "DX10" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_BC1_UNORM,"DXT1" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB,"DX10" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_BC4_UNORM, "BC4U" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_BC5_UNORM, "BC5U" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_BC6H_UF16, "DX10" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_BC7_UNORM, "DX10" },
			{ MHW_TEX_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB, "DX10" }
		};

		public readonly static List<MHW_TEX_FORMAT> TexWith4Bpp = new List<MHW_TEX_FORMAT> { MHW_TEX_FORMAT.DXGI_FORMAT_BC1_UNORM, MHW_TEX_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB, MHW_TEX_FORMAT.DXGI_FORMAT_BC4_UNORM };
		public readonly static List<MHW_TEX_FORMAT> TexWith16Bpp = new List<MHW_TEX_FORMAT> { MHW_TEX_FORMAT.DXGI_FORMAT_R8G8_UNORM };
		public readonly static List<MHW_TEX_FORMAT> TexOfNewDDS = new List<MHW_TEX_FORMAT> { MHW_TEX_FORMAT.DXGI_FORMAT_BC7_UNORM, MHW_TEX_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB, MHW_TEX_FORMAT.DXGI_FORMAT_BC6H_UF16 };

		public static int Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.Error.WriteLine("Usage: <Program_path> <tex_source_path>");
				Console.ReadLine();
				return -1;
			}

			foreach (string arg in args)
			{
				FileInfo fi = new FileInfo(arg);
				if (!fi.Exists)
				{
					Console.Error.WriteLine($"ERROR: {arg} not found");
					continue;
				}
				if (fi.Length < 0xC0)
				{
					Console.Error.WriteLine($"ERROR: {arg} is too small");
					continue;
				}

				string fileExt = Path.GetExtension(arg).ToUpper();
				if (fileExt == ".TEX")
				{
					using (BinaryReader reader = new BinaryReader(File.OpenRead(arg)))
					{
						int magicNumber = reader.ReadInt32();
						if (magicNumber != MagicNumberTex)
						{
							Console.Error.WriteLine($"ERROR: {arg} is not a valid tex file.");
							continue;
						}

						reader.BaseStream.Position = 0x14;
						int mipMapCount = reader.ReadInt32();
						int width = reader.ReadInt32();
						int height = reader.ReadInt32();
						reader.BaseStream.Position = 0x24;

						int type = reader.ReadInt32();

						reader.BaseStream.Position = 0xB8;

						long offset = reader.ReadInt64();
						int size;

						if (mipMapCount > 1)
							size = (int)(reader.ReadInt64() - offset);
						else
							size = (int)(fi.Length - offset);

						reader.BaseStream.Position = offset;

						string typeMagic = "";
						DXGI_FORMAT ddsFormat = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN;
						MHW_TEX_FORMAT texFormat = MHW_TEX_FORMAT.DXGI_FORMAT_UNKNOWN;
						Array knownFormats = Enum.GetValues(typeof(MHW_TEX_FORMAT));
						foreach (int known in knownFormats)
						{
							if (known == type)
							{
								ddsFormat = (DXGI_FORMAT)Enum.Parse(typeof(DXGI_FORMAT), Enum.GetName(typeof(MHW_TEX_FORMAT), type));
								texFormat = (MHW_TEX_FORMAT)type;
								typeMagic = FormatMagicMap[texFormat];
								break;
							}
						}

						if (texFormat == MHW_TEX_FORMAT.DXGI_FORMAT_UNKNOWN)
						{
							Console.Error.WriteLine($"ERROR: Unknown TEX format {type}. - {arg}");
							Console.ReadLine();
							continue;
						}

						string outfile_name = FormatTagMap[texFormat] + Path.GetFileName(arg);
						
						string destPath = Path.GetFullPath(Path.ChangeExtension(Path.GetDirectoryName(arg) + "\\" + outfile_name, ".dds"));
						Directory.CreateDirectory(Path.GetDirectoryName(destPath));
						byte[] data;
						data = reader.ReadBytes((int)new FileInfo(arg).Length);

						WriteDDS(destPath, height, width, texFormat, mipMapCount, typeMagic, ddsFormat, data);
					}
				}
				else if (fileExt == ".DDS")
				{
					string destPath = Path.GetFullPath(Path.ChangeExtension(arg, ".tex"));
					Directory.CreateDirectory(Path.GetDirectoryName(destPath));
					FileInfo destFile = new FileInfo(destPath);
					if (destFile.Exists)
					{
						if (File.Exists(destPath + ".old"))
							File.Delete(destPath + ".old");

						Directory.Move(destPath, destPath + ".old");
					}

					using (BinaryReader reader = new BinaryReader(File.OpenRead(arg)))
					{
						int magicNumber = reader.ReadInt32();
						if (magicNumber != MagicNumberDds)
						{
							Console.Error.WriteLine($"ERROR: {magicNumber} is not a valid DDS file.");
							continue;
						}

						// Header Flags
						reader.BaseStream.Position = 0x8;
						int ddsFlag = reader.ReadInt32();
						bool isRaw = false;
						if ((ddsFlag & 0x8) == 0x8)
							isRaw = true;

						// Height/Width
						int height = reader.ReadInt32();
						int width = reader.ReadInt32();

						// Mipmap Count
						reader.BaseStream.Position = 0x1C;
						int mipMapCount = reader.ReadInt32();

						// DDS Pixel Format
						reader.BaseStream.Position = 0x54;
						int fileTypeCode = reader.ReadInt32();
						reader.BaseStream.Position = 0x80;
						int ddsFormatInt = reader.ReadInt32();

						MHW_TEX_FORMAT texFormat = MHW_TEX_FORMAT.DXGI_FORMAT_UNKNOWN;
						switch (fileTypeCode)
						{
							case 0x30315844: // DX10
								{
									string formatName = Enum.GetName(typeof(DXGI_FORMAT), ddsFormatInt);
									foreach (string name in Enum.GetNames(typeof(MHW_TEX_FORMAT)))
									{
										if (formatName.Equals(name))
											texFormat = (MHW_TEX_FORMAT)Enum.Parse(typeof(MHW_TEX_FORMAT), formatName);
									}
									texFormat = (MHW_TEX_FORMAT)Enum.Parse(typeof(MHW_TEX_FORMAT), Enum.GetName(typeof(DXGI_FORMAT), ddsFormatInt));
									break;
								}
							case 0x31545844: // DXT1
								{
									texFormat = MHW_TEX_FORMAT.DXGI_FORMAT_BC1_UNORM;
									break;
								}
							case 0x55344342: // BC4U
								{
									texFormat = MHW_TEX_FORMAT.DXGI_FORMAT_BC4_UNORM;
									break;
								}
							case 0x55354342: // BC5U
							case 0x32495441: // ATI2
								{
									texFormat = MHW_TEX_FORMAT.DXGI_FORMAT_BC5_UNORM;
									break;
								}
							case 0x0: // Raw
								{
									if (isRaw)
										texFormat = MHW_TEX_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
									break;
								}
						}

						// Image Data
						reader.BaseStream.Position = (FormatMagicMap[texFormat].Equals("DX10") && !isRaw) ? 0x94 : 0x80;
						byte[] data = reader.ReadBytes((int)new FileInfo(arg).Length);

						if (texFormat == MHW_TEX_FORMAT.DXGI_FORMAT_UNKNOWN)
						{
							Console.Error.WriteLine($"ERROR: Unsupported DDS format {fileTypeCode}. - {arg}");
							Console.ReadLine();
							continue;
						}

						WriteTEX(destPath, height, width, texFormat, mipMapCount, isRaw, data);
					}
				}

				Console.WriteLine($"{Path.GetFileName(arg)} finished!");
			}

			return 0;
		}

		public static void WriteDDS(string filePath, int h, int w, MHW_TEX_FORMAT texFormat, int mipCount, string pxFormatMagic, DXGI_FORMAT ddsFormat, byte[] imgData)
		{
			using (FileStream fsWrite = new FileStream(filePath, FileMode.Create))
			{
				// DDS Header
				byte[] WMagicNumberHead = Utils.StringToByteArray(WMagicNumberDds);
				fsWrite.Write(WMagicNumberHead, 0, WMagicNumberHead.Length);

				// Texture Height/Width
				fsWrite.Write(Utils.intToBytesLittle(h), 0, 4);
				fsWrite.Write(Utils.intToBytesLittle(w), 0, 4);

				// Bits Per Pixel
				if (TexWith4Bpp.Contains(texFormat)) // 4bpp Texture Size
					fsWrite.Write(Utils.intToBytesLittle(w * h / 2), 0, 4);
				else if (TexWith16Bpp.Contains(texFormat)) // 16bpp Texture Size
					fsWrite.Write(Utils.intToBytesLittle(w * h * 2), 0, 4);
				else // 8bpp Texture Size
					fsWrite.Write(Utils.intToBytesLittle(w * h), 0, 4);

				// Depth
				fsWrite.Write(Utils.intToBytesLittle(1), 0, 4);

				// Mipmap Count
				fsWrite.Write(Utils.intToBytesLittle(mipCount), 0, 4);

				// Reserved Bytes x11
				byte[] EmptyByte11 = Utils.intToBytesLittle(0, 11);
				fsWrite.Write(EmptyByte11, 0, EmptyByte11.Length);

				// Pixel Format
				fsWrite.Write(Utils.intToBytesLittle(32), 0, 4);
				fsWrite.Write(Utils.intToBytesLittle(4), 0, 4);
				byte[] typeMagicBytes = Utils.AsciiStringToByteArray(pxFormatMagic);
				fsWrite.Write(typeMagicBytes, 0, typeMagicBytes.Length);
				byte[] EmptyByte5 = Utils.intToBytesLittle(0, 5);
				fsWrite.Write(EmptyByte5, 0, EmptyByte5.Length);

				// Surface Flags
				byte[] CompressOptionByte = Utils.StringToByteArray(CompressOption);
				fsWrite.Write(CompressOptionByte, 0, CompressOptionByte.Length);

				// Reserved Bytes x4
				byte[] EmptyByte4 = Utils.intToBytesLittle(0, 4);
				fsWrite.Write(EmptyByte4, 0, EmptyByte4.Length);

				// DirectX 10 Extended Header
				if (pxFormatMagic.Equals("DX10"))
				{
					fsWrite.Write(Utils.intToBytesLittle((int)ddsFormat), 0, 4);
					byte[] ArbNumByte = Utils.StringToByteArray(DX10FixedFlags);
					fsWrite.Write(ArbNumByte, 0, ArbNumByte.Length);
				}

				// Image Data
				fsWrite.Write(imgData, 0, imgData.Length);
			}
		}

		public static void WriteTEX(string filePath, int h, int w, MHW_TEX_FORMAT texFormat, int mipCount, bool isRaw, byte[] imgData)
		{
			using (FileStream fsWrite = new FileStream(filePath, FileMode.Create))
			{
				// TEX Header
				byte[] WMagicNumberHead = Utils.StringToByteArray(WMagicNumberTex);
				fsWrite.Write(WMagicNumberHead, 0, WMagicNumberHead.Length);

				// Mipmap Count
				fsWrite.Write(Utils.intToBytesLittle(mipCount), 0, 4);

				// Texture Width/Height
				fsWrite.Write(Utils.intToBytesLittle(w), 0, 4);
				fsWrite.Write(Utils.intToBytesLittle(h), 0, 4);

				// Texture Format
				fsWrite.Write(Utils.intToBytesLittle(1), 0, 4);
				fsWrite.Write(Utils.intToBytesLittle((int)texFormat), 0, 4);

				// TEX Unknown Value
				byte[] WTexSolid = Utils.StringToByteArray(TexFixedUnk);
				fsWrite.Write(WTexSolid, 0, WTexSolid.Length);

				// Texture Format
				if (TexOfNewDDS.Contains(texFormat))
					fsWrite.Write(Utils.intToBytesLittle(1), 0, 4);
				else
					fsWrite.Write(Utils.intToBytesLittle(0), 0, 4);

				// Reserved Bytes?
				fsWrite.Write(Utils.intToBytesLittle(0, 4), 0, 4 * 4);
				fsWrite.Write(Utils.intToBytesLittle(-1, 8), 0, 4 * 8);

				// Full Width/Half Width
				fsWrite.Write(Utils.intToBytesLittle(w), 0, 4);
				bool isFullWidth = isRaw || texFormat == MHW_TEX_FORMAT.DXGI_FORMAT_R8G8_UNORM;
				if (isFullWidth)
					fsWrite.Write(Utils.intToBytesLittle(w), 0, 2);
				else
					fsWrite.Write(Utils.intToBytesLittle(w / 2), 0, 2);
				fsWrite.Write(Utils.intToBytesLittle(w), 0, 2);
				fsWrite.Write(Utils.intToBytesLittle(0, 2), 0, 4 * 2);
				if (isFullWidth)
					fsWrite.Write(Utils.intToBytesLittle(w), 0, 2);
				else
					fsWrite.Write(Utils.intToBytesLittle(w / 2), 0, 2);
				fsWrite.Write(Utils.intToBytesLittle(w), 0, 2);
				fsWrite.Write(Utils.intToBytesLittle(0, 2), 0, 4 * 2);
				if (isFullWidth)
					fsWrite.Write(Utils.intToBytesLittle(w), 0, 2);
				else
					fsWrite.Write(Utils.intToBytesLittle(w / 2), 0, 2);
				fsWrite.Write(Utils.intToBytesLittle(w), 0, 2);
				fsWrite.Write(Utils.intToBytesLittle(0, 8), 0, 4 * 8);
				int cur_width = w;
				int cur_height = h;
				int base_loc = 0xb8 + mipCount * 8;
				for (int i = 0; i < mipCount; i++)
				{
					fsWrite.Write(Utils.intToBytesLittle(base_loc), 0, 4);
					fsWrite.Write(Utils.intToBytesLittle(0), 0, 4);
					int maxWidth = isRaw ? 2 : 4;

					if (TexWith4Bpp.Contains(texFormat))
						base_loc += cur_width * cur_height / 2;
					else if (TexWith16Bpp.Contains(texFormat))
						base_loc += cur_width * cur_height * 2;
					else if (isRaw)
						base_loc += cur_width * cur_height * 4;
					else
						base_loc += cur_width * cur_height;

					cur_width /= 2;
					cur_height /= 2;

					cur_width = cur_width > maxWidth ? cur_width : maxWidth;
					cur_height = cur_height > maxWidth ? cur_height : maxWidth;
				}

				// Image Data
				fsWrite.Write(imgData, 0, imgData.Length);
			}
		}
	}
}
