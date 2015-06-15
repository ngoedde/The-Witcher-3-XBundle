/*Copyright Message
* XBundle
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
* @category  XBundle
* @copyright  Copyright (c) 2014, ambax Software UG (http://www.ambax.de)
* @author ambax Software UG
*/

using System;
using System.Collections.Generic;
using System.IO;
using XBundle.Compression;
using XBundle.Exceptions;
using XBundle.Handler;
using XBundle.IO;
using XBundle.IO.Stream;
using FileNotFoundException = System.IO.FileNotFoundException;

namespace XBundle
{
    public class BundleArchive
    {
        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="BundleArchive" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public BundleArchive(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            //Setup internal variables
            Globals.Filepath = path;
            Globals.Index = new List<FileInfo>();
            Globals.Filepath = path;
            Globals.HeaderInfo = ReadHeader();

            //Read out the chains
            uint currentOffset = Constants.HeaderSize;
            for (var i = 0; i < HeaderInfo.Filecount; i++)
            {
                ReadChain(currentOffset);
                currentOffset += Constants.ChainLength;
            }

            Globals.Directories = new DirectoryHandler();
            Globals.Files = new FileHandler();

            //Registers the dafault compressors to the application
            RegisterDefaultCompressors();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        ///     Gets or sets the files.
        /// </summary>
        /// <value>
        ///     The files.
        /// </value>
        public FileHandler Files
        {
            get { return Globals.Files; }
        }

        public DirectoryHandler Directories
        {
            get { return Globals.Directories; }
        }

        /// <summary>
        ///     Gets the header.
        /// </summary>
        /// <value>
        ///     The header.
        /// </value>
        public HeaderInfo HeaderInfo
        {
            get { return Globals.HeaderInfo; }
        }

        /// <summary>
        ///     Gets the filepath.
        /// </summary>
        /// <value>
        ///     The filepath.
        /// </value>
        public string Filepath
        {
            get { return Globals.Filepath; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <returns></returns>
        public List<FileInfo> GetIndex()
        {
            return Globals.Index;
        }

        /// <summary>
        ///     Registers the default compressors.
        /// </summary>
        private void RegisterDefaultCompressors()
        {
            CompressionHandler.RegisterCompressor(new Default());
            CompressionHandler.RegisterCompressor(new Lz4());
            CompressionHandler.RegisterCompressor(new Compression.Doboz());
            CompressionHandler.RegisterCompressor(new Zip());
        }

        /// <summary>
        ///     Reads the header.
        /// </summary>
        private HeaderInfo ReadHeader()
        {
            byte[] headerData;
            using (var reader = new FileReader(Filepath))
                headerData = reader.ReadBytes(0L, Constants.HeaderSize);

            using (var streamReader = new StreamController(headerData, StreamOperation.Read))
            {
                //Predefinition of the result
                var result = new HeaderInfo { MagicNumbers = streamReader.ReadStringByLength(8) };

                if (result.MagicNumbers != "POTATO70")
                    throw new InvalidArchiveException(Globals.Filepath);

                //Read the actual header
                result.Filesize = streamReader.ReadUInt();
                result.DummySize = streamReader.ReadUInt();
                result.Indexsize = streamReader.ReadUInt();
                result.Unknown = streamReader.ReadUInt();

                result.Attributes = streamReader.ReadByteArray(8);

                return result;
            }
        }

        /// <summary>
        ///     Reads the chain at the given offset.
        /// </summary>
        /// <param name="offset">The offset of the chain</param>
        /// <returns></returns>
        private void ReadChain(uint offset)
        {
            byte[] chainData;
            using (var reader = new FileReader(Filepath))
                chainData = reader.ReadBytes(offset, Constants.ChainLength);

            using (var streamReader = new StreamController(chainData, StreamOperation.Read))
            {
                var file = new FileInfo
                {
                    ChainPosition = offset,
                    //Just the file's path.
                    Path = streamReader.ReadStringByLength(Constants.PathLength).Trim('\0'),
                    //it's none of the known HASH algorythms, also, the patch.bundle has just 00 00[...] in this part!
                    Hash = streamReader.ReadByteArray(16),
                    //00 00 00 00
                    Empty = streamReader.ReadUInt(),
                    //The actual, uncompressed and plain filesize
                    UncompressedSize = streamReader.ReadUInt(),
                    //The size, the file takes in the archive (compressed). If the file is uncompressed, it equals the UncompressedSize value.
                    CompressedSize = streamReader.ReadUInt(),
                    //The absolute offset of the data
                    DataOffset = streamReader.ReadUInt(),
                    //Some weird timestamp (IDK if it's really a timestamp, need to check this later!)
                    Timestamp = DateTime.FromFileTime(streamReader.ReadLong()),
                    //00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 (always, in every archive)
                    Zero = streamReader.ReadByteArray(16),
                    //Depending on the .bundle archive, this is either 0 (patch.bundle) or a random value
                    UniqueId = streamReader.ReadUInt(),
                    //The type of the compression
                    Compression = streamReader.ReadUInt()
                };
                Globals.Index.Add(file);
            }
        }

        #endregion Methods
    }
}