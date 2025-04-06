using Microsoft.AspNetCore.Components;

namespace Web.UIComponents
{
    public partial class NotificationWindow
    {
        [Parameter] public string Title { get; set; }
        [Parameter] public string Message { get; set; }
    }
}
