using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.ServiceCategories.Commands.AddCategory;
using Fixy.Application.Features.ServiceCategories.Commands.DeleteCategory;
using Fixy.Application.Features.ServiceCategories.Commands.EditCategory;
using Fixy.Application.Features.ServiceCategories.Queries.GetCategoriesList;
using Fixy.Application.Features.ServiceCategories.Queries.GetCategoryById;
using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

public class ServiceCategoriesController : AppControllerBase
{
    [HttpGet(Router.CategoryRouting.List)]
    public async Task<IActionResult> GetCategories()
    {
        return ToActionResult(await Mediator.Send(new GetCategoriesListQuery()));
    }

    [HttpGet(Router.CategoryRouting.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid Id)
    {
        return ToActionResult(await Mediator.Send(new GetCategoryByIdQuery(Id)));
    }
    [Authorize(Roles = Roles.Admin)]
    [HttpPost(Router.CategoryRouting.Create)]
    public async Task<IActionResult> CreateCategory([FromQuery] AddCategoryCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut(Router.CategoryRouting.Edit)]
    public async Task<IActionResult> EditCategory([FromQuery] EditCategoryCommand command)
    {
        return ToActionResult(await Mediator.Send(command));
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete(Router.CategoryRouting.Delete)]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid Id)
    {
        return ToActionResult(await Mediator.Send(new DeleteCategoryCommand(Id)));
    }
}
