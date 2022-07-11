/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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


using System;
using System.Security.Cryptography;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Security.Cryptography;

namespace ASC.Core.Encryption
{
    public class EncryptionSettings
    {

        internal string password;

        public string Password
        {
            get { return password; }
            set { password = (value ?? string.Empty).Replace('#', '_'); }
        }

        public EncryprtionStatus Status { get; set; }

        public bool NotifyUsers { get; set; }

        public EncryptionSettings(EncryptionSettingsProto encryptionSettingsProto)
        {
            Password = encryptionSettingsProto.Password;
            Status = encryptionSettingsProto.Status;
            NotifyUsers = encryptionSettingsProto.NotifyUsers;
        }

        public EncryptionSettings()
        {
            Password = string.Empty;
            Status = EncryprtionStatus.Decrypted;
            NotifyUsers = true;
        }
    }

    [Scope]
    public class EncryptionSettingsHelper
    {
        private const string key = "EncryptionSettings";

        private CoreConfiguration CoreConfiguration { get; }
        private AscCacheNotify AscCacheNotify { get; }
        private InstanceCrypto InstanceCrypto { get; }

        public EncryptionSettingsHelper(CoreConfiguration coreConfiguration, AscCacheNotify ascCacheNotify, InstanceCrypto instanceCrypto)
        {
            CoreConfiguration = coreConfiguration;
            AscCacheNotify = ascCacheNotify;
            InstanceCrypto = instanceCrypto;
        }

        public void Save(EncryptionSettings encryptionSettings)
        {
            var settings = Serialize(encryptionSettings);
            CoreConfiguration.SaveSetting(key, settings);

            AscCacheNotify.ClearCache();
        }

        public EncryptionSettings Load()
        {
            var settings = CoreConfiguration.GetSetting(key);

            return Deserialize(settings);
        }

        public string Serialize(EncryptionSettings encryptionSettings)
        {
            return string.Join("#",
                string.IsNullOrEmpty(encryptionSettings.password) ? string.Empty : InstanceCrypto.Encrypt(encryptionSettings.password),
                (int)encryptionSettings.Status,
                encryptionSettings.NotifyUsers
            );
        }

        public EncryptionSettings Deserialize(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new EncryptionSettings();
            }

            var parts = value.Split(new[] { '#' }, StringSplitOptions.None);

            var password = string.IsNullOrEmpty(parts[0]) ? string.Empty : InstanceCrypto.Decrypt(parts[0]);
            var status = int.Parse(parts[1]);
            var notifyUsers = bool.Parse(parts[2]);

            return new EncryptionSettings
            {
                Password = password,
                Status = (EncryprtionStatus)status,
                NotifyUsers = notifyUsers
            };
        }

        // source System.Web.Security.Membership.GeneratePassword
        public string GeneratePassword(int length, int numberOfNonAlphanumericCharacters)
        {
            var punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();

            if (length < 1 || length > 128)
            {
                throw new ArgumentException("password_length_incorrect", nameof(length));
            }

            if (numberOfNonAlphanumericCharacters > length || numberOfNonAlphanumericCharacters < 0)
            {
                throw new ArgumentException("min_required_non_alphanumeric_characters_incorrect", nameof(numberOfNonAlphanumericCharacters));
            }

            var array2 = new char[length];
            var num = 0;

            var array = RandomNumberGenerator.GetBytes(length);

            for (var i = 0; i < length; i++)
            {
                var num2 = array[i] % 87;
                if (num2 < 10)
                {
                    array2[i] = (char)(48 + num2);
                    continue;
                }

                if (num2 < 36)
                {
                    array2[i] = (char)(65 + num2 - 10);
                    continue;
                }

                if (num2 < 62)
                {
                    array2[i] = (char)(97 + num2 - 36);
                    continue;
                }

                array2[i] = punctuations[num2 - 62];
                num++;
            }

            if (num < numberOfNonAlphanumericCharacters)
            {
                for (var j = 0; j < numberOfNonAlphanumericCharacters - num; j++)
                {
                    int num3;
                    do
                    {
                        num3 = RandomNumberGenerator.GetInt32(0, length);
                    }
                    while (!char.IsLetterOrDigit(array2[num3]));
                    array2[num3] = punctuations[RandomNumberGenerator.GetInt32(0, punctuations.Length)];
                }
            }

            return new string(array2);
        }
    }
}
