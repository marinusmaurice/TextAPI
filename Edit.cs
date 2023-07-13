/*
Author: Maurice Marinus
Company: Symbolic Computing
Position: Symbolic Architect
Project: Edit - Minimal text edit api
Description: To show how to properly create a minimal text symbol api like architecture with lines
Date Started: 09 July 2023
Notes:
-This works on the notion of text only Symbols, NOT on classic style chars
-So "Maurice" is a single symbol, "M","a","u","r","i","c","e" is 7 single symbols
-Does not do automatic line splitting. So for Insert([char][CRLF][char]), "[char][CRLF][char]" is treated as a single symbol. The onus is on the developer to manually notify that a line break is needed. This also shouldn't be too difficult to do as one can simply loop the text BEFORE calling Append*.

LAST THING TO IMPLEMENT:
    set line number (will allow fast line jumping)
THEN TEST
  setting of current cell index and current line index

Todo (all easy enough - hence left out):
-Insert range of text
-Remove range of text
-Overwrite

*/

using System.Text;

namespace Edit
{
    public class Node
    { 
        private string _data;
        public Node() {}
        public Node(string value)
        {
            this._data = value;
        }
        public Node PreviousCell { get; set; }
        public Node NextCell { get; set; }
        public string Data { get => _data; set => _data = value; }
    } 

    public class LineNode : Node
    {
        public LineNode() : base(Environment.NewLine) { }
        public LineNode PreviousLine { get; set; }
        public LineNode NextLine { get; set; }
    } 
     
    public class TextAPI
    {
        private int _numberOfCells = -1;
        private int _numberOfLines = 0;
        private int _currentCellIndex = -2;
        private int _currentLineIndex = -1;
        private Node _currentCell;
        private LineNode _currentLine;

        public TextAPI()
        {
            AppendAfterCurrentNode(Environment.NewLine);
        }         
        private void SeekCells(int numberOfPositionsToSeek, SeekDirection seekDirection)
        {
            if (numberOfPositionsToSeek < 0)
                throw new IndexOutOfRangeException();

            int pos = 0;
            if (seekDirection == SeekDirection.Head)
            {
                while (_currentCell.NextCell != null && pos < numberOfPositionsToSeek)
                {
                    _currentCell = _currentCell.NextCell;
                    pos++;
                    _currentCellIndex++;

                    if (_currentCell.GetType() == typeof(LineNode))
                    {
                        if (_currentLine != null && _currentLine.NextLine != null)
                        {
                            _currentLine = _currentLine.NextLine;
                            _currentLineIndex++;
                        }
                    }
                }
            }
            else if (seekDirection == SeekDirection.Tail)
            {
                for(int x = 0; x <  numberOfPositionsToSeek; x++) 
                {
                    _currentCell = _currentCell.PreviousCell;
                    pos++;
                    _currentCellIndex--;

                    if (_currentCell.GetType() == typeof(LineNode))
                    {
                        if (_currentLine != null && _currentLine.PreviousLine != null)
                        {
                            _currentLine = _currentLine.PreviousLine;
                            _currentLineIndex--;
                        }
                    }
                }
            }
        }         
        public void AppendAfterCurrentNode(string value)
        {            
            Node node = (value != Environment.NewLine)? node = new Node(value): node = new LineNode();

            if (_currentCell == null)
            {
                _currentCell = node;
            }
            else
            {
                if (_currentCell.NextCell != null)
                {
                    _currentCell.NextCell.PreviousCell = node;
                    node.NextCell = _currentCell.NextCell;
                }

                _currentCell.NextCell = node;
                node.PreviousCell = _currentCell;
                _currentCell = node;
            }

            if (value == Environment.NewLine)
            { 
                if (_currentLine == null)
                {
                    _currentLine = (LineNode)node;
                }
                else
                {
                    if (_currentLine.NextLine != null)
                    {
                        _currentLine.NextLine.PreviousLine = ((LineNode)node);
                        ((LineNode)node).NextLine = _currentLine.NextLine;
                    }

                    _currentLine.NextLine = ((LineNode)node);
                    ((LineNode)node).PreviousLine = _currentLine;
                    _currentLine = ((LineNode)node);
                }
                _numberOfLines++;
                _currentLineIndex++;
            }

            _numberOfCells++;
            _currentCellIndex++;
        }
        public void AppendBeforeCurrentNode(string value)
        {
            Node node = (value != Environment.NewLine) ? node = new Node(value) : node = new LineNode();

            if (_currentCell == null)
            {
                _currentCell = node;
            }
            else
            {
                if (_currentCell.PreviousCell == null)
                {
                    //The least amount of symbols should be the newline.
                    //i.e. theres only 1 symbol
                    //we cannot append infront of it else its invalid given the line model im using
                    return;
                }

                Node curNext = _currentCell.NextCell;
                Node curPrevious = _currentCell.PreviousCell;
                Node cur = _currentCell;

                curPrevious.NextCell = node;
                node.PreviousCell = curPrevious;
                cur.PreviousCell = node;
                node.NextCell = cur;
                _currentCell = node;
            }

            if (value == Environment.NewLine)
            {
                if (_currentLine == null)
                {
                    _currentLine = (LineNode)node;
                }
                else
                {
                    LineNode curNext = _currentLine.NextLine;
                    LineNode curPrevious = _currentLine.PreviousLine;
                    LineNode cur = _currentLine;

                    curPrevious.NextLine = ((LineNode)node);
                    ((LineNode)node).PreviousLine = curPrevious;
                    cur.PreviousLine = ((LineNode)node);
                    ((LineNode)node).NextLine = cur;
                    _currentLine = ((LineNode)node); 
                }
                _numberOfLines++;
                _currentLineIndex++;
            }

            _numberOfCells++;
        }
        public void Delete(int index)
        {
            if (index < 0 || index >= _numberOfCells)
                throw new IndexOutOfRangeException();
             
            if (index > _currentCellIndex)
                SeekCells(index - _currentCellIndex, SeekDirection.Head);
            else
                SeekCells(_currentCellIndex - index, SeekDirection.Tail);
             
            Node pre = _currentCell.PreviousCell;
            Node aft = _currentCell.NextCell;
            if (pre != null)
                pre.NextCell = aft;
            if (aft != null)
                aft.PreviousCell = pre;

            if (_currentCell.GetType() == typeof(LineNode))
            {
                LineNode preLineNode = _currentLine.PreviousLine;
                LineNode aftLineNode = _currentLine.NextLine;

                if (preLineNode != null)
                    preLineNode.NextLine = aftLineNode;
                if (aftLineNode != null)
                    aftLineNode.PreviousLine = preLineNode;

                if (_currentLine != null)
                {
                    _currentLine.PreviousLine = null;
                    _currentLine.NextLine = null;
                    _currentLine = null;
                }

                if (aftLineNode != null && preLineNode == null)
                {
                    _currentLine = aftLineNode;
                }
                else if (preLineNode != null && aftLineNode == null)
                {
                    _currentLine = preLineNode;
                    _currentLineIndex--;
                }
                else if (preLineNode != null && aftLineNode != null)
                {
                    _currentLine = aftLineNode;
                }
                _numberOfLines--;
            }

            if (_currentCell != null)
            {
                _currentCell.PreviousCell = null;
                _currentCell.NextCell = null;
                _currentCell = null;
            }

            if (aft != null && pre == null)
            {
                _currentCell = aft;
            }
            else if (pre != null && aft == null)
            {
                _currentCell = pre;
                _currentCellIndex--;
            }
            else if (pre != null && aft != null)
            {
                _currentCell = aft;
            }
            _numberOfCells--;
        }
        public void Clear()
        {
            int x = CellCount;
            for (int i = 0; i < x; i++)
            {
                Delete(0);
            }
        }
        public Node this[int index]
        {
            get
            {
                if (index < 0 || index >= _numberOfCells)
                    throw new IndexOutOfRangeException();
                
                if (index > _currentCellIndex)
                    SeekCells(index - _currentCellIndex, SeekDirection.Head);
                else
                    SeekCells(_currentCellIndex - index, SeekDirection.Tail);
                return _currentCell;
            }
        }
        public int CellCount
        {
            get => _numberOfCells;
        }
        public int LineCount
        {
            get => _numberOfLines;
        }
        public int CellIndex
        {
            get => _currentCellIndex;
            set
            {
                if (value < 0 || value > CellCount - 1)
                    throw new IndexOutOfRangeException(); 

                if (value > _currentCellIndex)
                    SeekCells(value - _currentCellIndex, SeekDirection.Head);
                else
                    SeekCells(_currentCellIndex - value, SeekDirection.Tail);
            }
        }
        public int LineIndex
        {
            get => _currentLineIndex;
        }
        public override string ToString()
        {
            if (_numberOfCells <= 0)
                return string.Empty;
             
            int tmpIndex = CellIndex;
            CellIndex = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < CellCount; i++)
            {
                sb.Append(this[i].Data);
            }
            CellIndex = tmpIndex;
            return sb.ToString(); 
        }
    }

    internal enum SeekDirection
    {
        Head,
        Tail
    }
}
