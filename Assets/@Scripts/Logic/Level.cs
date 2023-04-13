using System;
using TetrisMayhem.Sound;
using TMPro;
using UnityEngine;

namespace TetrisMayhem.Logic
{
    public class Level : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _levelUpText;
		private AudioManager _soundEffectsManager;
		
		public int ScoreForLevel = 3000;
		public int IncrementSpeedBase = 1;
		public float IncrementPower = 1.1f;

		public TextMeshProUGUI LevelText;

		public int CurrentLevel { get; private set; }
		public int HighestLevel { get; set; }
		public bool HighestLevelReached { get; private set; }

		public event Action<int> OnLevelUped;

		private void Awake()
		{
			_soundEffectsManager = FindObjectOfType<AudioManager>();
		}

		private void Start()
		{
			HighestLevelReached = false;
			CurrentLevel = 1;
			OnLevelUped += OnLevelUp;
		}

		private void OnLevelUp(int currentLevel)
		{
			LevelText.text = "Level " + currentLevel;

			if (currentLevel > this.HighestLevel)
			{
				HighestLevel = currentLevel;
				HighestLevelReached = true;
			}

			_levelUpText.gameObject.SetActive(true);
			_soundEffectsManager.PlayLevelUpSound();
		}
	}
}
