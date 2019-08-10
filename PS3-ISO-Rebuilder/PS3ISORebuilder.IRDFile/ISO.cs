using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;

namespace PS3ISORebuilder.IRDFile
{
    public class ISO
    {
        public long Blocksize;

        private Stream internalreader;

        public VolumeDescriptor VolumeDescriptor;

        public Dictionary<DescriptorType, VolumeDescriptor> VolumeDescriptors;

        public Dictionary<string, DirectoryRecord> dirlist;

        public Dictionary<string, DirectoryRecord> filelist;

        private Dictionary<long, byte[]> filehashes;

        private DirectoryRecord root;

        public ulong Disksize;

        public ISO(Stream fstream, Dictionary<long, byte[]> Hashes)
        {
            Blocksize = 2048L;
            VolumeDescriptors = new Dictionary<DescriptorType, VolumeDescriptor>();
            dirlist = new Dictionary<string, DirectoryRecord>(StringComparer.OrdinalIgnoreCase);
            filelist = new Dictionary<string, DirectoryRecord>(StringComparer.OrdinalIgnoreCase);
            filehashes = Hashes;
            if (fstream != null)
            {
                internalreader = fstream;
                if (!Parse())
                {
                    Console.WriteLine("not a ISO File");
                }
            }
        }

        private bool Parse()
        {
            checked
            {
                try
                {
                    int num = 16;
                    bool flag = true;
                    while (flag)
                    {
                        byte[] b = readsector((ulong)num, (ulong)Blocksize);
                        BaseVolumeDescriptor baseVolumeDescriptor = new BaseVolumeDescriptor(b);
                        string standardIdentifier = baseVolumeDescriptor.StandardIdentifier;
                        if (Operators.CompareString(standardIdentifier, "CD001", TextCompare: false) == 0)
                        {
                            if (!VolumeDescriptors.ContainsKey(baseVolumeDescriptor.VolumeDescriptorType))
                            {
                                VolumeDescriptors.Add(baseVolumeDescriptor.VolumeDescriptorType, new VolumeDescriptor(b));
                            }
                        }
                        else
                        {
                            flag = false;
                        }
                        num++;
                    }
                    if (VolumeDescriptors.ContainsKey(DescriptorType.Supplementary))
                    {
                        VolumeDescriptor = VolumeDescriptors[DescriptorType.Supplementary];
                    }
                    else
                    {
                        if (!VolumeDescriptors.ContainsKey(DescriptorType.Primary))
                        {
                            return false;
                        }
                        VolumeDescriptor = VolumeDescriptors[DescriptorType.Primary];
                    }
                    Blocksize = VolumeDescriptor.LogicalBlockSize;
                    Disksize = (ulong)(unchecked((long)VolumeDescriptor.VolumeSpaceSize) * Blocksize);
                    root = VolumeDescriptor.DirectoryRecord;
                    root.name = "\\";
                    root.entrypath = "\\";
                    dirlist.Add(root.entrypath, root);
                    ReadDirectoryRecord(root);
                    return true;
                }
                catch (Exception projectError)
                {
                    ProjectData.SetProjectError(projectError);
                    bool result = false;
                    ProjectData.ClearProjectError();
                    return result;
                }
            }
        }

        public void ReadDirectoryRecord(DirectoryRecord root)
        {
            try
            {
                if ((long)root.dataLength > 0L)
                {
                    long num = 0L;
                    byte[] buffer = readsector(root.firstDataSector, root.dataLength);
                    checked
                    {
                        while (num > -1)
                        {
                            DirectoryRecord directoryRecord = new DirectoryRecord(buffer, (int)num, VolumeDescriptor.getencoding);
                            directoryRecord.parent = root;
                            if (directoryRecord.recordLength > 0)
                            {
                                if (!((Operators.CompareString(directoryRecord.name, ".", TextCompare: false) == 0) | (Operators.CompareString(directoryRecord.name, "..", TextCompare: false) == 0)))
                                {
                                    directoryRecord.entrypath = Path.Combine(root.entrypath, directoryRecord.name);
                                    if (directoryRecord.flags == FileFlags.Directory)
                                    {
                                        if (!dirlist.ContainsKey(directoryRecord.entrypath))
                                        {
                                            dirlist.Add(directoryRecord.entrypath, directoryRecord);
                                        }
                                        if (!root.directorys.ContainsKey(directoryRecord.entrypath))
                                        {
                                            root.directorys.Add(directoryRecord.entrypath, directoryRecord);
                                        }
                                        ReadDirectoryRecord(directoryRecord);
                                    }
                                    else if (filelist.ContainsKey(directoryRecord.entrypath))
                                    {
                                        DirectoryRecord directoryRecord2 = filelist[directoryRecord.entrypath];
                                        directoryRecord2.Length += directoryRecord.dataLength;
                                    }
                                    else
                                    {
                                        directoryRecord.md5 = filehashes[directoryRecord.firstDataSector];
                                        filelist.Add(directoryRecord.entrypath, directoryRecord);
                                        if (root.files.ContainsKey(directoryRecord.entrypath))
                                        {
                                            root.files.Add(directoryRecord.entrypath, directoryRecord);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                directoryRecord.recordLength = 1;
                            }
                            num += unchecked((long)directoryRecord.recordLength);
                            if (num >= root.dataLength)
                            {
                                num = -1L;
                            }
                        }
                    }
                }
            }
            catch (Exception projectError)
            {
                ProjectData.SetProjectError(projectError);
                ProjectData.ClearProjectError();
            }
        }

        public byte[] readsector(ulong sectorNumber, ulong length)
        {
            byte[] array = new byte[checked(Convert.ToInt32(decimal.Subtract(new decimal(length), decimal.One)) + 1)];
            internalreader.Seek(Convert.ToInt64(decimal.Multiply(new decimal(Blocksize), new decimal(sectorNumber))), SeekOrigin.Begin);
            internalreader.Read(array, 0, array.Length);
            return array;
        }
    }
}
