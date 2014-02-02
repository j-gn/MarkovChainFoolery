//
//  MarkovNode.cs
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
using System.Collections;
namespace MarkovChain
{
	public class MarkovNode{
		public static Random rand = new Random(); 
		public Dictionary<int, int> occurences = new Dictionary<int, int>();
		public List<KeyValuePair<int,int>> occurenceList;
		public int totalOccurences;

		public void Finialize(){
			occurenceList = new List<KeyValuePair<int, int>>(occurences);
			totalOccurences = 0;
			foreach(var val in occurences){
				totalOccurences += val.Value;
			}
		}
		public int GetRandom(){
			int l = (int)Math.Min(rand.NextDouble() * (double)totalOccurences, totalOccurences -1);
			int i = 0;
			for (; i < (occurenceList.Count - 1) && l > 0; ++i){
				l -= occurenceList[i].Value;
			}

			return occurenceList[i].Key;
		}
	}

}
