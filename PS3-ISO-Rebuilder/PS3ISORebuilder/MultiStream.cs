using System;
using System.Collections.Generic;
using System.IO;

namespace PS3ISORebuilder
{
    public class MultiStream : Stream
    {
        private List<Stream> streamList;

        private long m_position;

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length
        {
            get
            {
                long num = 0L;
                foreach (Stream stream in streamList)
                {
                    num = checked(num + stream.Length);
                }
                return num;
            }
        }

        public override long Position
        {
            get
            {
                return m_position;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        public MultiStream()
        {
            streamList = new List<Stream>();
            m_position = 0L;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long length = Length;
            checked
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        m_position = offset;
                        break;
                    case SeekOrigin.Current:
                        m_position += offset;
                        break;
                    case SeekOrigin.End:
                        m_position = length - offset;
                        break;
                }
                if (m_position > length)
                {
                    m_position = length;
                }
                else if (m_position < 0)
                {
                    m_position = 0L;
                }
                return m_position;
            }
        }

        public void Add(Stream stream)
        {
            streamList.Add(stream);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long num = 0L;
            int num2 = 0;
            int num3 = offset;
            checked
            {
                using (List<Stream>.Enumerator enumerator = streamList.GetEnumerator())
                {
                    while (true)
                    {
                        if (!enumerator.MoveNext())
                        {
                            return num2;
                        }
                        Stream current = enumerator.Current;
                        if (m_position < num + current.Length)
                        {
                            current.Position = m_position - num;
                            int num4 = current.Read(buffer, num3, count);
                            num2 += num4;
                            num3 += num4;
                            m_position += num4;
                            if (num4 >= count)
                            {
                                break;
                            }
                            count -= num4;
                        }
                        num += current.Length;
                    }
                    return num2;
                }
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
    }
}
