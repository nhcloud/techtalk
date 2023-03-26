using BenchmarkDotNet.Attributes;
using System.Text.RegularExpressions;

namespace Net7Console
{
    public partial class RegexDemo
    {
        [Params(RegexOptions.Compiled, RegexOptions.NonBacktracking)]
        public static RegexOptions Options
        {
            get;
            set;
        }
        public Regex regex = new(@"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        public Regex regex2 = new(@"^[a-zA-Z0-9\+/]*={0,3}$", Options);
        [GeneratedRegex(@"hello|goodbye",RegexOptions.IgnoreCase)]
        public partial Regex GreetingRegex();
     
        public bool IsGreeting(string candidateString)
        {
            var isMatch = GreetingRegex().IsMatch(candidateString);
            return isMatch;
        }
    }
}
