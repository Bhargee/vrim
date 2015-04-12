using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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
				PieceRange origRange = new PieceRange (currPiece, currPiece, true);
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
			PieceRange stackTop = undoStack.Pop ();
			if (stackTop.boundary) {
				// TODO
			} else {
				Piece before = stackTop.first.prev;
				Piece after = stackTop.last.next;
				PieceRange toReplace = new PieceRange (before.next, after.prev, false);
				SwapPieceRanges (toReplace, stackTop, false);
			}
		}

		public void PrintContentsTesting()
		{
			for (Piece curr = head.next; curr != tail; curr = curr.next) {
				if (curr.inFileBuf) {
					for (int i = 0; i < curr.length; i++)
						Console.Write (fileBuf [curr.offset + i]);
				} else {
					Console.Write (addBuf[curr.offset]);
				}
			}
		}

		private void SwapPieceRanges(PieceRange origRange, PieceRange newRange, bool undo)
		{
			Piece before = origRange.first.prev;
			Piece after = origRange.last.next;

			if (origRange.boundary) {
				before = origRange.first;
				after = origRange.first.next;
				Patch (before, newRange.first, after);
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
			return (pos >= 0 && pos < numChars);
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

			return null;

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
		
}

