using System;
using UnityEngine;

namespace TetrisMayhem.Logic
{
    public class GameInput : MonoBehaviour
    {
        public KeyCode moveLeft;
        public KeyCode moveRight;
        public KeyCode rotate1;
        public KeyCode rotate2;
        public KeyCode fallQuickly;

        public bool controlsDisabled;

        public event Action OnMoveLeft;
        public event Action OnMoveRight;
        public event Action OnRotate;
        public event Action OnFallQuickly;
        public event Action OnGamePause;

        private void Update()
        {
            if (!controlsDisabled)
            {
                if (Input.GetKeyDown(moveLeft))
                {
                    OnMoveLeft();
                }
                else if (Input.GetKeyDown(moveRight))
                {
                    OnMoveRight();
                }
                else if (Input.GetKeyDown(rotate1) || Input.GetKeyDown(rotate2))
                {
                    OnRotate();
                }
                else if (Input.GetKeyDown(fallQuickly))
                {
                    OnFallQuickly();
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnGamePause();
            }
        }
    }
}

