using Microsoft.Extensions.Localization;
using Skeleton.Application.Services.Interfaces;

namespace Skeleton.Infrastructure.Services;

public class LocalizationService : ILocalizationService
{
    private readonly IStringLocalizer _localizer;

    public LocalizationService(IStringLocalizerFactory factory)
    {
        // Binds to Resources/Messages.{culture}.resx
        _localizer = factory.Create("Messages", typeof(LocalizationService).Assembly.GetName().Name!);
    }

    public string Get(string key)
    {
        var val = _localizer[key];
        return val.ResourceNotFound ? key : val.Value;
    }

    public string Get(string key, params object[] args)
    {
        var val = _localizer[key];
        return val.ResourceNotFound ? key : string.Format(val.Value, args);
    }
}
