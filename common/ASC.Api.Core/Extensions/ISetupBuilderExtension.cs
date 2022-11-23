﻿// (c) Copyright Ascensio System SIA 2010-2022
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

namespace ASC.Api.Core.Extensions;

public static class ISetupBuilderExtension
{
    public static ISetupBuilder LoadConfiguration(this ISetupBuilder loggingBuilder, IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        var conf = new XmlLoggingConfiguration(CrossPlatform.PathCombine(configuration["pathToConf"], "nlog.config"));

        var settings = new ConfigurationExtension(configuration).GetSetting<NLogSettings>("log");

        if (!string.IsNullOrEmpty(settings.Name))
        {
            conf.Variables["name"] = settings.Name;
        }

        if (!string.IsNullOrEmpty(settings.Dir))
        {
            var dir = Path.IsPathRooted(settings.Dir) ? settings.Dir : CrossPlatform.PathCombine(hostEnvironment.ContentRootPath, settings.Dir);
            conf.Variables["dir"] = dir.TrimEnd('/').TrimEnd('\\') + Path.DirectorySeparatorChar;
        }

        foreach (var targetName in new[] { "aws", "aws_sql" })
        {
            var awsTarget = conf.FindTargetByName<AWSTarget>(targetName);

            if (awsTarget == null) continue;

            //hack
            if (!string.IsNullOrEmpty(settings.Name))
            {
                awsTarget.LogGroup = awsTarget.LogGroup.Replace("${var:name}", settings.Name);
            }


            var awsAccessKeyId = string.IsNullOrEmpty(settings.AWSAccessKeyId) ? configuration["aws:cloudWatch:accessKeyId"] : settings.AWSAccessKeyId;
            var awsSecretAccessKey = string.IsNullOrEmpty(settings.AWSSecretAccessKey) ? configuration["aws:cloudWatch:secretAccessKey"] : settings.AWSSecretAccessKey;

            if (!string.IsNullOrEmpty(awsAccessKeyId))
            {

                awsTarget.LogGroup = String.IsNullOrEmpty(configuration["aws:cloudWatch:logGroupName"]) ? awsTarget.LogGroup : configuration["aws:cloudWatch:logGroupName"];
                awsTarget.Region = String.IsNullOrEmpty(configuration["aws:cloudWatch:region"]) ? awsTarget.Region : configuration["aws:cloudWatch:region"];

                awsTarget.Credentials = new Amazon.Runtime.BasicAWSCredentials(awsAccessKeyId, awsSecretAccessKey);
            }
        }

        return loggingBuilder.LoadConfiguration(conf);
    }
}