using System.Collections;
using System.Collections.Generic;
using TetrisMayhem.Sound;
using UnityEngine;

namespace TetrisMayhem.Logic
{
    public partial class CellElement : MonoBehaviour
    {
        public int GravityAcc = 10;

        public Cell TargetCell;
        public ElementType ElementType;

        private GameGrid _grid;
        private AudioManager _audioManager;
        private RectTransform _rectTransform;
        private ElementReactor _elementReactor;
        private ElementRemovalAnimator _elementRemovalAnimator;

        public float Speed { get; set; }
        public int Lives { get; set; }

        public State CurrentState { get; private set; }

        // TODO: zenject
        private void Awake()
        {
            _grid = FindObjectOfType<GameGrid>();
            _audioManager = FindObjectOfType<AudioManager>();
            _elementReactor = FindObjectOfType<ElementReactor>();
            _elementRemovalAnimator = FindObjectOfType<ElementRemovalAnimator>();
        }

        public void Initialize(ElementType elementType, Cell targetCell)
        {
            Lives = 1;

            ElementType = elementType;
            TargetCell = targetCell;

            if (elementType.type == WeaponType.ELEMENT_BLOCK)
                Lives = Random.Range(2, 4);

            _rectTransform = gameObject.GetComponent<RectTransform>();
            _rectTransform.localPosition = _grid.CalculateWorldPosition(targetCell.X, targetCell.Y);
            _rectTransform.sizeDelta = new Vector2(Cell.Width, Cell.Width);

            Instantiate(elementType.prefab[Lives - 1], gameObject.transform);
            _elementRemovalAnimator.OnElementBeingRemoved += OnElementBeingRemoved;

            CurrentState = State.FALLING;
        }

        public void UpdateElement()
        {
            UpdateState();
            if (CurrentState == State.FALLING || CurrentState == State.AWAIT_EXTRA)
            {
                if (CurrentState == State.AWAIT_EXTRA)
                    SetLowestTargetCell();

                Speed += GravityAcc * Time.deltaTime;
                Fall(Speed);
            }
        }

        public void ReduceLives()
        {
            int lives = Lives;
            Lives = lives - 1;

            Destroy(transform.GetChild(0).gameObject);
            Instantiate(ElementType.prefab[Lives - 1], gameObject.transform);
        }

        public void ReactElement()
        {
            ResetFallSpeed();
            _elementReactor.React(this);
        }

        public bool ReachedTargetCell() => _rectTransform.localPosition.y == _grid.CalculateWorldPosition(TargetCell.X, TargetCell.Y).y;

        public void IncrementTargetCell()
        {
            if (_grid.GetElementWithTargetCell(TargetCell) == this)
                _grid.FreeCell(TargetCell);
            
            TargetCell = _grid.GetCell(TargetCell.X, TargetCell.Y - 1);
            _grid.FillCell(TargetCell, this);
        }

        public bool CanIncrementTargetCell() => _grid.IsCellFree(TargetCell.X, TargetCell.Y - 1);

        public void SetLowestTargetCell()
        {
            _grid.FreeCell(TargetCell);
            TargetCell = GetLowestFreeCell();
            _grid.FillCell(TargetCell, this);
        }

        public Cell GetLowestFreeCell()
        {
            for (int i = TargetCell.Y - 1; i >= 0; i--)
            {
                if (!_grid.IsCellFree(TargetCell.X, i))
                    return _grid.GetCell(TargetCell.X, i + 1);
            }

            return _grid.GetCell(TargetCell.X, 0);
        }

        public void SetPositionToCell(Cell cell)
        {
            int num = TargetCell.Y - cell.Y;
            _grid.FreeCell(TargetCell);
            TargetCell = cell;
            _grid.FillCell(TargetCell, this);

            _rectTransform.localPosition = new Vector3(
                _grid.CalculateWorldPosition(TargetCell.X, TargetCell.Y).x,
                _rectTransform.localPosition.y - num * Cell.Width,
                _rectTransform.localPosition.z);
        }

        public List<CellElement> GetNeighbouringElements()
        {
            List<CellElement> list = new List<CellElement>();

            CellElement elementInCell = _grid.GetElementInCell(TargetCell.X + 1, TargetCell.Y);
            if (elementInCell != null && elementInCell.CurrentState == State.STATIC)
                list.Add(elementInCell);

            elementInCell = _grid.GetElementInCell(TargetCell.X - 1, TargetCell.Y);
            if (elementInCell != null && elementInCell.CurrentState == State.STATIC)
                list.Add(elementInCell);

            elementInCell = _grid.GetElementInCell(TargetCell.X, TargetCell.Y + 1);
            if (elementInCell != null && elementInCell.CurrentState == State.STATIC)
                list.Add(elementInCell);

            elementInCell = _grid.GetElementInCell(TargetCell.X, TargetCell.Y - 1);
            if (elementInCell != null && elementInCell.CurrentState == State.STATIC)
                list.Add(elementInCell);

            return list;
        }

        public void ResetFallSpeed() => Speed = 0f;

        public void Fall(float speed)
        {
            _rectTransform.localPosition = Vector3.MoveTowards(_rectTransform.localPosition,
                new Vector3(
                    _rectTransform.localPosition.x,
                _grid.CalculateWorldPosition(TargetCell.X, TargetCell.Y).y,
                _rectTransform.localPosition.z), speed * Time.deltaTime);
        }

        public void RemoveElement()
        {
            _grid.FreeCell(TargetCell);
            Destroy(gameObject);
        }

        private void UpdateState()
        {
            if (CurrentState == State.BEING_REMOVED) return;
            if (CurrentState == State.AWAIT_EXTRA) CurrentState = State.FALLING;

            if (ReachedTargetCell())
            {
                if (CanIncrementTargetCell())
                {
                    CurrentState = State.AWAIT_EXTRA;
                    return;
                }
                if (CurrentState != State.STATIC)
                {
                    _audioManager.PlayLandSound();
                    CurrentState = State.STATIC;
                }
            }
        }

        private void OnElementBeingRemoved(CellElement element)
        {
            if (element == this)
                CurrentState = State.BEING_REMOVED;
        }

        private void OnNoReaction(CellElement element)
        {
            if (element == this)
                CurrentState = State.STATIC;
        }
    }
}
