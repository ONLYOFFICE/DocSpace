using System;

namespace ASC.Core.Common.Hosting;

public class InstanceRegistration
{
    public string InstanceRegistrationId { get; set; }
    public DateTime? LastUpdated { get; set; }
    public string WorkerTypeName { get; set; }
    public bool IsActive { get; set; }
}
