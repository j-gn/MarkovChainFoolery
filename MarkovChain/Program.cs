//
//  Program.cs
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
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace MarkovChain
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var stream = File.OpenRead("../../20000LeaguesUnderTheseasByJulesVerne.txt");
			var tr = new StreamReader(stream);
			var t = new WordTokenizer();
			var result = t.ParseText(tr);
		
			MarkovNetworkGenerator<string> generator = new MarkovNetworkGenerator<string>(4);
			generator.ParseData(result);

			var fb = new TextBeautyfier();
			foreach (string s in generator.Generate(1000))
			{
				Console.Write(fb.Parse(s));
			}

		

		}
	}
	public class TextBeautyfier{
		readonly char[] punctuation = new []{ '.', '?', '!', ';' };
		bool useCaption = true;
		public string Parse(string pText){
			for (int i = 0; i < pText.Length; i++)
			{

				if (useCaption)
				{
					if (char.IsLetter(pText[i]))
					{
						useCaption = false;
						var arr = pText.ToCharArray();
						arr[i] = char.ToUpper(pText[i]);
						return new string(arr);
					}
				}
				else
				{
					if (Contains(punctuation, pText[i]))
					{
						useCaption = true;
					}
				}
			}
			return pText;
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

	}

}
