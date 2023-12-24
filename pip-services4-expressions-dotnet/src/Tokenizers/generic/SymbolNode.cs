using System;

using PipServices4.Expressions.IO;
using PipServices4.Expressions.Tokenizers.Utilities;

namespace PipServices4.Expressions.Tokenizers.Generic
{
    /// <summary>
    /// A <code>SymbolNode</code> object is a member of a tree that contains all possible prefixes
    /// of allowable symbols. Multi-character symbols appear in a <code>SymbolNode</code> tree
    /// with one node for each character.
    /// <p/>
    /// For example, the symbol <code>=:~</code> will appear in a tree as three nodes. The first
    /// node contains an equals sign, and has a child; that child contains a colon and has a child;
    /// this third child contains a tilde, and has no children of its own. If the colon node had
    /// another child for a dollar sign character, then the tree would contain the symbol <code>=:$</code>.
    /// <p/>
    /// A tree of <code>SymbolNode</code> objects collaborate to read a (potentially multi-character)
    /// symbol from an input stream. A root node with no character of its own finds an initial node
    /// that represents the first character in the input. This node looks to see if the next character
    /// in the stream matches one of its children. If so, the node delegates its reading task to its child.
    /// This approach walks down the tree, pulling symbols from the input that match the path down the tree.
    /// <p/>
    /// When a node does not have a child that matches the next character, we will have read the longest
    /// possible symbol prefix. This prefix may or may not be a valid symbol.
    /// Consider a tree that has had <code>=:~</code> added and has not had <code>=:</code> added.
    /// In this tree, of the three nodes that contain <code>=:~</code>, only the first and third contain
    /// complete symbols. If, say, the input contains <code>=:a</code>, the colon node will not have
    /// a child that matches the 'a' and so it will stop reading. The colon node has to "unread": it must
    /// push back its character, and ask its parent to unread. Unreading continues until it reaches
    /// an ancestor that represents a valid symbol.
    /// </summary>
    public class SymbolNode
    {
        private SymbolNode _parent;
        private char _character;
        private CharReferenceMap<SymbolNode> _children;
        private TokenType _tokenType;
        private bool _valid;
        private string _ancestry;

        /// <summary>
        /// Constructs a SymbolNode with the given parent, representing the given character.
        /// </summary>
        /// <param name="parent">This node's parent</param>
        /// <param name="character">This node's associated character.</param>
        public SymbolNode(SymbolNode parent, char character)
        {
            _parent = parent;
            _character = character;
        }

        /// <summary>
        /// Find or create a child for the given character.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal SymbolNode EnsureChildWithChar(char value)
        {
            if (_children == null)
            {
                _children = new CharReferenceMap<SymbolNode>();
            }

            SymbolNode childNode = _children.Lookup(value);
            if (childNode == null)
            {
                childNode = new SymbolNode(this, value);
                _children.AddInterval(value, value, childNode);
            }
            return childNode;
        }

        /// <summary>
        /// Add a line of descendants that represent the characters in the given string.
        /// </summary>
        /// <param name="value"></param>
        internal void AddDescendantLine(string value, TokenType tokenType)
        {
            if (value.Length > 0)
            {
                SymbolNode childNode = EnsureChildWithChar(value[0]);
                childNode.AddDescendantLine(value.Substring(1), tokenType);
            }
            else
            {
                _valid = true;
                _tokenType = tokenType;
            }
        }

        /// <summary>
        /// Find the descendant that takes as many characters as possible from the input.
        /// </summary>
        /// <param name="scanner"></param>
        /// <returns></returns>
        internal SymbolNode DeepestRead(IScanner scanner)
        {
            char nextSymbol = scanner.Read();
            SymbolNode childNode = !CharValidator.IsEof(nextSymbol) ? FindChildWithChar(nextSymbol) : null;
            if (childNode == null)
            {
                scanner.Unread();
                return this;
            }
            return childNode.DeepestRead(scanner);
        }

        /// <summary>
        /// Find a child with the given character.
        /// </summary>
        /// <param name="value"></param>
        internal virtual SymbolNode FindChildWithChar(char value)
        {
            return _children != null ? _children.Lookup(value) : null;
        }

        /// <summary>
        /// Find a descendant which is down the path the given string indicates.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        //internal SymbolNode FindDescendant(string value)
        //{
        //    char tempChar = value.Length > 0 ? value[0] : CharValidator.Eof;
        //    SymbolNode childNode = FindChildWithChar(tempChar);
        //    if (!CharValidator.IsEof(tempChar) && childNode != null && value.Length > 1)
        //    {
        //        childNode = childNode.FindDescendant(value.Substring(1));
        //    }
        //    return childNode;
        //}


        /// <summary>
        /// Unwind to a valid node; this node is "valid" if its ancestry represents a complete symbol.
        /// If this node is not valid, put back the character and ask the parent to unwind.
        /// </summary>
        /// <param name="scanner"></param>
        /// <returns></returns>
        internal SymbolNode UnreadToValid(IScanner scanner)
        {
            if (!_valid && _parent != null)
            {
                scanner.Unread();
                return _parent.UnreadToValid(scanner);
            }
            return this;
        }

        //internal SymbolNode Parent { get { return _parent; } }
        //internal SymbolNode[] Children { get { return _children; } }
        //internal char Character { get { return _character; } }
        internal bool Valid
        {
            get { return _valid; }
            set { _valid = value; }
        }

        public TokenType TokenType
        {
            get { return _tokenType; }
            set { _tokenType = value; }
        }

        /// <summary>
        /// Show the symbol this node represents.
        /// </summary>
        /// <returns>The symbol this node represents</returns>
        public string Ancestry()
        {
            if (_ancestry == null)
            {
                _ancestry = (_parent != null ? _parent.Ancestry() : "")
                    + (_character != '\0' ? _character.ToString() : "");
            }
            return _ancestry;
        }
    }
}
