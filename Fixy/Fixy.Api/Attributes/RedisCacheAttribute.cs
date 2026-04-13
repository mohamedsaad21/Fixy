using Fixy.Application.Contracts.ExternalServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using StackExchange.Redis;
using System.Text;

namespace Fixy.Api.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RedisCacheAttribute : Attribute, IAsyncActionFilter
{
    private readonly int _timeInMinutes;
    public RedisCacheAttribute(int timeInMinutes)
    {
        _timeInMinutes = timeInMinutes;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cacheService = context.HttpContext.RequestServices
            .GetRequiredService<ICacheService>();

        var cacheKey = GenerateCacheKey(context.HttpContext.Request);

        try
        {
            var cachedValue = await cacheService.GetData<object>(cacheKey);
            if (cachedValue != null)
            {
                context.Result = new OkObjectResult(cachedValue);
                return;
            }
        }
        catch (RedisConnectionException ex)
        {
            Log.Warning(ex, "Redis unavailable, bypassing cache for {Key}", cacheKey);
        }

        var executedContext = await next();

        try
        {
            if (executedContext.Result is OkObjectResult okObjectResult)
                await cacheService.SetData(cacheKey, okObjectResult.Value,
                    DateTimeOffset.Now.AddMinutes(_timeInMinutes));
        }
        catch (RedisConnectionException ex)
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<RedisCacheAttribute>>();
            logger.LogWarning(ex, "Redis unavailable, could not store cache for {Key}", cacheKey);
        }
    }
    //public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    //{
    //    var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
    //    var cacheKey = GenerateCacheKey(context.HttpContext.Request);
    //    var cachedValue = await cacheService.GetData<object>(cacheKey);

    //    if (cachedValue != null)
    //    {
    //        context.Result = new OkObjectResult(cachedValue);
    //        return;
    //    }

    //    var executedContext = await next();
    //    if (executedContext.Result is OkObjectResult okObjectResult)
    //        await cacheService.SetData(cacheKey, okObjectResult.Value, DateTimeOffset.Now.AddMinutes(_timeInMinutes));
    //}

    private string GenerateCacheKey(HttpRequest request)
    {
        StringBuilder keyBuilder = new StringBuilder();
        keyBuilder.Append(request.Path);
        foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            keyBuilder.Append($"|{key}:{value}");
        return keyBuilder.ToString();
    }
}