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

using XBundle.IO.Stream;

namespace XBundle
{
    public class HeaderInfo
    {
        #region Methods

        /// <summary>
        ///     To the byte array.
        /// </summary>
        /// <returns></returns>
        internal byte[] ToByteArray()
        {
            using (var writer = new StreamController(null, StreamOperation.Write))
            {
                writer.WriteFixedString(MagicNumbers); //POTATO70
                writer.WriteUInt(Filesize);
                writer.WriteUInt(DummySize);
                writer.WriteUInt(Indexsize);
                writer.WriteUInt(Unknown);
                writer.WriteByteArray(Attributes);
                return writer.GetWriterBytes();
            }
        }

        #endregion Methods

        #region Properties

        /// <summary>
        ///     Gets the dummy size
        /// </summary>
        /// <value>
        /// </value>
        public uint DummySize { get; internal set; }

        /// <summary>
        ///     Gets the unknown.
        /// </summary>
        /// <value>
        ///     The unknown.
        /// </value>
        public uint Unknown { get; internal set; }

        /// <summary>
        ///     Gets or sets the magic numbers.
        /// </summary>
        /// <value>
        ///     The magic numbers.
        /// </value>
        public string MagicNumbers { get; internal set; }

        /// <summary>
        ///     Gets the indexsize.
        /// </summary>
        /// <value>
        ///     The indexsize.
        /// </value>
        public uint Indexsize { get; internal set; }

        /// <summary>
        ///     Gets the filesize.
        /// </summary>
        /// <value>
        ///     The filesize.
        /// </value>
        public uint Filesize { get; internal set; }

        /// <summary>
        ///     Gets the filecount.
        /// </summary>
        /// <value>
        ///     The filecount.
        /// </value>
        public uint Filecount
        {
            get { return Indexsize/Constants.ChainLength; }
        }

        /// <summary>
        ///     Gets the datasize.
        /// </summary>
        /// <value>
        ///     The datasize.
        /// </value>
        public uint Datasize
        {
            get { return Filesize - Indexsize - Constants.HeaderSize; }
        }

        /// <summary>
        ///     Gets the unknown attributes.
        /// </summary>
        /// <value>
        ///     The unknown attributes.
        /// </value>
        public byte[] Attributes { get; internal set; }

        /// <summary>
        ///     Gets the data offset.
        /// </summary>
        /// <value>
        ///     The data offset.
        /// </value>
        public uint DataOffset
        {
            get { return Indexsize + Constants.HeaderSize; }
        }

        #endregion Properties
    }
}