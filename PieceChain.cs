using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace vrim
{
	public class PieceChain
	{
		private char[] _fileBuf;
		private List<string> _addBuf;
		private Stack<_PieceRange> _editStack;
		private int _numChars = 0;

		private _Piece _head, _tail;

		public PieceChain ()
		{
			_fileBuf = null;
			_head = new _Piece (0, 0, false);
			_tail = new _Piece (0, 0, false);
			_head.next = _tail;
			_tail.next = _head;
			_addBuf = new List<string> ();
		}

		public PieceChain (char[] origFile)
		{
			_fileBuf = origFile;
			_head = new _Piece (0, 0, false);
			_tail = new _Piece (0, 0, false);
			_head.next = _tail;
			_tail.prev = _head;

			_Piece origPiece = new _Piece (0, _fileBuf.Length, true);
			_head.next = origPiece;
			origPiece.prev = _head;
			origPiece.next = _tail;
			_tail.prev = origPiece;

			_addBuf = new List<string> ();
			_numChars = _fileBuf.Length;

		}

		public void Insert(int pos, string addition)
		{
			if (!_IsAcceptablePos(pos))
				return;
			_numChars += addition.Length;
			Tuple<_Piece, int, bool> pieceAndPos = _PieceFromPos (pos);
			bool atEnd = pieceAndPos.Item3;
			_Piece currPiece = pieceAndPos.Item1;
			int currPos = pieceAndPos.Item2;
			if (atEnd) {

			} else {
				_Piece before = currPiece.prev;
				_Piece after = currPiece.next;
				_Piece insertFirst = new _Piece(currPiece.offset, pos - currPos, currPiece.inFileBuf);
				_Piece insertSecond = new _Piece(_addBuf.Count, addition.Length, false);
				_Piece insertThird = new _Piece(currPiece.offset + insertFirst.length, currPiece.length - insertFirst.length,currPiece.inFileBuf);
				_Patch (before, insertFirst, insertSecond);
				_Patch (insertSecond, insertThird, after);
			}
			_addBuf.Add(addition);

		}

		public void PrintContentsTesting()
		{
			for (_Piece curr = _head.next; curr != _tail; curr = curr.next) {
				if (curr.inFileBuf) {
					for (int i = 0; i < curr.length; i++)
						Console.Write (_fileBuf [curr.offset + i]);
				} else {
					Console.Write (_addBuf[curr.offset]);
				}
			}
		}


		private void _Patch(_Piece first, _Piece second, _Piece third)
		{
			first.next = second;
			second.prev = first;
			second.next = third;
			third.prev = second;
		}

		private bool _IsAcceptablePos(int pos)
		{
			return (pos >= 0 && pos < _numChars);
		}

		private Tuple<_Piece, int, bool> _PieceFromPos(int pos)
		{
			int currPos = 0;
			for (_Piece curr = _head.next; curr != _tail; curr = curr.next) {
				if (currPos + curr.length > pos) {
					return new Tuple<_Piece, int, bool> (curr, currPos, false);
				} else if (currPos + curr.length == pos) {
					return new Tuple<_Piece, int, bool> (curr, currPos, true);
				}
				currPos += curr.length;
			}

			return null;

		}

			
		private class _PieceRange
		{
			public _Piece first, last;
			public bool boundary;
			public int length;

			public _PieceRange(_Piece f, _Piece l, bool b, int s)
			{	
				first = f;
				last = l;
				boundary = b;
				length = s;
			}

		}
		private class _Piece
		{
			public _Piece next;
			public _Piece prev;
			public int offset, length;
			public bool inFileBuf;

			public _Piece(int offset, int length, bool inFileBuf)
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

