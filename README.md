# TextAPI
A minimal text api which caters for lines

Uses doubly linked list for item storage and traversal.
Insert at index and deletes are much faster than stringbuilder.
Exposes current line and cell number.
Does lines differently in that a default newline character always exists as the first character but not exposed.
Can easily move between lines and jump to nodes (api for that is not publicly exposed)

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
