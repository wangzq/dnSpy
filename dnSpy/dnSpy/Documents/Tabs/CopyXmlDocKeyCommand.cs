using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using dnlib.DotNet;
using dnSpy.Contracts.Decompiler.XmlDoc;
using dnSpy.Contracts.Documents.Tabs.DocViewer;
using dnSpy.Contracts.Menus;
using dnSpy.Contracts.TreeView;

namespace dnSpy.Documents.Tabs {
	static class CopyXmlDocKeyCommand
    {
		const string Header = "Copy XmlDoc Key";

		static void ExecuteInternal(IMemberDef memberDef) {
			if (memberDef != null) {
				try {
					Clipboard.SetText(XmlDocKeyProvider.GetKey(memberDef, new StringBuilder()).ToString());
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
}
