using System;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisMayhem.Logic
{
    public partial class ElementReactor : MonoBehaviour
    {
        [SerializeField] private int _explosionRadius = 4;
        [SerializeField] private int _numberOfElementsToDelete = 4;

        [SerializeField] private GameGrid _grid;
        [SerializeField] private ElementsContainer _elementsContainer;

        private bool _reacted;
        private List<CellElement> _globalBlockElements = new List<CellElement>();

        public event Action OnNoReaction;
        public event Action<ReactionType, List<CellElement>> OnElementsReacted;

        private void Start()
        {
            _reacted = false;
            _elementsContainer.OnElementsInRest += OnElementsInRest;
        }

        public void React(CellElement element)
        {
            if (element.ElementType.type == WeaponType.ELEMENT_BOMB)
            {
                List<CellElement> elements = new List<CellElement>();
                new ElementSearch(_grid).SearchAllElementsInRadius(element.TargetCell, _explosionRadius / 2, ref elements);

                _reacted = true;
                OnElementsReacted?.Invoke(ReactionType.REACTION_EXPLOSION, elements);
                return;
            }

            if (element.ElementType.type != WeaponType.ELEMENT_BLOCK)
            {
                ElementSearch elementSearch = new ElementSearch(_grid);

                List<CellElement> cellElementsList = new List<CellElement>();
                List<CellElement> secondCellElemetsList = new List<CellElement>();

                elementSearch.SearchReactableElements(element, ref cellElementsList, ref secondCellElemetsList, _globalBlockElements);
                if (cellElementsList.Count >= _numberOfElementsToDelete)
                {
                    _globalBlockElements.AddRange(secondCellElemetsList);

                    List<CellElement> list = new List<CellElement>();

                    foreach (CellElement element2 in secondCellElemetsList)
                    {
                        if (element2.Lives > 1)
                        {
                            list.Add(element2);
                        }
                        else
                        {
                            cellElementsList.Add(element2);
                        }
                    }
                    _reacted = true;

                    OnElementsReacted?.Invoke(ReactionType.REACTION_LOST_LIFE, list);
                    OnElementsReacted?.Invoke(ReactionType.REACTION_ELIMINATION, cellElementsList);

                    return;
                }
            }
        }

        public void ResetGlobalBlockElementsBuffer() => _globalBlockElements.Clear();

        private void OnElementsInRest()
        {
            if (!_reacted) OnNoReaction();

            _reacted = false;
        }
    }
}
