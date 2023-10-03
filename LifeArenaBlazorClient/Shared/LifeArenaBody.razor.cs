using System.Diagnostics;
using System.Numerics;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace LifeArenaBlazorClient.Shared;

public partial class LifeArenaBody
{
    private const double MinZoom = 0.1;
    private const double MaxZoom = 2;
    private const double MaxToMinZoomRatio = MaxZoom / MinZoom;
    private const double WheelZoomSpeed = 0.0004;
    private const string LifeArenaFieldCssClass = "life-arena-field";
    private const string LifeArenaBodyCssClass = "life-arena-body";
    private const string LifeArenaFieldId = "life-arena-field";

    private double _zoomPercentage = ZoomPercentageFromZoom(1);
    private double _zoom = 1;
    private Vector2 _fieldTranslate;
    private Vector2 _mousePosition;

    public event EventHandler? ZoomChanged;

    public required bool[,] LivingCells { get; set; } = new bool[0, 0];

    private double ZoomPercentage
    {
        get => _zoomPercentage;
        set
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_zoomPercentage == value) return;
            _zoomPercentage = Math.Clamp(value, 0, 1);
            SetZoomAsync(ZoomFromZoomPercentage(_zoomPercentage));
            ZoomChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [Inject]
    public required IJSRuntime JsRuntime { private get; set; }

    [JSInvokable]
    public void SetFieldTranslate(float x, float y)
    {
        _fieldTranslate = new Vector2(x, y);
        StateHasChanged();
    }

    public void InvokeStateHasChanged()
    {
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JsRuntime.InvokeVoidAsync("makeLifeArenaFieldDraggable", LifeArenaBodyCssClass, 
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

    private void OnMouseMove(MouseEventArgs e)
    {
        _mousePosition = new Vector2((float)e.ClientX, (float)e.ClientY);
    }

    private async void SetZoomAsync(double value)
    {
        // Focus is the object that should remain over same position in the field after zooming (mouse or screen center).
        // Relative focus position is the position of the focus in the coordinate system of the field.
        // Normalized relative focus position is the relative focus position if zoom is 1.
        // Relative focus position = normalized relative focus position * zoom.
        // Normalized relative focus position = relative focus position / zoom.
        // Normalized relative focus position should remain the same after zooming.
        
        var focusPosition = _mousePosition;
        var oldFieldPosition = await GetFieldPositionAsync();
        var oldRelativeFocusPosition = focusPosition - oldFieldPosition;
        var oldZoom = (float)_zoom;
        var normalizedRelativeFocusPosition = oldRelativeFocusPosition / oldZoom;

        _zoom = value;
        var newZoom = (float)value;
        StateHasChanged();

        var newRelativeFocusPosition = normalizedRelativeFocusPosition * newZoom;
        var newFieldPosition = focusPosition - newRelativeFocusPosition;
        var fieldPositionAfterZooming = await GetFieldPositionAsync();
        var fieldPositionAdjustment = newFieldPosition - fieldPositionAfterZooming;
        _fieldTranslate += fieldPositionAdjustment;
        StateHasChanged();
    }

    private async Task<Vector2> GetFieldPositionAsync()
    {
        var boundingClientRect =
            await JsRuntime.InvokeAsync<JsonElement>("getBoundingClientRect", LifeArenaFieldId);
        var x = boundingClientRect.GetProperty("left").GetSingle();
        var y = boundingClientRect.GetProperty("top").GetSingle();
        return new Vector2(x, y);
    }
}