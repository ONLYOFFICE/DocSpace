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

namespace ASC.Web.Files.Classes;

[Scope]
public class SubscriptionManager : IProductSubscriptionManager
{
    private readonly Guid _subscrTypeDocuSignComplete = new Guid("{0182E476-D63D-46ED-B928-104861507811}");
    private readonly Guid _subscrTypeDocuSignStatus = new Guid("{ED7F93CD-7575-40EB-86EB-82FBA23171D2}");
    private readonly Guid _subscrTypeShareDoc = new Guid("{552846EC-AC94-4408-AAC6-17C8989B8B38}");
    private readonly Guid _subscrTypeShareFolder = new Guid("{0292A4F4-0687-42a6-9CE4-E21215045ABE}");
    private readonly Guid _subscrTypeMailMerge = new Guid("{FB5858EC-046C-41E2-84C9-B44BF7884514}");
    private readonly Guid _subscrTypeEditorMentions = new Guid("{9D3CAB90-5718-4E82-959F-27EC83BFBC5F}");

    public GroupByType GroupByType => GroupByType.Simple;

    public SubscriptionManager(CoreBaseSettings coreBaseSettings, NotifySource notifySource)
    {
        _coreBaseSettings = coreBaseSettings;
        _notifySource = notifySource;
    }

    public List<SubscriptionObject> GetSubscriptionObjects(Guid subItem)
    {
        return new List<SubscriptionObject>();
    }

    public List<SubscriptionType> GetSubscriptionTypes()
    {
        var subscriptionTypes = new List<SubscriptionType>
                                    {
                                        new SubscriptionType
                                            {
                                                ID = _subscrTypeShareDoc,
                                                Name = FilesCommonResource.SubscriptForAccess,
                                                NotifyAction = NotifyConstants.EventShareDocument,
                                                Single = true,
                                                CanSubscribe = true
                                            },
                                        new SubscriptionType
                                            {
                                                ID = _subscrTypeShareFolder,
                                                Name = FilesCommonResource.ShareFolder,
                                                NotifyAction = NotifyConstants.EventShareFolder,
                                                Single = true,
                                                CanSubscribe = true
                                            },
                                        new SubscriptionType
                                            {
                                                ID = _subscrTypeMailMerge,
                                                Name = FilesCommonResource.SubscriptForMailMerge,
                                                NotifyAction = NotifyConstants.EventMailMergeEnd,
                                                Single = true,
                                                CanSubscribe = true
                                            },
                                        new SubscriptionType
                                            {
                                                ID = _subscrTypeEditorMentions,
                                                Name = FilesCommonResource.EditorMentions,
                                                NotifyAction = NotifyConstants.EventEditorMentions,
                                                Single = true,
                                                CanSubscribe = true
                                            },
                                    };

        if (_coreBaseSettings.CustomMode)
        {
            return subscriptionTypes;
        }

        subscriptionTypes.AddRange(new List<SubscriptionType>
                                    {
                                        new SubscriptionType
                                            {
                                                ID = _subscrTypeDocuSignComplete,
                                                Name = FilesCommonResource.SubscriptDocuSignComplete,
                                                NotifyAction = NotifyConstants.EventDocuSignComplete,
                                                Single = true,
                                                CanSubscribe = true
                                            },
                                        new SubscriptionType
                                            {
                                                ID = _subscrTypeDocuSignStatus,
                                                Name = FilesCommonResource.SubscriptDocuSignStatus,
                                                NotifyAction = NotifyConstants.EventDocuSignStatus,
                                                Single = true,
                                                CanSubscribe = true
                                            }
                                    });

        return subscriptionTypes;
    }

    public ISubscriptionProvider SubscriptionProvider => _notifySource.GetSubscriptionProvider();

    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly NotifySource _notifySource;

    public List<SubscriptionGroup> GetSubscriptionGroups()
    {
        return new List<SubscriptionGroup>();
    }
}
