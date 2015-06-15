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

namespace XBundle.IO
{
    internal class FileReader : IDisposable
    {
        #region Members

        /// <summary>
        ///     Lock
        ///     Locks this object to prevent multithreading problems
        /// </summary>
        private readonly object _lock = new object();

        #endregion Members

        #region Destructor

        /// <summary>
        ///     Finalizes an instance of the <see cref="FileReader" /> class.
        /// </summary>
        ~FileReader()
        {
            if (!IsDisposed) Dispose();
        }

        #endregion Destructor

        #region Properties

        /// <summary>
        ///     File Path
        ///     Represents the path to the file that is beeing buffered
        /// </summary>
        public string FilePath { get; private set; }

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

        /// <summary>
        ///     Offset
        ///     This is the point between file begin and length
        /// </summary>
        public long Offset { get; private set; }

        /// <summary>
        ///     Length
        ///     Represents the length of the buffer, that will be written
        /// </summary>
        public long Length { get; private set; }

        /// <summary>
        ///     The data of the file between offset and length
        /// </summary>
        public byte[] Buffer { get; private set; }

        #endregion Properties

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileReader" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <exception cref="System.ArgumentNullException">File</exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public FileReader(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) //The string is null or empty? -> Throw exception
                throw new ArgumentNullException("filePath");

            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            FilePath = filePath;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileReader" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="offset">The offset.</param>
        /// <exception cref="System.ArgumentNullException">File</exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public FileReader(string filePath, long offset)
        {
            if (string.IsNullOrEmpty(filePath)) //The string is null or empty? -> Throw exception
                throw new ArgumentNullException("filePath");

            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            Offset = offset;
            FilePath = filePath;

            Buffer = GetBuffer();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileReader" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <exception cref="System.ArgumentNullException">File</exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public FileReader(string filePath, long offset, long length)
        {
            if (string.IsNullOrEmpty(filePath)) //The string is null or empty? -> Throw exception
                throw new ArgumentNullException("filePath");

            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            Offset = offset;
            Length = length;
            FilePath = filePath;

            Buffer = GetBuffer();
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        ///     Dispose
        ///     Disposes the current object and releases its data
        ///     Exceptions:
        ///     ObjectDisposedException
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed || IsDisposing)
                throw new ObjectDisposedException(ToString());

            lock (_lock) //Prevent thread issues
            {
                IsDisposing = true;

                Buffer = null;
                FilePath = null;
                Length = 0;
                Offset = 0;

                IsDisposed = true;
                IsDisposing = false;
            }
        }

        /// <summary>
        ///     Gets the buffer.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.ObjectDisposedException">FileBuffer</exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        private byte[] GetBuffer()
        {
            if (IsDisposed || IsDisposing)
                throw new ObjectDisposedException("FileBuffer");

            if (!File.Exists(FilePath))
                throw new FileNotFoundException(FilePath);

            lock (_lock)
            {
                var buffer = new byte[Length]; //temporary byte[] containing the data of the file part

                using (var fStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                {
                    fStream.Position = Offset;
                    fStream.Read(buffer, 0, (int) Length);
                }

                return buffer;
            }
        }

        /// <summary>
        ///     Reads the bytes.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        /// <exception cref="System.ObjectDisposedException">FileBuffer</exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public byte[] ReadBytes(long offset, long length)
        {
            if (IsDisposed || IsDisposing)
                throw new ObjectDisposedException("FileBuffer");

            if (!File.Exists(FilePath))
                throw new FileNotFoundException(FilePath);

            lock (_lock)
            {
                Buffer = null; //Reset buffer to null
                Offset = offset;
                Length = length;
            }

            Buffer = GetBuffer(); //Get the buffer bytes

            return Buffer;
        }

        #endregion Methods
    }
}