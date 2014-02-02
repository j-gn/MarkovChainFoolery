//
//  MarkovNetworkGenerator.cs
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

	public class MarkovNetworkGenerator<T> where T : IEquatable<T>
	{
		public MarkovNetworkGenerator( int pPatternSize ){
			PATTERN_SIZE = pPatternSize;
			patternBuffer = new int[PATTERN_SIZE];
		}
		readonly int PATTERN_SIZE;
		readonly int[] patternBuffer;

		Dictionary<int[], MarkovNode> _markovNetwork = new Dictionary<int[], MarkovNode>(new IntArrayEqualityComparer());
		Dictionary<int, T> _atoms = new Dictionary<int, T>(); //a list of all possible atoms

		public void ParseData(List<T> pTokens)
		{

			List<int> hashcodes = new List<int>(pTokens.Count);

			for(int i = 0; i < pTokens.Count; ++i)
			{
				//convert the whole list to ints
				hashcodes.Add(pTokens[i].GetHashCode());
				//make a map from int -> object
				TryLearnAtom(hashcodes[i], pTokens[i]);
			}

			//from now on only handles ints
			int[] smallbuffer = new int[PATTERN_SIZE];
			for (int i = PATTERN_SIZE; i < hashcodes.Count; i++)
			{
				int currentCode = hashcodes[i];
				hashcodes.CopyTo(i - PATTERN_SIZE, smallbuffer, 0, PATTERN_SIZE);

				MarkovNode node;
				if (_markovNetwork.TryGetValue(smallbuffer, out node))
				{
					//Console.WriteLine( " found pattern !");
				}
				else
				{
					_markovNetwork[smallbuffer.Clone() as int[]] = node = new MarkovNode();
				}
				int count = 0;
				if (node.occurences.TryGetValue(currentCode, out count))
				{
				}
				node.occurences[currentCode] = count + 1;
			}

			foreach (var n in _markovNetwork.Values)
			{
				n.Finialize();
			}
		
		}

		void TryLearnAtom(int i, T t)
		{
			T r;
			if (_atoms.TryGetValue(i, out r))
			{
				if (!r.Equals(t))
				{
					//throw new Exception(string.Format("non unique hash discovered: {0} != {1}", t, r));
				}
			}
			else
			{
				_atoms[i] = t;
			}
		}


		public T FindNext( IList<T> pPattern ){
			int patternStartIndex = pPattern.Count - PATTERN_SIZE;
			for(int i = 0; i < PATTERN_SIZE; ++i){
				int patternIndex = patternStartIndex + i;
				int hash = pPattern[patternIndex].GetHashCode();
				patternBuffer[i] = hash;
				if (!_atoms.ContainsKey(hash))
				{
					throw new Exception("use of non existing atom " + pPattern[patternIndex]);
				}
			}
			MarkovNode o;
			if (_markovNetwork.TryGetValue(patternBuffer, out o))
			{
				return _atoms[o.GetRandom()];
			}
			else
			{
				throw new Exception("non existing pattern");
			}
		}
		int GenerateNext(IList<int> pBuffer, int pBufferSize){
			int startIndex = pBufferSize - PATTERN_SIZE;
			for(int i = 0; i < PATTERN_SIZE; ++i){
				int patternIndex = startIndex + i;
				int hash = pBuffer[patternIndex];
				patternBuffer[i] = hash;
			}
			MarkovNode o;
			if (_markovNetwork.TryGetValue(patternBuffer, out o))
			{
				return o.GetRandom();
			}
			else
			{
				throw new Exception("non existing pattern");
			}
		}

		public T[] Generate(int pCount)
		{
			var keys =  new List<int[]>(_markovNetwork.Keys);
			Random r = new Random();
			int number = (int)Math.Min(r.NextDouble() * (double)keys.Count, keys.Count - 1);

			var start = keys[number];
			int[] hashes = new int[pCount];
			int i = 0;
			for(;i < pCount && i < PATTERN_SIZE; ++i){
				hashes[i] = start[i];
			}

			for(; i < pCount; ++i){
				hashes[i] = GenerateNext(hashes, i);
			}

			T[] result = new T[pCount];
			for(i= 0 ; i <result.Length; ++i){
				result[i] = _atoms[hashes[i]];
			}
			return result;

		}
	}
}
