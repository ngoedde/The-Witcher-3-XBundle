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

using System.Text;
using XBundle.IO.Stream;

namespace XBundle.Utilities
{
    internal class ChainGenerator
    {
        #region Methods

        /// <summary>
        ///     Compiles the file chain.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static byte[] CompileFileChain(FileInfo file)
        {
            using (var writer = new StreamController(null, StreamOperation.Write))
            {
                //Convert the path to a byte[] with the perfect length of 256
                writer.WriteByteArray(PathToByteArray(file.Path));

                writer.WriteByteArray(file.Hash);
                writer.WriteUInt(file.Empty);
                writer.WriteUInt(file.UncompressedSize);
                writer.WriteUInt(file.CompressedSize);
                writer.WriteUInt(file.DataOffset);
                writer.WriteLong(file.Timestamp.ToFileTime());
                writer.WriteByteArray(file.Zero); //000000000..
                writer.WriteUInt(file.UniqueId);
                writer.WriteUInt(file.Compression);

                return writer.GetWriterBytes();
            }
        }

        /// <summary>
        ///     Creates a byte array from the given path. Automaticly adjusts the array size.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        internal static byte[] PathToByteArray(string path)
        {
            var finalPath = path;

            for (var i = finalPath.Length; i < Constants.PathLength; i++)
            {
                finalPath += "\0"; //Extend the path to 256 characters
            }

            return (Encoding.ASCII.GetBytes(finalPath));
        }

        #endregion Methods
    }
}