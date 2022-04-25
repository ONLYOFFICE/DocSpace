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

namespace ASC.Files.Tests;

public static class DocumentData
{
    public static IEnumerable<TestCaseData> GetCreateFolderItems()
    {
        yield return new TestCaseData("FolderOne");
    }

    public static IEnumerable<TestCaseData> GetFolderInfoItems()
    {
        yield return new TestCaseData("TestFolder");
    }

    public static IEnumerable<TestCaseData> GetRenameFolderItems()
    {
        yield return new TestCaseData("FoldTest");
    }

    public static IEnumerable<TestCaseData> GetDeleteFolderItems()
    {
        yield return new TestCaseData(false, true);
    }

    public static IEnumerable<TestCaseData> GetCreateFileItems()
    {
        yield return new TestCaseData("FileOne");
    }

    public static IEnumerable<TestCaseData> GetFileInfoItems()
    {
        yield return new TestCaseData("TestFile");
    }

    public static IEnumerable<TestCaseData> GetUpdateFileItems()
    {
        yield return new TestCaseData("FileTest", 3);
    }

    public static IEnumerable<TestCaseData> GetDeleteFileItems()
    {
        yield return new TestCaseData(false, true);
    }

    public static IEnumerable<TestCaseData> GetMoveBatchItems()
    {
        yield return new TestCaseData(" [ { \"folderIds\": [ 1, 2, 3 ] }, { \"fileIds\": [ 1 , 2 ] }, { \"destFolderId\": 4 } ]");
    }

    public static IEnumerable<TestCaseData> GetCopyBatchItems()
    {
        yield return new TestCaseData(" [ { \"folderIds\": [ 6 ] }, { \"fileIds\": [ 4 , 5 ] }, { \"destFolderId\": 5 } ]");
    }

    public static IEnumerable<TestCaseData> ShareParamToFolder()
    {
        yield return new TestCaseData(false, "folder_test");
    }
    public static IEnumerable<TestCaseData> ShareParamToFile()
    {
        yield return new TestCaseData(false, "folder_test");
    }
    public static IEnumerable<TestCaseData> ShareParamToRecentFile()
    {
        yield return new TestCaseData("TestFile", false, "folder_test");
    }
    public static IEnumerable<TestCaseData> ShareParamToFileRead()
    {
        yield return new TestCaseData(false, "folder_test");
    }
    public static IEnumerable<TestCaseData> GetSharedInfo()
    {
        yield return new TestCaseData("TestFileRead");
    }
    public static IEnumerable<TestCaseData> GetSharedInfoReadAndWrite()
    {
        yield return new TestCaseData("TestFileReadAndWrite");
    }
    public static IEnumerable<TestCaseData> GetSharedFolderInfoRead()
    {
        yield return new TestCaseData("TestFolderRead");
    }
    public static IEnumerable<TestCaseData> GetSharedFolderInfoReadAndWrite()
    {
        yield return new TestCaseData("TestFolderReadAndWrite");
    }
}
