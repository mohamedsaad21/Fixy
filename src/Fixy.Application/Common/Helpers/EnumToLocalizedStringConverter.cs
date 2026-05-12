using AutoMapper;
using Fixy.Application.Resources;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Common.Helpers;

public class EnumToLocalizedStringConverter<TEnum> : ITypeConverter<TEnum, string> where TEnum : Enum
{
    private readonly IStringLocalizer<SharedResources> _localizer;

    public EnumToLocalizedStringConverter(IStringLocalizer<SharedResources> localizer)
    {
        _localizer = localizer;
    }

    public string Convert(TEnum source, string destination, ResolutionContext context)
    {
        var key = $"{typeof(TEnum).Name}_{source}";
        return _localizer[key];
    }
}