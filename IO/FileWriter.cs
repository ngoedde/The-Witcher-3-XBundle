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
    internal class FileWriter : IDisposable
    {
        #region Members

        /// <summary>
        ///     Stream
        ///     This is the stream that is used to write to a file
        /// </summary>
        private FileStream _stream;

        #endregion Members

        #region Constructor

        /// <summary>
        ///     File Writer
        ///     Constructor which points to a file
        ///     Exceptions:
        ///     FileNotFoundException
        ///     ArgumentNullException
        /// </summary>
        /// <param name="path">The path to the file to write to</param>
        public FileWriter(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            FilePath = path;
        }

        #endregion Constructor

        #region Properties

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
        ///     File Path
        ///     This is the path to the file which is written to
        /// </summary>
        public string FilePath { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        ///     Writes the bytes to the file.
        ///     Uses a heap in order to save disc traffic. Ye 
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="heapSize">Size of the heap.</param>
        /// <returns></returns>
        public void Write(byte[] data, long offset, int heapSize = 10485760)
        {
            if (data == null || data.Length == 0)
                return;

            using (_stream = new FileStream(FilePath, FileMode.Open))
            {
                if (offset > _stream.Length)
                    return;

                //Most of these are neccessary to calculate the progress percentage, the others are stream offsets!
                var bytesLeft = data.Length;
                var heapOffset = 0;

                //Set the current position in the file depending to the current heap offset
                _stream.Position = offset + heapOffset;

                while (bytesLeft != 0)
                {
                    if (bytesLeft < heapSize)
                        heapSize = bytesLeft;

                    //Write to the file
                    _stream.Write(data, heapOffset, heapSize);

                    //Update values
                    heapOffset += heapSize;
                    bytesLeft -= heapSize;
                }
            }
        }

        /// <summary>
        ///     Moves an array of bytes from one point of the file to another one.
        ///     If there are byte at the destination location, they will be overwritten
        /// </summary>
        /// <param name="sourceOffset">Where to start</param>
        /// <param name="sourceLength">Wher to place them</param>
        /// <param name="destinationOffset"></param>
        /// <param name="heapSize">Amount of bytes beeing transfaired by one step</param>
        /// <returns>Wether the process was successfull or not</returns>
        public void MoveBytes(long sourceOffset, long sourceLength, long destinationOffset, int heapSize = 10485760)
        {
            var currentOffset = sourceOffset;
            var currentDestinationOffset = destinationOffset;

            var bytesLeft = sourceLength;

            while (bytesLeft != 0)
            {
                if (bytesLeft < heapSize)
                    heapSize = (int) bytesLeft;

                byte[] heapBuffer;
                using (var fBuffer = new FileReader(FilePath))
                {
                    heapBuffer = fBuffer.ReadBytes(currentOffset, heapSize); //Read the next stack from file
                    currentOffset += heapSize;
                    bytesLeft -= heapSize;
                }

                Write(heapBuffer, (int) currentDestinationOffset);
                currentDestinationOffset += heapSize;
            }
        }

        /// <summary>
        ///     Inserts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        public void Insert(byte[] data, int offset)
        {
            long oldFileLength;

            using (_stream = new FileStream(FilePath, FileMode.Open))
                oldFileLength = _stream.Length;

            Extend(data.Length, oldFileLength);
            MoveBytes(offset, data.Length, oldFileLength - offset);
            Write(data, offset);
        }

        /// <summary>
        ///     Extends the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        public void Extend(int size)
        {
            var zeroData = new byte[size];
            Write(zeroData, Globals.HeaderInfo.Filesize);
        }

        /// <summary>
        ///     Extends the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="fileSize">If the size of the file is known and does not need to be queried again</param>
        public void Extend(int size, long fileSize)
        {
            var zeroData = new byte[size];
            Write(zeroData, fileSize);
        }

        /// <summary>
        ///     Deletes data from the file using an offset and length.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        public void Cut(int offset, int length)
        {
            long copyStart, copyLength;

            using (_stream = new FileStream(FilePath, FileMode.Open))
            {
                if (offset > _stream.Length)
                    return;

                //Calculate vars
                copyStart = offset + length; //Overjump the bytes to cut
                copyLength = _stream.Length - copyStart; //From start to end of file
            }

            MoveBytes(copyStart, copyLength, offset); //Start to move that.
        }

        /// <summary>
        ///     Disposes this object and releases all resources
        /// </summary>
        /// <exception cref="System.ObjectDisposedException"></exception>
        public void Dispose()
        {
            if (IsDisposed || IsDisposing)
                throw new ObjectDisposedException("FileWriter");

            IsDisposing = true;
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
            FilePath = null;

            IsDisposing = false;
            IsDisposed = true;
        }

        #endregion Methods
    }
}