using System;
using UnityEngine;

namespace TetrisMayhem.Sound
{
// TODO: factory or addressables
	public class AudioManager : MonoBehaviour
	{
		[SerializeField]private AudioSource _landAudioSourcePrefab;
		[SerializeField] private AudioSource _levelUpAudioSourcePrefab;
		[SerializeField] private AudioSource _reactAudioSourcePrefab;
		[SerializeField] private AudioSource _explosionAudioSourcePrefab;

		private AudioSource _landAudioSource;
		private AudioSource _levelUpAudioSource;
		private AudioSource _reactAudioSource;
		private AudioSource _explosionAudioSource;

		private void Start()
		{
			_landAudioSource = Instantiate(_landAudioSourcePrefab, transform);
			_levelUpAudioSource = Instantiate(_levelUpAudioSourcePrefab, transform);
			_reactAudioSource = Instantiate(_reactAudioSourcePrefab, transform);
			_explosionAudioSource = Instantiate(_explosionAudioSourcePrefab, transform);
		}
        public void PlayReactSound(int level = 1)
		{
			_reactAudioSource.pitch = (float)Math.Pow(1.05946, (level - 1));
			_reactAudioSource.Play();
		}

        public void PlayLandSound() => _landAudioSource.Play();

        public void PlayLevelUpSound() => _levelUpAudioSource.Play();

        public void PlayExplosionSound() => _explosionAudioSource.Play();
    }
}

