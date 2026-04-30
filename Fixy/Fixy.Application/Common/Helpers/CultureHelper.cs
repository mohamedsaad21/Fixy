using System.Globalization;

namespace Fixy.Application.Common.Helpers;

public static class CultureHelper
{
    public static void SetCulture(string? lang)
    {
        var culture = new CultureInfo(lang ?? "en-US");

        CultureInfo.CurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
    }
}