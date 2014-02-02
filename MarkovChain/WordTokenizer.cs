//
//  WordTokenizer.cs
//
//  Author:
//       Johannes Gotlen <johannes.gotlen@hellothere.se>
//
//  Copyright (c) 2014 
//
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.IO;
using System.Collections.Generic;

namespace MarkovChain
{
	public class WordTokenizer
	{
		const int BUFFER_SIZE = 4096;
		const int MAX_WORDSIZE = 128;
		public char[] buffer = new char[BUFFER_SIZE];
		int currentWordSize = 0;
		readonly char[] currentWord = new char[MAX_WORDSIZE];
		readonly char[] punctuation = new []{ '.', ',', '?', '!', ';' };
		readonly List<string> result = new List<string>();

		public List<string> ParseText(TextReader pReader)
		{
			result.Clear();

			int	count = 0;
			do
			{
				count = pReader.ReadBlock(buffer, 0, BUFFER_SIZE);
				for (int i = 0; i < count; ++i)
				{
					char c = buffer[i];
					if (char.IsWhiteSpace(c))
					{
						ParseWhitespace();
					}
					else if (Contains(punctuation, c))
					{
						ParsePunctuation(c);
					}
					else if (char.IsLetterOrDigit(c))
					{
						ParseLetter(c);
					}else{
						ParseLetter(c);
					}
				}
			
			} while(count == BUFFER_SIZE);
			return result;
		}

		bool Contains(char[] pCollection, char pItem)
		{
			for (int i = 0; i < pCollection.Length; i++)
			{
				if (pItem == pCollection[i])
					return true;
			}
			return false;
		}

		bool Contains(string pCollection, char pItem)
		{
			for (int i = 0; i < pCollection.Length; i++)
			{
				if (pItem == pCollection[i])
					return true;
			}
			return false;
		}

		void ParsePunctuation(char c)
		{
			PushWord();
			AddLetter(c);
			AddLetter(' ');
			PushWord();
		}

		void ParseLetter(char c)
		{
			AddLetter(c);
		}

		void ParseWhitespace()
		{
			if (currentWordSize > 0)
			{
				PushWord();
				AddLetter(' ');
				PushWord();
			}
		}

		void PushWord()
		{
			string s = new String(currentWord, 0, currentWordSize);
			result.Add(s);
			currentWordSize = 0;
		}

		void AddLetter(char c)
		{
			currentWord[currentWordSize++] = char.ToLower(c);
		}
	}
}
