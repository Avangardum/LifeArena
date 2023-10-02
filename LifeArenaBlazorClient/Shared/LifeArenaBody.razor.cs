using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace LifeArenaBlazorClient.Shared;

public partial class LifeArenaBody
{
    private const int FieldWidth = 100;
    private const int FieldHeight = 100;
    private const double MinZoom = 0.1;
    private const double MaxZoom = 2;
    private const double MaxToMinZoomRatio = MaxZoom / MinZoom;
    private const double WheelZoomSpeed = 0.0004;
    private const string LifeArenaFieldClass = "life-arena-field";
    private const string LifeArenaBodyClass = "life-arena-body";

    private bool[,] _livingCells = new bool[FieldWidth, FieldHeight];
    private double _zoomPercentage = ZoomPercentageFromZoom(1);
    private double _zoom = 1;
    private double _fieldTranslateX;
    private double _fieldTranslateY;

    public event EventHandler? ZoomChanged;

    public bool[,] LivingCells
    {
        set
        {
            _livingCells = value;
            StateHasChanged();
        }
    }

    public double ZoomPercentage
    {
        get => _zoomPercentage;
        set
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_zoomPercentage == value) return;
            _zoomPercentage = Math.Clamp(value, 0, 1);
            _zoom = ZoomFromZoomPercentage(_zoomPercentage);
            ZoomChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    
    [Inject]
    public required IJSRuntime JsRuntime { private get; set; }

    [JSInvokable]
    public void SetFieldTranslate(double x, double y)
    {
        _fieldTranslateX = x;
        _fieldTranslateY = y;
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JsRuntime.InvokeVoidAsync("makeLifeArenaFieldDraggable", LifeArenaBodyClass, 
            DotNetObjectReference.Create(this));
    }

    private static double ZoomFromZoomPercentage(double zoomPercentage)
    {
        Debug.Assert(0 <= zoomPercentage && zoomPercentage <= 1);
        return MinZoom * Math.Pow(MaxToMinZoomRatio, zoomPercentage);
    }
    
    private static double ZoomPercentageFromZoom(double zoom)
    {
        Debug.Assert(MinZoom <= zoom && zoom <= MaxZoom);
        return Math.Log(zoom / MinZoom, MaxToMinZoomRatio);
    }

    private void OnWheel(WheelEventArgs e)
    {
        ZoomPercentage -= e.DeltaY * WheelZoomSpeed;
    }
}