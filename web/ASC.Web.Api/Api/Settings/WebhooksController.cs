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

namespace ASC.Web.Api.Controllers.Settings;

public class WebhooksController : BaseSettingsController
{
    private readonly DbWorker _webhookDbWorker;

    public WebhooksController(
        ApiContext apiContext,
        WebItemManager webItemManager,
        IMemoryCache memoryCache,
        DbWorker dbWorker) : base(apiContext, memoryCache, webItemManager)
    {
        _webhookDbWorker = dbWorker;
    }

    /// <summary>
    /// Add new config for webhooks
    /// </summary>
    [Create("webhook")]
    public void CreateWebhook(WebhooksConfig model)
    {
        if (model.Uri == null)
        {
            throw new ArgumentNullException("Uri");
        }

        if (model.SecretKey == null)
        {
            throw new ArgumentNullException("SecretKey");
        }

        _webhookDbWorker.AddWebhookConfig(model);
    }

    /// <summary>
    /// Update config for webhooks
    /// </summary>
    [Update("webhook")]
    public void UpdateWebhook(WebhooksConfig model)
    {
        if (model.Uri == null)
        {
            throw new ArgumentNullException("Uri");
        }

        if (model.SecretKey == null)
        {
            throw new ArgumentNullException("SecretKey");
        }

        _webhookDbWorker.UpdateWebhookConfig(model);
    }

    /// <summary>
    /// Remove config for webhooks
    /// </summary>
    [Delete("webhook")]
    public void RemoveWebhook(WebhooksConfig model)
    {
        if (model.Uri == null)
        {
            throw new ArgumentNullException("Uri");
        }

        if (model.SecretKey == null)
        {
            throw new ArgumentNullException("SecretKey");
        }

        _webhookDbWorker.RemoveWebhookConfig(model);
    }

    /// <summary>
    /// Read Webhooks history for actual tenant
    /// </summary>
    [Read("webhooks")]
    public List<WebhooksLog> TenantWebhooks()
    {
        return _webhookDbWorker.GetTenantWebhooks();
    }
}