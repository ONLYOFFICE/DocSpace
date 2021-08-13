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
using System.Linq.Expressions;

using ASC.Mail.Core.Dao.Entities;

namespace ASC.Mail.Core.Dao.Expressions.Mailbox
{
    public class SimpleMailboxesExp : IMailboxesExp
    {
        private readonly bool? _isRemoved;
        public string OrderBy { get; private set; }
        public bool? OrderAsc { get; private set; }
        public int? Limit { get; private set; }

        public SimpleMailboxesExp(bool? isRemoved = false)
        {
            _isRemoved = isRemoved;
            OrderBy = null;
            OrderAsc = false;
            Limit = null;
        }

        public virtual Expression<Func<MailMailbox, bool>> GetExpression()
        {
            if (!_isRemoved.HasValue)
                return mb => true;

            return mb => mb.IsRemoved == _isRemoved;
        }
    }

    public class ConcreteMailboxesExp : IMailboxesExp
    {
        private readonly bool? _isRemoved;
        public string OrderBy { get; private set; }
        public bool? OrderAsc { get; private set; }
        public int? Limit { get; private set; }
        public List<int> Ids { get; private set; }

        public ConcreteMailboxesExp(List<int> ids, bool? isRemoved = false)
        {
            _isRemoved = isRemoved;
            OrderBy = null;
            OrderAsc = false;
            Limit = null;
            Ids = ids;
        }

        public virtual Expression<Func<MailMailbox, bool>> GetExpression()
        {
            Expression<Func<MailMailbox, bool>> exp = mb => mb.IsRemoved == _isRemoved;

            exp = exp.And(m => m.Id == Ids[0]);

            if (Ids.Count == 1) return exp;

            foreach (var id in Ids.Skip(1))
            {
                exp = exp.Or(m => m.Id == id);
            }

            return exp;
        }
    }
}