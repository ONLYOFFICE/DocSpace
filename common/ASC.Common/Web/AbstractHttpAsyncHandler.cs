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
using System.Globalization;
using System.Security.Principal;
using System.Threading;

namespace ASC.Common.Web
{
    public abstract class AbstractHttpAsyncHandler // : IHttpAsyncHandler, IReadOnlySessionState
    {
        public bool IsReusable => false;

        private Action<Microsoft.AspNetCore.Http.HttpContext> _processRequest;
        private IPrincipal _principal;
        private CultureInfo _culture;

        public void ProcessRequest(Microsoft.AspNetCore.Http.HttpContext context)
        {
            Thread.CurrentThread.CurrentCulture = _culture;
            Thread.CurrentThread.CurrentUICulture = _culture;
            Thread.CurrentPrincipal = _principal;
            //HttpContext.Current = context;
            OnProcessRequest(context);
        }

        public IAsyncResult BeginProcessRequest(Microsoft.AspNetCore.Http.HttpContext context, AsyncCallback cb, object extraData)
        {
            _culture = Thread.CurrentThread.CurrentCulture;
            _principal = Thread.CurrentPrincipal;
            _processRequest = ProcessRequest;

            return _processRequest.BeginInvoke(context, cb, extraData);
        }

        public void EndProcessRequest(IAsyncResult result) => _processRequest.EndInvoke(result);

        public abstract void OnProcessRequest(Microsoft.AspNetCore.Http.HttpContext context);
    }
}