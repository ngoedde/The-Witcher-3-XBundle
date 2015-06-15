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

namespace XBundle.Compression
{
    public interface ICompression
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the valid types. (All the types that should also handle this compression)
        /// </summary>
        /// <value>
        /// The valid types.
        /// </value>
        uint[] Types { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Decompresses the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="uncompressedSize">Size of the uncompressed.</param>
        /// <returns></returns>
        byte[] Decompress(byte[] data, uint uncompressedSize = 0);

        /// <summary>
        /// Compresses the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        byte[] Compress(byte[] data);

        #endregion

    }
}
