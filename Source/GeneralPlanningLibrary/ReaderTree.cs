using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary
{
    /// <summary>
    /// Tree formed from input file.
    /// Each tree represents contents of single bracket pair. Children are other trees of words.
    /// </summary>
    public class ReaderTree
    {
        public bool Leaf;
        public string Text;
        public List<ReaderTree> Children;

        public int Line;
        public int IndexInLine;
        public int Index;
        public int Length;

        public ReaderTree(string text, int line, int indexInLine, int index) : this(text)
        {
            this.IndexInLine = indexInLine;
            this.Line = line;
            this.Index = index;
        }

        public ReaderTree(string text)
        {
            this.Leaf = true;
            this.Text = text;
            this.Length = Text.Length;
        }

        public ReaderTree()
        {
            Children = new List<ReaderTree>();
            Leaf = false;
        }

        public ReaderTree(List<ReaderTree> children)
        {
            Children = children;
            Leaf = false;
        }

        /// <summary>
        /// Returns child of given index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public ReaderTree this[int i]
        {
            get { return Children[i]; }
        }

        public override string ToString()
        {
            if (Leaf) return Text;

            string s = "";
            for (int i = 0; i < Children.Count; i++) {
                s += Children[i].ToString();
                if (i != Children.Count -1) s += " ";
            }
            return s;
        }

        /// <summary>
        /// Finished info that is used when showing errors in input.
        /// </summary>
        public void FinishTreeInfo()
        {
            if (Leaf) return;

            if (Children.Count == 0) return;

            foreach (var child in Children)
            {
                child.FinishTreeInfo();
            }

            Index = Children[0].Index;
            Length = Children[0].Length;
            IndexInLine = Children[0].IndexInLine;
            Line = Children[0].Line;
        }
    }
}
