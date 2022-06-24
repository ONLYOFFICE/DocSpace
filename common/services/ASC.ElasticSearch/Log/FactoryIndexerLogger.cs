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

namespace ASC.ElasticSearch.Log;
internal static partial class FactoryIndexerLogger
{
    [LoggerMessage(Level = LogLevel.Critical, Message = "IndexAll")]
    public static partial void CriticalIndexAll(this ILogger logger, Exception exception); 
    
    [LoggerMessage(Level = LogLevel.Debug, Message = "CheckState ping {debugInfo}")]
    public static partial void DebugCheckStatePing(this ILogger logger, string debugInfo); 
    
    [LoggerMessage(Level = LogLevel.Error, Message = "Select")]
    public static partial void ErrorSelect(this ILogger logger, Exception exception); 
    
    [LoggerMessage(Level = LogLevel.Error, Message = "Index")]
    public static partial void ErrorIndex(this ILogger logger, Exception exception);  
    
    [LoggerMessage(Level = LogLevel.Error, Message = "inner")]
    public static partial void ErrorInner(this ILogger logger, Exception exception);  
    
    [LoggerMessage(Level = LogLevel.Error, Message = "IndexAsync")]
    public static partial void ErrorIndexAsync(this ILogger logger, Exception exception);
    
    [LoggerMessage(Level = LogLevel.Error, Message = "Update")]
    public static partial void ErrorUpdate(this ILogger logger, Exception exception);  
    
    [LoggerMessage(Level = LogLevel.Error, Message = "Delete")]
    public static partial void ErrorDelete(this ILogger logger, Exception exception);    

    [LoggerMessage(Level = LogLevel.Error, Message = "FactoryIndexer CheckState")]
    public static partial void ErrorFactoryIndexerCheckState(this ILogger logger, Exception exception);
    
    [LoggerMessage(Level = LogLevel.Error, Message = "Queue")]
    public static partial void ErrorQueue(this ILogger logger, Exception exception);  
    
    [LoggerMessage(Level = LogLevel.Error, Message = "Ping false")]
    public static partial void ErrorPingFalse(this ILogger logger, Exception exception); 
}
