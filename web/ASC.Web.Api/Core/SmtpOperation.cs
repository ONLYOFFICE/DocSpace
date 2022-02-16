/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using AuthenticationException = System.Security.Authentication.AuthenticationException;
using SecurityContext = ASC.Core.SecurityContext;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace ASC.Api.Settings.Smtp;
public class SmtpOperation
{
    public const string OWNER = "SMTPOwner";
    public const string SOURCE = "SMTPSource";
    public const string PROGRESS = "SMTPProgress";
    public const string RESULT = "SMTPResult";
    public const string ERROR = "SMTPError";
    public const string FINISHED = "SMTPFinished";

    protected DistributedTask TaskInfo { get; set; }
    protected CancellationToken CancellationToken { get; private set; }
    protected int Progress { get; private set; }
    protected string Source { get; private set; }
    protected string Status { get; set; }
    protected string Error { get; set; }
    protected int CurrentTenant { get; private set; }
    protected Guid CurrentUser { get; private set; }

    private readonly UserManager _userManager;
    private readonly SecurityContext _securityContext;
    private readonly TenantManager _tenantManager;
    private readonly ILog _logger;
    private readonly SmtpSettingsWrapper _smtpSettings;

    private readonly string _messageSubject;
    private readonly string _messageBody;


    public SmtpOperation(
        SmtpSettingsWrapper smtpSettings,
        int tenant,
        Guid user,
        UserManager userManager,
        SecurityContext securityContext,
        TenantManager tenantManager,
        IOptionsMonitor<ILog> options)
    {
        _smtpSettings = smtpSettings;
        CurrentTenant = tenant;
        CurrentUser = user;
        _userManager = userManager;
        _securityContext = securityContext;
        _tenantManager = tenantManager;

        //todo
        _messageSubject = WebstudioNotifyPatternResource.subject_smtp_test;
        _messageBody = WebstudioNotifyPatternResource.pattern_smtp_test;

        Source = "";
        Progress = 0;
        Status = "";
        Error = "";
        Source = "";

        TaskInfo = new DistributedTask();

        _logger = options.CurrentValue;
    }

    public void RunJob(DistributedTask distributedTask, CancellationToken cancellationToken)
    {
        try
        {
            CancellationToken = cancellationToken;

            SetProgress(5, "Setup tenant");

            _tenantManager.SetCurrentTenant(CurrentTenant);

            SetProgress(10, "Setup user");

            _securityContext.AuthenticateMeWithoutCookie(CurrentUser); //Core.Configuration.Constants.CoreSystem);

            SetProgress(15, "Find user data");

            var currentUser = _userManager.GetUsers(_securityContext.CurrentAccount.ID);

            SetProgress(20, "Create mime message");

            var toAddress = new MailboxAddress(currentUser.UserName, currentUser.Email);

            var fromAddress = new MailboxAddress(_smtpSettings.SenderDisplayName, _smtpSettings.SenderAddress);

            var mimeMessage = new MimeMessage
            {
                Subject = _messageSubject
            };

            mimeMessage.From.Add(fromAddress);

            mimeMessage.To.Add(toAddress);

            var bodyBuilder = new BodyBuilder
            {
                TextBody = _messageBody
            };

            mimeMessage.Body = bodyBuilder.ToMessageBody();

            mimeMessage.Headers.Add("Auto-Submitted", "auto-generated");

            using var client = GetSmtpClient();
            SetProgress(40, "Connect to host");

            client.Connect(_smtpSettings.Host, _smtpSettings.Port.GetValueOrDefault(25),
                _smtpSettings.EnableSSL ? SecureSocketOptions.Auto : SecureSocketOptions.None, cancellationToken);

            if (_smtpSettings.EnableAuth)
            {
                SetProgress(60, "Authenticate");

                client.Authenticate(_smtpSettings.CredentialsUserName,
                    _smtpSettings.CredentialsUserPassword, cancellationToken);
            }

            SetProgress(80, "Send test message");

            client.Send(FormatOptions.Default, mimeMessage, cancellationToken);

        }
        catch (AuthorizingException authError)
        {
            Error = Resource.ErrorAccessDenied; // "No permissions to perform this action";
            _logger.Error(Error, new SecurityException(Error, authError));
        }
        catch (AggregateException ae)
        {
            ae.Flatten().Handle(e => e is TaskCanceledException || e is OperationCanceledException);
        }
        catch (SocketException ex)
        {
            Error = ex.Message; //TODO: Add translates of ordinary cases
            _logger.Error(ex.ToString());
        }
        catch (AuthenticationException ex)
        {
            Error = ex.Message; //TODO: Add translates of ordinary cases
            _logger.Error(ex.ToString());
        }
        catch (Exception ex)
        {
            Error = ex.Message; //TODO: Add translates of ordinary cases
            _logger.Error(ex.ToString());
        }
        finally
        {
            try
            {
                TaskInfo.SetProperty(FINISHED, true);
                PublishTaskInfo();

                _securityContext.Logout();
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("LdapOperation finalization problem. {0}", ex);
            }
        }
    }

    public SmtpClient GetSmtpClient()
    {
        var client = new SmtpClient
        {
            Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds
        };

        return client;
    }

    public virtual DistributedTask GetDistributedTask()
    {
        FillDistributedTask();
        return TaskInfo;
    }

    protected virtual void FillDistributedTask()
    {
        TaskInfo.SetProperty(SOURCE, Source);
        TaskInfo.SetProperty(OWNER, CurrentTenant);
        TaskInfo.SetProperty(PROGRESS, Progress < 100 ? Progress : 100);
        TaskInfo.SetProperty(RESULT, Status);
        TaskInfo.SetProperty(ERROR, Error);
        //TaskInfo.SetProperty(PROCESSED, successProcessed);
    }

    protected int GetProgress()
    {
        return Progress;
    }

    const string PROGRESS_STRING = "Progress: {0}% {1} {2}";

    public void SetProgress(int? currentPercent = null, string currentStatus = null, string currentSource = null)
    {
        if (!currentPercent.HasValue && currentStatus == null && currentSource == null)
            return;

        if (currentPercent.HasValue)
            Progress = currentPercent.Value;

        if (currentStatus != null)
            Status = currentStatus;

        if (currentSource != null)
            Source = currentSource;

        _logger.InfoFormat(PROGRESS_STRING, Progress, Status, Source);

        PublishTaskInfo();
    }

    protected void PublishTaskInfo()
    {
        FillDistributedTask();
        TaskInfo.PublishChanges();
    }
}