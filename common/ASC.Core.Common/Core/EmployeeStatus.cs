namespace ASC.Core.Users;

[Flags]
public enum EmployeeStatus
{
    Active = 1,
    Terminated = 2,
    LeaveOfAbsence = 4,
    All = Active | Terminated | LeaveOfAbsence,
    Default = Active | LeaveOfAbsence
}
