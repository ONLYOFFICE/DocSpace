using NUnit.Framework;

using System.Collections.Generic;

namespace ASC.Files.Tests
{
    public static class DocumentData
    {
        public static IEnumerable<TestCaseData> GetCreateFolderItems()
        {
            yield return new TestCaseData("FolderOne");
            yield return new TestCaseData("FolderTwo");
        }

        public static IEnumerable<TestCaseData> GetFolderItems()
        {
            yield return new TestCaseData(14, true, 0, 0);
        }

        public static IEnumerable<TestCaseData> GetFolderInfoItems()
        {
            yield return new TestCaseData(1, "FolderOne");
        }

        public static IEnumerable<TestCaseData> GetRenameFolderItems()
        {
            yield return new TestCaseData(1, "FoldTest");
        }

        public static IEnumerable<TestCaseData> GetDeleteFolderItems()
        {
            yield return new TestCaseData(1, false, true);
        }

        public static IEnumerable<TestCaseData> GetCreateFileItems()
        {
            yield return new TestCaseData("FileOne.docx");
            yield return new TestCaseData("FileTwo.docx");
        }

        public static IEnumerable<TestCaseData> GetFileInfoItems()
        {
            yield return new TestCaseData(1, "FileOne.docx");
        }

        public static IEnumerable<TestCaseData> GetUpdateFileItems()
        {
            yield return new TestCaseData(1, "FileTest", 3);
        }

        public static IEnumerable<TestCaseData> GetDeleteFileItems()
        {
            yield return new TestCaseData(1, false, true);
        }

        public static IEnumerable<TestCaseData> GetMoveBatchItems()
        {
            yield return new TestCaseData(" [ { \"folderIds\": [ 1, 2, 3 ] }, { \"fileIds\": [ 1 , 2 ] }, { \"destFolderId\": 4 } ]");
        }

        public static IEnumerable<TestCaseData> GetCopyBatchItems()
        {
            yield return new TestCaseData(" [ { \"folderIds\": [ 6 ] }, { \"fileIds\": [ 4 , 5 ] }, { \"destFolderId\": 5 } ]");
        }
    }
}
