using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using dnlib.DotNet;
using dnSpy.Contracts.Decompiler.XmlDoc;
using dnSpy.Contracts.Documents.Tabs.DocViewer;
using dnSpy.Contracts.Menus;
using dnSpy.Contracts.TreeView;

namespace Wangzq.CopyLink.Extension {
	static class CopyLinkCommand
    {
		const string Header = "Copy Link";

		internal static void ExecuteInternal(IMemberDef memberDef) {
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

		[ExportMenuItem(Header = Header, InputGestureText = "Ctrl+Shift+C", Group = MenuConstants.GROUP_CTX_DOCVIEWER_TOKENS, Order = 0)]
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

		[ExportMenuItem(Header = Header, InputGestureText = "Ctrl+Shift+C", Group = MenuConstants.GROUP_CTX_DOCUMENTS_TOKENS, Order = 0)]
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
}
