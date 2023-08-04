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

namespace ASC.Web.Studio.UserControls.Management;

public class AuthService
{
    public Consumer Consumer { get; set; }

    public string Name { get { return Consumer.Name; } }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public string Instruction { get; private set; }

    public bool CanSet { get { return Consumer.CanSet; } }

    public int? Order { get { return Consumer.Order; } }

    public List<AuthKey> Props { get; private set; }

    public AuthService(Consumer consumer)
    {
        Consumer = consumer;
        Title = ConsumerExtension.GetResourceString(consumer.Name) ?? consumer.Name;
        Description = ConsumerExtension.GetResourceString(consumer.Name + "Description");
        Instruction = ConsumerExtension.GetResourceString(consumer.Name + "Instruction");
        Props = new List<AuthKey>();

        foreach (var item in consumer.ManagedKeys)
        {
            Props.Add(new AuthKey { Name = item, Value = Consumer[item], Title = ConsumerExtension.GetResourceString(item) ?? item });
        }
    }
}

public static class ConsumerExtension
{
    public static string GetResourceString(string resourceKey)
    {
        try
        {
            Resource.ResourceManager.IgnoreCase = true;
            return Resource.ResourceManager.GetString("Consumers" + resourceKey);
        }
        catch
        {
            return null;
        }
    }
}

[DebuggerDisplay("({Name},{Value})")]
public class AuthKey
{
    public string Name { get; set; }

    public string Value { get; set; }

    public string Title { get; set; }
}
