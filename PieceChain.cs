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
		private int textLength = 0;

		private Piece head, tail;

		public PieceChain ()
		{
			fileBuf = null;
			head = new Piece (0, 0, false);
			tail = new Piece (0, 0, false);
			head.next = tail;
			tail.prev = head;
			addBuf = new List<string> ();
			undoStack = new Stack<PieceRange> ();
			redoStack = new Stack<PieceRange> ();
		}

		public PieceChain (char[] origFile)
		{
			fileBuf = origFile;
			head = new Piece (0, 0, false);
			tail = new Piece (0, 0, false);
			Piece origPiece = new Piece (0, fileBuf.Length, true);
			head.next = origPiece;
			origPiece.prev = head;
			origPiece.next = tail;
			tail.prev = origPiece;

			addBuf = new List<string> ();
			textLength = fileBuf.Length;

			undoStack = new Stack<PieceRange> ();
			redoStack = new Stack<PieceRange> ();

		}

		public bool Insert(int pos, string toInsert)
		{
			if (pos > textLength)
				return false;

			Tuple<Piece, int> pp = PieceFromPos (pos);
			if (pp == null)
				return false;
				
			Piece piece = pp.Item1;
			int piecePos = pp.Item2;
			int insOffset = pos - piecePos;

			addBuf.Add (toInsert);
			PieceRange oldRange;

			// general case #1 - inserting at boundary
			if (insOffset == 0) {
				oldRange = PieceRange.InitUndo (pos, toInsert.Length);
				oldRange.PieceBoundary (piece.prev, piece);
				PieceRange newRange = new PieceRange (pos);
				newRange.Append (new Piece (addBuf.Count - 1, toInsert.Length, false));
				SwapPieceRange (oldRange, newRange);
			} else {
				oldRange = PieceRange.InitUndo (pos, toInsert.Length);
				oldRange.Append (piece);
				PieceRange newRange = new PieceRange (pos);
				newRange.Append (new Piece (piece.offset, insOffset, true));
				newRange.Append (new Piece (addBuf.Count - 1, toInsert.Length, false));
				newRange.Append (new Piece (piece.offset + insOffset, piece.length - insOffset, true));
				SwapPieceRange (oldRange, newRange);
			}
			undoStack.Push (oldRange);
			textLength += toInsert.Length;
			return true;

		}

		public bool Undo()
		{
			return UndoRedo (undoStack, redoStack);
		}

		public bool Redo()
		{
			return UndoRedo (redoStack, undoStack);
		}

		private bool UndoRedo(Stack<PieceRange> src, Stack<PieceRange> dest)
		{
			if (src.Count == 0)
				return false;

			do {
				PieceRange toRestore = src.Pop ();
				dest.Push (toRestore);

				bool undoOrRedo = (src == undoStack);

				RestorePieceRange (toRestore, undoOrRedo);
			} while (src.Count != 0);

			return true;
		}

		private void RestorePieceRange(PieceRange toRestore, bool undoOrRedo)
		{
			if(toRestore.boundary) {
				Piece first = toRestore.first.next;
				Piece last = toRestore.last.prev;

				// unlink pieces from main chain
				toRestore.first.next = toRestore.last;
				toRestore.last.prev = toRestore.first;
				// store piecerange we just removed
				toRestore.first = first;
				toRestore.last = last;
				toRestore.boundary = true;
			} else {
				Piece first = toRestore.first.prev;
				Piece last = toRestore.last.next;
				// are we moving into an "empty" region (like between two pieces)?
				if (first.next == last) {
					// move old pieces back into old positions
					first.next = toRestore.first;
					last.prev = toRestore.last;
					// store the range we just removed
					toRestore.first = first;
					toRestore.last = last;
					toRestore.boundary = true;
				}
				// we are replacing a range of pieces, so swap that range with the top of our undo stack
				else {
					// find the range in the chain
					first = first.next;
					last = last.prev;
					// unlink the pieces from the main chain
					first.prev.next = toRestore.first;
					last.next.prev = toRestore.last;
					//store the range we just removed
					toRestore.first = first;
					toRestore.last = last;
					toRestore.boundary = false;
				}
			}
			// TODO update length
		}

		private void SwapPieceRange(PieceRange oldRange, PieceRange newRange)
		{
			if (oldRange.boundary) {
				if (!newRange.boundary) {
					oldRange.first.next = newRange.first;
					oldRange.last.prev = newRange.last;
					newRange.first.prev = oldRange.first;
					newRange.last.next = oldRange.last;
				}
			} else {
				if (newRange.boundary) {
					oldRange.first.prev.next = oldRange.last.next;
					oldRange.last.next.prev = oldRange.first.prev;
				} else {
					oldRange.first.prev.next = newRange.first;
					oldRange.last.next.prev = newRange.last;
					newRange.first.prev = oldRange.first.prev;
					newRange.last.next = oldRange.last.next;
				}
			}
		}

		private Tuple<Piece, int> PieceFromPos(int pos)
		{
			Piece currPiece;
			int currPos = 0;

			for (currPiece = head.next; currPiece.next != null; currPiece = currPiece.next) {
				if (pos >= currPos && pos < currPos + currPiece.length) {
					return new Tuple<Piece, int> (currPiece, currPos);
				}
				currPos += currPiece.length;
			}
			// insert at tail
			if (currPiece != null && pos == currPos) {
				return new Tuple<Piece, int> (currPiece, currPos);
			}

			return null;
		}

		public string GetContentsTesting()
		{
			StringBuilder sb = new StringBuilder ("");
			Piece curr;
			for (curr = head.next; curr.next != null; curr = curr.next) {
				if (curr.inFileBuf) {
					for (int i = 0; i < curr.length; i++) {
						sb.Append (fileBuf [curr.offset + i]);
					}
				} else {
					sb.Append (addBuf [curr.offset]);
				}
			}
			return sb.ToString ();
		}
					
		private class PieceRange
		{
			public Piece first = null; 
			public Piece last = null;
			public bool boundary;
			public int length;
			public int index;

			private PieceRange(int index, int length)
			{
				this.index = index;
				this.length = length;
			}

			public PieceRange(int index)
			{
				length = 0;
				this.index = -1;
			}

			public static PieceRange InitUndo(int index, int length)
			{
				return new PieceRange (index, length);
			}

			public void PieceBoundary(Piece before, Piece after)
			{
				this.first = before;
				this.last = after;
				boundary = true;
			}

			public void Append(Piece p)
			{
				if (p != null) {
					// first span to be added?
					if (this.first == null) {
						first = p;
					} else {
						last.next = p;
						p.prev = last;
					}
					last = p;
					boundary = false;

					length += p.length;
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
			Assert.AreEqual (chain.GetContentsTesting (), "abcde");
			chain.Redo ();
			Assert.AreEqual ("abc12345de", chain.GetContentsTesting ());
		}

		[Test]
		public void FullPieceChainBoundaryInsertBackAndUndo()
		{
			PieceChain chain = new PieceChain (new char[] { 'a', 'b', 'c', 'd', 'e' });
			chain.Insert (5, "12345");
			Assert.AreEqual ("abcde12345", chain.GetContentsTesting ());
			chain.Undo ();
			Assert.AreEqual ("abcde", chain.GetContentsTesting());
			chain.Redo ();
			Assert.AreEqual ("abcde12345", chain.GetContentsTesting ());
		}

		[Test]
		public void FullPieceChainBoundaryInsertFrontAndUndo()
		{
			PieceChain chain = new PieceChain (new char[] { 'a', 'b', 'c', 'd', 'e' });
			chain.Insert (0, "12345");
			Assert.AreEqual ("12345abcde", chain.GetContentsTesting ());
			chain.Undo ();
			Assert.AreEqual ("abcde", chain.GetContentsTesting ());
			chain.Redo ();
			Assert.AreEqual ("1234abcde", chain.GetContentsTesting ());
		}

		[Test]
		public void FullPieceChainInsertMidAndUndoRedo()
		{
			PieceChain chain = new PieceChain (new char[] { 'a', 'b', 'c', 'd', 'e' });
			chain.Insert (3, "12345");
			Assert.AreEqual ("abc12345de", chain.GetContentsTesting ());
			chain.Undo ();
			Assert.AreEqual ("abcde", chain.GetContentsTesting ());
			chain.Redo ();
			Assert.AreEqual ("abc12345de", chain.GetContentsTesting ());
		}

	}
		
}

