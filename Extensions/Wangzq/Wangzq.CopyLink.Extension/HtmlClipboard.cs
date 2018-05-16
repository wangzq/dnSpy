using System;
using System.Text;
using System.Windows;

namespace Wangzq.CopyLink.Extension
{
	internal static class HtmlClipboard {
		public static void Set(string html, string text = null) {
			Encoding enc = Encoding.UTF8;
			string html_total = WithHtmlHeaders(html);
			DataObject obj = new DataObject();
			obj.SetData(DataFormats.Html, new System.IO.MemoryStream(
				enc.GetBytes(html_total)));
			if (text != null) {
				obj.SetData(DataFormats.UnicodeText, text);
			}
			Clipboard.SetDataObject(obj, true);
		}

		public static string WithHtmlHeaders(string html) {
			Encoding enc = Encoding.UTF8;

			string begin = "Version:0.9\r\nStartHTML:{0:000000}\r\nEndHTML:{1:000000}"
			               + "\r\nStartFragment:{2:000000}\r\nEndFragment:{3:000000}\r\n";

			string html_begin = "<html>\r\n<head>\r\n"
			                    + "<meta http-equiv=\"Content-Type\""
			                    + " content=\"text/html; charset=" + enc.WebName + "\">\r\n"
			                    + "<title>HTML clipboard</title>\r\n</head>\r\n<body>\r\n"
			                    + "<!--StartFragment-->";

			string html_end = "<!--EndFragment-->\r\n</body>\r\n</html>\r\n";

			string begin_sample = String.Format(begin, 0, 0, 0, 0);

			int count_begin = enc.GetByteCount(begin_sample);
			int count_html_begin = enc.GetByteCount(html_begin);
			int count_html = enc.GetByteCount(html);
			int count_html_end = enc.GetByteCount(html_end);

			string html_total = String.Format(
				                    begin
				                    , count_begin
				                    , count_begin + count_html_begin + count_html + count_html_end
				                    , count_begin + count_html_begin
				                    , count_begin + count_html_begin + count_html
			                    ) + html_begin + html + html_end;

			return html_total;
		}
	}
}
