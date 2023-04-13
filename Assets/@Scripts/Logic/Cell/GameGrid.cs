using System;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisMayhem.Logic
{
    public class GameGrid : MonoBehaviour
    {
		public int ColumnCount = 7;
		public int RowCount = 12;

		public DimentionResolution Resolution;

		private Cell[,] _cell;

		private void Awake()
		{
			Cell.Width = Resolution.Dimention.x / ColumnCount;
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

		public List<Cell> GetNeighbouringCells(Cell cell)
		{
			List<Cell> list = new List<Cell>();
			if (cell.X < ColumnCount - 1)
			{
				list.Add(_cell[cell.X + 1, cell.Y]);
			}
			if (cell.X > 0)
			{
				list.Add(_cell[cell.X - 1, cell.Y]);
			}
			if (cell.Y < RowCount - 1)
			{
				list.Add(_cell[cell.X, cell.Y + 1]);
			}
			if (cell.Y > 0)
			{
				list.Add(_cell[cell.X, cell.Y - 1]);
			}
			return list;
		}

		public Vector2 CalculateWorldPosition(int cellX, int cellY)
		{
			float x = cellX * Cell.Width - Resolution.Dimention.x / 2f + Cell.Width / 2f;
			float y = cellY * Cell.Width - Resolution.Dimention.y / 2f + Cell.Width / 2f;

			return new Vector3(x, y, 0f);
		}

		// TODO: delete?
		public bool ShiftCellsUp()
		{
			bool result = true;

			for (int i = 0; i < ColumnCount; i++)
			{
				for (int j = 0; j < RowCount; j++)
				{
					Cell cell = GetCell(i, j);
					if (!IsCellFree(cell))
					{
						CellElement element = cell.Element;
					}
				}
			}
			return result;
		}

		public List<CellElement> GetSameTypeElements(WeaponType type)
		{
			List<CellElement> list = new List<CellElement>();
			for (int i = 0; i < ColumnCount; i++)
			{
				for (int j = 0; j < RowCount; j++)
				{
					if (!IsCellFree(i, j))
					{
						CellElement element = GetCell(i, j).Element;
						if (element.CurrentState == State.STATIC && element.ElementType.type == type)
						{
							list.Add(GetElementInCell(i, j));
						}
					}
				}
			}
			return list;
		}

		public CellElement GetElementInCell(int cellX, int cellY)
		{
			if (!CellExists(cellX, cellY)) return null;
			
			return GetElementInCell(_cell[cellX, cellY]);
		}

		public CellElement GetElementInCell(Cell cell)
		{
			CellElement element = cell.Element;
			if (!IsCellFree(cell) && !element.CanIncrementTargetCell() && (element.CurrentState == State.STATIC || (element.CurrentState != State.BEING_REMOVED && element.ReachedTargetCell())))
			{
				return element;
			}
			return null;
		}
        public CellElement GetElementWithTargetCell(Cell cell)
		{
			if (IsCellFree(cell))
			{
				return null;
			}
			return cell.Element;
		}

		public bool IsTwoCellColumnFree(int bottomCellX, int bottomCellY) => (!CellExists(bottomCellX, bottomCellY + 1)
	|| !(_cell[bottomCellX, bottomCellY + 1].Element != null))
	&& IsCellFree(bottomCellX, bottomCellY);
		
		public void FreeCell(Cell cell) => cell.Element = null;
		
		public void FillCell(Cell cell, CellElement element) => cell.Element = element;
		
		public bool IsCellFree(int cellX, int cellY) => CellExists(cellX, cellY) && IsCellFree(_cell[cellX, cellY]);
        
		public bool IsCellFree(Cell cell) => cell.Element == null;
        
		public Cell GetSpawnCell() => _cell[(int)Math.Floor((ColumnCount / 2f)), RowCount - 1];
		
		public bool CellExists(int cellX, int cellY) => cellX >= 0 && cellX < ColumnCount && cellY >= 0 && cellY < RowCount;
        
		public Cell GetCell(int cellX, int cellY) => _cell[cellX, cellY];
    }
}
