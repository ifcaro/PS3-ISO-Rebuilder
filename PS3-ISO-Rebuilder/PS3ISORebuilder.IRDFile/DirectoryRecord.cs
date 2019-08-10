using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace PS3ISORebuilder.IRDFile
{
    public class DirectoryRecord
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

        public string entrypath;

        public bool isDirectory;

        public byte FileVersion;

        public DirectoryRecord parent;

        public Dictionary<string, DirectoryRecord> directorys;

        public Dictionary<string, DirectoryRecord> files;

        public ulong Length;

        public byte[] md5;

        public string md5String => BitConverter.ToString(md5).Replace("-", "").ToUpper();

        public DirectoryRecord(byte[] buffer, int offset, Encoding enc)
        {
            name = "";
            FileVersion = 0;
            directorys = new Dictionary<string, DirectoryRecord>();
            files = new Dictionary<string, DirectoryRecord>();
            checked
            {
                recordLength = buffer[0 + offset];
                if (recordLength <= 0)
                {
                    return;
                }
                sectorsInExtendedRecord = buffer[1 + offset];
                firstDataSector = BitConverter.ToUInt32(buffer, 2 + offset);
                dataLength = BitConverter.ToUInt32(buffer, 10 + offset);
                Length = dataLength;
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
            }
            flags = (FileFlags)buffer[checked(25 + offset)];
            checked
            {
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
        }
    }
}
