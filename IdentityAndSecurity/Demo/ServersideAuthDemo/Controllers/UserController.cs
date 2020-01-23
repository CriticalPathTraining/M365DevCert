using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.Graph;

namespace ServersideAuthDemo.Controllers
{
  [Authorize]
  public class MessagesController : Controller
  {
    private readonly ILogger<MessagesController> _logger;
    private readonly GraphServiceClient _graphServiceClient;

    public MessagesController(ILogger<MessagesController> logger, GraphServiceClient graphServiceClient)
    {
      _logger = logger;
      _graphServiceClient = graphServiceClient;
    }

    public async Task<IActionResult> Index()
    {
      var request = this._graphServiceClient.Me.Request().GetHttpRequestMessage();
      request.Properties["User"] = HttpContext.User;
      var response = await this._graphServiceClient.HttpProvider.SendAsync(request);
      var handler = new ResponseHandler(new Serializer());
      var user = await handler.HandleResponse<User>(response);

      return View(user);
    }
  }
}