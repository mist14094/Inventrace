//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lucene.Net.Analysis.Snowball
{
    public class RuStemming
    {    
        const string CVower = "���������";
        const string CPerfectiveground = "((��|����|������|��|����|������)|((?<=[��])(�|���|�����)))$";
        const string CReflexive = "(�[��])$";
        const string CAdjective = "(��|��|��|��|��|���|���|��|��|��|��|��|��|��|��|���|���|���|��|��|��|��|��|��)$";
        const string CParticiple = "((���|���|���)|((?<=[��])(��|��|��|��|�)))$";
        const string CVerb = "((���|���|���|����|����|���|���|���|��|��|��|��|��|��|���|���|���|���|��|�)|((?<=[��])(��|��|���|���|��|�|�|��|�|��|��|��|��|��|��|���|���)))$";
        const string CNoun = "(�|��|��|��|��|�|����|���|���|��|��|�|���|��|��|��|�|�|���|��|���|��|��|��|�|�|��|���|��|�|�|��|��|�|��|��|�)$";
        const string CRvre = "^(.*?[���������])(.*)$";
        const string CDerivational = "[^���������][���������]+[^���������]+[���������].*(?<=�)���?$";

        public RuStemming() { }

        bool RegexReplace(ref string Original, string regx, string value)
        {
            string original = Original;
            Regex reg = new Regex(regx);
            Original = reg.Replace(Original, value);
            return (Original != original);
        }

        Match RegexMatch(string original, string regx)
        {
            Regex reg = new Regex(regx);
            return reg.Match(original);
        }

        MatchCollection RegexMatches(string original, string regx)
        {
            Regex reg = new Regex(regx, RegexOptions.Multiline);
            return reg.Matches(original);
        }

        public string Parse(string query)
        {
            Regex reg = new Regex(@"[ ,\.\?!=\&\*\+]");
            string[] words = reg.Split(query);
            IList<string> swords = new List<string>();

            for (int i = 0; i < words.Length; i++)
                if (!string.IsNullOrEmpty(words[i].Trim()))
                    swords.Add(Stem(words[i].Trim()));
            string[] swordsArray = new string[swords.Count];
            for (int i = 0; i < swordsArray.Length; i++)
                swordsArray[i] = swords[i];
            string result = string.Join("%", swordsArray);

            return string.Format("%{0}%", result);
        }

        public string Stem(string Word)
        {
            string word = Word.ToLower().Trim().Replace("�", "�");
            string value = string.Empty;
            do
            {
                MatchCollection matches = RegexMatches(word, CRvre);
                if (matches.Count < 1)
                {
                    Match matchEnglishOrDigits = RegexMatch(word, "[a-z0-9]");
                    if (matchEnglishOrDigits.Success)
                        value = word;

                    break;
                }
                    
                
                string rv = matches[0].Value;

                // ��� 1
                if (!RegexReplace(ref rv, CPerfectiveground, string.Empty))
                {
                    RegexReplace(ref rv, CReflexive, string.Empty);

                    if (RegexReplace(ref rv, CAdjective, string.Empty))
                        RegexReplace(ref rv, CParticiple, string.Empty);
                    else
                        if (!RegexReplace(ref rv, CVerb, string.Empty))
                            RegexReplace(ref rv, CNoun, string.Empty);                    
                }

                // ��� 2
                RegexReplace(ref rv, "�$", string.Empty);

                // ��� 3
                Match match = RegexMatch(rv, CDerivational);
                if (match.Success)
                    RegexReplace(ref rv, "����?$", string.Empty);

                // ��� 4
                if (!RegexReplace(ref rv, "�$", string.Empty))
                {
                    RegexReplace(ref rv, "����?", string.Empty);
                    RegexReplace(ref rv, "��$", "�");
                }

                value = rv;      
                
            } while (false);

            return value;
        }

    }
}
