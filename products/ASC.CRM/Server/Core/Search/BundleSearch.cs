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
using ASC.CRM.Core;
using ASC.CRM.Core.Enums;
using ASC.ElasticSearch;

namespace ASC.Web.CRM.Core.Search
{
    public class BundleSearch
    {
        public BundleSearch(FactoryIndexer<ContactsWrapper> contactsWrapperFactoryIndexer,
                            FactoryIndexer<InfoWrapper> infoWrapperFactoryIndexer,
                            FactoryIndexer<FieldsWrapper> fieldsWrapperFactoryIndexer,
                            FactoryIndexer<EventsWrapper> eventsWrapperFactoryIndexer,
                            FactoryIndexer<DealsWrapper> dealsWrapperFactoryIndexer,
                            FactoryIndexer<TasksWrapper> tasksWrapperFactoryIndexer,
                            FactoryIndexer<CasesWrapper> casesWrapperFactoryIndexer,
                            FactoryIndexer<InvoicesWrapper> invoicesWrapperFactoryIndexer,
                            FactoryIndexerHelper factoryIndexerHelper,
                            ContactsWrapper contactsWrapper,
                            InfoWrapper infoWrapper,
                            FieldsWrapper fieldsWrapper,
                            EventsWrapper eventsWrapper,
                            DealsWrapper dealsWrapper,
                            TasksWrapper tasksWrapper,
                            CasesWrapper casesWrapper,
                            InvoicesWrapper invoicesWrapper)
        {
            ContactsWrapperFactoryIndexer = contactsWrapperFactoryIndexer;
            InfoWrapperFactoryIndexer = infoWrapperFactoryIndexer;
            FieldsWrapperFactoryIndexer = fieldsWrapperFactoryIndexer;
            EventsWrapperFactoryIndexer = eventsWrapperFactoryIndexer;
            DealsWrapperFactoryIndexer = dealsWrapperFactoryIndexer;
            TasksWrapperFactoryIndexer = tasksWrapperFactoryIndexer;
            CasesWrapperFactoryIndexer = casesWrapperFactoryIndexer;
            InvoicesWrapperFactoryIndexer = invoicesWrapperFactoryIndexer;
            FactoryIndexerHelper = factoryIndexerHelper;

            ContactsWrapper = contactsWrapper;
            InfoWrapper = infoWrapper;
            FieldsWrapper = fieldsWrapper;
            EventsWrapper = eventsWrapper;
            DealsWrapper = dealsWrapper;
            TasksWrapper = tasksWrapper;
            CasesWrapper = casesWrapper;
            InvoicesWrapper = invoicesWrapper;

        }

        public ContactsWrapper ContactsWrapper { get; }
        public InfoWrapper InfoWrapper { get; }
        public FieldsWrapper FieldsWrapper { get; }
        public EventsWrapper EventsWrapper { get; }
        public DealsWrapper DealsWrapper { get; }
        public TasksWrapper TasksWrapper { get; }
        public CasesWrapper CasesWrapper { get; }
        public InvoicesWrapper InvoicesWrapper { get; }

        public FactoryIndexerHelper FactoryIndexerHelper { get; }
        public FactoryIndexer<ContactsWrapper> ContactsWrapperFactoryIndexer { get; }
        public FactoryIndexer<InfoWrapper> InfoWrapperFactoryIndexer { get; }
        public FactoryIndexer<FieldsWrapper> FieldsWrapperFactoryIndexer { get; }
        public FactoryIndexer<EventsWrapper> EventsWrapperFactoryIndexer { get; }
        public FactoryIndexer<DealsWrapper> DealsWrapperFactoryIndexer { get; }
        public FactoryIndexer<TasksWrapper> TasksWrapperFactoryIndexer { get; }
        public FactoryIndexer<CasesWrapper> CasesWrapperFactoryIndexer { get; }
        public FactoryIndexer<InvoicesWrapper> InvoicesWrapperFactoryIndexer { get; }

        public bool Support(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Person:
                case EntityType.Contact:
                case EntityType.Company:
                    return FactoryIndexerHelper.Support(ContactsWrapper) &&
                           FactoryIndexerHelper.Support(InfoWrapper) &&
                           FactoryIndexerHelper.Support(FieldsWrapper) &&
                           FactoryIndexerHelper.Support(EventsWrapper);
                case EntityType.Opportunity:
                    return FactoryIndexerHelper.Support(DealsWrapper) &&
                           FactoryIndexerHelper.Support(FieldsWrapper) &&
                           FactoryIndexerHelper.Support(EventsWrapper);
                case EntityType.RelationshipEvent:
                    return FactoryIndexerHelper.Support(EventsWrapper);
                case EntityType.Task:
                    return FactoryIndexerHelper.Support(TasksWrapper);
                case EntityType.Case:
                    return FactoryIndexerHelper.Support(CasesWrapper) &&
                           FactoryIndexerHelper.Support(FieldsWrapper) &&
                           FactoryIndexerHelper.Support(EventsWrapper);
                case EntityType.Invoice:
                    return FactoryIndexerHelper.Support(InvoicesWrapper);
            }

            return false;
        }

        public bool TrySelectCase(string text, out List<int> result)
        {
            var success = false;
            result = new List<int>();

            List<int> casesId;
            if (CasesWrapperFactoryIndexer.TrySelectIds(s => s.MatchAll(text), out casesId))
            {
                result.AddRange(casesId);
                success = true;
            }

            IReadOnlyCollection<FieldsWrapper> casesCustom;
            if (FieldsWrapperFactoryIndexer.TrySelect(s => s.MatchAll(text).Where(r => r.EntityType, 7), out casesCustom))
            {
                result.AddRange(casesCustom.Select(r => r.EntityId).ToList());
                success = true;
            }

            IReadOnlyCollection<EventsWrapper> events;
            if (!EventsWrapperFactoryIndexer.TrySelect(s => s.MatchAll(text).Where(r => r.EntityType, 7).Gt(r => r.EntityId, 0), out events))
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

            if (ContactsWrapperFactoryIndexer.TrySelectIds(s => s.MatchAll(text), out contactsId))
            {
                result.AddRange(contactsId);
                success = true;
            }

            IReadOnlyCollection<InfoWrapper> infos;

            if (InfoWrapperFactoryIndexer.TrySelect(s => s.MatchAll(text), out infos))
            {
                result.AddRange(infos.Select(r => r.ContactId).ToList());
                success = true;
            }

            IReadOnlyCollection<FieldsWrapper> personCustom;

            if (FieldsWrapperFactoryIndexer.TrySelect(s => s.MatchAll(text).In(r => r.EntityType, new[] { 0, 4, 5 }), out personCustom))
            {
                result.AddRange(personCustom.Select(r => r.EntityId).ToList());
                success = true;
            }

            IReadOnlyCollection<EventsWrapper> events;

            if (EventsWrapperFactoryIndexer.TrySelect(s => s.MatchAll(text).Gt(r => r.ContactId, 0), out events))
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
            if (DealsWrapperFactoryIndexer.TrySelectIds(s => s.MatchAll(text), out dealsId))
            {
                result.AddRange(dealsId);
                success = true;
            }

            IReadOnlyCollection<FieldsWrapper> casesCustom;
            if (FieldsWrapperFactoryIndexer.TrySelect(s => s.MatchAll(text).Where(r => r.EntityType, 1), out casesCustom))
            {
                result.AddRange(casesCustom.Select(r => r.EntityId).ToList());
                success = true;
            }

            IReadOnlyCollection<EventsWrapper> events;
            if (!EventsWrapperFactoryIndexer.TrySelect(s => s.MatchAll(text).Where(r => r.EntityType, 1).Gt(r => r.EntityId, 0), out events))
            {
                result.AddRange(events.Select(r => r.EntityId).ToList());
                success = true;
            }

            return success;
        }
    }

    public static class BundleSearchExtention
    {
        public static DIHelper AddBundleSearchService(this DIHelper services)
        {                       
            return services.AddFactoryIndexerService<ContactsWrapper>()
                           .AddFactoryIndexerService<InfoWrapper>()
                           .AddFactoryIndexerService<FieldsWrapper>()
                           .AddFactoryIndexerService<EventsWrapper>()
                           .AddFactoryIndexerService<DealsWrapper>()
                           .AddFactoryIndexerService<TasksWrapper>()
                           .AddFactoryIndexerService<CasesWrapper>()
                           .AddFactoryIndexerService<InvoicesWrapper>()
                           .AddFactoryIndexerHelperService()
                           .AddContactsWrapperService()
                           .AddInfoWrapperService()
                           .AddFieldsWrapperService()
                           .AddEventsWrapperService()
                           .AddDealsWrapperService()
                           .AddTasksWrapperService()
                           .AddCasesWrapperService()
                           .AddInvoicesWrapperService();
        }
    }
}