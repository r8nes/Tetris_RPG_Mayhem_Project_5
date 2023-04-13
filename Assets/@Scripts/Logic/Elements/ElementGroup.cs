using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TetrisMayhem.Logic
{
    public partial class ElementGroup : MonoBehaviour
    {
        private readonly int ELEMENT_GROUP_DEFAULT_SIZE = 3;

        [SerializeField] private float _fallSpeed = 30f;
        [SerializeField] private float _quickFallSpeed = 1500f;

        public GameGrid Grid;
        public GameInput Input;
        public ElementsContainer ElementsContainer;

        private bool _isFallingQuickly;
        private Cell _spawnCell;

        private List<CellElement> _elements = new List<CellElement>();

        private CellRotationType _rotation;
        private FallingStateType _currentState;

        public int BombsToSpawn { get; set; }

        public event Action OnElementGroupSpawned;
        public event Action OnElementGroupStatic;
        public event Action OnReachedTargetCell;

        private void Start()
        {
            BombsToSpawn = 0;
            _currentState = FallingStateType.NOT_SPAWNED;

            _spawnCell = Grid.GetSpawnCell();

            Input.OnMoveLeft += OnMoveLeft;
            Input.OnMoveRight += OnMoveRight;
            Input.OnFallQuickly += OnFallQuickly;
            Input.OnRotate += OnRotate;

            ElementsContainer.OnElementsInRest += OnElementsInRest;
            OnReachedTargetCell += ReachTargetCell;
        }

        public void UpdateElementGroup()
        {
            switch (_currentState)
            {
                case FallingStateType.NOT_SPAWNED:
                    Spawn();
                    return;
                case FallingStateType.FALLING:
                    UpdatePosition();
                    return;
                case FallingStateType.LANDED:
                    OnElementGroupStatic();
                    return;
            }
        }

        private void UpdatePosition()
        {
            if (!ReachedTargetCell())
            {
                Fall();
                return;
            }
            OnReachedTargetCell();
        }

        private void ReachTargetCell()
        {
            if (CanIncrementTargetCells())
            {
                IncrementTargetCells();
                Fall();
                return;
            }
            ResetElementsFallSpeed();
            _currentState = FallingStateType.LANDED;
        }

        private void Spawn()
        {
            int num = (BombsToSpawn > 0) ? 1 : ELEMENT_GROUP_DEFAULT_SIZE;
            int num2 = (int)Math.Floor((num / 2f));
            int num3 = _spawnCell.X - num2;

            _elements.Clear();

            for (int i = 0; i < num; i++)
            {
                CellElement element;

                if (BombsToSpawn > 0)
                {
                    element = ElementsContainer.CreateBomb(num3, _spawnCell.Y);

                    int bombsToSpawn = BombsToSpawn;
                    BombsToSpawn = bombsToSpawn - 1;
                }
                else
                {
                    element = ElementsContainer.CreateRandomElement(num3, _spawnCell.Y);
                }

                if (element == null) return;

                element.Speed = _fallSpeed;
                _elements.Add(element);
                num3++;
            }

            _isFallingQuickly = false;

            _rotation = CellRotationType.ZERO_DEGREES;
            _currentState = FallingStateType.FALLING;

            UpdatePosition();
            OnElementGroupSpawned();
        }

        private bool ReachedTargetCell()
        {
            using (List<CellElement>.Enumerator enumerator = _elements.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (!enumerator.Current.ReachedTargetCell())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CanIncrementTargetCells()
        {
            if (_rotation == CellRotationType.ZERO_DEGREES || _rotation == CellRotationType.ONE_EIGHTY_DEGREES)
            {
                using (List<CellElement>.Enumerator enumerator = _elements.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (!enumerator.Current.CanIncrementTargetCell())
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            if (_rotation == CellRotationType.NINETY_DEGREES)
            {
                if (!_elements.Last<CellElement>().CanIncrementTargetCell())
                    return false;
            }
            else if (!_elements.First<CellElement>().CanIncrementTargetCell())
                return false;
            
            return true;
        }

        private void IncrementTargetCells()
        {
            foreach (CellElement element in _elements)
            {
                element.IncrementTargetCell();
            }
        }

        private void ResetElementsFallSpeed()
        {
            foreach (CellElement element in _elements)
            {
                element.ResetFallSpeed();
            }
        }

        private void OnRotate() => Rotate();

        private void OnFallQuickly()
        {
            if (_currentState == FallingStateType.FALLING && !_isFallingQuickly)
            {
                SetLowestTargetCells();
                ToggleQuickFall();
            }
        }

        public void SetLowestTargetCells()
        {
            int num = 0;
            if (_rotation == CellRotationType.ZERO_DEGREES || _rotation == CellRotationType.ONE_EIGHTY_DEGREES)
            {
                using (List<CellElement>.Enumerator enumerator = _elements.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        CellElement element = enumerator.Current;
                        Cell lowestFreeCell = element.GetLowestFreeCell();

                        if (lowestFreeCell.Y > num)
                        {
                            num = lowestFreeCell.Y;
                        }
                    }
                }
            }
            if (_rotation == CellRotationType.NINETY_DEGREES)
            {
                num = _elements[ELEMENT_GROUP_DEFAULT_SIZE - 1].GetLowestFreeCell().Y;
                num += ELEMENT_GROUP_DEFAULT_SIZE - 1;
            }
            else
            {
                num = _elements[0].GetLowestFreeCell().Y;
            }

            foreach (CellElement element2 in _elements)
            {
                if (Grid.GetElementWithTargetCell(element2.TargetCell) == element2)
                {
                    Grid.FreeCell(element2.TargetCell);
                }
                element2.TargetCell = Grid.GetCell(element2.TargetCell.X, num);
                Grid.FillCell(element2.TargetCell, element2);
                if (_rotation == CellRotationType.NINETY_DEGREES)
                {
                    num--;
                }
                else if (_rotation == CellRotationType.TWO_SEVENTY_DEGREES)
                {
                    num++;
                }
            }
        }

        private void ToggleQuickFall() => _isFallingQuickly = true;

        private void OnElementsInRest() => _currentState = FallingStateType.NOT_SPAWNED;

        private void OnMoveRight() => MoveRight();

        private void OnMoveLeft() => MoveLeft();

        private bool CanMoveLeft()
        {
            switch (_rotation)
            {
                case CellRotationType.ZERO_DEGREES:
                    return Grid.IsTwoCellColumnFree(_elements.First().TargetCell.X - 1, _elements.First().TargetCell.Y);
                case CellRotationType.NINETY_DEGREES:
                case CellRotationType.TWO_SEVENTY_DEGREES:

                        foreach (CellElement element in _elements)
                        {
                            if (!Grid.IsTwoCellColumnFree(element.TargetCell.X - 1, element.TargetCell.Y))
                                return false;
                        }
                        return true;  

                case CellRotationType.ONE_EIGHTY_DEGREES:
                    return Grid.IsTwoCellColumnFree(_elements.Last<CellElement>().TargetCell.X - 1, _elements.Last().TargetCell.Y);
                default:
                    return false;
            }
        }

        private bool CanMoveRight()
        {
            switch (_rotation)
            {
                case CellRotationType.ZERO_DEGREES:
                    return Grid.IsTwoCellColumnFree(_elements.Last().TargetCell.X + 1, _elements.Last().TargetCell.Y);
                case CellRotationType.NINETY_DEGREES:
                case CellRotationType.TWO_SEVENTY_DEGREES:
                    
                        foreach (CellElement element in _elements)
                        {
                            if (!Grid.IsTwoCellColumnFree(element.TargetCell.X + 1, element.TargetCell.Y))
                                return false;
                        }
                        return true;
                    
                case CellRotationType.ONE_EIGHTY_DEGREES:
                    return Grid.IsTwoCellColumnFree(_elements.First().TargetCell.X + 1, _elements.First().TargetCell.Y);
                default:
                    return false;
            }
        }

        private void MoveLeft()
        {
            if (!_isFallingQuickly && _currentState == FallingStateType.FALLING && CanMoveLeft())
            {
                List<CellElement> list = _elements;

                if (_rotation == CellRotationType.ONE_EIGHTY_DEGREES)
                {
                    list = list.AsEnumerable().Reverse<CellElement>().ToList<CellElement>();
                }

                foreach (CellElement element in list)
                {
                    element.SetPositionToCell(Grid.GetCell(element.TargetCell.X - 1, element.TargetCell.Y));
                }
            }
        }

        private void MoveRight()
        {
            if (!_isFallingQuickly && _currentState == FallingStateType.FALLING && CanMoveRight())
            {
                List<CellElement> list = _elements;
                if (_rotation == CellRotationType.ZERO_DEGREES)
                {
                    list = list.AsEnumerable().Reverse<CellElement>().ToList<CellElement>();
                }
                foreach (CellElement element in list)
                {
                    element.SetPositionToCell(Grid.GetCell(element.TargetCell.X + 1, element.TargetCell.Y));
                }
            }
        }

        private void Rotate()
        {
            if (_isFallingQuickly || _currentState != FallingStateType.FALLING || _elements.Count == 1 || !CanRotate()) return;

            if (_rotation != CellRotationType.TWO_SEVENTY_DEGREES)
                _rotation++;
            else
                _rotation = CellRotationType.ZERO_DEGREES;

            switch (_rotation)
            {
                case CellRotationType.ZERO_DEGREES:
                    _elements.First<CellElement>().SetPositionToCell(Grid.GetCell(_elements.First().TargetCell.X - 1, _elements.First().TargetCell.Y + 1));
                    _elements.Last<CellElement>().SetPositionToCell(Grid.GetCell(_elements.Last().TargetCell.X + 1, _elements.Last().TargetCell.Y - 1));
                    return;
                case CellRotationType.NINETY_DEGREES:
                    _elements.First<CellElement>().SetPositionToCell(Grid.GetCell(_elements.First().TargetCell.X + 1, _elements.First().TargetCell.Y + 1));
                    _elements.Last<CellElement>().SetPositionToCell(Grid.GetCell(_elements.Last().TargetCell.X - 1, _elements.Last().TargetCell.Y - 1));
                    return;
                case CellRotationType.ONE_EIGHTY_DEGREES:
                    _elements.First<CellElement>().SetPositionToCell(Grid.GetCell(_elements.First().TargetCell.X + 1, _elements.First().TargetCell.Y - 1));
                    _elements.Last<CellElement>().SetPositionToCell(Grid.GetCell(_elements.Last().TargetCell.X - 1, _elements.Last().TargetCell.Y + 1));
                    return;
                case CellRotationType.TWO_SEVENTY_DEGREES:
                    _elements.First<CellElement>().SetPositionToCell(Grid.GetCell(_elements.First().TargetCell.X - 1, _elements.First().TargetCell.Y - 1));
                    _elements.Last<CellElement>().SetPositionToCell(Grid.GetCell(_elements.Last().TargetCell.X + 1, _elements.Last().TargetCell.Y + 1));
                    return;
            }
        }

        private bool CanRotate()
        {
            switch (_rotation)
            {
                case CellRotationType.ZERO_DEGREES:
                case CellRotationType.ONE_EIGHTY_DEGREES:
                    if (Grid.IsCellFree(_elements[1].TargetCell.X, _elements[1].TargetCell.Y - 1))
                    {
                        return true;
                    }
                    break;
                case CellRotationType.NINETY_DEGREES:
                case CellRotationType.TWO_SEVENTY_DEGREES:
                    if (Grid.IsTwoCellColumnFree(_elements[1].TargetCell.X - 1, _elements[1].TargetCell.Y) && Grid.IsTwoCellColumnFree(_elements[1].TargetCell.X + 1, _elements[1].TargetCell.Y))
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        private void Fall()
        {
            float speed = _fallSpeed;

            if (_isFallingQuickly && _fallSpeed < _quickFallSpeed)
                speed = _quickFallSpeed;
            
            foreach (CellElement element in _elements)
            {
                element.Fall(speed);
            }
        }
    }
}

