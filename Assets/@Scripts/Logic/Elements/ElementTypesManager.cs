using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace TetrisMayhem.Logic
{
    public class ElementTypesManager : MonoBehaviour
	{
		public Level LevelSystem;
		public ElementType[] ElementTypes;

		private Randomizer _weightedRandomizer = new Randomizer();

		public ElementType GetRandomElementType()
		{
			ElementType[] array = (from x in ElementTypes
								   where x.levelRequirements <= LevelSystem.CurrentLevel
								   select x).ToArray();
			
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			
			for (int i = 0; i < array.Count<ElementType>(); i++)
			{
				dictionary.Add(i, array[i].spawnChance);
			}
			
			return array[_weightedRandomizer.GetRandomIndex(dictionary)];
		}

		public ElementType GetElementType(WeaponType type)
		{
			ElementType elementType = (from x in ElementTypes
									   where x.type == type
									   select x).First<ElementType>();

			if (elementType == null)
			{
				throw new Exception("Type is invalid.");
			}

			return elementType;
		}
	}
}
