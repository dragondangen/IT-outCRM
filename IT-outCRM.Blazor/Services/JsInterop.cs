using Microsoft.JSInterop;

namespace IT_outCRM.Blazor.Services
{
    public static class JsInterop
    {
        public static async Task<bool> Confirm(IJSRuntime jsRuntime, string message)
        {
            return await jsRuntime.InvokeAsync<bool>("confirm", message);
        }
    }
}

