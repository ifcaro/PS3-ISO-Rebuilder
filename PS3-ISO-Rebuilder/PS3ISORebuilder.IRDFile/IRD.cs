using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PS3ISORebuilder.IRDFile
{
    public class IRD
    {
        public struct Region
        {
            public uint RegionIdx;

            public uint Start;

            public uint Length;

            public byte[] SourceHash;

            public bool isPlain;

            public uint End
            {
                get
                {
                    checked
                    {
                        return (uint)(unchecked((long)checked(Start + Length)) - 1L);
                    }
                }
            }

            public string md5 => BitConverter.ToString(SourceHash).Replace("-", "").ToLower();

            public string startHEX => Conversion.Hex(Start);

            public string endHEX => Conversion.Hex(End);

            public Region(int region, uint s, uint l, bool plain, byte[] hash)
            {
                this = default(Region);
                checked
                {
                    RegionIdx = (uint)region;
                    Start = s;
                    Length = (uint)(unchecked((long)checked(l - s)) + 1L);
                    isPlain = plain;
                    SourceHash = hash;
                }
            }
        }

        private FileStream fileStream;

        private const int IrdVersion = 9;

        public int[] CompatibleVersions;

        public byte[] MAGIC;

        public int version;

        public BinaryReader reader;

        public string GAMEID;

        public string GAMENAME;

        public string UpdateVersion;

        public string GameVersion;

        public string AppVersion;

        public byte[] PIC;

        public byte[] Data1;

        public byte[] Data2;

        public Stream header;

        public Stream footer;

        public Dictionary<int, Region> Regions;

        public List<byte[]> RegionHashes;

        public Dictionary<long, byte[]> FileHashes;

        public uint UniqueIdentifier;

        public int crc;

        public bool valid;

        public ulong disksize;

        public ISO isoheader;

        public void Close()
        {
            fileStream.Close();
        }

        public IRD(string path)
        {
            CompatibleVersions = new int[4]
            {
                6,
                7,
                8,
                9
            };
            MAGIC = new byte[4]
            {
                51,
                73,
                82,
                68
            };
            UpdateVersion = "0000";
            GameVersion = "00000";
            AppVersion = "00000";
            Regions = new Dictionary<int, Region>();
            RegionHashes = new List<byte[]>();
            valid = false;
            if (!Parse(path))
            {
                Console.WriteLine("could not parse IRD");
            }
        }

        public BinaryReader Open(string path)
        {
            fileStream = File.OpenRead(path);
            byte[] array = new byte[4];
            fileStream.Read(array, 0, 4);
            fileStream.Seek(0L, SeekOrigin.Begin);
            Stream stream;
            if (Operators.CompareString(AsString(array), AsString(MAGIC), TextCompare: false) == 0)
            {
                fileStream.Seek(0L, SeekOrigin.Begin);
                stream = fileStream;
            }
            else
            {
                GZipStream gZipStream = new GZipStream(fileStream, CompressionMode.Decompress);
                stream = gZipStream;
            }
            byte[] buffer;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                buffer = memoryStream.ToArray();
            }
            MemoryStream input = new MemoryStream(buffer);
            return new BinaryReader(input);
        }

        public static string AsString(byte[] val)
        {
            return BitConverter.ToString(val).Replace("-", "");
        }

        public uint Swap(uint value)
        {
            checked
            {
                return (uint)unchecked(((uint)((int)value & -16777216) >> 24) | (((long)value & 16711680L) >> 8) | (((long)value & 65280L) << 8) | (((long)value & 255L) << 24));
            }
        }

        public bool Parse(string path)
        {
            reader = Open(path);
            byte[] val = reader.ReadBytes(4);
            if (Operators.CompareString(AsString(val).ToString(), AsString(MAGIC).ToString(), TextCompare: false) != 0)
            {
                return false;
            }
            version = reader.ReadByte();
            if (version != 9 && !CompatibleVersions.Any((int v) => v == 9))
            {
                return false;
            }
            GAMEID = Encoding.ASCII.GetString(reader.ReadBytes(9)).Trim();
            GAMENAME = reader.ReadString();
            UpdateVersion = Encoding.ASCII.GetString(reader.ReadBytes(4)).Trim();
            GameVersion = Encoding.ASCII.GetString(reader.ReadBytes(5)).Trim();
            AppVersion = Encoding.ASCII.GetString(reader.ReadBytes(5)).Trim();
            if (version == 7)
            {
                reader.ReadBytes(4);
            }
            header = Uncompress(reader);
            footer = Uncompress(reader);
            int num = reader.ReadByte();
            checked
            {
                int num2 = num - 1;
                for (int i = 0; i <= num2; i++)
                {
                    byte[] item = reader.ReadBytes(16);
                    RegionHashes.Add(item);
                }
                int num3 = reader.ReadInt32();
                FileHashes = new Dictionary<long, byte[]>(num3);
                int num4 = num3 - 1;
                for (int j = 0; j <= num4; j++)
                {
                    long key = reader.ReadInt64();
                    byte[] value = reader.ReadBytes(16);
                    FileHashes.Add(key, value);
                }
                reader.ReadUInt16();
                reader.ReadUInt16();
                PIC = new byte[115];
                if (version >= 9)
                {
                    PIC = reader.ReadBytes(115);
                }
                Data1 = reader.ReadBytes(16);
                Data2 = reader.ReadBytes(16);
                if (version < 9)
                {
                    PIC = reader.ReadBytes(115);
                }
                if (version > 7)
                {
                    UniqueIdentifier = reader.ReadUInt32();
                }
                long position = reader.BaseStream.Position;
                this.crc = reader.ReadInt32();
                reader.BaseStream.Position = 0L;
                CRC32 cRC = new CRC32();
                Stream stream = reader.BaseStream;
                int crc = cRC.GetCrc32(ref stream, position);
                if (this.crc == crc)
                {
                    valid = true;
                }
                else
                {
                    valid = false;
                }
                BinaryReader binaryReader = new BinaryReader(header, Encoding.ASCII);
                binaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
                uint num5 = (uint)(unchecked((long)Swap(binaryReader.ReadUInt32())) * 2L - 1);
                binaryReader.BaseStream.Seek(4L, SeekOrigin.Current);
                uint s = Swap(binaryReader.ReadUInt32());
                bool flag = true;
                for (uint num6 = 0u; num6 < num5; num6 = (uint)(unchecked((long)num6) + 1L))
                {
                    uint num7 = Swap(binaryReader.ReadUInt32());
                    if (flag)
                    {
                        Dictionary<int, Region> regions = Regions;
                        int key2 = (int)num6;
                        Region value2 = new Region((int)num6, s, num7, flag, RegionHashes[(int)num6]);
                        regions.Add(key2, value2);
                    }
                    else
                    {
                        Dictionary<int, Region> regions2 = Regions;
                        int key3 = (int)num6;
                        Region value2 = new Region((int)num6, s, num7, flag, RegionHashes[(int)num6]);
                        regions2.Add(key3, value2);
                    }
                    flag = !flag;
                    s = num7;
                }
                binaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
                try
                {
                    isoheader = new ISO(header, FileHashes);
                    disksize = (ulong)(unchecked((long)isoheader.VolumeDescriptor.VolumeSpaceSize) * isoheader.Blocksize);
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Exception ex2 = ex;
                    bool result = false;
                    ProjectData.ClearProjectError();
                    return result;
                }
                return true;
            }
        }

        public Stream Uncompress(BinaryReader input)
        {
            checked
            {
                byte[] array = new byte[(int)(unchecked((long)input.ReadUInt32()) - 1L) + 1];
                input.Read(array, 0, array.Length);
                MemoryStream memoryStream = new MemoryStream();
                GZipStream gZipStream = new GZipStream(new MemoryStream(array), CompressionMode.Decompress);
                gZipStream.CopyTo(memoryStream);
                gZipStream.Close();
                memoryStream.Position = 0L;
                return memoryStream;
            }
        }

        [CompilerGenerated]
        private static bool _Lambda_0024__5(int v)
        {
            return v == 9;
        }
    }
}
