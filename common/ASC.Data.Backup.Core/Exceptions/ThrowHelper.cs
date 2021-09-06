/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Data.Backup.Exceptions
{
    internal static class ThrowHelper
    {
        public static DbBackupException CantDetectTenant(string tableName)
        {
            return new DbBackupException(string.Format("Can't detect tenant column for table {0}.", tableName));
        }

        public static DbBackupException CantOrderTables(IEnumerable<string> conflictingTables)
        {
            return new DbBackupException(string.Format("Can't order tables [\"{0}\"].", string.Join("\", \"", conflictingTables.ToArray())));
        }

        public static DbBackupException CantOrderModules(IEnumerable<Type> conflictingTypes)
        {
            return new DbBackupException(string.Format("Can't order modules [\"{0}\"].", string.Join("\", \"", conflictingTypes.Select(x => x.Name).ToArray())));
        }

        public static DbBackupException CantRestoreTable(string tableName, Exception reason)
        {
            return new DbBackupException(string.Format("Can't restore table {0}.", tableName), reason);
        }

        public static DbBackupException CantBackupTable(string tableName, Exception reason)
        {
            return new DbBackupException(string.Format("Can't backup table {0}.", tableName), reason);
        }

        public static DbBackupException CantDeleteTable(string tableName, Exception reason)
        {
            return new DbBackupException(string.Format("Can't delete table {0}.", tableName), reason);
        }
    }
}
