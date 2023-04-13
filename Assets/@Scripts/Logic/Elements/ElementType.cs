using System;
using UnityEngine;

namespace TetrisMayhem.Logic
{
    [Serializable]
    public class ElementType
    {
        public int score;
        public int spawnChance;
        public int levelRequirements;

        public WeaponType type;
        public GameObject[] prefab = new GameObject[3];
    }
}
