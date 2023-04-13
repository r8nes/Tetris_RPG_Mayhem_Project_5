using System.Collections.Generic;
using System.Linq;
using System;

namespace TetrisMayhem.Logic
{
    public class Randomizer
	{
		private readonly Random random = new Random();

		public int GetRandomIndex(Dictionary<int, int> weights)
		{
			List<KeyValuePair<int, int>> list = Sort(weights);
			int num = 0;
			foreach (KeyValuePair<int, int> keyValuePair in list)
			{
				num += keyValuePair.Value;
			}

			int key = list[list.Count - 1].Key;
			int num2 = random.Next(num);

			for (int i = 0; i < list.Count(); i++)
			{
				if (num2 < list[i].Value)
				{
					key = list[i].Key;
					break;
				}
				num2 -= list[i].Value;
			}
			return key;
		}

		private List<KeyValuePair<int, int>> Sort(Dictionary<int, int> weights)
		{
			List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>(weights);
			list.Sort((KeyValuePair<int, int> x, KeyValuePair<int, int> y) => x.Value.CompareTo(y.Value));
			return list;
		}
	}
}
