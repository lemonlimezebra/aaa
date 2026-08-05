namespace JSLSApp.LspTypes;

public class TextDocumentHoverRequest
{
    public int id { get; set; }
    public string method { get; set; }
    public TextDocumentHoverRequestParams @params { get; set; }
}
