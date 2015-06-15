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
using System.IO;
using System.Linq;
using XBundle.Exceptions;
using XBundle.IO;
using XBundle.Utilities;
using FileNotFoundException = XBundle.Exceptions.FileNotFoundException;

namespace XBundle.Handler
{
    public class FileHandler
    {
        #region Methods

        /// <summary>
        ///     Indicates wether the file at the requested location exists or not.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public bool Exists(string path)
        {
            var tempFileInfo = (from file in Globals.Index where file.Path == path select file).FirstOrDefault();

            return tempFileInfo != null;
        }

        /// <summary>
        ///     Gets the specified path. Returns null if the file does not exist.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public FileInfo Get(string path)
        {
            return (from file in Globals.Index where file.Path == path select file).FirstOrDefault();
        }

        /// <summary>
        ///     Gets the paths of all files that have been found within the archive.
        /// </summary>
        /// <returns></returns>
        public string[] GetAllFiles()
        {
            return (from file in Globals.Index select file.Path).ToArray();
        }

        /// <summary>
        ///     Gets all files that exist in the root folder of the archive (where path like \file.ext)
        /// </summary>
        /// <returns></returns>
        public string[] GetRootFiles()
        {
            return (from file in Globals.Index where file.Parent == string.Empty select file.Path).ToArray();
        }

        /// <summary>
        ///     Gets an array of file paths that a specific folder contain.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <returns></returns>
        public string[] GetFiles(string folderPath)
        {
            return (from file in Globals.Index where file.Parent == folderPath select file.Path).ToArray();
        }

        /// <summary>
        ///     Reads all bytes from a specific file and decompresses it if required.
        ///     Attention, currently only Zip compression is being supported!
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="decompress">if set to <c>true</c> [decompress].</param>
        /// <returns></returns>
        /// <exception cref="Exceptions.FileNotFoundException"></exception>
        public byte[] ReadAllBytes(string path, bool decompress = true)
        {
            if (!Exists(path))
                throw new FileNotFoundException(path);

            var file = Get(path);

            //Just in case, the file could not be parsed correctly somehow (shouldn't realy happen)
            if (file == null) throw new FileNotFoundException(path);

            byte[] data;
            using (var reader = new FileReader(Globals.Filepath))
                data = reader.ReadBytes(file.DataOffset, file.CompressedSize);

            return decompress == false ? data : CompressionHandler.GetCompressor(file.Compression).Decompress(data, file.UncompressedSize);
        }

        /// <summary>
        /// Finds files by the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string[] Find(string name)
        {
            return (from file in Globals.Index where file.Name.Contains(name) select file.Path).ToArray();
        }

        /// <summary>
        ///     Creates the specified data.
        ///     Compression:
        ///     0 = none
        ///     1 = ZIP
        ///     2&3 = Doboz
        ///     4&5 = LZ4
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="path">The path.</param>
        /// <param name="compression">The compression.</param>
        /// <param name="overwrite">If set to true, an existing file will be overwritten automaticly</param>
        /// <exception cref="FileAlreadyExistsException"></exception>
        public void Create(byte[] data, string path, bool overwrite = false, uint compression = 0)
        {
            if (Exists(path) && !overwrite) //The file exists and should not be overwritten
                throw new FileAlreadyExistsException(path);
            if (Exists(path) && overwrite) //The file exists and should be overwritten
            {
                OverwriteFile(data, path);
                return;
            }

            //Okay, the file does not exist at the desired destination, now create a completly new file. ->

            var hash = IdGenerator.GenerateHashBytes(); //Just anything?
            var uniqueId = IdGenerator.GenerateUid(); //Just anything?

            //Initializes a new instance of FileInfo with the currently known values
            var info = new FileInfo
            {
                Path = path,
                UncompressedSize = (uint)data.Length,
                Timestamp = DateTime.Now,
                Compression = compression,
                Zero = new byte[16],
                Hash = hash,
                Dummy0 = 0,
                ChainPosition = Globals.HeaderInfo.Indexsize + Constants.HeaderSize,
                //The end of the index +32 as header size
                UniqueId = uniqueId,

                //Now it gets tricky. Since the new file size have not been calculated yet, we have to guess it at this point. The below calculation
                //says, that the file should be at the absolute end of the file (the current file size + the chain, we are generating below).
                DataOffset = Globals.HeaderInfo.Filesize + Constants.ChainLength
            };

            //Simply compress the data
            var compressedData = CompressionHandler.GetCompressor(info.Compression).Compress(data);

            //Now we can set the compressed size information
            info.CompressedSize = (uint)compressedData.Length;

            var chainData = ChainGenerator.CompileFileChain(info);
            using (var writer = new FileWriter(Globals.Filepath))
            {
                //Insert the new chain at the end of the index
                writer.Insert(chainData, (int)Globals.HeaderInfo.Indexsize + Constants.HeaderSize);

                //Now we know the new sizes:
                Globals.HeaderInfo.Indexsize += Constants.ChainLength;
                Globals.HeaderInfo.Filesize += Constants.ChainLength;

                //Now we have to write the data to the end of the file
                writer.Write(compressedData, Globals.HeaderInfo.Filesize);

                //And we have to update the header again!
                Globals.HeaderInfo.Filesize += (uint)compressedData.Length;

                //Now we write the new header to the file
                var headerData = Globals.HeaderInfo.ToByteArray();
                writer.Write(headerData, 0, Constants.HeaderSize);

                //After we have inserted additional 320B for the chain, we have to update the other chains data offsets.
                foreach (var file in Globals.Index)
                {
                    file.DataOffset += Constants.ChainLength;

                    var newDataOffset = BitConverter.GetBytes(file.DataOffset);

                    writer.Write(newDataOffset, file.DataOffsetPosition);
                }
            }

            //Everything is done, we can add the new file to the index.
            Globals.Index.Add(info);

            //Recreates the index for the folder structure
            Globals.Directories.RecreateIndex();
        }

        /// <summary>
        ///     Renames the specified file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="newName">The new name.</param>
        public void Rename(string path, string newName)
        {
            if (!Exists(path))
                throw new FileNotFoundException(path);

            var info = (from file in Globals.Index where file.Path == path select file).FirstOrDefault();

            //Should not happen, because we have checked it earlier but better be sure, before any kind of NullReferenceException occurs
            if (info == null) throw new FileNotFoundException(path);

            var newPath = info.Parent + "\\" + newName;

            if (Exists(newPath))
                throw new FileAlreadyExistsException(newPath);

            //Convert the filepath to byte[] (automaticaly extended to 256B!)
            var pathByteArray = ChainGenerator.PathToByteArray(newPath);

            using (var writer = new FileWriter(Globals.Filepath))
                writer.Write(pathByteArray, info.ChainPosition);

            info.Path = newPath;

            //Recreates the index for the folder structure
            Globals.Directories.RecreateIndex();
        }

        /// <summary>
        ///     This will overwrite an existing file. Information like hash and id will stay the same afterwards
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        private void OverwriteFile(byte[] data, string path)
        {
            if (!Exists(path))
                throw new FileNotFoundException(path);

            var info = Get(path);

            //Temporarily save the old values (used later)
            var oldDataOffset = info.DataOffset;
            var oldCompressedSize = info.CompressedSize;

            var compressedData = CompressionHandler.GetCompressor(info.Compression).Compress(data);

            //Update the FileInfo in info object
            info.UncompressedSize = (uint)data.Length;
            info.CompressedSize = (uint)compressedData.Length;

            //The old size matches the new size.. simply overwrite the existing data
            if (oldCompressedSize == info.CompressedSize || info.CompressedSize < oldCompressedSize)
            {
                //Compile the new chain
                var chainData = ChainGenerator.CompileFileChain(info);
                using (var writer = new FileWriter(Globals.Filepath))
                {
                    writer.Write(chainData, info.ChainPosition, Constants.ChainLength); //simply overwrite old chain
                    writer.Write(compressedData, info.DataOffset, (int)info.CompressedSize);
                    //simply overwrite old data
                }

                //That's it, nothing else require updates.
                return;
            }

            //It's not that easy.
            //Step 1: Overwrite the current chain with the new data, nothing else like offsets chainge at that step.
            //Step 2: Overwrite the old file's data with 00 00 {...}. Still, nothing else changes
            //Step 3: Append the new data to the end of the file. The filesize itself changes now, but that's it.
            //Step 4: Overwrite the header with the new filesize
            using (var writer = new FileWriter(Globals.Filepath))
            {
                info.DataOffset = Globals.HeaderInfo.Filesize;

                //overwrite old file with 0
                var zero = new byte[oldCompressedSize];
                writer.Write(zero, oldDataOffset);

                //write new data to the end of the file
                writer.Write(compressedData, info.DataOffset);

                //overwrite old chain
                var chainData = ChainGenerator.CompileFileChain(info);
                writer.Write(chainData, info.ChainPosition, Constants.ChainLength);

                //update header
                Globals.HeaderInfo.Filesize -= oldCompressedSize;
                Globals.HeaderInfo.Filesize += info.CompressedSize;

                //Write the header
                var newHeaderData = Globals.HeaderInfo.ToByteArray();
                writer.Write(newHeaderData, 0, Constants.HeaderSize);
            }
        }

        /// <summary>
        ///     Moves a file from a source to a new destination.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="overwrite"></param>
        public void Move(string path, string destination, bool overwrite = false)
        {
            if (!Exists(path))
                return;

            if (Exists(destination) && !overwrite)
                throw new FileAlreadyExistsException(destination);
            if (Exists(destination) && overwrite)
            {
                //Overwrite the destination with the current data
                OverwriteFile(ReadAllBytes(path), destination);

                //At least, delete the old file
                Delete(path);
            }
            else
            {
                var info = (from file in Globals.Index where file.Path == path select file).FirstOrDefault();
                if (info == null) throw new FileNotFoundException(path);
                //Get the new folderpath as byte array
                var pathByteArray = ChainGenerator.PathToByteArray(destination);
                using (var writer = new FileWriter(Globals.Filepath))
                    writer.Write(pathByteArray, info.ChainPosition, Constants.ChainLength);

                info.Path = destination;
            }

            //Recreates the index for the folder structure
            Globals.Directories.RecreateIndex();
        }

        /// <summary>
        ///     Deletes the specified paht.
        /// </summary>
        /// <param name="path">The path.</param>
        public void Delete(string path)
        {
            if (!Exists(path))
                throw new FileNotFoundException(path);

            var info = Get(path);

            //Step 1: Cut the old chain and data
            using (var writer = new FileWriter(Globals.Filepath))
            {
                writer.Cut((int)info.ChainPosition, Constants.ChainLength); //Cut the old chain from the archive!

                Globals.HeaderInfo.Filesize -= Constants.ChainLength;
                Globals.HeaderInfo.Indexsize -= Constants.ChainLength;

                writer.Cut((int)info.DataOffset - Constants.ChainLength, (int)info.CompressedSize);
                //Cut the old file

                Globals.HeaderInfo.Filesize -= info.CompressedSize;
            }

            //Step 2: Every chain, that follow the chain we deleted, has moved "left" by 320B, therefore we need to update them
            var chainsThatRequirePositionUpdate =
                (from chain in Globals.Index where chain.ChainPosition > info.ChainPosition select chain).ToList();
            foreach (var chainInfo in chainsThatRequirePositionUpdate)
                chainInfo.ChainPosition -= Constants.ChainLength;

            //Step 3: The WHOLE file data <see cref="HeaderInfo.DataOffset"> has moved left by 320B, we need to update the offsets
            foreach (var chainInfo in Globals.Index)
            {
                chainInfo.DataOffset -= Constants.ChainLength;

                using (var writer = new FileWriter(Globals.Filepath))
                {
                    var newDataOffset = BitConverter.GetBytes(chainInfo.DataOffset);
                    writer.Write(newDataOffset, chainInfo.DataOffsetPosition, 4);
                }
            }

            //Step 4: Since we deleted the old file, we need to update the data offsets of the filedata that were behind the one that we deleted
            var chainsThatRequireDataOffsetUpdate =
                (from chain in Globals.Index where chain.DataOffset > info.DataOffset select chain).ToList();
            foreach (var nfo in chainsThatRequireDataOffsetUpdate)
            {
                nfo.DataOffset -= info.CompressedSize;

                using (var writer = new FileWriter(Globals.Filepath))
                {
                    var newDataOffset = BitConverter.GetBytes(nfo.DataOffset);
                    writer.Write(newDataOffset, nfo.DataOffsetPosition);
                }
            }

            Globals.Index.Remove(info);

            using (var writer = new FileWriter(Globals.Filepath))
                writer.Write(Globals.HeaderInfo.ToByteArray(), 0, Constants.HeaderSize);

            //Recreates the index for the folder structure
            Globals.Directories.RecreateIndex();
        }

        /// <summary>
        ///     Exports the specified files.
        ///     It will skip files that do not exist in the archive instead of throwing an exception.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="outputFolder">The output folder.</param>
        /// <param name="decompress"></param>
        public void Export(string[] files, string outputFolder, bool decompress = true)
        {
            foreach (var file in files)
            {
                if (!Exists(file)) continue;
                var fileData = ReadAllBytes(file, decompress);
                File.WriteAllBytes(outputFolder + "\\" + Path.GetFileName(file), fileData);
            }
        }

        /// <summary>
        ///     Exports the specified file.
        /// </summary>
        /// <param name="path">The files.</param>
        /// <param name="outputFolder">The output folder.</param>
        /// <param name="decompress"></param>
        public void Export(string path, string outputFolder, bool decompress = true)
        {
            if (!Exists(path))
                throw new FileNotFoundException(path);

            var fileData = ReadAllBytes(path, decompress);
            File.WriteAllBytes(outputFolder + "\\" +Path.GetFileName(path), fileData);
        }

        #endregion Methods
    }
}