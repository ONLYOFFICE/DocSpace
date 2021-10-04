using System;

using ASC.Files.Model;

namespace ASC.Files.Core.Model
{
    public class ChangeOwnerModel : BaseBatchModel
    {
        public Guid UserId { get; set; }
    }
}
