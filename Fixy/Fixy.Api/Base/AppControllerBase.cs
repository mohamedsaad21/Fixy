using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Base;

[Route("api/[controller]")]
[ApiController]
public class AppControllerBase : ControllerBase
{
}
