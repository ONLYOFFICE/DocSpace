using System;

namespace ASC.Files.Tests.Infrastructure
{
    public class UserOptions
    {
        public const string User = "User";

        public Guid Id { get; set; }
        public int TenantId { get; set; }
    }

    public class DocumentsOptions
    {
        public FolderOptions FolderOptions { get; set; } = new FolderOptions();
        public FileOptions FileOptions { get; set; } = new FileOptions();
    }

    public class FolderOptions
    {
        public CreateItems CreateItems { get; set; }
        public GetItems GetItems { get; set; }
        public GetInfoItems GetInfoItems { get; set; }
        public RenameItems RenameItems { get; set; }
        public DeleteItems DeleteItems { get; set; }
    }

    public class FileOptions
    {
        public CreateItems CreateItems { get; set; }
        public GetInfoItems GetInfoItems { get; set; }
        public UpdateItems UpdateItems { get; set; }
        public DeleteItems DeleteItems { get; set; }
    }

    public class CreateItems
    {
        public string TitleOne { get; set; }
        public string TitleTwo { get; set; }
    }

    public class GetItems
    {
        public int Id { get; set; }
        public bool WithSubFolders { get; set; }
    }

    public class GetInfoItems
    {
        public int id { get; set; }
        public string TitleExpected { get; set; }
    }

    public class RenameItems
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class DeleteItems
    {
        public int Id { get; set; }
        public bool DeleteAfter { get; set; }
        public bool Immediately { get; set; }
    }

    public class UpdateItems
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int LastVersion { get; set; }
    }
}
