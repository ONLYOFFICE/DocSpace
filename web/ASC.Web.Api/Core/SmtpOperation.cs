// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using AuthenticationException = System.Security.Authentication.AuthenticationException;
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
    private readonly ILogger<SmtpOperation> _logger;
    private readonly SmtpSettingsDto _smtpSettings;

    private readonly string _messageSubject;
    private readonly string _messageBody;


    public SmtpOperation(
        SmtpSettingsDto smtpSettings,
        int tenant,
        Guid user,
        UserManager userManager,
        SecurityContext securityContext,
        TenantManager tenantManager,
        ILogger<SmtpOperation> logger)
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

        _logger = logger;
    }

    public async Task RunJob(DistributedTask distributedTask, CancellationToken cancellationToken)
    {
        try
        {
            CancellationToken = cancellationToken;

            SetProgress(5, "Setup tenant");

            await _tenantManager.SetCurrentTenantAsync(CurrentTenant);

            SetProgress(10, "Setup user");

            await _securityContext.AuthenticateMeWithoutCookieAsync(CurrentUser); //Core.Configuration.Constants.CoreSystem);

            SetProgress(15, "Find user data");

            var currentUser = await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID);

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
            _logger.ErrorWithException(new SecurityException(Error, authError));
        }
        catch (AggregateException ae)
        {
            ae.Flatten().Handle(e => e is TaskCanceledException || e is OperationCanceledException);
        }
        catch (SocketException ex)
        {
            Error = ex.Message; //TODO: Add translates of ordinary cases
            _logger.ErrorWithException(ex);
        }
        catch (AuthenticationException ex)
        {
            Error = ex.Message; //TODO: Add translates of ordinary cases
            _logger.ErrorWithException(ex);
        }
        catch (Exception ex)
        {
            Error = ex.Message; //TODO: Add translates of ordinary cases
            _logger.ErrorWithException(ex);
        }
        finally
        {
            try
            {
                TaskInfo[FINISHED] = true;
                PublishTaskInfo();

                _securityContext.Logout();
            }
            catch (Exception ex)
            {
                _logger.ErrorLdapOperationFinalizationProblem(ex);
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
        TaskInfo[SOURCE] = Source;
        TaskInfo[OWNER] = CurrentTenant;
        TaskInfo[PROGRESS] = Progress < 100 ? Progress : 100;
        TaskInfo[RESULT] = Status;
        TaskInfo[ERROR] = Error;
        //TaskInfo.SetProperty(PROCESSED, successProcessed);
    }

    protected int GetProgress()
    {
        return Progress;
    }

    public void SetProgress(int? currentPercent = null, string currentStatus = null, string currentSource = null)
    {
        if (!currentPercent.HasValue && currentStatus == null && currentSource == null)
        {
            return;
        }

        if (currentPercent.HasValue)
        {
            Progress = currentPercent.Value;
        }

        if (currentStatus != null)
        {
            Status = currentStatus;
        }

        if (currentSource != null)
        {
            Source = currentSource;
        }

        _logger.InformationProgress(Progress, Status, Source);

        PublishTaskInfo();
    }

    protected void PublishTaskInfo()
    {
        FillDistributedTask();
        TaskInfo.PublishChanges();
    }
}