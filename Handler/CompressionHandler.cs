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
using System.Linq;
using XBundle.Compression;

namespace XBundle.Handler
{
    public static class CompressionHandler
    {
        #region Members

        /// <summary>
        ///     Private field of RegisteredCompressors
        /// </summary>
        private static readonly List<ICompression> RegisteredCompressors = new List<ICompression>();

        #endregion Members

        #region Methods

        /// <summary>
        ///     Registers a new compressor to this application. If the desired types already exist in the list, the old compressor
        ///     type will be removed.
        /// </summary>
        /// <param name="value">The value.</param>
        public static void RegisterCompressor(ICompression value)
        {
            if (RegisteredCompressors == null)
                return;

            var existingCompressor = GetCompressor(value.Types);
            if (existingCompressor != null)
                RegisteredCompressors.Remove(existingCompressor);

            RegisteredCompressors.Add(value);
        }

        /// <summary>
        ///     Unregisters the compressor of a specific type
        /// </summary>
        /// <param name="type">The type.</param>
        public static void UnregisterCompressor(uint type)
        {
            var compressor = GetCompressor(type);
            RegisteredCompressors.Remove(compressor);
        }

        /// <summary>
        ///     Gets the compressor for a specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static ICompression GetCompressor(uint type)
        {
            return
                (from compressor in RegisteredCompressors
                    where compressor.Types.Contains(type)
                    select compressor)
                    .FirstOrDefault();
        }

        /// <summary>
        ///     Gets the compressor for a specified type.
        /// </summary>
        /// <param name="types">The type.</param>
        /// <returns></returns>
        private static ICompression GetCompressor(uint[] types)
        {
            return
                (from compressor in RegisteredCompressors
                    where compressor.Types == types
                    select compressor)
                    .FirstOrDefault();
        }

        #endregion Methods
    }
}