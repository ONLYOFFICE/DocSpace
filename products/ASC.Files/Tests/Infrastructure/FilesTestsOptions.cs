using System;

namespace ASC.Files.Tests.Infrastructure
{
    public class UserOptions
    {
        public const string User = "User";

        public Guid Id { get; set; }
        public int TenantId { get; set; }

    }
}
