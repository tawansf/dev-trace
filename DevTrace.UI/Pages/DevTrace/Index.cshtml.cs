using DevTrace.Shared.Models;
using DevTrace.Shared.Stores;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevTrace.UI.Pages.DevTrace;

public class IndexModel : PageModel
{
    public IReadOnlyList<RequestLog> Logs { get; private set; } = [];
    public void OnGet()
    {
        Logs = RequestLogStore.Logs.OrderByDescending(l => l.Timestamp).Take(100).ToList();
    }
}