namespace JSLSApp.LspTypes;

public class TextDocumentDocumentSymbolRequest
{
    public int id { get; set; }
    public string method { get; set; }
    public TextDocumentDocumentSymbolRequestParams @params { get; set; }
}
