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
using System.IO;
using System.Linq;

namespace XBundle.Handler
{
    public class DirectoryHandler
    {
        #region Internals

        /// <summary>
        ///     Gets or sets all folders.
        ///     This is required to reduce the calculation time of the get all folders function!!!
        /// </summary>
        /// <value>
        ///     All folders.
        /// </value>
        internal string[] DirectoryIndex { get; set; }

        #endregion Internals

        #region Methods

        /// <summary>
        ///     Gets all folders.
        /// </summary>
        /// <returns></returns>
        public string[] GetAllDirectories()
        {
            if (DirectoryIndex == null)
                RecreateIndex();

            return DirectoryIndex;
        }

        /// <summary>
        ///     Recreates the index.
        /// </summary>
        internal void RecreateIndex()
        {
            var result = new List<string>();
            foreach (var file in Globals.Files.GetAllFiles())
            {
                var splits = file.Split(Constants.PathSeperator);
                var lastSplit = splits[splits.Length - 1];

                var newPath = "";
                foreach (var split in splits.Where(split => !string.IsNullOrEmpty(split) && split != lastSplit))
                {
                    newPath += Constants.PathSeperator + split;
                    if (newPath.StartsWith("\\"))
                        newPath = newPath.Substring(1, newPath.Length - 1);
                    if (!result.Contains(newPath))
                        result.Add(newPath);
                }
            }

            DirectoryIndex = result.ToArray();
        }

        /// <summary>
        ///     Gets the folders.
        /// </summary>
        /// <param name="parentFolder">The parent folder.</param>
        /// <returns></returns>
        public string[] GetSubDirectories(string parentFolder)
        {
            if (parentFolder == "")
                return new string[0];

            var result = GetAllDirectories();

            return
                (from folder in result
                    where !string.IsNullOrEmpty(folder) && Path.GetDirectoryName(folder) == parentFolder
                    select folder).ToArray();
        }

        /// <summary>
        ///     Gets the root folders.
        /// </summary>
        /// <returns></returns>
        public string[] GetRootDirectories()
        {
            var folders = GetAllDirectories();
            var result = (from folder in folders select folder.Split('\\')[0]).ToArray().Distinct().ToArray();

            return result;
        }

        #endregion Methods
    }
}