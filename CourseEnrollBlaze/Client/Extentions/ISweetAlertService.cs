using CourseEnrollBlaze.Shared.Models;
using Microsoft.JSInterop;
using System.Threading.Tasks;

public interface ISweetAlertService
{
    Task ShowAlert(string title, string message, SweetAlertIcon icon);
    Task<bool> ShowConfirmation(string title, string message);
}

public class SweetAlertService : ISweetAlertService
{
    private readonly IJSRuntime _jsRuntime;

    public SweetAlertService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task ShowAlert(string title, string message, SweetAlertIcon icon)
    {
        await _jsRuntime.InvokeVoidAsync("Swal.fire", title, message, icon.ToString().ToLower());
    }

    public async Task<bool> ShowConfirmation(string title, string message)
    {
        var result = await _jsRuntime.InvokeAsync<SweetAlertResult>("Swal.fire", new
        {
            title,
            text = message,
            icon = "warning",
            showCancelButton = true,
            confirmButtonColor = "#3085d6",
            cancelButtonColor = "#d33",
            confirmButtonText = "Yes, delete it!"
        });

        return result?.IsConfirmed ?? false;
    }


}

public enum SweetAlertIcon
{
    Success,
    Error,
    Warning,
    Info
}
