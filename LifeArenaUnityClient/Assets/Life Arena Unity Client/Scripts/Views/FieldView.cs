using System;
using Avangardum.LifeArena.UnityClient.Data;
using Avangardum.LifeArena.UnityClient.Interfaces;
using UnityEngine;
using UnityEngine.Assertions;

namespace Avangardum.LifeArena.UnityClient.Views
{
    public class FieldView : MonoBehaviour, IFieldView
    {
        private const float MaxZoom = 2f;
        private const float MinZoom = 0.1f;
        private const float DefaultZoom = 0.5f;
        private const float MinZoomToShowBorder = 0.3f;
        private const float CellSize = 50;

        // When above 1, zoom will change faster when close to the max value.
        private const float SmoothZoomFactor = 2f;
        
        [SerializeField] private GameObject _cellViewPrefab;
        
        private CellView[,] _cells;
        
        public event EventHandler<CellClickedEventArgs> CellClicked;
        public event EventHandler<ZoomChangedEventArgs> ZoomChanged;
        
        public bool[,] LivingCells
        {
            set
            {
                var shouldRecreateCells = _cells == null || CellsWidth != value.GetLength(0) || 
                    CellsHeight != value.GetLength(1);
                if (shouldRecreateCells)
                {
                    ClearCells();
                    CreateCells(value.GetLength(0), value.GetLength(1));
                    ResetPositionAndZoom();
                }
                ColorCells(value);
            }
        }

        public float ZoomPercentage
        {
            get => ZoomToZoomPercentage(Zoom);
            set
            {
                var clampedValue = Mathf.Clamp(value, 0, 1);
                if (clampedValue == ZoomPercentage) return;
                Zoom = ZoomPercentageToZoom(clampedValue);
            }
        }

        public ZoomFocusPointMode ZoomFocusPointMode { private get; set; }

        private float Zoom
        {
            get => transform.localScale.x;
            set
            {
                if (value == Zoom) return;
                
                // Focus is the object that should remain over same position in the field after zooming (mouse or screen center).
                // Relative focus position is the position of the focus in the coordinate system of the field.
                // Normalized relative focus position is the relative focus position if zoom is 1.
                // Relative focus position = normalized relative focus position * zoom.
                // Normalized relative focus position = relative focus position / zoom.
                // Normalized relative focus position should remain the same after zooming.
                var focusPosition = ZoomFocusPointMode switch
                {
                    ZoomFocusPointMode.Mouse => (Vector2)Input.mousePosition,
                    ZoomFocusPointMode.ScreenCenter => ScreenCenter,
                    _ => throw new ArgumentOutOfRangeException()
                };
                var oldRelativeFocusPosition = focusPosition - (Vector2)transform.position;
                var normalizedRelativeFocusPosition = oldRelativeFocusPosition / Zoom;
                
                var newZoom = Mathf.Clamp(value, MinZoom, MaxZoom);
                transform.localScale = new Vector3(newZoom, newZoom, 1f);
                
                var newRelativeFocusPosition = normalizedRelativeFocusPosition * newZoom;
                
                var newPosition = focusPosition - newRelativeFocusPosition;
                transform.position = newPosition;

                SetCellBorderVisibility();

                ZoomChanged?.Invoke(this, new ZoomChangedEventArgs(Zoom, ZoomPercentage));
            }
        }
        
        private float ZoomedCellSize => CellSize * Zoom;

        private Vector2 ScreenCenter => ScreenSize / 2;
        
        private Vector2 ScreenCenterRelativeToCanvas => Vector2.zero;
        
        private Vector2 ScreenSize => new(Screen.width, Screen.height);
        
        private int CellsWidth => _cells.GetLength(0);
        
        private int CellsHeight => _cells.GetLength(1);
        
        public void Move(Vector2 movement)
        {
            transform.Translate(movement, Space.World);
            SetCellBorderVisibility();
        }

        private void ClearCells()
        {
            if (_cells == null) return;
            foreach (var cell in _cells)
            {
                Destroy(cell.gameObject);
            }
            _cells = null;
        }

        private void CreateCells(int width, int height)
        {
            Assert.IsTrue(width > 0 && height > 0);
            _cells = new CellView[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var cell = Instantiate(_cellViewPrefab, transform).GetComponent<CellView>();
                    Assert.IsNotNull(cell);
                    cell.gameObject.name = $"Cell {x} {y}";
                    cell.transform.localPosition = new Vector3(x, y) * CellSize;
                    var pinnedX = x;
                    var pinnedY = y;
                    cell.Clicked += (_, _) => OnCellClicked(pinnedX, pinnedY);
                    _cells[x, y] = cell;
                }
            }
        }

        private void OnCellClicked(int x, int y)
        {
            CellClicked?.Invoke(this, new CellClickedEventArgs(x, y));
        }

        private void ColorCells(bool[,] livingCells)
        {
            Assert.IsTrue(livingCells.GetLength(0) == CellsWidth && livingCells.GetLength(1) == CellsHeight);
            for (var x = 0; x < CellsWidth; x++)
            {
                for (var y = 0; y < CellsHeight; y++)
                {
                    _cells[x, y].IsAlive = livingCells[x, y];
                }
            }
        }
        
        private void ResetPositionAndZoom()
        {
            Zoom = DefaultZoom;
            
            // Middle cell should be in the middle of the screen.
            // Zero cell position is equal to field position
            var middleCellPosition = ScreenCenter;
            var middleCellIndex = new Vector2Int(CellsWidth / 2, CellsHeight / 2);
            var zeroCellPosition = middleCellPosition - (Vector2)middleCellIndex * (CellSize * Zoom);
            transform.position = zeroCellPosition;
            
            SetCellBorderVisibility();
        }

        private float ZoomPercentageToZoom(float zoomPercentage)
        {
            Assert.IsTrue(zoomPercentage is >= 0 and <= 1);
            return MinZoom + Mathf.Pow(zoomPercentage, SmoothZoomFactor) * (MaxZoom - MinZoom);
        }
        
        private float ZoomToZoomPercentage(float zoom)
        {
            Assert.IsTrue(zoom is >= MinZoom and <= MaxZoom);
            return Mathf.Pow((zoom - MinZoom) / (MaxZoom - MinZoom), 1 / SmoothZoomFactor);
        }
        
        private void SetCellBorderVisibility()
        {
            if (_cells == null) return;
            
            var isBorderVisible = Zoom >= MinZoomToShowBorder;
            
            // Only set border visibility of cells currently visible on the screen.
            var borderIndices = GetVisibleCellsBorderIndices();
            for (var x = borderIndices.MinX; x <= borderIndices.MaxX; x++)
            {
                for (var y = borderIndices.MinY; y <= borderIndices.MaxY; y++)
                {
                    _cells[x, y].IsBorderVisible = isBorderVisible;
                }
            }
        }

        private (int MinX, int MaxX, int MinY, int MaxY) GetVisibleCellsBorderIndices()
        {
            var cellAtScreenCenterIndex = GetCellAtScreenCenterIndex();
            var screenHalfSizeInCellsFloat = ScreenSize / ZoomedCellSize / 2;
            var screenHalfSizeInCells = new Vector2Int(Mathf.CeilToInt(screenHalfSizeInCellsFloat.x),
                Mathf.CeilToInt(screenHalfSizeInCellsFloat.y));
            var minX = Mathf.Max(0, cellAtScreenCenterIndex.x - screenHalfSizeInCells.x);
            var maxX = Mathf.Min(CellsWidth - 1, cellAtScreenCenterIndex.x + screenHalfSizeInCells.x);
            var minY = Mathf.Max(0, cellAtScreenCenterIndex.y - screenHalfSizeInCells.y);
            var maxY = Mathf.Min(CellsHeight - 1, cellAtScreenCenterIndex.y + screenHalfSizeInCells.y);
            
            return (minX, maxX, minY, maxY);
        }
        
        private Vector2Int GetCellAtScreenCenterIndex()
        {
            var screenCenterPositionRelativeToField = ScreenCenterRelativeToCanvas - (Vector2)transform.localPosition;
            var cellIndexFloat = screenCenterPositionRelativeToField / ZoomedCellSize;
            var cellIndex = new Vector2Int(Mathf.RoundToInt(cellIndexFloat.x), Mathf.RoundToInt(cellIndexFloat.y));
            return cellIndex;
        }
    }
}