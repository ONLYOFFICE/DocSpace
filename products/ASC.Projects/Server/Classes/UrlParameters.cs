/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;

using ASC.Common;
using ASC.Projects.Core.Domain.Reports;

using Microsoft.AspNetCore.Http;

namespace ASC.Projects.Classes
{
    [Scope]
    public class UrlParameters
    {
        private HttpContext HttpContext { get; }

        public UrlParameters(HttpContext httpContext)
        {
            HttpContext = httpContext;
        }

        public UrlAction? ActionType
        {
            get
            {
                UrlAction result;
                if (Enum.TryParse(HttpContext.Request.Headers[UrlConstant.Action].ToString() ?? string.Empty, true, out result))
                {
                    return result;
                }
                return null;
            }
        }

        public int EntityID
        {
            get
            {
                int result;
                if (int.TryParse(HttpContext.Request.Headers[UrlConstant.EntityID].ToString() ?? string.Empty, out result))
                {
                    return result;
                }
                return -1;
            }
        }

        public int ProjectID
        {
            get
            {
                int result;
                if (int.TryParse(HttpContext.Request.Headers[UrlConstant.ProjectID].ToString() ?? string.Empty, out result))
                {
                    return result;
                }
                return -1;
            }
        }

        public ReportType ReportType
        {
            get
            {
                ReportType result;
                if (Enum.TryParse(HttpContext.Request.Headers[UrlConstant.ReportType].ToString() ?? string.Empty, out result))
                {
                    return result;
                }
                return ReportType.EmptyReport;
            }
        }
    }
}
