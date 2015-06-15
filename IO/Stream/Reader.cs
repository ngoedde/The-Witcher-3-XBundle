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

using System.IO;

namespace XBundle.IO.Stream
{
    internal class Reader : BinaryReader
    {
        #region Constructor

        /// <summary>
        ///     Constructor - Simple
        ///     Initializes a new binary reader from a stream
        /// </summary>
        /// <param name="buffer">Buffer to read from</param>
        public Reader(byte[] buffer)
            : base(new MemoryStream(buffer, false))
        {
        }

        /// <summary>
        ///     Constructor - Advanced
        ///     Initializes a new binary reader from a stream
        /// </summary>
        /// <param name="buffer">Buffer to read from</param>
        /// <param name="offset">Where to start</param>
        /// <param name="length">Where to end</param>
        public Reader(byte[] buffer, int offset, int length)
            : base(new MemoryStream(buffer, offset, length, false))
        {
        }

        #endregion Constructor
    }
}