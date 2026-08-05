namespace JSLSApp.LspTypes;

public class TextDocumentHoverRequestParams
{
    public TextDocumentIdentifier textDocument { get; set; }
    public Position position { get; set; }
}
