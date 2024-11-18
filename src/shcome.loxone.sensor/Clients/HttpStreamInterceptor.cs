using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace shcome.loxone.sensor.Clients
{
    public class HttpStreamInterceptor : Stream
    {
        private readonly NetworkStream _child;
        public HttpStreamInterceptor(NetworkStream child)
        {
            _child = child;
        }

        public override bool CanRead => _child.CanRead;

        public override bool CanSeek => _child.CanSeek;

        public override bool CanWrite => _child.CanWrite;

        public override long Length => _child.Length;

        public override long Position { get => _child.Position; set => _child.Position = value; }

        public override void Flush()
        {
            _child.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = _child.Read(buffer, offset, count);
            if(read > 0)
            {
                string decoded = Encoding.UTF8.GetString(buffer);
                Debug.WriteLine(decoded);
            }
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _child.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _child.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _child.Write(buffer, offset, count);
        }
    }
}
