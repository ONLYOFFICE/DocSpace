using ASC.Core.Users;

using AutoMapper;

namespace ASC.AuditTrail.Models.Mapping.Resolvers
{
    public class LoginEventUserNameValueResolver : IValueResolver<LoginEventQuery, LoginEvent, string>
    {
        private readonly UserFormatter _formatter;

        public LoginEventUserNameValueResolver(UserFormatter userFormatter) => _formatter = userFormatter;

        public string Resolve(LoginEventQuery source, LoginEvent destination, string destMember, ResolutionContext context) =>
            (!string.IsNullOrEmpty(source.User?.FirstName) && !string.IsNullOrEmpty(source.User?.LastName))
                ? _formatter.GetUserName(source.User.FirstName, source.User.LastName)
                : !string.IsNullOrWhiteSpace(source.LoginEvents.Login)
                    ? source.LoginEvents.Login
                    : source.LoginEvents.UserId == Core.Configuration.Constants.Guest.ID
                        ? AuditReportResource.GuestAccount
                        : AuditReportResource.UnknownAccount;
    }

    public class AuditEventUserNameValueResolver : IValueResolver<AuditEventQuery, AuditEvent, string>
    {
        private readonly UserFormatter _formatter;

        public AuditEventUserNameValueResolver(UserFormatter userFormatter) => _formatter = userFormatter;

        public string Resolve(AuditEventQuery source, AuditEvent destination, string destMember, ResolutionContext context) =>
            (source.User.FirstName != null && source.User.LastName != null) ? _formatter.GetUserName(source.User.FirstName, source.User.LastName) :
                    source.AuditEvent.UserId == Core.Configuration.Constants.CoreSystem.ID ? AuditReportResource.SystemAccount :
                        source.AuditEvent.UserId == Core.Configuration.Constants.Guest.ID ? AuditReportResource.GuestAccount :
                            source.AuditEvent.Initiator ?? AuditReportResource.UnknownAccount;
    }
}
