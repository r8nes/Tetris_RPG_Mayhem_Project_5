using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TetrisMayhem.Logic
{
    public class GameGrid : MonoBehaviour
    {
		public int ColumnCount = 7;
		public int RowCount = 12;

		private Cell[,] _cell;
		public Canvas Canvas;
		private Vector2 _dimensions;

		private void Awake()
		{
			_dimensions = new Vector2(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height);
		
			Cell.Width = _dimensions.x / ColumnCount;
			InitializeCells();
		}

		private void InitializeCells()
		{
			_cell = new Cell[ColumnCount, RowCount];
			for (int i = 0; i < ColumnCount; i++)
			{
				for (int j = 0; j < RowCount; j++)
				{
					_cell[i, j] = new Cell(i, j);
				}
			}
		}

		public Vector2 CalculateWorldPosition(int cellX, int cellY)
		{
			float x = cellX * Cell.Width - _dimensions.x / 2f + Cell.Width / 2f;
			float y = cellY * Cell.Width - _dimensions.y / 2f + Cell.Width / 2f;

			return new Vector3(x, y, 0f);
		}

        public Cell GetSpawnCell() => _cell[(int)Math.Floor((ColumnCount / 2f)), RowCount - 1];

        public bool CellExists(int cellX, int cellY) => cellX >= 0 && cellX < ColumnCount && cellY >= 0 && cellY < RowCount;

        public Cell GetCell(int cellX, int cellY) => _cell[cellX, cellY];
    }
}
