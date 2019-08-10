using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Text;

namespace PS3ISORebuilder.IRDFile
{
    public class VolumeDescriptor : BaseVolumeDescriptor
    {
        public byte VolumeFlags;

        public string SystemIdentifier;

        public string VolumeIdentifier;

        public uint VolumeSpaceSize;

        public ushort VolumeSetSize;

        public ushort VolumeSequenceNumber;

        public ushort LogicalBlockSize;

        public uint PathTableSize;

        public uint TypeLPathTable;

        public uint OptTypeLPathTable;

        public uint TypeMPathTable;

        public uint OptTypeMPathTable;

        public DirectoryRecord DirectoryRecord;

        public string VolumeSetIdentifier;

        public string PublisherIdentifier;

        public string DataPreparerIdentifier;

        public string ApplicationIdentifier;

        public string CopyrightFileIdentifier;

        public string AbstractFileIdentifier;

        public string BibliographicFileIdentifier;

        public DateTime VolumeCreationDateandTime;

        public DateTime VolumeModificationDateandTime;

        public DateTime VolumeExpirationDateandTime;

        public DateTime VolumeEffectiveDateandTime;

        public byte FileStructureVersion;

        public VolumeDescriptor(byte[] b)
            : base(b)
        {
            VolumeFlags = b[7];
            SystemIdentifier = Encoding.ASCII.GetString(b, 8, 32).Trim();
            VolumeIdentifier = getencoding.GetString(b, 40, 32).Trim();
            VolumeSpaceSize = BitConverter.ToUInt32(b, 80);
            VolumeSetSize = BitConverter.ToUInt16(b, 120);
            VolumeSequenceNumber = BitConverter.ToUInt16(b, 124);
            LogicalBlockSize = BitConverter.ToUInt16(b, 128);
            PathTableSize = BitConverter.ToUInt32(b, 132);
            TypeLPathTable = BitConverter.ToUInt32(b, 140);
            OptTypeLPathTable = BitConverter.ToUInt32(b, 144);
            TypeMPathTable = Swap(BitConverter.ToUInt32(b, 148));
            OptTypeMPathTable = Swap(BitConverter.ToUInt32(b, 152));
            DirectoryRecord = new DirectoryRecord(ToByteArray(b, 156, 34), 0, getencoding);
            VolumeSetIdentifier = getencoding.GetString(b, 190, 128).Trim();
            PublisherIdentifier = Encoding.ASCII.GetString(b, 318, 128).Trim();
            DataPreparerIdentifier = Encoding.ASCII.GetString(b, 446, 128).Trim();
            ApplicationIdentifier = Encoding.ASCII.GetString(b, 574, 128).Trim();
            CopyrightFileIdentifier = getencoding.GetString(b, 702, 36).Trim();
            AbstractFileIdentifier = getencoding.GetString(b, 739, 36).Trim();
            BibliographicFileIdentifier = getencoding.GetString(b, 776, 36).Trim();
            VolumeCreationDateandTime = getDate(ToByteArray(b, 813, 16));
            VolumeModificationDateandTime = getDate(ToByteArray(b, 830, 16));
            VolumeExpirationDateandTime = getDate(ToByteArray(b, 847, 16));
            VolumeEffectiveDateandTime = getDate(ToByteArray(b, 864, 16));
            FileStructureVersion = b[881];
        }

        private DateTime getDate(byte[] data)
        {
            checked
            {
                try
                {
                    string @string = Encoding.ASCII.GetString(data, 0, 16);
                    @string = @string.Replace('\0', '0');
                    int year = Conversions.ToInteger(@string.Substring(0, 4));
                    int month = Conversions.ToInteger(@string.Substring(4, 2));
                    int day = Conversions.ToInteger(@string.Substring(6, 2));
                    int hour = Conversions.ToInteger(@string.Substring(8, 2));
                    int minute = Conversions.ToInteger(@string.Substring(10, 2));
                    int second = Conversions.ToInteger(@string.Substring(12, 2));
                    int num = Conversions.ToInteger(@string.Substring(14, 2));
                    return new DateTime(year, month, day, hour, minute, second, num * 10, DateTimeKind.Utc).AddHours(4 * unchecked((int)data[15]));
                }
                catch (Exception projectError)
                {
                    ProjectData.SetProjectError(projectError);
                    DateTime minValue = DateTime.MinValue;
                    ProjectData.ClearProjectError();
                    return minValue;
                }
            }
        }

        public uint Swap(uint value)
        {
            checked
            {
                return (uint)unchecked((ulong)((((long)value & 255L) << 24) | (((long)value & 65280L) << 8) | (long)((ulong)((long)value & 16711680L) >> 8) | (long)((ulong)((long)value & 4278190080L) >> 24)));
            }
        }

        public byte[] ToByteArray(byte[] buffer, int offset, int length)
        {
            byte[] array = new byte[checked(length - 1 + 1)];
            Array.Copy(buffer, offset, array, 0, length);
            return array;
        }
    }
}
