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

namespace ASC.Core.Notify;

public abstract class NotifySource : INotifySource
{
    private readonly IDictionary<CultureInfo, IActionProvider> _actions = new Dictionary<CultureInfo, IActionProvider>();
    private readonly IDictionary<CultureInfo, IPatternProvider> _patterns = new Dictionary<CultureInfo, IPatternProvider>();

    protected ISubscriptionProvider _subscriprionProvider;
    protected IRecipientProvider _recipientsProvider;
    protected IActionProvider ActionProvider => GetActionProvider();
    protected IPatternProvider PatternProvider => GetPatternProvider();
    public string Id { get; private set; }

    private readonly UserManager _userManager;
    private readonly SubscriptionManager _subscriptionManager;

    protected NotifySource(string id, UserManager userManager, IRecipientProvider recipientsProvider, SubscriptionManager subscriptionManager)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(id);

        Id = id;
        _userManager = userManager;
        _recipientsProvider = recipientsProvider;
        _subscriptionManager = subscriptionManager;
    }

    protected NotifySource(Guid id, UserManager userManager, IRecipientProvider recipientsProvider, SubscriptionManager subscriptionManager)
        : this(id.ToString(), userManager, recipientsProvider, subscriptionManager)
    {
    }

    public IActionProvider GetActionProvider()
    {
        lock (_actions)
        {
            var culture = CultureInfo.CurrentCulture;
            if (!_actions.ContainsKey(culture))
            {
                _actions[culture] = CreateActionProvider();
            }

            return _actions[culture];
        }
    }

    public IPatternProvider GetPatternProvider()
    {
        lock (_patterns)
        {
            var culture = CultureInfo.CurrentCulture;
            if (CultureInfo.CurrentUICulture != culture)
            {
                CultureInfo.CurrentUICulture = culture;
            }
            if (!_patterns.ContainsKey(culture))
            {
                _patterns[culture] = CreatePatternsProvider();
            }

            return _patterns[culture];
        }
    }

    public IRecipientProvider GetRecipientsProvider()
    {
        return CreateRecipientsProvider();
    }

    public ISubscriptionProvider GetSubscriptionProvider()
    {
        return CreateSubscriptionProvider();
    }

    protected abstract IPatternProvider CreatePatternsProvider();

    protected abstract IActionProvider CreateActionProvider();


    protected virtual ISubscriptionProvider CreateSubscriptionProvider()
    {
        var subscriptionProvider = new DirectSubscriptionProvider(Id, _subscriptionManager, _recipientsProvider);

        return new TopSubscriptionProvider(_recipientsProvider, subscriptionProvider, WorkContext.DefaultClientSenders)
            ?? throw new NotifyException("Provider ISubscriprionProvider not instanced.");
    }

    protected virtual IRecipientProvider CreateRecipientsProvider()
    {
        return new RecipientProviderImpl(_userManager)
            ?? throw new NotifyException("Provider IRecipientsProvider not instanced.");
    }
}
