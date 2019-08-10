using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PS3ISORebuilder
{
    public class SFOReader
    {
        public enum SFODataType : byte
        {
            Binary = 0,
            String = 2,
            Integer = 4
        }

        public class SFOHeader
        {
            public string Magic;

            public uint PsfVersion;

            public uint LabelOffset;

            public uint DataOffset;

            public uint SectionCount;

            public SFOHeader(byte[] buffer)
            {
                Magic = Encoding.ASCII.GetString(buffer, 1, 3);
                PsfVersion = BitConverter.ToUInt32(buffer, 4);
                LabelOffset = BitConverter.ToUInt32(buffer, 8);
                DataOffset = BitConverter.ToUInt32(buffer, 12);
                SectionCount = BitConverter.ToUInt32(buffer, 16);
            }
        }

        public class SFOSection
        {
            public ushort LabelOffset;

            public byte DataAlignment;

            public SFODataType DataType;

            public uint DataFieldUsed;

            public uint DataFieldSize;

            public uint DataOffset;

            public SFOSection(byte[] buffer)
            {
                LabelOffset = BitConverter.ToUInt16(buffer, 0);
                DataAlignment = buffer[2];
                DataType = (SFODataType)buffer[3];
                DataFieldUsed = BitConverter.ToUInt32(buffer, 4);
                DataFieldSize = BitConverter.ToUInt32(buffer, 8);
                DataOffset = BitConverter.ToUInt32(buffer, 12);
            }
        }

        public class SFOEntry
        {
            public string Label;

            public object Data;

            public SFODataType DataType;
        }

        public Dictionary<string, SFOEntry> Entries;

        public SFOEntry this[string label]
        {
            get
            {
                if (Entries.ContainsKey(label))
                {
                    return Entries[label];
                }
                return null;
            }
        }

        public SFOReader(Stream stream)
        {
            Entries = new Dictionary<string, SFOEntry>();
            BinaryReader reader = new BinaryReader(stream);
            if (!Parse(reader))
            {
                Console.WriteLine("SFOReader could not parse SFO");
            }
        }

        private bool Parse(BinaryReader reader)
        {
            List<SFOSection> list = new List<SFOSection>();
            SFOHeader sFOHeader = new SFOHeader(reader.ReadBytes(20));
            checked
            {
                if (Operators.CompareString(sFOHeader.Magic, "PSF", TextCompare: false) == 0)
                {
                    int num = (int)(unchecked((long)sFOHeader.SectionCount) - 1L);
                    for (int i = 0; i <= num; i++)
                    {
                        SFOSection item = new SFOSection(reader.ReadBytes(16));
                        list.Add(item);
                    }
                    foreach (SFOSection item2 in list)
                    {
                        SFOEntry sFOEntry = new SFOEntry();
                        sFOEntry.DataType = item2.DataType;
                        reader.BaseStream.Seek(sFOHeader.LabelOffset + item2.LabelOffset, SeekOrigin.Begin);
                        StringBuilder stringBuilder = new StringBuilder();
                        int num2 = 0;
                        do
                        {
                            char c = reader.ReadChar();
                            if (c == '\0')
                            {
                                break;
                            }
                            stringBuilder.Append(c);
                            num2++;
                        }
                        while (num2 <= 511);
                        sFOEntry.Label = stringBuilder.ToString();
                        reader.BaseStream.Seek(sFOHeader.DataOffset + item2.DataOffset, SeekOrigin.Begin);
                        switch (sFOEntry.DataType)
                        {
                            case SFODataType.Binary:
                                sFOEntry.Data = reader.ReadBytes((int)item2.DataFieldSize);
                                break;
                            case SFODataType.Integer:
                                sFOEntry.Data = reader.ReadInt32();
                                break;
                            case SFODataType.String:
                                sFOEntry.Data = Encoding.UTF8.GetString(reader.ReadBytes((int)item2.DataFieldSize)).Trim().Replace("\0", "");
                                break;
                        }
                        if (!Entries.ContainsKey(sFOEntry.Label))
                        {
                            Entries.Add(sFOEntry.Label, sFOEntry);
                        }
                    }
                }
                return true;
            }
        }
    }
}
