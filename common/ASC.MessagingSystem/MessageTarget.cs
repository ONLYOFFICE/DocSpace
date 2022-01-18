/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
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

using ASC.Common;
using ASC.Common.Logging;

using Microsoft.Extensions.Options;

namespace ASC.MessagingSystem
{
    [Singletone]
    public class MessageTarget
    {
        private IEnumerable<string> _items;

        public ILog Log { get; set; }
        private IOptionsMonitor<ILog> Option { get; }

        public MessageTarget(IOptionsMonitor<ILog> option)
        {
            Log = option.Get("ASC.Messaging");
            Option = option;
        }

        public MessageTarget Create<T>(T value)
        {
            try
            {
                var res = new List<string>();

                if (value is System.Collections.IEnumerable ids)
                {
                    res.AddRange(from object id in ids select id.ToString());
                }
                else
                {
                    res.Add(value.ToString());
                }

                return new MessageTarget(Option)
                {
                    _items = res.Distinct()
                };
            }
            catch (Exception e)
            {
                Log.Error("EventMessageTarget exception", e);
                return null;
            }

        }

        public MessageTarget Create(IEnumerable<string> value)
        {
            try
            {
                return new MessageTarget(Option)
                {
                    _items = value.Distinct()
                };
            }
            catch (Exception e)
            {
                Log.Error("EventMessageTarget exception", e);
                return null;
            }
        }

        public MessageTarget Parse(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            var items = value.Split(',');

            if (items.Length == 0) return null;

            return new MessageTarget(Option)
            {
                _items = items
            };
        }

        public override string ToString()
        {
            return string.Join(",", _items);
        }
    }
}