
using ASC.Common;
using ASC.Core.Common.EF.Model;
using ASC.Projects.EF;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class WebProjectsContext : BaseDbContext
    {
        public DbSet<DbComment> Comment { get; set; }
        public DbSet<DbMessage> Message { get; set; }
        public DbSet<DbProject> Project { get; set; }
        public DbSet<DbParticipant> Participant { get; set; }
        public DbSet<DbTagToProject> TagToProject { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<DbMilestone> Milestone { get; set; }
        public DbSet<DbTask> Task { get; set; }
        public DbSet<DbFollowingProject> FollowingProject { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<DbCrmToProject> CrmToProject { get; set; }
        public DbSet<DbTasksResponsible> TasksResponsible { get; set; }
        public DbSet<DbTimeTracking> TimeTracking { get; set; }
        public DbSet<DbTasksOrder> TasksOrder { get; set; }
        public DbSet<DbSubtask> Subtask { get; set; }
        public DbSet<DbTag> Tag { get; set; }
        public DbSet<DbReportTemplate> ReportTemplate { get; set; }
        public DbSet<DbReport> Report { get; set; }
        public DbSet<DbStatus> Status { get; set; }
        public DbSet<DbTemplate> Template { get; set; }
        public DbSet<DbLink> Link { get; set; }
        public DbSet<DbTaskRecurrence> TaskRecurrence { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
            .From(modelBuilder, Provider)
            .AddComments()
            .AddMessage()
            .AddProject()
            .AddParticipant()
            .AddUserGroup()
            .AddMilestone()
            .AddTask()
            .AddProjectToParticipant()
            .AddUser()
            .AddCrmToProject()
            .AddTasksToResponsible()
            .AddTimeTracking()
            .AddTasksOrder()
            .AddSubtasks()
            .AddTag()
            .AddReportTemplate()
            .AddReport()
            .AddStatus()
            .AddTemplate()
            .AddLink()
            .AddTaskRecurrence();
        }
    }

    public static class WebProjectsContextExtension
    {
        public static DIHelper AddWebProjectsContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<WebProjectsContext>();
        }
    }
}
