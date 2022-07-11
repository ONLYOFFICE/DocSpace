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


using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.CRM.Core.EF;
using ASC.ElasticSearch;

namespace ASC.Web.CRM.Core.Search
{
    [Scope]
    public class BundleSearch
    {
        private readonly FactoryIndexerContact _factoryIndexerContact;
        private readonly FactoryIndexerContactInfo _factoryIndexerContactInfo;
        private readonly FactoryIndexerFieldValue _factoryIndexerFieldValue;
        private readonly FactoryIndexerEvents _factoryIndexerEvents;
        private readonly FactoryIndexerDeal _factoryIndexerDeal;
        private readonly FactoryIndexerTask _factoryIndexerTask;
        private readonly FactoryIndexerCase _factoryIndexerCase;
        private readonly FactoryIndexerInvoice _factoryIndexerInvoice;
        private readonly FactoryIndexer _factoryIndexer;

        public BundleSearch(        FactoryIndexer factoryIndexer,
                                    FactoryIndexerContact factoryIndexerContact,
                                    FactoryIndexerContactInfo factoryIndexerContactInfo,
                                    FactoryIndexerFieldValue factoryIndexerFieldValue,
                                    FactoryIndexerEvents factoryIndexerEvents,
                                    FactoryIndexerDeal factoryIndexerDeal,
                                    FactoryIndexerTask factoryIndexerTask,
                                    FactoryIndexerCase factoryIndexerCase,
                                    FactoryIndexerInvoice factoryIndexerInvoice)
        {
            _factoryIndexerContact = factoryIndexerContact;
            _factoryIndexerContactInfo = factoryIndexerContactInfo;
            _factoryIndexerFieldValue = factoryIndexerFieldValue;
            _factoryIndexerEvents = factoryIndexerEvents;
            _factoryIndexerDeal = factoryIndexerDeal;
            _factoryIndexerTask = factoryIndexerTask;
            _factoryIndexerCase = factoryIndexerCase;
            _factoryIndexerInvoice = factoryIndexerInvoice;
            _factoryIndexer = factoryIndexer;
        }


        public bool CheckFullTextSearchEnable()
        {
           return _factoryIndexer.CheckState();
        }

        public bool TrySelectCase(string text, out List<int> result)
        {
            var success = false;
            result = new List<int>();

            List<int> casesId;

            if (_factoryIndexerCase.TrySelectIds(s => s.MatchAll(text), out casesId))
            {
                result.AddRange(casesId);
                success = true;
            }

            IReadOnlyCollection<DbFieldValue> casesCustom;
            if (_factoryIndexerFieldValue.TrySelect(s => s.MatchAll(text).Where(r => (int)r.EntityType, 7), out casesCustom))
            {
                result.AddRange(casesCustom.Select(r => r.EntityId).ToList());
                success = true;
            }

            IReadOnlyCollection<DbRelationshipEvent> events;

            if (!_factoryIndexerEvents.TrySelect(s => s.MatchAll(text).Where(r => (int)r.EntityType, 7).Gt(r => r.EntityId, 0), out events))
            {
                result.AddRange(events.Select(r => r.EntityId).ToList());
                success = true;
            }

            return success;
        }

        public bool TrySelectContact(string text, out List<int> result)
        {
            var success = false;
            result = new List<int>();

            List<int> contactsId;

            if (_factoryIndexerContact.TrySelectIds(s => s.MatchAll(text), out contactsId))
            {
                result.AddRange(contactsId);
                success = true;
            }

            IReadOnlyCollection<DbContactInfo> infos;

            if (_factoryIndexerContactInfo.TrySelect(s => s.MatchAll(text), out infos))
            {
                result.AddRange(infos.Select(r => r.ContactId).ToList());
                success = true;
            }

            IReadOnlyCollection<DbFieldValue> personCustom;

            if (_factoryIndexerFieldValue.TrySelect(s => s.MatchAll(text).In(r => r.EntityType, new[] { 0, 4, 5 }), out personCustom))
            {
                result.AddRange(personCustom.Select(r => r.EntityId).ToList());
                success = true;
            }

            IReadOnlyCollection<DbRelationshipEvent> events;

            if (_factoryIndexerEvents.TrySelect(s => s.MatchAll(text).Gt(r => r.ContactId, 0), out events))
            {
                result.AddRange(events.Select(r => r.ContactId).ToList());
                success = true;
            }

            return success;
        }

        public bool TrySelectOpportunity(string text, out List<int> result)
        {
            var success = false;
            result = new List<int>();

            List<int> dealsId;

            if (_factoryIndexerDeal.TrySelectIds(s => s.MatchAll(text), out dealsId))
            {
                result.AddRange(dealsId);
                success = true;
            }

            IReadOnlyCollection<DbFieldValue> casesCustom;

            if (_factoryIndexerFieldValue.TrySelect(s => s.MatchAll(text).Where(r => (int)r.EntityType, 1), out casesCustom))
            {
                result.AddRange(casesCustom.Select(r => r.EntityId).ToList());
                success = true;
            }

            IReadOnlyCollection<DbRelationshipEvent> events;

            if (!_factoryIndexerEvents.TrySelect(s => s.MatchAll(text).Where(r => (int)r.EntityType, 1).Gt(r => r.EntityId, 0), out events))
            {
                result.AddRange(events.Select(r => r.EntityId).ToList());
                success = true;
            }

            return success;
        }
    }
}