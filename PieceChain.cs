using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

namespace vrim
{
	public class PieceChain
	{
		private char[] fileBuf;
		private List<string> addBuf;
		private Stack<PieceRange> undoStack;
		private Stack<PieceRange> redoStack;
		private int numChars = 0;

		private Piece head, tail;

		public PieceChain ()
		{
			fileBuf = null;
			head = new Piece (0, 0, false);
			tail = new Piece (0, 0, false);
			head.next = tail;
			tail.next = head;
			addBuf = new List<string> ();
			undoStack = new Stack<PieceRange> ();
			redoStack = new Stack<PieceRange> ();
		}

		public PieceChain (char[] origFile)
		{
			fileBuf = origFile;
			head = new Piece (0, 0, false);
			tail = new Piece (0, 0, false);
			head.next = tail;
			tail.prev = head;

			Piece origPiece = new Piece (0, fileBuf.Length, true);
			head.next = origPiece;
			origPiece.prev = head;
			origPiece.next = tail;
			tail.prev = origPiece;

			addBuf = new List<string> ();
			numChars = fileBuf.Length;

			undoStack = new Stack<PieceRange> ();
			redoStack = new Stack<PieceRange> ();

		}

		public void Insert(int pos, string addition)
		{
			if (!IsAcceptablePos(pos))
				return;
			numChars += addition.Length;
			Tuple<Piece, int, bool> pieceAndPos = PieceFromPos (pos);
			bool isBoundary = pieceAndPos.Item3;
			Piece currPiece = pieceAndPos.Item1;
			int currPos = pieceAndPos.Item2;
			if (isBoundary) {
				Piece insertedPiece = new Piece (addBuf.Count, addition.Length, false);
				PieceRange newRange = new PieceRange (insertedPiece, insertedPiece, false);
				PieceRange origRange = new PieceRange (currPiece, currPiece.next, true);
				SwapPieceRanges (origRange, newRange, true);
			} else {
				Piece insertFirst = new Piece(currPiece.offset, pos - currPos, currPiece.inFileBuf);
				Piece insertSecond = new Piece(addBuf.Count, addition.Length, false);
				Piece insertThird = new Piece(currPiece.offset + insertFirst.length, currPiece.length - insertFirst.length,currPiece.inFileBuf);
				Patch (insertFirst, insertSecond, insertThird);

				PieceRange newRange = new PieceRange (insertFirst, insertThird, false);
				PieceRange oldRange = new PieceRange (currPiece, currPiece, false);
				SwapPieceRanges (oldRange, newRange, true);

			}
			addBuf.Add(addition);

		}

		public void Undo()
		{
			UndoRedo (undoStack);
		}

		public void Redo()
		{
			UndoRedo (redoStack);
		}

		private void UndoRedo(Stack<PieceRange> stack)
		{
			PieceRange stackTop = stack.Pop ();
			if (stackTop.boundary) {
				PieceRange boundary = new PieceRange (stackTop.first, stackTop.last, true);
				SwapPieceRanges (boundary, stackTop, false);
				return;
			}
			Piece before = stackTop.first.prev;
			Piece after = stackTop.last.next;
			PieceRange toReplace = new PieceRange (before.next, after.prev, false);
			SwapPieceRanges (toReplace, stackTop, false);
		}

		public string GetContentsTesting()
		{
			StringBuilder sb = new StringBuilder ("");
			for (Piece curr = head.next; curr != tail; curr = curr.next) {
				if (curr.inFileBuf) {
					for (int i = 0; i < curr.length; i++)
						sb.Append ((fileBuf [curr.offset + i]));
				} else {
					sb.Append ((addBuf [curr.offset]));
				}
			}
			return sb.ToString ();
		}

		private void SwapPieceRanges(PieceRange origRange, PieceRange newRange, bool undo)
		{
			Piece before = origRange.first.prev;
			Piece after = origRange.last.next;

			if (origRange.boundary) {
				before = origRange.first;
				after = origRange.last;
				Piece clone = new Piece (origRange.first.offset, origRange.first.length, origRange.first.inFileBuf);
				clone.next = origRange.first.next;
				clone.prev = origRange.last.prev;
				Patch (before, newRange.first, after);
				origRange = new PieceRange (clone, clone, true);
			} else {
				before.next = newRange.first;
				newRange.first.prev = before;
				newRange.last.next = after;
				after.prev = newRange.last;
			}

			if (undo)
				undoStack.Push (origRange);
			else
				redoStack.Push (origRange);

		}
			
		private void Patch(Piece first, Piece second, Piece third)
		{
			first.next = second;
			second.prev = first;
			second.next = third;
			third.prev = second;
		}

		private bool IsAcceptablePos(int pos)
		{
			return (pos >= 0 && pos <= numChars);
		}

		private Tuple<Piece, int, bool> PieceFromPos(int pos)
		{
			int currPos = 0;
			for (Piece curr = head.next; curr != tail; curr = curr.next) {
				if (currPos + (curr.length - 1) > pos) {
					return new Tuple<Piece, int, bool> (curr, currPos, false);
				} else if (currPos + (curr.length - 1) == pos) {
					return new Tuple<Piece, int, bool> (curr, currPos, true);
				}
				currPos += curr.length;
			}

			return new Tuple<Piece, int, bool> (head, 0, true);

		}
			
		private class PieceRange
		{
			public Piece first, last;
			public bool boundary;
			public int length;

			public PieceRange(Piece f, Piece l, bool b, int s)
			{	
				first = f;
				last = l;
				boundary = b;
				length = s;
			}

			public PieceRange(Piece first, Piece last, bool isBoundary)
			{
				this.first = first;
				this.last = last;
				boundary = isBoundary;
				length = 0;
				for (Piece curr = first; curr != last.next; curr = curr.next) {
					length += curr.length;
				}
			}
		}
		private class Piece
		{
			public Piece next;
			public Piece prev;
			public int offset, length;
			public bool inFileBuf;

			public Piece(int offset, int length, bool inFileBuf)
			{
				this.offset = offset;
				this.length = length;
				this.inFileBuf = inFileBuf;
				next = null;
				prev = null;
			}
		}
	}
	[TestFixture]
	public class PieceChainTest
	{
		[Test]
		public void EmptyPieceChainInsertAtStart()
		{
			PieceChain chain = new PieceChain ();
			string inserted = "I am not a number, I am a free man!";
			chain.Insert (0, inserted);
			Assert.AreEqual (chain.GetContentsTesting (), inserted);
		}

		[Test]
		public void EmptyPieceChainInsertAndUndo()
		{
			PieceChain chain = new PieceChain ();
			string inserted = "I am not a number, I am a free man!";
			chain.Insert (0, inserted);
			chain.Undo ();
			Assert.AreEqual (chain.GetContentsTesting(), "");
			chain.Redo ();
			Assert.AreEqual (chain.GetContentsTesting (), inserted);
		}

		[Test]
		public void FullPieceChainInsertAtStart()
		{
			PieceChain chain = new PieceChain (new char[] { 'a', 'b', 'c', 'd', 'e' });
			chain.Insert (0, "12345");
			Assert.AreEqual (chain.GetContentsTesting (), "12345abcde");
		}

		[Test]
		public void FullPieceChainInsertAndUndo()
		{
			PieceChain chain = new PieceChain (new char[] { 'a', 'b', 'c', 'd', 'e' });
			chain.Insert (3, "12345");
			chain.Undo ();
			Assert.AreEqual ("abcde", chain.GetContentsTesting ());
		}

		[Test]
		public void FullPieceChainInsertUndoRedo()
		{
			PieceChain chain = new PieceChain (new char[] { 'a', 'b', 'c', 'd', 'e' });
			chain.Insert (3, "12345");
			chain.Undo ();
			chain.Redo ();
			Assert.AreEqual ("abc12345de", chain.GetContentsTesting ());
		}

		[Test]
		public void FullPieceChainBoundaryInsertBack()
		{
			PieceChain chain = new PieceChain (new char[] { 'a', 'b', 'c', 'd', 'e' });
			chain.Insert (5, "12345");
			Assert.AreEqual ("12345abcde", chain.GetContentsTesting ());
		}

		[Test]
		public void FullPieceChainBoundaryInsertFront()
		{
			PieceChain chain = new PieceChain (new char[] { 'a', 'b', 'c', 'd', 'e' });
			chain.Insert (0, "12345");
			Assert.AreEqual ("12345abcde", chain.GetContentsTesting ());
		}


	}
		
}

