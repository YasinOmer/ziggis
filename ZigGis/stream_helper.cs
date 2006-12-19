using System;
using System.Text;
using System.Runtime.InteropServices.ComTypes;

namespace ZigGis.Utilities
{
    public class StreamHelper
    {
        public StreamHelper(ESRI.ArcGIS.esriSystem.IStream stream)
        {
            m_stream = (IStream)stream;
        }

        private IStream m_stream;
        protected IStream stream { get { return m_stream; } }

        private IntPtr m_tmpPtr = new IntPtr();

        public void read(byte[] dataBuffer, int count)
        {
            stream.Read(dataBuffer, count, m_tmpPtr);
        }

        public int readInt()
        {
            byte [] data = new byte[sizeof(int)];
            read(data, data.Length);
            return BitConverter.ToInt32(data, 0);
        }

        public bool readBool()
        {
            byte [] data = new byte[sizeof(bool)];
            read(data, data.Length);
            return BitConverter.ToBoolean(data, 0);
        }

        public double readDouble()
        {
            byte [] data = new byte[sizeof(double)];
            read(data, data.Length);
            return BitConverter.ToDouble(data, 0);
        }

        public string readString()
        {
            // Get the length of the string.
            int len = readInt();

            string retVal = "";
            if (len > 0)
            {
                // Read in the string bytes.
                byte [] data = new byte[len];
                read(data, len);

                // Convert to a normal string.
                retVal = Encoding.ASCII.GetString(data);
            }

            return retVal;
        }

        public void write(byte[] data)
        {
            stream.Write(data, data.Length, m_tmpPtr);
        }

        public void writeInt(int value)
        {
            byte [] data = BitConverter.GetBytes(value);
            write(data);
        }

        public void writeString(string value)
        {
            // Write the length.
            int len = value.Length;
            writeInt(len);

            // Write the string.
            if (len > 0)
            {
                byte[] data = Encoding.ASCII.GetBytes(value);
                write(data);
            }
        }
    }
}
