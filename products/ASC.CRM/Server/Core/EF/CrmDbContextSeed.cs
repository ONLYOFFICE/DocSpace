using System;

using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Configuration;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.Web.CRM.Classes;

namespace ASC.CRM.Core.EF
{
    public static class CrmDbContextSeed
    {
        public static void SeedInitPortalData(SettingsManager settingsManager,
                                          DaoFactory daoFactory,
                                          CoreConfiguration coreConfiguration)
        {
            var tenantSettings = settingsManager.Load<CrmSettings>();

            if (!tenantSettings.IsConfiguredPortal)
            {
                // Task Category
                var listItemDao = daoFactory.GetListItemDao();

                listItemDao.CreateItem(ListType.TaskCategory, new ListItem(CRMTaskResource.TaskCategory_Call, "task_category_call.png"));
                listItemDao.CreateItem(ListType.TaskCategory, new ListItem(CRMTaskResource.TaskCategory_Deal, "task_category_deal.png"));
                listItemDao.CreateItem(ListType.TaskCategory, new ListItem(CRMTaskResource.TaskCategory_Demo, "task_category_demo.png"));
                listItemDao.CreateItem(ListType.TaskCategory, new ListItem(CRMTaskResource.TaskCategory_Email, "task_category_email.png"));
                listItemDao.CreateItem(ListType.TaskCategory, new ListItem(CRMTaskResource.TaskCategory_Fax, "task_category_fax.png"));
                listItemDao.CreateItem(ListType.TaskCategory, new ListItem(CRMTaskResource.TaskCategory_FollowUP, "task_category_follow_up.png"));
                listItemDao.CreateItem(ListType.TaskCategory, new ListItem(CRMTaskResource.TaskCategory_Lunch, "task_category_lunch.png"));
                listItemDao.CreateItem(ListType.TaskCategory, new ListItem(CRMTaskResource.TaskCategory_Meeting, "task_category_meeting.png"));
                listItemDao.CreateItem(ListType.TaskCategory, new ListItem(CRMTaskResource.TaskCategory_Note, "task_category_note.png"));
                listItemDao.CreateItem(ListType.TaskCategory, new ListItem(CRMTaskResource.TaskCategory_Ship, "task_category_ship.png"));
                listItemDao.CreateItem(ListType.TaskCategory, new ListItem(CRMTaskResource.TaskCategory_SocialNetworks, "task_category_social_networks.png"));
                listItemDao.CreateItem(ListType.TaskCategory, new ListItem(CRMTaskResource.TaskCategory_ThankYou, "task_category_thank_you.png"));

                // Deal Milestone New
                var milestoneDao = daoFactory.GetDealMilestoneDao();

                milestoneDao.Create(new DealMilestone
                {
                    Title = CRMDealResource.DealMilestone_InitialContact_Title,
                    Description = CRMDealResource.DealMilestone_InitialContact_Description,
                    Probability = 1,
                    Color = "#e795c1",
                    Status = DealMilestoneStatus.Open
                });

                milestoneDao.Create(new DealMilestone
                {
                    Title = CRMDealResource.DealMilestone_Preapproach_Title,
                    Description = CRMDealResource.DealMilestone_Preapproach_Description,
                    Probability = 2,
                    Color = "#df7895",
                    Status = DealMilestoneStatus.Open
                });

                milestoneDao.Create(new DealMilestone
                {
                    Title = CRMDealResource.DealMilestone_Suspect_Title,
                    Description = CRMDealResource.DealMilestone_Suspect_Description,
                    Probability = 3,
                    Color = "#f48454",
                    SortOrder = 1,
                    Status = DealMilestoneStatus.Open
                });

                milestoneDao.Create(new DealMilestone
                {
                    Title = CRMDealResource.DealMilestone_Champion_Title,
                    Description = CRMDealResource.DealMilestone_Champion_Description,
                    Probability = 20,
                    Color = "#b58fd6",
                    SortOrder = 2,
                    Status = DealMilestoneStatus.Open
                });

                milestoneDao.Create(new DealMilestone
                {
                    Title = CRMDealResource.DealMilestone_Opportunity_Title,
                    Description = CRMDealResource.DealMilestone_Opportunity_Description,
                    Probability = 50,
                    Color = "#d28cc8",
                    SortOrder = 3,
                    Status = DealMilestoneStatus.Open
                });

                milestoneDao.Create(new DealMilestone
                {
                    Title = CRMDealResource.DealMilestone_Prospect_Title,
                    Description = CRMDealResource.DealMilestone_Prospect_Description,
                    Probability = 75,
                    Color = "#ffb45e",
                    SortOrder = 4,
                    Status = DealMilestoneStatus.Open
                });

                milestoneDao.Create(new DealMilestone
                {
                    Title = CRMDealResource.DealMilestone_Verbal_Title,
                    Description = CRMDealResource.DealMilestone_Verbal_Description,
                    Probability = 90,
                    Color = "#ffd267",
                    SortOrder = 5,
                    Status = DealMilestoneStatus.Open
                });

                milestoneDao.Create(new DealMilestone
                {
                    Title = CRMDealResource.DealMilestone_Won_Title,
                    Description = CRMDealResource.DealMilestone_Won_Description,
                    Probability = 100,
                    Color = "#6bbd72",
                    SortOrder = 6,
                    Status = DealMilestoneStatus.ClosedAndWon
                });

                milestoneDao.Create(new DealMilestone
                {
                    Title = CRMDealResource.DealMilestone_Lost_Title,
                    Description = CRMDealResource.DealMilestone_Lost_Description,
                    Probability = 0,
                    Color = "#f2a9be",
                    SortOrder = 7,
                    Status = DealMilestoneStatus.ClosedAndLost
                });

                // Contact Status
                listItemDao.CreateItem(ListType.ContactStatus, new ListItem { Title = CRMContactResource.ContactStatus_Cold, Color = "#8a98d8", SortOrder = 1 });
                listItemDao.CreateItem(ListType.ContactStatus, new ListItem { Title = CRMContactResource.ContactStatus_Warm, Color = "#ffd267", SortOrder = 2 });
                listItemDao.CreateItem(ListType.ContactStatus, new ListItem { Title = CRMContactResource.ContactStatus_Hot, Color = "#df7895", SortOrder = 3 });
                // Contact Type
                listItemDao.CreateItem(ListType.ContactType, new ListItem { Title = CRMContactResource.ContactType_Client, SortOrder = 1 });
                listItemDao.CreateItem(ListType.ContactType, new ListItem { Title = CRMContactResource.ContactType_Supplier, SortOrder = 2 });
                listItemDao.CreateItem(ListType.ContactType, new ListItem { Title = CRMContactResource.ContactType_Partner, SortOrder = 3 });
                listItemDao.CreateItem(ListType.ContactType, new ListItem { Title = CRMContactResource.ContactType_Competitor, SortOrder = 4 });

                // History Category
                listItemDao.CreateItem(ListType.HistoryCategory, new ListItem(CRMCommonResource.HistoryCategory_Note, "event_category_note.png"));
                listItemDao.CreateItem(ListType.HistoryCategory, new ListItem(CRMCommonResource.HistoryCategory_Email, "event_category_email.png"));
                listItemDao.CreateItem(ListType.HistoryCategory, new ListItem(CRMCommonResource.HistoryCategory_Call, "event_category_call.png"));
                listItemDao.CreateItem(ListType.HistoryCategory, new ListItem(CRMCommonResource.HistoryCategory_Meeting, "event_category_meeting.png"));

                // Tags
                daoFactory.GetTagDao().AddTag(EntityType.Contact, CRMContactResource.Lead, true);
                daoFactory.GetTagDao().AddTag(EntityType.Contact, CRMContactResource.Customer, true);
                daoFactory.GetTagDao().AddTag(EntityType.Contact, CRMContactResource.Supplier, true);
                daoFactory.GetTagDao().AddTag(EntityType.Contact, CRMContactResource.Staff, true);

                tenantSettings.WebFormKey = Guid.NewGuid();
                tenantSettings.IsConfiguredPortal = true;

                if (!settingsManager.Save(tenantSettings))
                {
                    throw new Exception("not save CRMSettings");
                }
            }

            if (!tenantSettings.IsConfiguredSmtp)
            {
                var smtp = settingsManager.Load<CrmSettings>().SMTPServerSettingOld;

                if (smtp != null && coreConfiguration.SmtpSettings.IsDefaultSettings)
                {
                    var newSettings = new SmtpSettings(smtp.Host, smtp.Port, smtp.SenderEmailAddress,
                        smtp.SenderDisplayName)
                    {
                        EnableSSL = smtp.EnableSSL,
                        EnableAuth = smtp.RequiredHostAuthentication,
                    };

                    if (!string.IsNullOrEmpty(smtp.HostLogin) && !string.IsNullOrEmpty(smtp.HostPassword))
                    {
                        newSettings.SetCredentials(smtp.HostLogin, smtp.HostPassword);
                    }

                    coreConfiguration.SmtpSettings = newSettings;

                }

                tenantSettings.IsConfiguredSmtp = true;

                if (!settingsManager.Save(tenantSettings))
                {
                    throw new Exception("not save CRMSettings");
                }
            }
        }
    }
}
