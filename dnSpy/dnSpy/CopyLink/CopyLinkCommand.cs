using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Windows;
using dnlib.DotNet;
using dnSpy.Contracts.Decompiler.XmlDoc;
using dnSpy.Contracts.Documents.Tabs.DocViewer;
using dnSpy.Contracts.Documents.TreeView;
using dnSpy.Contracts.Menus;
using dnSpy.Contracts.TreeView;

namespace dnSpy.Documents.Tabs {
	static class CopyLinkCommand
    {
		const string Header = "Copy Link";

		static void ExecuteInternal(IMemberDef memberDef) {
			if (memberDef != null) {
				var asm = memberDef.Module.Assembly;
				var location = asm.ManifestModule.Location;
				var xmlDocId = XmlDocKeyProvider.GetKey(memberDef, new StringBuilder()).ToString();
				// This will require a custom protocol handler I wrote to invoke dnspy with the assembly path and selects the xmldocid.
				var fullcmd = $"\"{location}\" --select \"{xmlDocId}\"";
				var text = $"dnspy://{HttpUtility.UrlEncode(fullcmd)}";
				var html = $"<a href=\"{text}\">{memberDef.Name}</a>";
				try {
					HtmlClipboard.Set(html, text);
				}
				catch (ExternalException) { }
			}
		}

		[ExportMenuItem(Header = Header, Group = MenuConstants.GROUP_CTX_DOCVIEWER_TOKENS, Order = 0)]
		sealed class CodeCommand : MenuItemBase {
			public override bool IsVisible(IMenuItemContext context) => GetReference(context) != null;
			public override void Execute(IMenuItemContext context) => ExecuteInternal(GetReference(context));

			static IMemberDef GetReference(IMenuItemContext context) => GetReference(context, MenuConstants.GUIDOBJ_DOCUMENTVIEWERCONTROL_GUID);
			internal static IMemberDef GetReference(IMenuItemContext context, string guid) {
				if (context.CreatorObject.Guid != new Guid(guid))
					return null;
				var @ref = context.Find<TextReference>();
				return @ref?.Reference as IMemberDef;
			}
		}

		[ExportMenuItem(Header = Header, Group = MenuConstants.GROUP_CTX_SEARCH_TOKENS, Order = 0)]
		sealed class SearchCommand : MenuItemBase {
			public override bool IsVisible(IMenuItemContext context) => GetReference(context) != null;
			public override void Execute(IMenuItemContext context) => ExecuteInternal(GetReference(context));
			static IMemberDef GetReference(IMenuItemContext context) => CodeCommand.GetReference(context, MenuConstants.GUIDOBJ_SEARCH_GUID);
		}

		[ExportMenuItem(Header = Header, Group = MenuConstants.GROUP_CTX_DOCUMENTS_TOKENS, Order = 0)]
		sealed class DocumentsCommand : MenuItemBase {
			public override bool IsVisible(IMenuItemContext context) => GetReference(context) != null;
			public override void Execute(IMenuItemContext context) => ExecuteInternal(GetReference(context));
			static IMemberDef GetReference(IMenuItemContext context) => GetReference(context, MenuConstants.GUIDOBJ_DOCUMENTS_TREEVIEW_GUID);

			internal static IMemberDef GetReference(IMenuItemContext context, string guid) {
				if (context.CreatorObject.Guid != new Guid(guid))
					return null;
				var nodes = context.Find<TreeNodeData[]>();
				if (nodes?.Length == 0)
					return null;
				var node = nodes[0] as IMDTokenNode;
				return node?.Reference as IMemberDef;
			}
		}

		[ExportMenuItem(Header = Header, Group = MenuConstants.GROUP_CTX_ANALYZER_TOKENS, Order = 0)]
		sealed class AnalyzerCommand : MenuItemBase {
			public override bool IsVisible(IMenuItemContext context) => GetReference(context) != null;
			public override void Execute(IMenuItemContext context) => ExecuteInternal(GetReference(context));
			static IMemberDef GetReference(IMenuItemContext context) => DocumentsCommand.GetReference(context, MenuConstants.GUIDOBJ_ANALYZER_TREEVIEW_GUID);
		}
	}

	public static class HtmlClipboard {
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
