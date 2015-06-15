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
using ZLibNet;

namespace XBundle.Compression
{
    internal class Zip : ICompression
    {
        #region Methods

        /// <summary>
        /// Decompresses the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="uncompressedSize">Size of the uncompressed. This value is not required for this type of compression.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public byte[] Decompress(byte[] data, uint uncompressedSize = 0)
        {
            return ZLibCompressor.DeCompress(data);
        }

        /// <summary>
        /// Compresses the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public byte[] Compress(byte[] data)
        {
            return ZLibCompressor.Compress(data);
        }

        #endregion Methods

        #region Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public string Name
        {
            get { return "ZIP"; }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>
        ///     The type.
        /// </value>
        /// <exception cref="System.NotImplementedException">
        /// </exception>
        public uint[] Types
        {
            get { return new uint[] { 0x01 }; }
            set { throw new NotImplementedException(); }
        }

        #endregion Properties
    }
}