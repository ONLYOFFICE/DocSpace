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




namespace ASC.AuditTrail.Mappers;

public class MessageMaps
{
    public ActionType ActionType { get; }
    public ProductType ProductType { get; protected set; }
    public ModuleType ModuleType { get; }
    public EntryType EntryType1 { get; }
    public EntryType EntryType2 { get; }

    public string ActionTextResourceName { get; }


    public MessageMaps(string actionTextResourceName,
        ActionType? actionType = null,
        ProductType? productType = null,
        ModuleType? moduleType = null,
        EntryType? entryType1 = null,
        EntryType? entryType2 = null)
    {
        ActionTextResourceName = actionTextResourceName;

        if (actionType.HasValue)
        {
            ActionType = actionType.Value;
        }

        if (productType.HasValue)
        {
            ProductType = productType.Value;
        }

        if (moduleType.HasValue)
        {
            ModuleType = moduleType.Value;
        }

        if (entryType1.HasValue)
        {
            EntryType1 = entryType1.Value;
        }

        if (entryType2.HasValue)
        {
            EntryType2 = entryType2.Value;
        }
    }

    public string GetActionTypeText()
    {
        try
        {
            return AuditReportResource.ResourceManager.GetString(ActionType.ToString() + "ActionType");
        }
        catch
        {
            return null;
        }
    }

    public string GetActionText()
    {
        try
        {
            return AuditReportResource.ResourceManager.GetString(ActionTextResourceName);
        }
        catch
        {
            return null;
        }
    }

    public string GetProductText()
    {
        try
        {
            return AuditReportResource.ResourceManager.GetString(ProductType == ProductType.CRM ? "CrmProduct" : ProductType.ToString() + "Product");
        }
        catch
        {
            return null;
        }
    }

    public string GetModuleText()
    {
        try
        {
            return AuditReportResource.ResourceManager.GetString(ModuleType.ToString() + "Module");
        }
        catch
        {
            return null;
        }
    }
}

internal class MessageMapsDictionary : IDictionary<MessageAction, MessageMaps>
{
    private readonly ProductType _productType;
    private readonly ModuleType _moduleType;
    private IDictionary<MessageAction, MessageMaps> Actions { get; }

    public ICollection<MessageAction> Keys
    {
        get
        {
            return Actions.Keys;
        }
    }

    public ICollection<MessageMaps> Values
    {
        get
        {
            return Actions.Values;
        }
    }

    public int Count
    {
        get
        {
            return Actions.Count;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return Actions.IsReadOnly;
        }
    }

    public MessageMaps this[MessageAction key]
    {
        get
        {
            return Actions[key];
        }
        set
        {
            Actions[key] = value;
        }
    }

    public MessageMapsDictionary()
    {
        Actions = new Dictionary<MessageAction, MessageMaps>();
    }

    public MessageMapsDictionary(ProductType productType, ModuleType moduleType) : this()
    {
        _productType = productType;
        _moduleType = moduleType;
    }

    public void Add(MessageAction[] value)
    {
        foreach (var item in value)
        {
            Add(item);
        }
    }

    public void Add(EntryType key, Dictionary<ActionType, MessageAction[]> value)
    {
        foreach (var item in value)
        {
            foreach (var messageAction in item.Value)
            {
                Add(messageAction, item.Key, key);
            }
        }
    }

    public void Add(EntryType key, Dictionary<ActionType, MessageAction[]> value, Dictionary<ActionType, MessageAction> value1)
    {
        foreach (var item in value)
        {
            foreach (var messageAction in item.Value)
            {
                Add(messageAction, item.Key, key);
            }
        }

        Add(key, value1);
    }

    public void Add(EntryType key, Dictionary<ActionType, MessageAction> value)
    {
        foreach (var item in value)
        {
            Add(item.Value, item.Key, key);
        }
    }

    public void Add(ActionType key, MessageAction[] value)
    {
        foreach (var item in value)
        {
            Add(item, key);
        }
    }

    public void Add(EntryType entryType1, EntryType entryType2, Dictionary<ActionType, MessageAction> value)
    {
        foreach (var item in value)
        {
            Add(item.Value, item.Key, entryType1, entryType2);
        }
    }

    public void Add(EntryType entryType1, EntryType entryType2, Dictionary<ActionType, MessageAction[]> value)
    {
        foreach (var item in value)
        {
            foreach (var messageAction in item.Value)
            {
                Add(messageAction, item.Key, entryType1, entryType2);
            }
        }
    }

    public MessageMapsDictionary Add(MessageAction action,
        ActionType? actionType = null,
        EntryType? entryType1 = null,
        EntryType? entryType2 = null,
        ProductType? productType = null,
        ModuleType? moduleType = null)
    {
        var map = new MessageMaps(action.ToString(), actionType, productType ?? _productType, moduleType ?? _moduleType, entryType1, entryType2);
        Actions.Add(new KeyValuePair<MessageAction, MessageMaps>(action, map));
        return this;
    }

    public bool ContainsKey(MessageAction key)
    {
        return Actions.ContainsKey(key);
    }

    public void Add(MessageAction key, MessageMaps value)
    {
        Actions.Add(key, value);
    }

    public bool Remove(MessageAction key)
    {
        return Actions.Remove(key);
    }

    public bool TryGetValue(MessageAction key, out MessageMaps value)
    {
        return Actions.TryGetValue(key, out value);
    }

    public void Add(KeyValuePair<MessageAction, MessageMaps> item)
    {
        Actions.Add(item);
    }

    public void Clear()
    {
        Actions.Clear();
    }

    public bool Contains(KeyValuePair<MessageAction, MessageMaps> item)
    {
        return Actions.Contains(item);
    }

    public void CopyTo(KeyValuePair<MessageAction, MessageMaps>[] array, int arrayIndex)
    {
        Actions.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<MessageAction, MessageMaps> item)
    {
        return Actions.Remove(item);
    }

    public IEnumerator<KeyValuePair<MessageAction, MessageMaps>> GetEnumerator()
    {
        return Actions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Actions.GetEnumerator();
    }
}