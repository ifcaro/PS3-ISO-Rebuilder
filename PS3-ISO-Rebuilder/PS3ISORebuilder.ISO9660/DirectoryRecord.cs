using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PS3ISORebuilder.ISO9660
{
    public class DirectoryRecord : Stream
    {
        public byte recordLength;

        public byte sectorsInExtendedRecord;

        public uint firstDataSector;

        public uint dataLength;

        public DateTime RecordingDateAndTime;

        public FileFlags flags;

        public byte fileUnitSize;

        public byte interleaveGap;

        public ushort volSeqNumber;

        public byte nameLength;

        public string name;

        public string fullname;

        public bool isDirectory;

        public byte FileVersion;

        public DirectoryRecord parent;

        public Dictionary<string, DirectoryRecord> directorys;

        public Dictionary<string, DirectoryRecord> files;

        public ulong _Length;

        public uint blocksize;

        private int currentSector;

        private long currentOffset;

        private int sectorOffset;

        private byte[] sectorBuffer;

        private ISO9660 internalReader;

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => checked((long)_Length);

        public override long Position
        {
            get
            {
                return currentOffset;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        public DirectoryRecord(byte[] buffer, int offset, Encoding enc, ISO9660 reader)
        {
            name = "";
            fullname = "";
            FileVersion = 0;
            directorys = new Dictionary<string, DirectoryRecord>();
            files = new Dictionary<string, DirectoryRecord>();
            checked
            {
                recordLength = buffer[0 + offset];
                if (recordLength > 0)
                {
                    sectorsInExtendedRecord = buffer[1 + offset];
                    firstDataSector = BitConverter.ToUInt32(buffer, 2 + offset);
                    dataLength = BitConverter.ToUInt32(buffer, 10 + offset);
                    try
                    {
                        DateTime dateTime = new DateTime(1900 + unchecked((int)buffer[checked(18 + offset)]), buffer[19 + offset], buffer[20 + offset], buffer[21 + offset], buffer[22 + offset], buffer[23 + offset]);
                        DateTime dateTime2 = dateTime;
                        RecordingDateAndTime = dateTime2.AddHours(4 * unchecked((int)buffer[checked(24 + offset)]));
                    }
                    catch (Exception projectError)
                    {
                        ProjectData.SetProjectError(projectError);
                        RecordingDateAndTime = DateTime.Now;
                        ProjectData.ClearProjectError();
                    }
                    unchecked
                    {
                        flags = (FileFlags)buffer[checked(25 + offset)];
                    }
                    fileUnitSize = buffer[26 + offset];
                    interleaveGap = buffer[27 + offset];
                    volSeqNumber = BitConverter.ToUInt16(buffer, 28 + offset);
                    nameLength = buffer[32 + offset];
                    if (nameLength == 1 && (buffer[33 + offset] == 0 || buffer[33 + offset] == 1))
                    {
                        if (buffer[33 + offset] == 0)
                        {
                            name = ".";
                        }
                        else
                        {
                            name = "..";
                        }
                    }
                    else
                    {
                        name = enc.GetString(buffer, 33 + offset, nameLength);
                        if (Strings.InStr(name, ";") != 0)
                        {
                            FileVersion = (byte)Conversions.ToInteger(Strings.Split(name, ";")[1]);
                            name = Strings.Split(name, ";")[0];
                        }
                    }
                    isDirectory = Conversions.ToBoolean(Interaction.IIf(flags == FileFlags.Directory, true, false));
                }
                _Length = dataLength;
                currentSector = (int)firstDataSector;
                currentOffset = 0L;
                sectorOffset = 0;
                internalReader = reader;
                blocksize = reader.Blocksize;
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                return 0;
            }
            checked
            {
                if (offset < 0 || count < 0 || count > buffer.Length - offset)
                {
                    return 0;
                }
                sectorBuffer = internalReader.readsector((ulong)currentSector);
                int num = 0;
                int num2 = readInternal(buffer, offset, (int)Math.Min(count, unchecked((long)blocksize) - unchecked((long)sectorOffset)));
                offset += num2;
                count -= num2;
                num += num2;
                while (count >= blocksize && decimal.Compare(new decimal(currentOffset), new decimal(_Length)) < 0)
                {
                    readnextsector();
                    int num3 = readInternal(buffer, offset, (int)blocksize);
                    offset += num3;
                    count -= num3;
                    num += num3;
                }
                if (count > 0)
                {
                    readnextsector();
                    int num4 = readInternal(buffer, offset, count);
                    num += num4;
                }
                return num;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            checked
            {
                switch (origin)
                {
                    case SeekOrigin.Current:
                        offset = currentOffset + offset;
                        break;
                    case SeekOrigin.End:
                        offset = Length - offset;
                        break;
                }
                if ((decimal.Compare(new decimal(offset), new decimal(_Length)) > 0) | (offset < 0))
                {
                    Console.WriteLine("Seek offset " + Conversions.ToString(offset) + " out of bounds.");
                    offset = 0L;
                }
                currentOffset = offset;
                currentSector = (int)Math.Round(unchecked((double)firstDataSector + Conversion.Fix((double)offset / (double)blocksize)));
                sectorOffset = (int)unchecked(offset % (long)blocksize);
                return currentOffset;
            }
        }

        public override void SetLength(long value)
        {
            _Length = checked((ulong)value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
        }

        private int readInternal(byte[] b, int off, int len)
        {
            checked
            {
                if (len > 0)
                {
                    if (decimal.Compare(new decimal(len), decimal.Subtract(new decimal(_Length), new decimal(currentOffset))) > 0)
                    {
                        len = Convert.ToInt32(decimal.Subtract(new decimal(_Length), new decimal(currentOffset)));
                    }
                    Array.Copy(sectorBuffer, sectorOffset, b, off, len);
                    sectorOffset += len;
                    currentOffset += len;
                }
                return len;
            }
        }

        private void readnextsector()
        {
            checked
            {
                if (sectorOffset == blocksize)
                {
                    currentSector++;
                    if (currentSector < internalReader.VolumeDescriptor.VolumeSpaceSize)
                    {
                        sectorBuffer = internalReader.readsector((ulong)currentSector);
                        sectorOffset = 0;
                    }
                }
            }
        }

        public void reset()
        {
            Seek(0L, SeekOrigin.Begin);
        }
    }
}
