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

namespace XBundle
{
    public class FileInfo
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the path.
        /// </summary>
        /// <value>
        ///     The path.
        /// </value>
        public string Path { get; internal set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name
        {
            get { return System.IO.Path.GetFileName(Path); }
        }

        /// <summary>
        ///     Gets the parent.
        /// </summary>
        /// <value>
        ///     The parent.
        /// </value>
        public string Parent
        {
            get { return System.IO.Path.GetDirectoryName(Path); }
        }

        /// <summary>
        ///     Gets or sets the chain position.
        /// </summary>
        /// <value>
        ///     The chain position.
        /// </value>
        public uint ChainPosition { get; internal set; }

        /// <summary>
        ///     Gets or sets the timestamp.
        /// </summary>
        /// <value>
        ///     The timestamp.
        /// </value>
        public byte[] Hash { get; internal set; }

        /// <summary>
        ///     Gets or sets the empty data.
        /// </summary>
        /// <value>
        ///     UNKNOWN (always 0000000[...])
        /// </value>
        public uint Empty { get; internal set; }

        /// <summary>
        ///     Gets or sets the size when the file is not compressed.
        /// </summary>
        /// <value>
        ///     The size.
        /// </value>
        public uint UncompressedSize { get; set; }

        /// <summary>
        ///     Gets or sets the size, while the file is compressed.
        /// </summary>
        /// <value>
        ///     The empty size.
        /// </value>
        public uint CompressedSize { get; set; }

        /// <summary>
        ///     Gets or sets the data offset.
        /// </summary>
        /// <value>
        ///     The data offset.
        /// </value>
        public uint DataOffset { get; set; }

        /// <summary>
        ///     Gets or sets the timestamp.
        /// </summary>
        /// <value>
        ///     The timestamp.
        /// </value>
        public DateTime Timestamp { get; set; }

        /// <summary>
        ///     Gets or sets the zero.
        /// </summary>
        /// <value>
        ///     The zero.
        /// </value>
        public byte[] Zero { get; set; }

        /// <summary>
        ///     Gets or sets the dummy0.
        /// </summary>
        /// <value>
        ///     The dumm y0.
        /// </value>
        public uint Dummy0 { get; set; }

        /// <summary>
        ///     Gets or sets the compression.
        /// </summary>
        /// <value>
        ///     The compression.
        /// </value>
        public uint Compression { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        ///     The unique identifier.
        /// </value>
        public uint UniqueId { get; set; }

        /// <summary>
        ///     Gets a value that points to the absolute position where da file data offset is stored. (ChainPosition + 284B)
        /// </summary>
        /// <value>
        ///     Offset of the data offset in the chain
        /// </value>
        internal uint DataOffsetPosition
        {
            get { return ChainPosition + 284; }
        }

        #endregion Properties
    }
}