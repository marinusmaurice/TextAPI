# TextAPI
A minimal text api which caters for lines

Uses doubly linked list for item storage and traversal unlike other text api's which typically use arrays.  
Insert at index are much faster than stringbuilder. E.g. Inserting 10 million items at index 0 takes ~3.8 seconds. StringBuilder take ~5 minutes for 1 million  
Exposes current line number and cell count.  
Does lines differently in that a default newline character always exists as the first character but not exposed.  
Can easily move between lines and jump to nodes from lines (api for that is not publicly exposed)  

Easy enough to extend to add more functionality

# Sample Code
```
  TextAPI textAPI = new TextAPI();
  textAPI.AppendAfterCurrentNode("a");
  textAPI.AppendBeforeCurrentNode("b");
  textAPI.AppendAfterCurrentNode(Environment.NewLine);
  //Should end up with
  //b
  //a
  Console.WriteLine(textAPI.ToString());
  Console.WriteLine("-----------");
  textAPI.CellIndex = 1;
  textAPI.AppendAfterCurrentNode("c");
  //should end up with
  //b
  //ca
  Console.WriteLine(textAPI.ToString());  
  //should be
  //Line number : 1
  Console.WriteLine($"Line number : {textAPI.LineIndex}");
```
