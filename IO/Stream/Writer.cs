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
    internal class Writer : BinaryWriter
    {
        #region Members

        /// <summary>
        ///     Stream
        ///     Represents the stream this class uses for writing stuff
        /// </summary>
        private readonly MemoryStream m_Stream;

        #endregion Members

        #region Methods

        /// <summary>
        ///     Get Bytes
        ///     Returns the stream as buffer array
        /// </summary>
        /// <returns>Buffer</returns>
        public byte[] GetBuffer()
        {
            return m_Stream.ToArray();
        }

        #endregion Methods

        #region Constructor

        /// <summary>
        ///     Constructor - Simple
        ///     Initializes this class and that's it.
        /// </summary>
        public Writer()
        {
            m_Stream = new MemoryStream();
            OutStream = m_Stream;
        }

        /// <summary>
        ///     Constructor - Advanced
        ///     Initializes this class including a buffer
        /// </summary>
        /// <param name="buffer">Data to write to</param>
        public Writer(byte[] buffer)
        {
            m_Stream = new MemoryStream(buffer);
            OutStream = m_Stream;
        }

        #endregion Constructor
    }
}