using System;

namespace ASC.Core.Common.Services;

public class InstanceRegistrationEntry
{
    public DateTime? LastUpdated { get; set; }
    public string WorkerName { get; set; }
    public bool IsActive { get; set; }
    public string InstanceRegistrationId { get; set; }
}
