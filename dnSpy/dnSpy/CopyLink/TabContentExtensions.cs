using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnSpy.Contracts.Documents.Tabs;
using dnSpy.Contracts.Documents.TreeView;
using dnSpy.Contracts.Tabs;
using dnSpy.Documents.Tabs;
using dnSpy.Documents.Tabs.DocViewer;

namespace dnSpy.Tabs
{
    public static class TabContentExtensions
    {
		public static string GetAssembly(this ITabContent tabContent) {
			var tabContentImpl = tabContent as TabContentImpl;
			if (tabContentImpl != null) {
				var c = tabContentImpl.Content;
				return GetAssembly(c);
			}

			return null;
		}

		public static string GetAssembly(this DocumentTabContent c) {
			var doc = c as DecompileDocumentTabContent;
			if (doc != null) {
				var node = doc.Nodes.FirstOrDefault();
				if (node != null) {
					var assemblyNode = node.GetAssemblyNode();
					if (assemblyNode != null) {
						return assemblyNode.Document.Filename;
					}
				}
			}

			return null;
		}
	}
}
