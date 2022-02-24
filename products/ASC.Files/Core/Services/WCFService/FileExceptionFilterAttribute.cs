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

namespace ASC.Web.Files.Services.WCFService
{
    class FileExceptionFilterAttribute : IExceptionFilter
    {
        private readonly ILog _logger;

        public FileExceptionFilterAttribute(IOptionsMonitor<ILog> options)
        {
            _logger = options.Get("ASC.Files");
        }

        public void OnException(ExceptionContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)
            {
                var fileError = new FileError(actionExecutedContext.Exception);
                actionExecutedContext.Result = new ObjectResult(fileError)
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            LogException(actionExecutedContext.Exception);
        }


        [Conditional("DEBUG")]
        private void LogException(Exception err)
        {
            while (err != null)
            {
                _logger.Error(err);
                err = err.InnerException;
            }
        }

        class FileError
        {
            public string Detail { get; set; }
            public string Message { get; set; }
            public FileErrorInner Inner { get; set; }

            internal class FileErrorInner
            {
                public string Message { get; set; }
                public string Type { get; set; }
                public string Source { get; set; }
                public string Stack { get; set; }
            }

            public FileError() { }

            public FileError(Exception error)
            {
                Detail = error.Message;
                Message = FilesCommonResource.ErrorMassage_BadRequest;
                Inner = new FileErrorInner
                {
                    Message = error.Message,
                    Type = error.GetType().FullName,
                    Source = error.Source ?? string.Empty,
                    Stack = error.StackTrace ?? string.Empty,
                };
            }
        }
    }
}