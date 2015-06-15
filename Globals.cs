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

using System.Collections.Generic;
using XBundle.Handler;

namespace XBundle
{
    internal class Globals
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the index.
        /// </summary>
        /// <value>
        ///     The index.
        /// </value>
        public static List<FileInfo> Index { get; set; }

        /// <summary>
        ///     Gets or sets the filepath.
        /// </summary>
        /// <value>
        ///     The filepath.
        /// </value>
        public static string Filepath { get; set; }

        /// <summary>
        ///     Gets or sets the header.
        ///     This is required for the real-time file import!
        /// </summary>
        /// <value>
        ///     The header.
        /// </value>
        public static HeaderInfo HeaderInfo { get; set; }

        /// <summary>
        ///     Gets or sets the files.
        /// </summary>
        /// <value>
        ///     The files.
        /// </value>
        public static FileHandler Files { get; set; }

        /// <summary>
        ///     Gets or sets the directories.
        /// </summary>
        /// <value>
        ///     The directories.
        /// </value>
        public static DirectoryHandler Directories { get; set; }

        #endregion Properties
    }
}