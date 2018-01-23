using System;
using System.Web;

namespace dnSpy.CopyLink {
	public static class LinkParser
    {
		public static string[] CheckArgs(string[] args) 
		{
			const string linkPrefix = "dnspy://";
			if (args.Length == 1 && args[0].StartsWith(linkPrefix, StringComparison.OrdinalIgnoreCase)) {
				args = CommandLineHelper.ToArgs(HttpUtility.UrlDecode(args[0].Substring(linkPrefix.Length)));
			}
			return args;
		}
    }
}
