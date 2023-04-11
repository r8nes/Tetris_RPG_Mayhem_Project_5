using UnityEngine;

namespace TetrisMayhem.Logic
{
    public class DimentionResolution : MonoBehaviour
	{
		public Canvas Canvas;
        public Vector2 Dimention { get; private set; }

        public void CalcualateDimention() => Dimention = new Vector2(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height);
    }
}
