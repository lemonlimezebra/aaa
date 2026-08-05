/*
 Do not forget to re-publish when applicable
 */

using JSLSApp;
using JSLSApp.LspTypes;
using System.Text;
using System.Text.Json;
using Range = JSLSApp.LspTypes.Range;


var str = @"
public class Foo {
}
";
//var str = File.ReadAllText("C:\\Users\\hunte\\Repos\\New folder (4)\\arbezemilnomel\\src\\main.cjs");
var javaScriptDocument = new JavaScriptDocument(str.ToList());
var javaScriptParser = new JavaScriptParser(javaScriptDocument);
javaScriptDocument.CompilationUnit = javaScriptParser.Parse();
var aaa = javaScriptDocument.CompilationUnit.FunctionDefinitionStartPositionList;
var bbb = javaScriptDocument.CompilationUnit.GetString();

/*
{
 "type": "Program",
 "body": [
  {
   "type": ClassDeclarationNode,
   "id": { "type": "Identifier", "name": "Foo" },
  },
 ],
}
*/

var stdoutChunkObjects = new List<StdoutChunkObject>();
var stdoutChunkFirstEntryMetadataSubstringIndexStart = 0;
var stdoutChunkFirstEntryMetadataContentLengthNumber = 0;

JavaScriptWorkspace? _javaScriptWorkspace = null;

string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

if (homePath != "C:\\Users\\hunte")
{
    Console.WriteLine(@"if (homePath != ""C:\\Users\\hunte"")");
    return;
}

//using StreamWriter writer = new StreamWriter("C:\\Users\\hunte\\Repos\\file.txt");
var myPath = "C:\\Users\\hunte\\Repos\\file.txt";
File.WriteAllText(myPath, Environment.ProcessId.ToString() + '\n');

using StreamReader reader = new StreamReader(Console.OpenStandardInput());

while (true) // I'm getting the warning: "Do not use 'reader.EndOfStream' in an async method"
{
    //string? line = reader.ReadLine();
    /*int length = reader.ReadBlock(buffer); // hmm
    if (length > 0)
    {
        var str = new string(buffer, 0, length); // TODO: Don't do this work with the buffer directly.
        File.AppendAllText(myPath, str);
        File.AppendAllText(myPath, "\n====\n");
        MAIN_decodeMessage(str);
        // Process line
    }*/

    /*var text = await reader.ReadToEndAsync(); // hmm
    if (text is not null)
    {
        File.AppendAllText(myPath, text);
        File.AppendAllText(myPath, "\n====\n");
        MAIN_decodeMessage(text);
        // Process line
    }*/

    var text = reader.ReadLine(); // hmm
    File.AppendAllText(myPath, $"\n====TESTINGAAA_LSreader.ReadLine()====\n");
    if (text is not null)
    {
        File.AppendAllText(myPath, text);
        File.AppendAllText(myPath, "\n====\n");
        MAIN_decodeMessage(text);
        // Process line
    }
}

/*
32060
Content-Length: 163
====

====
{"method":"initialize","id":0,"params":{"processId":13544,"clientInfo":{"name":"TextEditor123","version":"0.0.1"},"rootUri":"C:\\Users\\hunte\\Repos\\JavaScript"}}
====

*/

/**
 * @param {string} json 
 * @returns {object | null}
 * 
 * // TODO: you probably can reinvoke this method if you have extra unread content beyond the length needed to read the message
 * //     TODO: You could incrementally approach an optimized and correct answer by having this re-invocation for the time being just be a substring of the remaining text.
 * // TODO: Preferably neither of these would allocate a "substring" But they both will for the time being because I'm using JSON.parse and at the moment I know not of any other way than providing this a string.
*/
object? MAIN_decodeMessage(string json)
{
    File.AppendAllText(myPath, $"\n====MAIN_decodeMessage====\n");
    try
    {
        // I've seen both the header and content in a single 'MAIN_decodeMessage' while debugging.
        // But just the same I've seen only the header in a 'MAIN_decodeMessage' with a separate invocation for the content.
        //
        // So the seemingly non-deterministic nature of this is something to note.
        //
        // In both scenarios the total content "seemed" equivalent at a glance but I didn't do thorough checking

        //var json = jsonBytes.ToString(); // TODO: Don't toString() this, work with the bytes directly until the end (does JSON.parse take bytes as input? If so never have to do a toString()?).

        if (stdoutChunkObjects.Count == 0)
        {
            // Parse Content-Length
            var indexOfContentLengthToken = json.IndexOf("Content-Length: ");
            if (indexOfContentLengthToken == -1) return null;
            var substringIndexStart = indexOfContentLengthToken + 16; /* 16 === 'Content-Length: '.length */
            var substringIndexEnd = substringIndexStart;
            for (; substringIndexEnd < json.Length; substringIndexEnd++)
            {
                switch (json[substringIndexEnd])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                    default:
                        goto afterOuterForLoop;
                }
            }
            afterOuterForLoop:
            if (substringIndexEnd == substringIndexStart) return null;
            var contentLengthString = json.Substring(substringIndexStart, substringIndexEnd - substringIndexStart);
            if (!int.TryParse(contentLengthString, out var contentLengthNumber))
            {
                File.AppendAllText(myPath, $"\n====if (!int.TryParse(contentLengthString, out var contentLengthNumber))====\n");
                return null;
            }

            File.AppendAllText(myPath, $"\n====contentLengthNumber:{contentLengthNumber}====\n");

            // Parse Content
            var indexOfSearchTerm = json.IndexOf("\r\n\r\n");
            File.AppendAllText(myPath, $"\n====indexOfSearchTerm:{indexOfSearchTerm}====\n");
            if (indexOfSearchTerm == -1)
            {
                // TODO: This is a little scuffed because readline is losing the line endings that delimiter header from content...
                // ...
                File.AppendAllText(myPath, $"\n====indexOfSearchTerm == -1-delaying====\n");
                // ... continue delaying
                stdoutChunkObjects.Add(new StdoutChunkObject(json));
                stdoutChunkFirstEntryMetadataSubstringIndexStart = json.Length;
                stdoutChunkFirstEntryMetadataContentLengthNumber = contentLengthNumber;
                return null;
            }
            substringIndexStart = indexOfSearchTerm + 4; /* 4 === "\r\n\r\n".length */

            // Payload
            if (substringIndexStart + contentLengthNumber <= json.Length)
            {
                // ... read
                var content = json.Substring(substringIndexStart, (substringIndexStart + contentLengthNumber) - substringIndexStart);
                File.AppendAllText(myPath, $"\n====single-event-content:{content}====\n");
                return DeserializeContent(content);
            }
            else
            {
                File.AppendAllText(myPath, $"\n====single-event-continue-delaying====\n");
                // ... continue delaying
                stdoutChunkObjects.Add(new StdoutChunkObject(json));

                stdoutChunkFirstEntryMetadataSubstringIndexStart = substringIndexStart;
                stdoutChunkFirstEntryMetadataContentLengthNumber = contentLengthNumber;
                return null;
            }
        }
        else
        {
            // Parse Content
            // 0th
            var sumUnreadStdout = stdoutChunkObjects[0].BytesDecoded.Length - stdoutChunkFirstEntryMetadataSubstringIndexStart; // initialize to the remaining length that was in the first message of the batch

            // >first && <last
            for (var i = 1; i < stdoutChunkObjects.Count; i++)
            { // TODO: You could determine the necessary length of the NEXT chunk that will cause the necessary length requirement to be met then avoid an 'n complexity' and just have 'constant'.
              // TODO: Further commenting about determining the necessary length of the NEXT chunk, that is what the original 'if' block is doing on the first message. Perhaps these two conditional branches are equivalent when following a "necessary length" implementation.
                sumUnreadStdout += stdoutChunkObjects[i].BytesDecoded.Length;
            }

            // current
            sumUnreadStdout += json.Length;

            // Payload
            if (stdoutChunkFirstEntryMetadataContentLengthNumber <= sumUnreadStdout)
            {
                // ... read
                var builder = new StringBuilder();

                // 0th
                var lenZeroth = stdoutChunkObjects[0].BytesDecoded.Length - stdoutChunkFirstEntryMetadataSubstringIndexStart;
                if (lenZeroth != 0)
                {
                    var zerothSubstring = stdoutChunkObjects[0].BytesDecoded.Substring(stdoutChunkFirstEntryMetadataSubstringIndexStart, stdoutChunkObjects[0].BytesDecoded.Length);
                    builder.Append(zerothSubstring); // initialize to the remaining length that was in the first message of the batch
                }

                // >first && <last
                for (var i = 1; i < stdoutChunkObjects.Count; i++)
                { // TODO: You could determine the necessary length of the NEXT chunk that will cause the necessary length requirement to be met then avoid an 'n complexity' and just have 'constant'.
                  // TODO: Further commenting about determining the necessary length of the NEXT chunk, that is what the original 'if' block is doing on the first message. Perhaps these two conditional branches are equivalent when following a "necessary length" implementation.
                    builder.Append(stdoutChunkObjects[i].BytesDecoded);
                }

                // current
                builder.Append(json);

                var joinedJson = builder.ToString();

                stdoutChunkObjects.Clear(); // TODO: clear the array entries to permit garbage collection (since stdoutChunkObjects is always in the app's scope any entries would as well never be collected)

                string content;

                if (joinedJson.Length == stdoutChunkFirstEntryMetadataContentLengthNumber)
                {
                    content = joinedJson;
                }
                else
                {
                    content = joinedJson.Substring(0, stdoutChunkFirstEntryMetadataContentLengthNumber - 0);
                    // I can't decide on what to put here, at the end of the day just make sure this case has something instrusive so its incompleteness isn't swept under the rug
                    // maybe I should throw an error I can't describe how "confused" I am at the moment I am just pushing to make progress with every last bit of energy I have
                    // and all the anxiety and decisions i.e.: you get a message box idk
                    throw new NotImplementedException();
                }

                File.AppendAllText(myPath, $"\n====multi-event-content:{content}====\n");
                return DeserializeContent(content);

            }
            else
            {
                File.AppendAllText(myPath, $"\n====multi-event-continue-delaying====\n");
                // ... continue delaying
                stdoutChunkObjects.Add(new StdoutChunkObject(json));
                return null;
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return null;
    }
}

string MAIN_encodeMessageObject(object messageObject)
{
    var content = JsonSerializer.Serialize(messageObject);
    var spacing = "\r\n\r\n";
    return $"Content-Length: {content.Length}{spacing}{content}";
}

object? DeserializeContent(string content)
{
    File.AppendAllText(myPath, $"\n====DeserializeContent====\n");
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var request = JsonSerializer.Deserialize<Message>(content, options);
    if (request is null)
    {
        File.AppendAllText(myPath, $"\n====request is null====\n");
    }
    if (!string.IsNullOrWhiteSpace(request?.Method))
    {
        File.AppendAllText(myPath, $"\n====request?.Method:{request?.Method}====\n");
    }
    else
    {
        File.AppendAllText(myPath, $"\n====ELSE====\n");
    }
    switch (request?.Method)
    {
        case "initialize":
            //File.AppendAllText(myPath, $"\n====preTSTSTS====\n");
            var initializeRequest = JsonSerializer.Deserialize<InitializeRequest>(content);
            //try
            //{
            //    initializeRequest = JsonSerializer.Deserialize<InitializeRequest>(content);
            //}
            //catch (Exception e)
            //{
            //    File.AppendAllText(myPath, $"\n====eTSTSTS {e.ToString()}====\n");
            //    throw;
            //}
            //File.AppendAllText(myPath, $"\n====postTSTSTS====\n");

            File.AppendAllText(myPath, $"\n====Id:{initializeRequest?.Id ?? -123}====\n");
            if (!string.IsNullOrWhiteSpace(initializeRequest?.@params?.rootUri))
            {
                File.AppendAllText(myPath, $"\n====initializeRequest?.Params?.RootUri:{initializeRequest?.@params?.rootUri}====\n");
                _javaScriptWorkspace = new JavaScriptWorkspace(initializeRequest?.@params?.rootUri);
                File.AppendAllText(myPath, $"\n====_javaScriptWorkspace.SourceFileList.Count:{_javaScriptWorkspace.SourceFileAbsolutePathList.Count}====\n");
                // ====_javaScriptWorkspace.SourceFileList.Count:12====
            }
            else
            {
                File.AppendAllText(myPath, $"\n====initializeRequest?.Params?.RootUri:null====\n");
                if (initializeRequest?.@params?.workspaceFolders is null)
                {
                    File.AppendAllText(myPath, $"\n====initializeRequest?.Params?.workspaceFolders:null====\n");
                }
                else
                {
                    _javaScriptWorkspace = new JavaScriptWorkspace(initializeRequest?.@params?.workspaceFolders);
                    File.AppendAllText(myPath, $"\n====initializeRequest?.Params?.workspaceFolders...====\n");
                    foreach (var workspaceFolder in initializeRequest?.@params?.workspaceFolders)
                    {
                        File.AppendAllText(myPath, $"\n====workspaceFolder: name->{workspaceFolder.name} | uri->{workspaceFolder.uri}====\n");
                    }
                }
            }
            var initializeResponse = new InitializeResponse(new InitializeResponseResult());
            Console.Out.WriteLine(MAIN_encodeMessageObject(initializeResponse));
            return initializeRequest;
        case "textDocument/didOpen":
            File.AppendAllText(myPath, $"\n====teeesttextDocument/didOpen====\n");
            var didOpenTextDocumentNotification = JsonSerializer.Deserialize<DidOpenTextDocumentNotification>(content);
            var p = didOpenTextDocumentNotification?.@params is null ? "null" : "nn";
            File.AppendAllText(myPath, $"\n====dotdn...params:{p}====\n");
            var td = didOpenTextDocumentNotification?.@params?.textDocument is null ? "null" : "nn";
            File.AppendAllText(myPath, $"\n====dotdn...textDocument:{td}====\n");
            File.AppendAllText(myPath, $"\n====dotdn...uri:{didOpenTextDocumentNotification?.@params?.textDocument?.uri ?? "null"}====\n");
            if (didOpenTextDocumentNotification?.@params?.textDocument?.uri is not null)
            {
                if (_javaScriptWorkspace is null)
                {
                    File.AppendAllText(myPath, $"\n====_javaScriptWorkspace is null====\n");
                }
                else
                {
                    File.AppendAllText(myPath, $"\n====_javaScriptWorkspace is NOT null====\n");
                    _javaScriptWorkspace.DidOpenTextDocumentNotification(
                        myPath,
                        didOpenTextDocumentNotification?.@params?.textDocument?.uri,
                        didOpenTextDocumentNotification?.@params?.textDocument?.text);
                }
            }
            return didOpenTextDocumentNotification;
        case "textDocument/didClose":
            var didCloseTextDocumentNotification = JsonSerializer.Deserialize<DidCloseTextDocumentNotification>(content);
            File.AppendAllText(myPath, $"\n____====DidCloseTextDocumentNotification {didCloseTextDocumentNotification?.@params?.textDocument?.uri ?? "null"} ====\n");
            if (_javaScriptWorkspace is null)
            {
                File.AppendAllText(myPath, $"\n====_javaScriptWorkspace is null====\n");
            }
            else
            {
                File.AppendAllText(myPath, $"\n____====_javaScriptWorkspace is NOT null====\n");
                _javaScriptWorkspace.DidCloseTextDocumentNotification(myPath, didCloseTextDocumentNotification?.@params?.textDocument?.uri);
            }
            return request;
        case "textDocument/didChange":
            var didChangeTextDocumentNotification = JsonSerializer.Deserialize<DidChangeTextDocumentNotification>(content);
            if (didChangeTextDocumentNotification.@params.textDocument is null)
            {
                File.AppendAllText(myPath, $"\n====didChangeTextDocumentNotification.@params.textDocument is null====\n");
            }
            else
            {
                File.AppendAllText(myPath, $"\n====didChangeTextDocumentNotification.@params.textDocument.uri: {didChangeTextDocumentNotification.@params.textDocument.uri}====\n");
                File.AppendAllText(myPath, $"\n====didChangeTextDocumentNotification.@params.textDocument.version: {didChangeTextDocumentNotification.@params.textDocument.version}====\n");
            }

            if (didChangeTextDocumentNotification.@params.contentChanges is null)
            {
                File.AppendAllText(myPath, $"\n====didChangeTextDocumentNotification.@params.contentChanges is null====\n");
            }
            else
            {
                if (didChangeTextDocumentNotification.@params.contentChanges.Length == 0)
                {
                    File.AppendAllText(myPath, $"\n====didChangeTextDocumentNotification.@params.contentChanges.Length == 0====\n");
                }
                else
                {
                    foreach (var item in didChangeTextDocumentNotification.@params.contentChanges)
                    {
                        if (item.range is null)
                        {
                            File.AppendAllText(myPath, $"\n====didChangeTextDocumentNotification item range is null====\n");
                        }
                        else
                        {
                            File.AppendAllText(myPath, $"\n====didChangeTextDocumentNotification item range start line: {item.range.start.line}====\n");
                            File.AppendAllText(myPath, $"\n====didChangeTextDocumentNotification item range start character: {item.range.start.character}====\n");

                            File.AppendAllText(myPath, $"\n====didChangeTextDocumentNotification item range end line: {item.range.end.line}====\n");
                            File.AppendAllText(myPath, $"\n====didChangeTextDocumentNotification item range end character: {item.range.end.character}====\n");
                        }

                        if (item.rangeLength is null)
                        {
                            File.AppendAllText(myPath, $"\n====didChangeTextDocumentNotification item rangeLength is null====\n");
                        }
                        else
                        {
                            File.AppendAllText(myPath, $"\n====didChangeTextDocumentNotification item rangeLength: {item.rangeLength.Value}====\n");
                        }

                        if (item.text is null)
                        {
                            File.AppendAllText(myPath, $"\n====didChangeTextDocumentNotification item text is null====\n");
                        }
                        else
                        {
                            File.AppendAllText(myPath, $"\n====didChangeTextDocumentNotification item text: {item.text}====\n");
                        }
                    }
                }

                if (_javaScriptWorkspace is null)
                {
                    File.AppendAllText(myPath, $"\n====_javaScriptWorkspace is null====\n");
                }
                else
                {
                    File.AppendAllText(myPath, $"\n____====_javaScriptWorkspace is NOT null====\n");
                    _javaScriptWorkspace.DidChangeTextDocumentNotification(myPath, didChangeTextDocumentNotification?.@params?.textDocument?.uri, didChangeTextDocumentNotification.@params.contentChanges);
                }
            }

            return request;
        case "textDocument/documentSymbol":
            var symbolRequest = JsonSerializer.Deserialize<TextDocumentDocumentSymbolRequest>(content);
            File.AppendAllText(myPath, $"\n====RECEIVED DOCUMENT SYMBOL {symbolRequest.@params.textDocument.uri}====\n");
            File.AppendAllText(myPath, $"\n====TEEEEEEEEEEEST====\n");

            if (_javaScriptWorkspace?.OpenedSourceFileAbsolutePathToInMemoryContentMap.TryGetValue(symbolRequest.@params.textDocument.uri, out var javaScriptDocument) ?? false)
            {
                File.AppendAllText(myPath, $"\n====v2TEEEEEEEEEEEST====\n");

                var javascriptParser = new JavaScriptParser(javaScriptDocument);
                javaScriptDocument.CompilationUnit = javascriptParser.Parse();
                //if (!javaScriptDocument.HasBeenParsedAtLeastOnce)
                //{
                //    var javascriptParser = new JavaScriptParser();
                //    javaScriptDocument.CompilationUnit = javascriptParser.Parse(javaScriptDocument);
                //}
                var documentSymbolArray = new DocumentSymbol[javaScriptDocument.CompilationUnit.FunctionDefinitionStartPositionList.Count];
                File.AppendAllText(myPath, $"\n====documentSymbolList.length:{documentSymbolArray.Length}====\n");
                for (int i = 0; i < javaScriptDocument.CompilationUnit.FunctionDefinitionStartPositionList.Count; i++)
                {
                    var functionDefinition = javaScriptDocument.CompilationUnit.FunctionDefinitionStartPositionList[i];
                    documentSymbolArray[i] = new DocumentSymbol
                    {
                        //name
                        kind = SymbolKind.Function,
                        name = functionDefinition.Name,
                        range = new Range
                        {
                            start = functionDefinition.StartPosition,
                            end = functionDefinition.StartPosition
                        }
                    };
                }
                
                var textDocumentDocumentSymbolResponse = new TextDocumentDocumentSymbolResponse(symbolRequest.id, documentSymbolArray);
                Console.Out.WriteLine(MAIN_encodeMessageObject(textDocumentDocumentSymbolResponse));
            }

            return request;
        case "textDocument/hover":
            var hoverRequest = JsonSerializer.Deserialize<TextDocumentHoverRequest>(content);
            File.AppendAllText(myPath, $"\n====RECEIVED HOVER {hoverRequest.@params.textDocument.uri}====\n");
            File.AppendAllText(myPath, $"\n====TEEEEEEEEEEEST====\n");

            if (_javaScriptWorkspace?.OpenedSourceFileAbsolutePathToInMemoryContentMap.TryGetValue(hoverRequest.@params.textDocument.uri, out var bbbjavaScriptDocument) ?? false)
            {
                File.AppendAllText(myPath, $"\n====v2TEEEEEEEEEEEST====\n");

                var javascriptParser = new JavaScriptParser(bbbjavaScriptDocument);
                bbbjavaScriptDocument.CompilationUnit = javascriptParser.Parse();
                //if (!bbbjavaScriptDocument.HasBeenParsedAtLeastOnce)
                //{
                //    var javascriptParser = new JavaScriptParser();
                //    bbbjavaScriptDocument.CompilationUnit = javascriptParser.Parse(bbbjavaScriptDocument);
                //}
                var documentSymbolArray = new DocumentSymbol[bbbjavaScriptDocument.CompilationUnit.FunctionDefinitionStartPositionList.Count];
                File.AppendAllText(myPath, $"\n====documentSymbolList.length:{documentSymbolArray.Length}====\n");
                for (int i = 0; i < bbbjavaScriptDocument.CompilationUnit.FunctionDefinitionStartPositionList.Count; i++)
                {
                    var functionDefinition = bbbjavaScriptDocument.CompilationUnit.FunctionDefinitionStartPositionList[i];
                    documentSymbolArray[i] = new DocumentSymbol
                    {
                        //name
                        kind = SymbolKind.Function,
                        name = functionDefinition.Name,
                        range = new Range
                        {
                            start = functionDefinition.StartPosition,
                            end = functionDefinition.StartPosition
                        }
                    };
                }

                var textDocumentHoverResponse = new TextDocumentHoverResponse(hoverRequest.id, $"tooltip example for {hoverRequest.@params.textDocument.uri}");
                Console.Out.WriteLine(MAIN_encodeMessageObject(textDocumentHoverResponse));
            }

            return request;
        case "textDocument/CustomFullFileLexRequest":
            var customFullFileLexRequest = JsonSerializer.Deserialize<CustomFullFileLexRequest>(content);
            File.AppendAllText(myPath, $"\n====RECEIVED customFullFileLexRequest {customFullFileLexRequest.@params.textDocument.uri}====\n");
            File.AppendAllText(myPath, $"\n====TEEEEEEEEEEEST====\n");

            if (_javaScriptWorkspace?.OpenedSourceFileAbsolutePathToInMemoryContentMap.TryGetValue(customFullFileLexRequest.@params.textDocument.uri, out var aaajavaScriptDocument) ?? false)
            {
                File.AppendAllText(myPath, $"\n====v2TEEEEEEEEEEEST====\n");

                var javascriptParser = new JavaScriptParser(aaajavaScriptDocument);
                aaajavaScriptDocument.CompilationUnit = javascriptParser.Parse();
                //if (!aaajavaScriptDocument.HasBeenParsedAtLeastOnce)
                //{
                //    var javascriptParser = new JavaScriptParser();
                //    aaajavaScriptDocument.CompilationUnit = javascriptParser.Parse(aaajavaScriptDocument);
                //}
                //var documentSymbolArray = new DocumentSymbol[aaajavaScriptDocument.CompilationUnit.FunctionDefinitionStartPositionList.Count];
                //File.AppendAllText(myPath, $"\n====documentSymbolList.length:{documentSymbolArray.Length}====\n");
                //for (int i = 0; i < aaajavaScriptDocument.CompilationUnit.FunctionDefinitionStartPositionList.Count; i++)
                //{
                //    var functionDefinition = aaajavaScriptDocument.CompilationUnit.FunctionDefinitionStartPositionList[i];
                //    documentSymbolArray[i] = new DocumentSymbol
                //    {
                //        //name
                //        kind = SymbolKind.Function,
                //        name = functionDefinition.Name,
                //        range = new Range
                //        {
                //            start = functionDefinition.StartPosition,
                //            end = functionDefinition.StartPosition
                //        }
                //    };
                //}

                var textDocumentDocumentSymbolResponse = new CustomFullFileLexResponse(customFullFileLexRequest.id, javascriptParser.PsuedoFourFieldTrackedSyntaxList.ToArray());
                Console.Out.WriteLine(MAIN_encodeMessageObject(textDocumentDocumentSymbolResponse));
            }

            return request;
        default:
            return request;
    }
}


/*

Google AI:
 
> Given the following JavaScript:
> 
> ```js
> class Foo {
> 	Bar() {
> 		console.log('hello world');
> 	}
> }
> ```
> 
> What would the abstract syntax tree look like?

< For standard JavaScript parsers (like Babel or Acorn), this code generates an abstract syntax tree composed of nodes that represent the structural hierarchy of the class.
< ...
< {
<   "type": "Program",
<   "body": [
<     {
<       "type": "ClassDeclaration",
<       "id": {
<         "type": "Identifier",
<         "name": "Foo"
<       },
<       "superClass": null,
<       "body": {
<         "type": "ClassBody",
<         "body": [
<           {
<             "type": "MethodDefinition",
<             "key": {
<               "type": "Identifier",
<               "name": "Bar"
<             },
<             "value": {
<               "type": "FunctionExpression",
<               "id": null,
<               "params": [],
<               "body": {
<                 "type": "BlockStatement",
<                 "body": [
<                   {
<                     "type": "ExpressionStatement",
<                     "expression": {
<                       "type": "CallExpression",
<                       "callee": {
<                         "type": "MemberExpression",
<                         "object": {
<                           "type": "Identifier",
<                           "name": "console"
<                         },
<                         "property": {
<                           "type": "Identifier",
<                           "name": "log"
<                         },
<                         "computed": false
<                       },
<                       "arguments": [
<                         {
<                           "type": "Literal",
<                           "value": "hello world",
<                           "raw": "'hello world'"
<                         }
<                       ]
<                     }
<                   }
<                 ]
<               }
<             },
<             "kind": "method",
<             "static": false
<           }
<         ]
<       }
<     }
<   ],
<   "sourceType": "script"
< }
< ...
< If you were parsing this file using your iterative development strategy on Day 1, your AST might look like a heavily simplified, fallback version of this tree.
< {
<   "type": "ClassDeclaration",
<   "id": { "type": "Identifier", "name": "Foo" },
<   "body": {
<     "type": "ClassBody",
<     "body": [
<       {
<         "type": "UnregisteredNode",
<         "tokens": ['Bar', '(', ')', '{', 'console', '.', 'log', ...],
<         "start": { "line": 2, "column": 1 },
<         "end": { "line": 4, "column": 2 }
<       }
<     ]
<   }
< }

I think the steps are:

```js
// hover here nothing
public class Foo { // hover here see class definition 'Foo'
}
// hover here nothing
```


----------------------
Then:


```js
// nothing
public class Foo { // hover here see class definition 'Foo'
}
// nothing
public class Bar { // hover here see class definition 'Bar'
}
// nothing
```
 
 */
