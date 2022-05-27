using System;

using WeCantSpell.Hunspell;

namespace Frontend.Translations.Tests
{
    public static class SpellCheck
    {
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

        public static string GetDictionaryLanguage(string lng)
        {
            // az,bg,cs,de,el,en,en-US,es,fi,fr,it,ja,ko,lo,lv,nl,pl,pt,pt-BR,ro,ru,sk,sl,tr,uk,vi,zh-CN
            switch (lng)
            {
                case "az":
                    return "az_Latn_AZ";
                case "bg":
                    return "bg_BG";
                case "cs":
                    return "cs_CZ";
                case "de":
                    return "de_DE";
                case "el":
                    return "el_GR";
                case "en":
                    return "en_GB";
                case "en-US":
                    return "en_US";
                case "es":
                    return "es_ES";
                //case "fi":
                //    return "";
                case "fr":
                    return "fr_FR";
                case "it":
                    return "it_IT";
                //case "ja":
                //    return "";
                case "ko":
                    return "ko_KR";
                //case "lo":
                //    return "";
                case "lv":
                    return "lv_LV";
                case "nl":
                    return "nl_NL";
                case "pl":
                    return "pl_PL";
                case "pt":
                    return "pt_PT";
                case "pt-BR":
                    return "pt_BR";
                case "ro":
                    return "ro_RO";
                case "ru":
                    return "ru_RU";
                case "sk":
                    return "sk_SK";
                case "sl":
                    return "sl_SI";
                case "tr":
                    return "tr_TR";
                case "uk":
                    return "uk_UA";
                case "vi":
                    return "vi_VN";
                //case "zh-CN":
                //    return "";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
