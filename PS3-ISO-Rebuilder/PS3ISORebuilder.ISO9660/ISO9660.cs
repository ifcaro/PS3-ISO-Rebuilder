using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace PS3ISORebuilder.ISO9660
{
    public class ISO9660
    {
        private bool compresion;

        public uint Blocksize;

        private Stream internalstream;

        public ulong Disksize;

        private DirectoryRecord root;

        public VolumeDescriptor VolumeDescriptor;

        private Dictionary<DescriptorType, VolumeDescriptor> VolumeDescriptors;

        public Dictionary<string, DirectoryRecord> dirlist;

        public Dictionary<string, DirectoryRecord> filelist;

        public ISO9660(string filename)
        {
            compresion = false;
            Blocksize = 2048u;
            VolumeDescriptors = new Dictionary<DescriptorType, VolumeDescriptor>();
            dirlist = new Dictionary<string, DirectoryRecord>(StringComparer.OrdinalIgnoreCase);
            filelist = new Dictionary<string, DirectoryRecord>(StringComparer.OrdinalIgnoreCase);
            if (File.Exists(filename))
            {
                internalstream = File.OpenRead(filename);
                if (!Parse())
                {
                    Console.WriteLine("not a ISO File");
                }
            }
        }

        public ISO9660(Stream fstream)
        {
            compresion = false;
            Blocksize = 2048u;
            VolumeDescriptors = new Dictionary<DescriptorType, VolumeDescriptor>();
            dirlist = new Dictionary<string, DirectoryRecord>(StringComparer.OrdinalIgnoreCase);
            filelist = new Dictionary<string, DirectoryRecord>(StringComparer.OrdinalIgnoreCase);
            if (fstream != null)
            {
                internalstream = fstream;
                if (!Parse())
                {
                    Console.WriteLine("not a ISO File");
                }
            }
        }

        private bool Parse()
        {
            byte[] array = new byte[16];
            internalstream.Seek(0L, SeekOrigin.Begin);
            internalstream.Read(array, 0, array.Length);
            checked
            {
                if (Operators.CompareString(Encoding.Default.GetString(array, 0, 4), "CPS3", TextCompare: false) == 0)
                {
                    compresion = true;
                    ulong num = BitConverter.ToUInt64(array, 4);
                    Blocksize = BitConverter.ToUInt32(array, 12);
                    Disksize = num * Blocksize;
                }
                try
                {
                    int num2 = 16;
                    bool flag = true;
                    while (flag)
                    {
                        byte[] b = readsector((ulong)num2);
                        BaseVolumeDescriptor baseVolumeDescriptor = new BaseVolumeDescriptor(b);
                        string standardIdentifier = baseVolumeDescriptor.StandardIdentifier;
                        if (Operators.CompareString(standardIdentifier, "CD001", TextCompare: false) == 0)
                        {
                            if (!VolumeDescriptors.ContainsKey(baseVolumeDescriptor.VolumeDescriptorType))
                            {
                                VolumeDescriptors.Add(baseVolumeDescriptor.VolumeDescriptorType, new VolumeDescriptor(b, this));
                            }
                        }
                        else
                        {
                            flag = false;
                        }
                        num2++;
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
                    Blocksize = (uint)unchecked((long)VolumeDescriptor.LogicalBlockSize);
                    Disksize = (ulong)(unchecked((long)VolumeDescriptor.VolumeSpaceSize) * unchecked((long)Blocksize));
                    root = VolumeDescriptor.DirectoryRecord;
                    root.name = "\\";
                    root.fullname = "\\";
                    dirlist.Add(root.fullname, root);
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

        public void close()
        {
            internalstream.Close();
        }

        public void ReadDirectoryRecord(DirectoryRecord root)
        {
            try
            {
                if ((long)root.dataLength > 0L)
                {
                    long num = 0L;
                    checked
                    {
                        byte[] buffer = new byte[(int)(unchecked((long)root.dataLength) - 1L) + 1];
                        root.Read(buffer, 0, (int)root.dataLength);
                        root.reset();
                        while (num > -1)
                        {
                            DirectoryRecord directoryRecord = new DirectoryRecord(buffer, (int)num, VolumeDescriptor.getencoding, this);
                            directoryRecord.parent = root;
                            if (directoryRecord.recordLength > 0)
                            {
                                if (!((Operators.CompareString(directoryRecord.name, ".", TextCompare: false) == 0) | (Operators.CompareString(directoryRecord.name, "..", TextCompare: false) == 0)))
                                {
                                    directoryRecord.fullname = Path.Combine(root.fullname, directoryRecord.name);
                                    if (directoryRecord.flags == FileFlags.Directory)
                                    {
                                        if (!dirlist.ContainsKey(directoryRecord.fullname))
                                        {
                                            dirlist.Add(directoryRecord.fullname, directoryRecord);
                                        }
                                        if (!root.directorys.ContainsKey(directoryRecord.fullname))
                                        {
                                            root.directorys.Add(directoryRecord.fullname, directoryRecord);
                                        }
                                        ReadDirectoryRecord(directoryRecord);
                                    }
                                    else if (filelist.ContainsKey(directoryRecord.fullname))
                                    {
                                        filelist[directoryRecord.fullname].SetLength(filelist[directoryRecord.fullname].Length + directoryRecord.Length);
                                    }
                                    else
                                    {
                                        filelist.Add(directoryRecord.fullname, directoryRecord);
                                        if (root.files.ContainsKey(directoryRecord.fullname))
                                        {
                                            root.files.Add(directoryRecord.fullname, directoryRecord);
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

        public ulong readSectorOffset(ulong sector)
        {
            internalstream.Seek(Convert.ToInt64(decimal.Add(decimal.Multiply(new decimal(sector), new decimal(8L)), new decimal(16L))), SeekOrigin.Begin);
            byte[] array = new byte[8];
            internalstream.Read(array, 0, array.Length);
            return BitConverter.ToUInt64(array, 0);
        }

        public byte[] readsector(ulong SectorNumber)
        {
            checked
            {
                byte[] array = new byte[(int)(unchecked((long)Blocksize) - 1L) + 1];
                if (!compresion)
                {
                    internalstream.Seek((long)(Blocksize * SectorNumber), SeekOrigin.Begin);
                    internalstream.Read(array, 0, array.Length);
                    return array;
                }
                ulong num = readSectorOffset(SectorNumber);
                ulong num2 = readSectorOffset(Convert.ToUInt64(decimal.Add(new decimal(SectorNumber), decimal.One)));
                int num3 = (int)(num2 - num);
                if (num3 == Blocksize)
                {
                    internalstream.Seek((long)num, SeekOrigin.Begin);
                    internalstream.Read(array, 0, array.Length);
                    return array;
                }
                byte[] array2 = new byte[num3 - 1 + 1];
                internalstream.Seek((long)num, SeekOrigin.Begin);
                internalstream.Read(array2, 0, array2.Length);
                int num4 = new DeflateStream(new MemoryStream(array2), CompressionMode.Decompress, leaveOpen: false).Read(array, 0, (int)Blocksize);
                return array;
            }
        }

        public object direxist(string dirname)
        {
            if (dirlist.ContainsKey(dirname))
            {
                return true;
            }
            return false;
        }

        public bool fileexist(string filename)
        {
            if (filelist.ContainsKey(filename))
            {
                return true;
            }
            return false;
        }

        public DirectoryRecord findfile(string filename)
        {
            if (fileexist(filename))
            {
                return filelist[filename];
            }
            return null;
        }
    }
}
