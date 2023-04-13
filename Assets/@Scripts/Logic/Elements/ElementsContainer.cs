using System;
using System.Collections.Generic;
using TetrisMayhem.Sound;
using UnityEngine;

namespace TetrisMayhem.Logic
{
    public class ElementsContainer : MonoBehaviour
    {
        [SerializeField] private Animator _gameFieldAnimator;
        
        public GameGrid Grid;
        public ElementGroup ElementGroup;
        public ElementReactor ElementReactor;
        public ElementTypesManager ElementTypesManager;
        public ElementRemovalAnimator ElementRemovalAnimator;

        public GameObject ElementContainerPrefab;

        private AudioManager _audioManager;

        public event Action OnElementCantBeCreated;
        public event Action OnElementsInRest;

        private void Awake()
        {
            // TODO: zenject
            _audioManager = FindObjectOfType<AudioManager>();
        }

        private void Start()
        {
            ElementGroup.OnElementGroupStatic += OnElementGroupStatic;
            ElementReactor.OnElementsReacted += OnElementsReacted;
            ElementReactor.OnNoReaction += OnNoReaction;
        }

        private void Update()
        {
            ElementGroup.UpdateElementGroup();
        }

        public void SetElementsToRemove(List<CellElement> elements) => ElementRemovalAnimator.AnimateRemoval(elements);

        public void CreateRandomElements()
        {
            for (int i = 0; i < Grid.ColumnCount; i++)
            {
                for (int j = 0; j < Grid.RowCount; j++)
                {
                    CreateRandomElement(i, j);
                }
            }
        }

        public CellElement CreateRandomElement(int cellX, int cellY)
        {
            ElementType randomElementType = ElementTypesManager.GetRandomElementType();
            return CreateElement(randomElementType, cellX, cellY);
        }

        public CellElement CreateBomb(int cellX, int cellY)
        {
            ElementType elementType;

            elementType = ElementTypesManager.GetElementType(WeaponType.ELEMENT_BOMB);
            if (elementType == null)
            {
                Debug.LogError("No element type");
                return null;
            }

            return CreateElement(elementType, cellX, cellY);
        }

        public CellElement CreateElement(ElementType type, int cellX, int cellY)
        {
            Cell cell = Grid.GetCell(cellX, cellY);

            if (Grid.IsCellFree(cell))
            {
                CellElement component = Instantiate(ElementContainerPrefab, transform).GetComponent<CellElement>();
                component.Initialize(type, cell);
                Grid.FillCell(cell, component);

                return component;
            }

            OnElementCantBeCreated();
            return null;
        }

        private void UpdateElements()
        {
            int num = 0;
            for (int i = 0; i < Grid.ColumnCount; i++)
            {
                for (int j = 0; j < Grid.RowCount; j++)
                {
                    CellElement element = Grid.GetCell(i, j).Element;
                    if (element != null)
                    {
                        element.UpdateElement();
                        if (element.CurrentState != State.STATIC)
                        {
                            num++;
                        }
                    }
                }
            }
            if (num == 0)
            {
                ReactElements();
            }
        }

        private void ReactElements()
        {
            int num = 0;
            for (int i = 0; i < Grid.ColumnCount; i++)
            {
                for (int j = 0; j < Grid.RowCount; j++)
                {
                    CellElement element = Grid.GetCell(i, j).Element;
                    if (element != null && element.CurrentState == State.STATIC)
                    {
                        element.ReactElement();
                        if (element.CurrentState != State.STATIC)
                        {
                            num++;
                        }
                    }
                }
            }
            ElementReactor.ResetGlobalBlockElementsBuffer();

            if (num == 0)
                OnElementsInRest();
        }

        private void OnElementsReacted(ReactionType reaction, List<CellElement> elements)
        {
            if (reaction == ReactionType.REACTION_ELIMINATION || reaction == ReactionType.REACTION_EXPLOSION)
            {
                SetElementsToRemove(elements);

                if (reaction == ReactionType.REACTION_EXPLOSION)
                {
                    _audioManager.PlayExplosionSound();
                    return;
                }
            }
            else if (reaction == ReactionType.REACTION_LOST_LIFE)
            {
                foreach (CellElement element in elements)
                {
                    element.ReduceLives();
                }
            }
        }

        private void OnElementGroupStatic() => UpdateElements();

        private void OnNoReaction() => Debug.Log("No Raction");
    }
}

