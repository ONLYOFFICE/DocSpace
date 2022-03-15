namespace ASC.AuditTrail.Mappers;

internal static class LoginActionsMapper
{
    public static Dictionary<MessageAction, MessageMaps> GetMaps() =>
        new Dictionary<MessageAction, MessageMaps>
        {
                    { MessageAction.LoginSuccess, new MessageMaps { ActionTextResourceName = "LoginSuccess"} },
                    { MessageAction.LoginSuccessViaSocialAccount, new MessageMaps { ActionTextResourceName = "LoginSuccessSocialAccount"} },
                    { MessageAction.LoginSuccessViaSocialApp, new MessageMaps { ActionTextResourceName = "LoginSuccessSocialApp"} },
                    { MessageAction.LoginSuccessViaSms, new MessageMaps { ActionTextResourceName = "LoginSuccessViaSms"} },
                    { MessageAction.LoginSuccessViaApi, new MessageMaps { ActionTextResourceName = "LoginSuccessViaApi"} },
                    { MessageAction.LoginSuccessViaApiSms, new MessageMaps { ActionTextResourceName = "LoginSuccessViaApiSms"} },
                    { MessageAction.LoginSuccessViaApiTfa, new MessageMaps { ActionTextResourceName = "LoginSuccessViaApiTfa"} },
                    { MessageAction.LoginSuccessViaApiSocialAccount, new MessageMaps { ActionTextResourceName = "LoginSuccessViaSocialAccount"} },
                    { MessageAction.LoginSuccessViaSSO, new MessageMaps { ActionTextResourceName = "LoginSuccessViaSSO"} },
                    { MessageAction.LoginSuccesViaTfaApp, new MessageMaps { ActionTextResourceName = "LoginSuccesViaTfaApp"} },
                    { MessageAction.LoginFailInvalidCombination, new MessageMaps { ActionTextResourceName = "LoginFailInvalidCombination" } },
                    { MessageAction.LoginFailSocialAccountNotFound, new MessageMaps { ActionTextResourceName = "LoginFailSocialAccountNotFound" } },
                    { MessageAction.LoginFailDisabledProfile, new MessageMaps { ActionTextResourceName = "LoginFailDisabledProfile" } },
                    { MessageAction.LoginFail, new MessageMaps { ActionTextResourceName = "LoginFail" } },
                    { MessageAction.LoginFailViaSms, new MessageMaps { ActionTextResourceName = "LoginFailViaSms" } },
                    { MessageAction.LoginFailViaApi, new MessageMaps { ActionTextResourceName = "LoginFailViaApi" } },
                    { MessageAction.LoginFailViaApiSms, new MessageMaps { ActionTextResourceName = "LoginFailViaApiSms" } },
                    { MessageAction.LoginFailViaApiTfa, new MessageMaps { ActionTextResourceName = "LoginFailViaApiTfa" } },
                    { MessageAction.LoginFailViaApiSocialAccount, new MessageMaps { ActionTextResourceName = "LoginFailViaApiSocialAccount" } },
                    { MessageAction.LoginFailViaTfaApp, new MessageMaps { ActionTextResourceName = "LoginFailViaTfaApp" } },
                    { MessageAction.LoginFailIpSecurity, new MessageMaps { ActionTextResourceName = "LoginFailIpSecurity" } },
                    { MessageAction.LoginFailViaSSO, new MessageMaps { ActionTextResourceName = "LoginFailViaSSO"}},
                    { MessageAction.LoginFailBruteForce, new MessageMaps { ActionTextResourceName = "LoginFailBruteForce" } },
                    { MessageAction.LoginFailRecaptcha, new MessageMaps { ActionTextResourceName = "LoginFailRecaptcha" } },
                    { MessageAction.Logout, new MessageMaps { ActionTextResourceName = "Logout" } },
                    { MessageAction.SessionStarted, new MessageMaps { ActionTextResourceName = "SessionStarted" } },
                    { MessageAction.SessionCompleted, new MessageMaps { ActionTextResourceName = "SessionCompleted" } }
        };
}