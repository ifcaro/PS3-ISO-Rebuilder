using System.IO;

namespace PS3ISORebuilder.IRDFile
{
    public class CRC32
    {
        private int[] crc32Table;

        private const int BUFFER_SIZE = 1024;

        public int GetCrc32(ref Stream stream, long lenght)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            byte[] array = new byte[checked((int)(lenght - 1) + 1)];
            stream.Read(array, 0, array.Length);
            return GetCrc32(new MemoryStream(array));
        }

        public int GetCrc32(Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            int num = -1;
            byte[] array = new byte[1025];
            int count = 1024;
            int num2 = stream.Read(array, 0, count);
            while (num2 > 0)
            {
                int num4 = checked(num2 - 1);
                for (int i = 0; i <= num4; i = checked(i + 1))
                {
                    int num5 = (num & 0xFF) ^ array[i];
                    num = (((num & -256) / 256) & 0xFFFFFF);
                    num ^= crc32Table[num5];
                }
                num2 = stream.Read(array, 0, count);
            }
            return ~num;
        }

        public CRC32()
        {
            int num = -306674912;
            crc32Table = new int[257];
            int num2 = 0;
            checked
            {
                do
                {
                    int num3 = num2;
                    int num4 = 8;
                    do
                    {
                        if ((num3 & 1) != 0)
                        {
                            num3 = (int)(unchecked((long)(num3 & -2) / 2L) & int.MaxValue);
                            num3 ^= num;
                        }
                        else
                        {
                            num3 = (int)(unchecked((long)(num3 & -2) / 2L) & int.MaxValue);
                        }
                        num4 += -1;
                    }
                    while (num4 >= 1);
                    crc32Table[num2] = num3;
                    num2++;
                }
                while (num2 <= 255);
            }
        }
    }
}
