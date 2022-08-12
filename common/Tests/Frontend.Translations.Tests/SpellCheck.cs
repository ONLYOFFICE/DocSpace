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

using System;
using System.IO;

using WeCantSpell.Hunspell;

namespace Frontend.Translations.Tests;

public static class SpellCheck
{
    public class DicPaths
    {
        public string DictionaryPath { get; set; }
        public string AffixPath { get; set; }
    }

    public static Models.SpellCheckResult HasSpellIssues(string text, string language, WordList dictionary)
    {
        var result = new Models.SpellCheckResult(text, language);

        foreach (var word in result.Words)
        {
            if (!dictionary.Check(word))
            {
                result.SpellIssues.Add(new Models.SpellIssue(word, dictionary.Suggest(word)));
            }
        }

        return result;
    }

    public static DicPaths GetDictionaryPaths(string lng)
    {
        const string dictionariesPath = @"..\..\..\dictionaries";
        const string additionalPath = @"..\..\..\additional";

        var path = dictionariesPath;
        string language;

        // az,bg,cs,de,el,en,en-US,es,fi,fr,it,ja,ko,lo,lv,nl,pl,pt,pt-BR,ro,ru,sk,sl,tr,uk,vi,zh-CN
        switch (lng)
        {
            case "az":
                language = "az_Latn_AZ";
                break;
            case "bg":
                language = "bg_BG";
                break;
            case "cs":
                language = "cs_CZ";
                break;
            case "de":
                language = "de_DE";
                break;
            case "el":
                language = "el_GR";
                break;
            case "en":
                language = "en_GB";
                break;
            case "en-US":
                language = "en_US";
                break;
            case "es":
                language = "es_ES";
                break;
            case "fi":
                language = "fi_FI";
                path = additionalPath;
                break;
            case "fr":
                language = "fr_FR";
                break;
            case "it":
                language = "it_IT";
                break;
            //case "ja":
            //    language = "";
            //break;
            case "ko":
                language = "ko_KR";
                break;
            case "lo":
                language = "lo_LA";
                path = additionalPath;
                break;
            case "lv":
                language = "lv_LV";
                break;
            case "nl":
                language = "nl_NL";
                break;
            case "pl":
                language = "pl_PL";
                break;
            case "pt":
                language = "pt_PT";
                break;
            case "pt-BR":
                language = "pt_BR";
                break;
            case "ro":
                language = "ro_RO";
                break;
            case "ru":
                language = "ru_RU";
                break;
            case "sk":
                language = "sk_SK";
                break;
            case "sl":
                language = "sl_SI";
                break;
            case "tr":
                language = "tr_TR";
                break;
            case "uk":
                language = "uk_UA";
                break;
            case "vi":
                language = "vi_VN";
                break;
            //case "zh-CN":
            //    language = "";
            //break;
            default:
                throw new NotSupportedException();
        }

        var dicPath = Path.Combine(path, language, $"{language}.dic");
        var affPath = Path.Combine(path, language, $"{language}.aff");

        return new DicPaths
        {
            DictionaryPath = dicPath,
            AffixPath = affPath
        };
    }
}
