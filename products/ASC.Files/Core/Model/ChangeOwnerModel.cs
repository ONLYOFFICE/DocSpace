using System;
using System.Text.Json;

using ASC.Files.Model;

namespace ASC.Files.Core.Model
{
    public class ChangeOwnerModel: BaseBatchModel<JsonElement>
    {
        public Guid UserId { get; set; }
    }
}
