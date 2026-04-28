using Fixy.Application.Resources;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Common.Helpers;

public static class EnumLocalizer
{
    public static string Localize<TEnum>(TEnum value, IStringLocalizer<SharedResources> localizer) where TEnum : Enum
    {
        var key = $"{typeof(TEnum).Name}_{value}";
        return localizer[key];
    }
}