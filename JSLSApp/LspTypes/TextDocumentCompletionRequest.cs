namespace JSLSApp.LspTypes;

public class TextDocumentCompletionRequest
{
    public int id { get; set; }
    public string method { get; set; }
    public TextDocumentCompletionRequestParams @params { get; set; }
}
