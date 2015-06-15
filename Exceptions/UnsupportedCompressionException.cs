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

namespace XBundle.Exceptions
{
    public class UnsupportedCompressionException : Exception
    {
        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileNotFoundException" /> class.
        /// </summary>
        public UnsupportedCompressionException(uint compressionType)
        {
            CompressionType = compressionType;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        ///     Gets the type of the compression.
        /// </summary>
        /// <value>
        ///     The type of the compression.
        /// </value>
        public uint CompressionType { get; private set; }

        /// <summary>
        ///     Gets a message that describes the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return
                    "While trying to decompress the requested data, the application detected the unknown compression type [" +
                    CompressionType.ToString("X") + "]";
            }
        }

        #endregion Properties
    }
}