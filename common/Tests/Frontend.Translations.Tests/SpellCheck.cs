using System;
using System.IO;

using WeCantSpell.Hunspell;

namespace Frontend.Translations.Tests
{
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
}
