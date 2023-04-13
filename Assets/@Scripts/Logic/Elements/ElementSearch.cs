using System.Collections.Generic;
using UnityEngine;

namespace TetrisMayhem.Logic
{
    public class ElementSearch
    {
        private readonly GameGrid _grid;

        public ElementSearch(GameGrid grid)
        {
            _grid = grid;
        }

        public void SearchReactableElements(CellElement element, ref List<CellElement> sameTypeElements, ref List<CellElement> blockElements, List<CellElement> globalBlockElements)
        {
            foreach (CellElement element2 in element.GetNeighbouringElements())
            {
                if (element.ElementType == element2.ElementType && !sameTypeElements.Contains(element2))
                {
                    sameTypeElements.Add(element2);
                    SearchReactableElements(element2, ref sameTypeElements, ref blockElements, globalBlockElements);
                }
                else if (element2.ElementType.type == WeaponType.ELEMENT_BLOCK && !globalBlockElements.Contains(element2) && !blockElements.Contains(element2))
                {
                    blockElements.Add(element2);
                }
            }
        }

        public void SearchAllElementsInRadius(Cell cell, int radius, ref List<CellElement> elements)
        {
            CellElement elementInCell = _grid.GetElementInCell(cell);

            if (elementInCell != null && !elements.Contains(elementInCell))
                elements.Add(elementInCell);
            
            foreach (Cell cell2 in _grid.GetNeighbouringCells(cell))
            {
                if (radius > 0)
                    SearchAllElementsInRadius(cell2, radius - 1, ref elements);
            }
        }
    }
}
