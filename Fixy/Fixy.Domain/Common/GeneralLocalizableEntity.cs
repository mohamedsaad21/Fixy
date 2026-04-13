using System.Globalization;

namespace Fixy.Domain.Common;

public class GeneralLocalizableEntity
{
    public string Localize(string textAr, string textEn)
    {
        CultureInfo culture = Thread.CurrentThread.CurrentCulture;
        if (culture.TwoLetterISOLanguageName.ToLower() == "ar")
            return textAr;
        return textEn;
    }
}
