using System.ComponentModel.Composition;
using System.Windows.Input;
using dnlib.DotNet;
using dnSpy.Contracts.Controls;
using dnSpy.Contracts.Documents.Tabs;
using dnSpy.Contracts.Documents.Tabs.DocViewer;
using dnSpy.Contracts.Extension;

namespace Wangzq.CopyLink.Extension
{
	[ExportAutoLoaded]
	sealed class CopyLinkCommandsLoader : IAutoLoaded {
		readonly IDocumentTabService documentTabService;
		static readonly RoutedCommand copyLinkCommand = new RoutedCommand("CopyLinkRoutedCommand", typeof(CopyLinkCommandsLoader));

		[ImportingConstructor]
		public CopyLinkCommandsLoader(IWpfCommandService wpfCommandService, IDocumentTabService documentTabService) {
			this.documentTabService = documentTabService;
			var cmds = wpfCommandService.GetCommands(ControlConstants.GUID_DOCUMENTVIEWER_UICONTEXT);
			cmds.Add(copyLinkCommand,
				(s, e) => { CopyLinkCommand.ExecuteInternal(GetCurrentMember()); },
				(s, e) => e.CanExecute = GetCurrentMember() != null,
				ModifierKeys.Control | ModifierKeys.Shift,
				Key.C
			);

		}
		IMemberDef GetCurrentMember() {
			var selectedReferenceData = (documentTabService.ActiveTab?.UIContext as IDocumentViewer)?.SelectedReference?.Data;
			return selectedReferenceData?.Reference as IMemberDef;
		}
	}
}