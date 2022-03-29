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
using System.Linq;
using System.Web;

using ASC.Common;
using ASC.Common.Threading;
using ASC.Common.Threading.Progress;
using ASC.Core;

using log4net;

using Microsoft.AspNetCore.Http;

namespace ASC.Web.CRM.Classes
{
    [Transient]
    public class PdfQueueWorker
    {
        private readonly DistributedTaskQueue _queue;
        private readonly int _tenantId;
        private readonly Guid _userId;
        private readonly object Locker = new object();
        private PdfProgressItem _pdfProgressItem;

        public PdfQueueWorker(DistributedTaskQueueOptionsManager queueOptions,
                              PdfProgressItem pdfProgressItem,
                              TenantManager tenantProvider,
                              SecurityContext securityContext)
        {
            _queue = queueOptions.Get<PdfProgressItem>();
            _pdfProgressItem = pdfProgressItem;
            _tenantId = tenantProvider.GetCurrentTenant().TenantId;
            _userId = securityContext.CurrentAccount.ID;
        }


        public string GetTaskId(int tenantId, int invoiceId)
        {
            return string.Format("{0}_{1}", tenantId, invoiceId);
        }

        public PdfProgressItem GetTaskStatus(int tenantId, int invoiceId)
        {
            var id = GetTaskId(tenantId, invoiceId);

            var findedItem = _queue.GetTasks<PdfProgressItem>().FirstOrDefault(x => x.Id == id);

            return findedItem;
        }

        public void TerminateTask(int invoiceId)
        {
            var item = GetTaskStatus(_tenantId, invoiceId);

            if (item != null)
                _queue.RemoveTask(item.Id);
        }

        public PdfProgressItem StartTask(int invoiceId)
        {
            lock (Locker)
            {
                var task = GetTaskStatus(_tenantId, invoiceId);

                if (task != null && task.IsCompleted)
                {
                    _queue.RemoveTask(task.Id);
                    task = null;
                }

                if (task == null)
                {
                    _pdfProgressItem.Configure(GetTaskId(_tenantId, invoiceId), _tenantId, _userId, invoiceId);
                    _queue.QueueTask(_pdfProgressItem);

                }

                return task;
            }
        }
    }

    [Transient]
    public class PdfProgressItem : DistributedTaskProgress, IProgressItem
    {
        private readonly string _contextUrl;
        private int _tenantId;
        private int _invoiceId;
        private Guid _userId;

        public object Error { get; set; }
        private readonly PdfCreator _pdfCreator;
        private readonly SecurityContext _securityContext;
        private readonly TenantManager _tenantManager;

        public PdfProgressItem(IHttpContextAccessor httpContextAccessor,
                               PdfCreator pdfCreator,
                               SecurityContext securityContext,
                               TenantManager tenantManager)
        {
            _contextUrl = httpContextAccessor.HttpContext != null ? httpContextAccessor.HttpContext.Request.GetUrlRewriter().ToString() : null;
            _pdfCreator = pdfCreator;
            _securityContext = securityContext;
            _tenantManager = tenantManager;

            Error = null;
            Percentage = 0;
            IsCompleted = false;
        }

        public void Configure(object id,
                                int tenantId,
                               Guid userId,
                               int invoiceId)
        {
            Id = id.ToString();
            _tenantId = tenantId;
            _invoiceId = invoiceId;
            _userId = userId;
        }


        protected override void DoJob()
        {
            try
            {
                Percentage = 0;

                _tenantManager.SetCurrentTenant(_tenantId);

                _securityContext.AuthenticateMeWithoutCookie(_userId);

                //if (HttpContext.Current == null && !WorkContext.IsMono)
                //{
                //    HttpContext.Current = new HttpContext(
                //        new HttpRequest("hack", _contextUrl, string.Empty),
                //        new HttpResponse(new System.IO.StringWriter()));
                //}

                _pdfCreator.CreateAndSaveFileAsync(_invoiceId).Wait();

                Percentage = 100;
                PublishChanges();

                Status = DistributedTaskStatus.Completed;
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("ASC.Web").Error(ex);

                Percentage = 0;
                Status = DistributedTaskStatus.Failted;
                Error = ex.Message;
            }
            finally
            {
                // fake httpcontext break configuration manager for mono
                if (!WorkContext.IsMono)
                {
                    //if (HttpContext.Current != null)
                    //{
                    //    new DisposableHttpContext(HttpContext.Current).Dispose();
                    //    HttpContext.Current = null;
                    //}
                }

                IsCompleted = true;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj is PdfProgressItem)
            {
                return ((PdfProgressItem)obj).Id == Id;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}