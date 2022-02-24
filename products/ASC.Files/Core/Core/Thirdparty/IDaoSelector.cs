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

namespace ASC.Files.Thirdparty
{
    internal interface IDaoSelector : IDisposable
    {
        bool IsMatch(string id);
        IFileDao<string> GetFileDao(string id);
        IFolderDao<string> GetFolderDao(string id);
        ISecurityDao<string> GetSecurityDao(string id);
        ITagDao<string> GetTagDao(string id);
        string ConvertId(string id);
        string GetIdCode(string id);
    }

    internal interface IDaoSelector<T> where T : class, IProviderInfo
    {
        bool IsMatch(string id);
        IFileDao<string> GetFileDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, IFileDao<string>;
        IFolderDao<string> GetFolderDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, IFolderDao<string>;
        ISecurityDao<string> GetSecurityDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, ISecurityDao<string>;
        ITagDao<string> GetTagDao<T1>(string id) where T1 : ThirdPartyProviderDao<T>, ITagDao<string>;
        string ConvertId(string id);
        string GetIdCode(string id);
    }
}