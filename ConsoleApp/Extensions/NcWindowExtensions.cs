using ConsoleApp.Windows.Generic;
using Display;

namespace ConsoleApp.Extensions;
internal static class NcWindowExtensions
{
    internal static string CreateErrorPopup(this NcWindow window, string message)
    {
        var popup = new ErrorPopup();
        popup.Initialize(window.Size.GetHorizontalPartial(3, 1).GetVerticalPartial(3, 1));
        popup.Message = message;
        window.Children.Add(popup);
        return popup.Id;
    }

    internal static string RemovePopup(this NcWindow window, string popupId)
    {
        var popup = window.Children.FirstOrDefault(x => x.Id == popupId);
        if (popup is not null)
        {
            popup.Dispose();
            window.Children.Remove(popup);
        }

        return string.Empty;
    }
}
