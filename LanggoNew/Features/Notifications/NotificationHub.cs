using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LanggoNew.Features.Notifications;

[Authorize]
public class NotificationHub : Hub
{
}
