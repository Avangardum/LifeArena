using System.Diagnostics;
using System.Numerics;
using System.Text.Json;
using LifeArenaBlazorClient.Data;
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
    private const string LifeArenaBodyId = "life-arena-body";
    private static readonly TimeSpan ZoomChangeCycleInterval = TimeSpan.FromMilliseconds(50);

    private double _zoom = 1;
    private Vector2 _fieldTranslate;
    private Vector2 _mousePosition;
    private double _wheelZoomAccumulatedDelta;

    public event EventHandler? ZoomChangedWithWheel;
    
    public double ZoomPercentage { get; private set; } = ZoomPercentageFromZoom(1);

    public bool[,] LivingCells { get; set; } = new bool[0, 0];

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
        if (firstRender) await OnAfterFirstRenderAsync();
    }

    private async Task OnAfterFirstRenderAsync()
    {
        await JsRuntime.InvokeVoidAsync("makeLifeArenaFieldDraggable", LifeArenaBodyCssClass, 
            DotNetObjectReference.Create(this));
        await ZoomChangeCycleAsync();
    }

    private async Task ZoomChangeCycleAsync()
    {
        while (true)
        {
            await Task.Delay(ZoomChangeCycleInterval);
            if (_wheelZoomAccumulatedDelta == 0) continue;
            ChangeZoomPercentageWithWheelAsync(_wheelZoomAccumulatedDelta);
            _wheelZoomAccumulatedDelta = 0;
        }
        // ReSharper disable once FunctionNeverReturns
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
        _wheelZoomAccumulatedDelta -= e.DeltaY * WheelZoomSpeed;
    }

    private async void ChangeZoomPercentageWithWheelAsync(double delta)
    {
        ZoomPercentage = Math.Clamp(ZoomPercentage + delta, 0, 1);
        await SetZoomAsync(ZoomFromZoomPercentage(ZoomPercentage), _mousePosition);
        ZoomChangedWithWheel?.Invoke(this, EventArgs.Empty);
    }

    public async void SetZoomPercentageWithHeaderAsync(double value)
    {
        ZoomPercentage = Math.Clamp(value, 0, 1);
        var bodyCenter = await GetBodyCenterAsync();
        await SetZoomAsync(ZoomFromZoomPercentage(ZoomPercentage), bodyCenter);
    }

    private void OnMouseMove(MouseEventArgs e)
    {
        _mousePosition = new Vector2((float)e.ClientX, (float)e.ClientY);
    }

    private async Task SetZoomAsync(double value, Vector2 focusPosition)
    {
        // Focus is the object that should remain over same position in the field after zooming (mouse or screen center).
        // Relative focus position is the position of the focus in the coordinate system of the field.
        // Normalized relative focus position is the relative focus position if zoom is 1.
        // Relative focus position = normalized relative focus position * zoom.
        // Normalized relative focus position = relative focus position / zoom.
        // Normalized relative focus position should remain the same after zooming.
        
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
        var boundingClientRect = await GetBoundingClientRectAsync(LifeArenaFieldId);
        return new Vector2((float)boundingClientRect.X, (float)boundingClientRect.Y);
    }

    private async Task<Vector2> GetBodyCenterAsync()
    {
        var boundingClientRect = await GetBoundingClientRectAsync(LifeArenaBodyId);
        return new Vector2((float)boundingClientRect.Width / 2, (float)boundingClientRect.Height / 2);
    }

    private async Task<DomRect> GetBoundingClientRectAsync(string elementId)
    {
        var boundingClientRect =
            await JsRuntime.InvokeAsync<DomRect>("getBoundingClientRect", elementId);
        return boundingClientRect;
    }
}