/*Copyright Message
* PKC Manager
* Copyright © 2012 ambax Software UG (haftungsbeschränkt)
*
* According to our dual licensing model, this program can be used either
* under the terms of the GNU Affero General Public License, version 3,
* or under a proprietary license.
*
* The texts of the GNU Affero General Public License with an additional
* permission and of our proprietary license can be found at and
* in the LICENSE file you have received along with this program.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* The licensing of the program under the AGPLv3 does not imply a
* trademark license. Therefore any rights, title and interest in
* our trademarks remain entirely with us.
*
* @category   PKC
* @copyright  Copyright (c) 2014, ambax Software UG (http://www.ambax.de)
* @author ambax Software UG
*/

using System;
using System.IO;
using System.Text;

namespace XBundle.IO.Stream
{
    /// <summary>
    ///     This class handles all the stream data. It can read and write data from/to it and
    ///     safe it back to a file
    /// </summary>
    internal class StreamController : IDisposable
    {
        #region Members

        /// <summary>
        ///     Lock
        ///     Represents the lock so the data cannot be damaged while using multiple threads
        /// </summary>
        private readonly object _mLock = new object();

        #endregion Members

        #region Constructor

        /// <summary>
        ///     Constructor
        ///     Initializes the StreamController with a default buffer
        /// </summary>
        /// <param name="buffer">Data</param>
        /// <param name="engine">Type</param>
        public StreamController(byte[] buffer, StreamOperation engine)
        {
            switch (engine)
            {
                case StreamOperation.Read:
                    SReader = new Reader(buffer);
                    break;

                case StreamOperation.ReadWrite:
                    SReader = new Reader(buffer);

                    SWriter = buffer != null ? new Writer(buffer) : new Writer();
                    break;

                case StreamOperation.Write:
                    SWriter = buffer != null ? new Writer(buffer) : new Writer();
                    break;
            }
        }

        #endregion Constructor

        #region Destructor

        ~StreamController()
        {
            if (!IsDisposing && !IsDisposed) Dispose();
        }

        #endregion Destructor

        #region Properties

        /// <summary>
        ///     Stream Reader
        ///     Represents the buffer reader that is used in this class
        /// </summary>
        public Reader SReader { get; private set; }

        /// <summary>
        ///     Stream Writer
        ///     Represents the buffer writer that is used in this class
        /// </summary>
        public Writer SWriter { get; set; }

        /// <summary>
        ///     Buffer
        ///     Represents the buffer data
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        ///     Engine
        ///     Represents the engine, this class uses for controlling streams
        /// </summary>
        public StreamOperation Engine { get; private set; }

        /// <summary>
        ///     Is Disposed
        ///     Indicates wether this object is disposed
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Is Disposing
        ///     Indicates wether this object is beeing disposed
        /// </summary>
        public bool IsDisposing { get; set; }

        #endregion Properties

        #region Methods

        #region Read

        /// <summary>
        ///     Read Byte
        ///     Reads the next byte from the stream
        /// </summary>
        /// <returns>Next byte</returns>
        public byte ReadByte()
        {
            lock (_mLock)
            {
                return SReader.ReadByte();
            }
        }

        /// <summary>
        ///     Reads the byte array.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public byte[] ReadByteArray(int count)
        {
            lock (_mLock)
            {
                var result = new byte[count];
                for (var i = 0; i < count; i++)
                {
                    result[i] = ReadByte();
                }

                return result;
            }
        }

        /// <summary>
        ///     Read SByte
        ///     Reads the next sbyte from the stream
        /// </summary>
        /// <returns>Next sbyte</returns>
        public sbyte ReadSByte()
        {
            lock (_mLock)
            {
                return SReader.ReadSByte();
            }
        }

        /// <summary>
        ///     Read UShort
        ///     Reads the next ushort from the stream
        /// </summary>
        /// <returns>Next ushort</returns>
        public ushort ReadUShort()
        {
            lock (_mLock)
            {
                return SReader.ReadUInt16();
            }
        }

        /// <summary>
        ///     Read Short
        ///     Reads the next short from the stream
        /// </summary>
        /// <returns>Next short</returns>
        public short ReadShort()
        {
            lock (_mLock)
            {
                return SReader.ReadInt16();
            }
        }

        /// <summary>
        ///     Read UInt
        ///     Reads the next uinteger from the stream
        /// </summary>
        /// <returns>Next uinteger</returns>
        public uint ReadUInt()
        {
            lock (_mLock)
            {
                return SReader.ReadUInt32();
            }
        }

        /// <summary>
        ///     Read Int
        ///     Reads the next integer from the stream
        /// </summary>
        /// <returns>Next integer</returns>
        public int ReadInt()
        {
            lock (_mLock)
            {
                return SReader.ReadInt32();
            }
        }

        /// <summary>
        ///     Read ULong
        ///     Reads the next ulong from the stream
        /// </summary>
        /// <returns>Next ulong</returns>
        public ulong ReadULong()
        {
            lock (_mLock)
            {
                return SReader.ReadUInt64();
            }
        }

        /// <summary>
        ///     Read Long
        ///     Reads the next long from the stream
        /// </summary>
        /// <returns>Next long</returns>
        public long ReadLong()
        {
            lock (_mLock)
            {
                return SReader.ReadInt64();
            }
        }

        /// <summary>
        ///     Read Float
        ///     Reads the next float from the stream
        /// </summary>
        /// <returns>Next float</returns>
        public float ReadFloat()
        {
            lock (_mLock)
            {
                return SReader.ReadSingle();
            }
        }

        /// <summary>
        ///     Read Double
        ///     Reads the next double from the stream
        /// </summary>
        /// <returns>Next double</returns>
        public double ReadDouble()
        {
            lock (_mLock)
            {
                return SReader.ReadDouble();
            }
        }

        /// <summary>
        ///     Read String
        ///     Reads the next string from the stream by the default codepage of '1252'
        /// </summary>
        /// <returns>Next string</returns>
        public string ReadString()
        {
            lock (_mLock)
            {
                return ReadString(1252);
            }
        }

        /// <summary>
        ///     Read String
        ///     Reads the next string from the stream by a specific codepage
        /// </summary>
        /// <returns>Next string</returns>
        public string ReadString(int codepage)
        {
            lock (_mLock)
            {
                var length = SReader.ReadUInt16();
                var bytes = SReader.ReadBytes(length);
                return Encoding.GetEncoding(codepage).GetString(bytes);
            }
        }

        /// <summary>
        ///     Read String
        ///     Reads the next string from the stream by a specific codepage
        /// </summary>
        /// <returns>Next string</returns>
        public string ReadStringByLength(int length)
        {
            lock (_mLock)
            {
                var bytes = SReader.ReadBytes(length);
                return Encoding.GetEncoding(1252).GetString(bytes);
            }
        }

        /// <summary>
        ///     Read Unicode
        ///     Reads the next unicode from the stream
        /// </summary>
        /// <returns>Next unicode</returns>
        public string ReadUnicode()
        {
            lock (_mLock)
            {
                var length = SReader.ReadUInt16();
                var bytes = SReader.ReadBytes(length*2);

                return Encoding.Unicode.GetString(bytes);
            }
        }

        /// <summary>
        ///     Read Bool
        ///     Reads a boolean from the current buffer
        /// </summary>
        /// <returns>Next boolean</returns>
        public bool ReadBool()
        {
            lock (_mLock)
            {
                return SReader.ReadBoolean();
            }
        }

        /// <summary>
        ///     Read Bool Array
        ///     Reads a boolean array from the buffer by using a count
        /// </summary>
        /// <param name="count">Count of booleans in the array</param>
        /// <returns>Next boolean array</returns>
        public bool[] ReadBoolArray(int count)
        {
            lock (_mLock)
            {
                var boolArray = new bool[count];
                for (var i = 0; i < count; i++)
                {
                    boolArray[i] = SReader.ReadBoolean();
                }
                return boolArray;
            }
        }

        /// <summary>
        ///     Read Floag Array
        ///     Reads a float array from the buffer by using a count
        /// </summary>
        /// <param name="count">Count of floats in the array</param>
        /// <returns>Next float array</returns>
        public float[] ReadFloatArray(int count)
        {
            lock (_mLock)
            {
                var numArray = new float[count];
                for (var i = 0; i < count; i++)
                {
                    numArray[i] = SReader.ReadSingle();
                }
                return numArray;
            }
        }

        /// <summary>
        ///     Read Integer Array
        ///     Reads a integer array from the buffer by using a count
        /// </summary>
        /// <param name="count">Count of integer in the array</param>
        /// <returns>Next integer array</returns>
        public int[] ReadIntArray(int count)
        {
            lock (_mLock)
            {
                var numArray = new int[count];
                for (var i = 0; i < count; i++)
                {
                    numArray[i] = SReader.ReadInt32();
                }
                return numArray;
            }
        }

        /// <summary>
        ///     Read Long Array
        ///     Reads a long array from the buffer by using a count
        /// </summary>
        /// <param name="count">Count of longs in the array</param>
        /// <returns>Next long array</returns>
        public long[] ReadLongArray(int count)
        {
            lock (_mLock)
            {
                var numArray = new long[count];
                for (var i = 0; i < count; i++)
                {
                    numArray[i] = SReader.ReadInt64();
                }
                return numArray;
            }
        }

        /// <summary>
        ///     Read SByte Array
        ///     Reads a sbyte array from the buffer by using a count
        /// </summary>
        /// <param name="count">Count of sbytes in the array</param>
        /// <returns>Next sbyte array</returns>
        public sbyte[] ReadSByteArray(int count)
        {
            lock (_mLock)
            {
                var numArray = new sbyte[count];
                for (var i = 0; i < count; i++)
                {
                    numArray[i] = SReader.ReadSByte();
                }
                return numArray;
            }
        }

        /// <summary>
        ///     Read Short Array
        ///     Reads a sbyte array from the buffer by using a count
        /// </summary>
        /// <param name="count">Count of shorts in the array</param>
        /// <returns>Next short array</returns>
        public short[] ReadShortArray(int count)
        {
            lock (_mLock)
            {
                var numArray = new short[count];
                for (var i = 0; i < count; i++)
                {
                    numArray[i] = SReader.ReadInt16();
                }
                return numArray;
            }
        }

        /// <summary>
        ///     Read UInteger Array
        ///     Reads a uinteger array from the buffer by using a count
        /// </summary>
        /// <param name="count">Count of uintegers in the array</param>
        /// <returns>Next uinteger array</returns>
        public uint[] ReadUIntArray(int count)
        {
            lock (_mLock)
            {
                var numArray = new uint[count];
                for (var i = 0; i < count; i++)
                {
                    numArray[i] = SReader.ReadUInt32();
                }
                return numArray;
            }
        }

        /// <summary>
        ///     Read ULong Array
        ///     Reads a ulong array from the buffer by using a count
        /// </summary>
        /// <param name="count">Count of ulongs in the array</param>
        /// <returns>Next ulong array</returns>
        public ulong[] ReadULongArray(int count)
        {
            lock (_mLock)
            {
                var numArray = new ulong[count];
                for (var i = 0; i < count; i++)
                {
                    numArray[i] = SReader.ReadUInt64();
                }
                return numArray;
            }
        }

        /// <summary>
        ///     Read Unicode Array
        ///     Reads a unicode array from the buffer by using a count
        /// </summary>
        /// <param name="count">Count of unicodes in the array</param>
        /// <returns>Next unicode array</returns>
        public string[] ReadUnicodeArray(int count)
        {
            lock (_mLock)
            {
                var strArray = new string[count];
                for (var i = 0; i < count; i++)
                {
                    var num2 = SReader.ReadUInt16();
                    var bytes = SReader.ReadBytes(num2*2);
                    strArray[i] = Encoding.Unicode.GetString(bytes);
                }
                return strArray;
            }
        }

        /// <summary>
        ///     Read UShort Array
        ///     Reads a ushort array from the buffer by using a count
        /// </summary>
        /// <param name="count">Count of ushorts in the array</param>
        /// <returns>Next ushort array</returns>
        public ushort[] ReadUShortArray(int count)
        {
            lock (_mLock)
            {
                var numArray = new ushort[count];
                for (var i = 0; i < count; i++)
                {
                    numArray[i] = SReader.ReadUInt16();
                }
                return numArray;
            }
        }

        /// <summary>
        ///     Read String Array
        ///     Reads a string array from the buffer by using a count
        /// </summary>
        /// <param name="count">Count of strings in the array</param>
        /// <returns>Next string array</returns>
        public string[] ReadStringArray(int count)
        {
            return ReadStringArray(0x4e4);
        }

        /// <summary>
        ///     Read String Array
        ///     Reads a string array from the buffer by using a codepage and a count
        /// </summary>
        /// <param name="codepage"></param>
        /// <param name="count">Count of strings in the array</param>
        /// <returns>Next string array</returns>
        public string[] ReadStringArray(int codepage, int count)
        {
            lock (_mLock)
            {
                var strArray = new string[count];
                for (var i = 0; i < count; i++)
                {
                    var num2 = SReader.ReadUInt16();
                    var bytes = SReader.ReadBytes(num2);
                    strArray[i] = Encoding.UTF7.GetString(bytes);
                }
                return strArray;
            }
        }

        /// <summary>
        ///     Seek Read
        ///     Sets the position in the current reader stream
        /// </summary>
        /// <param name="offset">Startpoint</param>
        /// <param name="origin"></param>
        /// <returns>Seek</returns>
        public long SeekRead(long offset, SeekOrigin origin)
        {
            lock (_mLock)
            {
                return SReader.BaseStream.Seek(offset, origin);
            }
        }

        #endregion Read

        #region Write

        /// <summary>
        ///     Append
        ///     Appends a block of data to the writer
        /// </summary>
        /// <param name="data">Block of data</param>
        public void Append(byte[] data)
        {
            WriteByteArray(data);
        }

        /// <summary>
        ///     Extends the writer to a new size.
        /// </summary>
        /// <param name="finalSize"></param>
        public void ExtendTo(int finalSize)
        {
            var dataToAppend = new byte[finalSize - SWriter.BaseStream.Length];
            for (var i = 0; i < dataToAppend.Length; i++)
                dataToAppend[i] = 0;

            Append(dataToAppend);
        }

        /// <summary>
        ///     Append
        ///     Appends a block of data to the writer
        /// </summary>
        /// <param name="stream">StreamController to append</param>
        public void Append(StreamController stream)
        {
            WriteByteArray(stream.GetWriterBytes());
        }

        /// <summary>
        ///     Sets the current position within the stream
        /// </summary>
        /// <param name="position"></param>
        public void SeekWrite(long position)
        {
            SWriter.BaseStream.Position = position;
        }

        public void WriteString(object value)
        {
            WriteAscii(value, 0x4e4);
        }

        public void WriteString(string value)
        {
            WriteAscii(value, 0x4e4);
        }

        public void WriteFixedString(string value)
        {
            lock (_mLock)
            {
                var bytes = Encoding.GetEncoding(0x4e4).GetBytes(value);
                var s = Encoding.UTF7.GetString(bytes);
                var buffer = Encoding.Default.GetBytes(s);
                SWriter.Write(buffer);
            }
        }

        public void WriteAscii(object value, int codepage)
        {
            lock (_mLock)
            {
                var bytes = Encoding.GetEncoding(codepage).GetBytes(value.ToString());
                var s = Encoding.UTF7.GetString(bytes);
                var buffer = Encoding.Default.GetBytes(s);
                SWriter.Write((ushort) buffer.Length);
                SWriter.Write(buffer);
            }
        }

        public void WriteAscii(string value, int codepage)
        {
            lock (_mLock)
            {
                var bytes = Encoding.GetEncoding(codepage).GetBytes(value);
                var s = Encoding.UTF7.GetString(bytes);
                var buffer = Encoding.Default.GetBytes(s);
                SWriter.Write((ushort) buffer.Length);
                SWriter.Write(buffer);
            }
        }

        public void WriteAsciiArray(object[] values)
        {
            WriteAsciiArray(values, 0, values.Length, 0x4e4);
        }

        public void WriteAsciiArray(string[] values)
        {
            WriteAsciiArray(values, 0, values.Length, 0x4e4);
        }

        public void WriteAsciiArray(object[] values, int codepage)
        {
            WriteAsciiArray(values, 0, values.Length, codepage);
        }

        public void WriteAsciiArray(string[] values, int codepage)
        {
            WriteAsciiArray(values, 0, values.Length, codepage);
        }

        public void WriteAsciiArray(object[] values, int index, int count)
        {
            WriteAsciiArray(values, index, count, 0x4e4);
        }

        public void WriteAsciiArray(string[] values, int index, int count)
        {
            WriteAsciiArray(values, index, count, 0x4e4);
        }

        public void WriteAsciiArray(object[] values, int index, int count, int codepage)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteAscii(values[i].ToString(), codepage);
                }
            }
        }

        public void WriteAsciiArray(string[] values, int index, int count, int codepage)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteAscii(values[i], codepage);
                }
            }
        }

        public void WriteBool(bool value)
        {
            lock (_mLock)
            {
                SWriter.Write(value);
            }
        }

        public void WriteBool(object value)
        {
            lock (_mLock)
            {
                SWriter.Write((byte) (Convert.ToUInt64(value) & 0xffL));
            }
        }

        public void WriteBoolArray(bool[] values)
        {
            WriteBoolArray(values, 0, values.Length);
        }

        public void WriteBoolArray(object[] values)
        {
            WriteBoolArray(values, 0, values.Length);
        }

        public void WriteBoolArray(bool[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    SWriter.Write(values[i]);
                }
            }
        }

        public void WriteBoolArray(object[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteBool(values[i]);
                }
            }
        }

        public void WriteByte(byte value)
        {
            lock (_mLock)
            {
                SWriter.Write(value);
            }
        }

        public void WriteByte(object value)
        {
            lock (_mLock)
            {
                SWriter.Write((byte) (Convert.ToUInt64(value) & 0xffL));
            }
        }

        public void WriteByteArray(byte[] values)
        {
            lock (_mLock)
            {
                SWriter.Write(values);
            }
        }

        public void WriteByteArray(object[] values)
        {
            lock (_mLock)
            {
                WriteByteArray(values, 0, values.Length);
            }
        }

        public void WriteByteArray(byte[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    SWriter.Write(values[i]);
                }
            }
        }

        public void WriteByteArray(object[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteByte(values[i]);
                }
            }
        }

        public void WriteDouble(double value)
        {
            lock (_mLock)
            {
                SWriter.Write(value);
            }
        }

        public void WriteDouble(object value)
        {
            lock (_mLock)
            {
                SWriter.Write(Convert.ToDouble(value));
            }
        }

        public void WriteDoubleArray(double[] values)
        {
            WriteDoubleArray(values, 0, values.Length);
        }

        public void WriteDoubleArray(object[] values)
        {
            WriteDoubleArray(values, 0, values.Length);
        }

        public void WriteDoubleArray(double[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    SWriter.Write(values[i]);
                }
            }
        }

        public void WriteDoubleArray(object[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteDouble(values[i]);
                }
            }
        }

        public void WriteFloat(object value)
        {
            lock (_mLock)
            {
                SWriter.Write(Convert.ToSingle(value));
            }
        }

        public void WriteFloat(float value)
        {
            lock (_mLock)
            {
                SWriter.Write(value);
            }
        }

        public void WriteFloatArray(object[] values)
        {
            WriteFloatArray(values, 0, values.Length);
        }

        public void WriteFloatArray(float[] values)
        {
            WriteFloatArray(values, 0, values.Length);
        }

        public void WriteFloatArray(object[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteFloat(values[i]);
                }
            }
        }

        public void WriteFloatArray(float[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    SWriter.Write(values[i]);
                }
            }
        }

        public void WriteInteger(int value)
        {
            lock (_mLock)
            {
                SWriter.Write(value);
            }
        }

        public void WriteInt(object value)
        {
            lock (_mLock)
            {
                SWriter.Write((int) (((ulong) Convert.ToInt64(value)) & 0xffffffffL));
            }
        }

        public void WriteIntArray(int[] values)
        {
            WriteIntArray(values, 0, values.Length);
        }

        public void WriteIntArray(object[] values)
        {
            WriteIntArray(values, 0, values.Length);
        }

        public void WriteIntArray(int[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    SWriter.Write(values[i]);
                }
            }
        }

        public void WriteIntArray(object[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteInt(values[i]);
                }
            }
        }

        public void WriteLong(long value)
        {
            lock (_mLock)
            {
                SWriter.Write(value);
            }
        }

        public void WriteLong(object value)
        {
            lock (_mLock)
            {
                SWriter.Write(Convert.ToInt64(value));
            }
        }

        public void WriteLongArray(long[] values)
        {
            WriteLongArray(values, 0, values.Length);
        }

        public void WriteLongArray(object[] values)
        {
            WriteLongArray(values, 0, values.Length);
        }

        public void WriteLongArray(long[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    SWriter.Write(values[i]);
                }
            }
        }

        public void WriteLongArray(object[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteLong(values[i]);
                }
            }
        }

        public void WriteSByte(object value)
        {
            lock (_mLock)
            {
                SWriter.Write((sbyte) (Convert.ToInt64(value) & 0xffL));
            }
        }

        public void WriteSByte(sbyte value)
        {
            lock (_mLock)
            {
                SWriter.Write(value);
            }
        }

        public void WriteSByteArray(object[] values)
        {
            WriteSByteArray(values, 0, values.Length);
        }

        public void WriteSByteArray(object[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteSByte(values[i]);
                }
            }
        }

        public void WriteShort(short value)
        {
            lock (_mLock)
            {
                SWriter.Write(value);
            }
        }

        public void WriteShort(object value)
        {
            lock (_mLock)
            {
                SWriter.Write((ushort) (Convert.ToInt64(value) & 0xffffL));
            }
        }

        public void WriteShortArray(short[] values)
        {
            WriteShortArray(values, 0, values.Length);
        }

        public void WriteShortArray(object[] values)
        {
            WriteShortArray(values, 0, values.Length);
        }

        public void WriteShortArray(short[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    SWriter.Write(values[i]);
                }
            }
        }

        public void WriteShortArray(object[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteShort(values[i]);
                }
            }
        }

        public void WriteUInt(object value)
        {
            lock (_mLock)
            {
                SWriter.Write((uint) (Convert.ToUInt64(value) & 0xffffffffL));
            }
        }

        public void WriteUInt(uint value)
        {
            lock (_mLock)
            {
                SWriter.Write(value);
            }
        }

        public void WriteUIntArray(object[] values)
        {
            WriteUIntArray(values, 0, values.Length);
        }

        public void WriteUIntArray(uint[] values)
        {
            WriteUIntArray(values, 0, values.Length);
        }

        public void WriteUIntArray(object[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteUInt(values[i]);
                }
            }
        }

        public void WriteUIntArray(uint[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    SWriter.Write(values[i]);
                }
            }
        }

        public void WriteULong(object value)
        {
            lock (_mLock)
            {
                SWriter.Write(Convert.ToUInt64(value));
            }
        }

        public void WriteULong(ulong value)
        {
            lock (_mLock)
            {
                SWriter.Write(value);
            }
        }

        public void WriteULongArray(object[] values)
        {
            WriteULongArray(values, 0, values.Length);
        }

        public void WriteULongArray(ulong[] values)
        {
            WriteULongArray(values, 0, values.Length);
        }

        public void WriteULongArray(object[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteULong(values[i]);
                }
            }
        }

        public void WriteULongArray(ulong[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    SWriter.Write(values[i]);
                }
            }
        }

        public void WriteUnicode(object value)
        {
            lock (_mLock)
            {
                var bytes = Encoding.Unicode.GetBytes(value.ToString());
                SWriter.Write((ushort) value.ToString().Length);
                SWriter.Write(bytes);
            }
        }

        public void WriteUnicode(string value)
        {
            lock (_mLock)
            {
                var bytes = Encoding.Unicode.GetBytes(value);
                SWriter.Write((ushort) value.Length);
                SWriter.Write(bytes);
            }
        }

        public void WriteUnicodeArray(object[] values)
        {
            WriteUnicodeArray(values, 0, values.Length);
        }

        public void WriteUnicodeArray(string[] values)
        {
            WriteUnicodeArray(values, 0, values.Length);
        }

        public void WriteUnicodeArray(object[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteUnicode(values[i].ToString());
                }
            }
        }

        public void WriteUnicodeArray(string[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteUnicode(values[i]);
                }
            }
        }

        public void WriteUShort(object value)
        {
            lock (_mLock)
            {
                SWriter.Write((ushort) (Convert.ToUInt64(value) & 0xffffL));
            }
        }

        public void WriteUShort(ushort value)
        {
            lock (_mLock)
            {
                SWriter.Write(value);
            }
        }

        public void WriteUShortArray(object[] values)
        {
            WriteUShortArray(values, 0, values.Length);
        }

        public void WriteUShortArray(ushort[] values)
        {
            WriteUShortArray(values, 0, values.Length);
        }

        public void WriteUShortArray(object[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    WriteUShort(values[i]);
                }
            }
        }

        public void WriteUShortArray(ushort[] values, int index, int count)
        {
            lock (_mLock)
            {
                for (var i = index; i < (index + count); i++)
                {
                    SWriter.Write(values[i]);
                }
            }
        }

        #endregion Write

        /// <summary>
        ///     Get Writer Bytes
        ///     Returns the current buffer of the writer
        /// </summary>
        /// <returns></returns>
        public byte[] GetWriterBytes()
        {
            return SWriter.GetBuffer();
        }

        /// <summary>
        ///     Dispose
        ///     Lets the current object dispose
        ///     Exceptions:
        ///     ObjectDisposedException
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed || IsDisposing)
                throw new ObjectDisposedException("StreamController");

            lock (_mLock)
            {
                IsDisposing = true;

                if (SReader != null)
                {
                    SReader.Close();
                    SReader = null;
                }

                if (SWriter != null)
                {
                    SWriter.Close();
                    SWriter = null;
                }
                Buffer = null;

                IsDisposing = false;
                IsDisposed = true;
            }
        }

        #endregion Methods
    }
}