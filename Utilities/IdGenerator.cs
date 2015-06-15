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

namespace XBundle.Utilities
{
    internal class IdGenerator
    {
        /// <summary>
        ///     Generates a new unique identifier
        /// </summary>
        /// <returns></returns>
        public static uint GenerateUid()
        {
            var rnd = new Random();
            return (uint) rnd.Next(0, int.MaxValue);
        }

        /// <summary>
        ///     Generates the hash bytes.
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateHashBytes()
        {
            var result = new byte[0x10];

            var rnd = new Random();
            for (byte i = 0; i < 0x10; i++)
            {
                result[i] = (byte) rnd.Next(0, 256);
            }

            return result;
        }
    }
}