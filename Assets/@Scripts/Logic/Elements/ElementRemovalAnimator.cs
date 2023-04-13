using System;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisMayhem.Logic
{
    public class ElementRemovalAnimator : MonoBehaviour
    {
        public event Action<CellElement> OnElementBeingRemoved;

        public void AnimateRemoval(List<CellElement> elementsToAnimate)
        {
            foreach (CellElement element in elementsToAnimate)
            {
                // TODO: LATER

                OnElementBeingRemoved?.Invoke(element);
            }
        }
    }
}

