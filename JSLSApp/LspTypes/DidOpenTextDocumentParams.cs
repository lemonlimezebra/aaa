namespace JSLSApp.LspTypes;

public class DidOpenTextDocumentParams
{
    /**
	 * The document that was opened.
	 */
    public TextDocumentItem? textDocument { get; set; }
}
