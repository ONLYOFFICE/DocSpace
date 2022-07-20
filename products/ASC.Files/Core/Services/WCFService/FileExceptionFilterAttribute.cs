// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Web.Files.Services.WCFService;

class FileExceptionFilterAttribute : IExceptionFilter
{
    private readonly ILogger _logger;

    public FileExceptionFilterAttribute(ILoggerProvider options)
    {
        _logger = options.CreateLogger("ASC.Files");
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
            _logger.ErrorLogException(err);
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
