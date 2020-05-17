using System.Threading;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class BreakpointExtensions
    {
        public static void SendBreakpoint(this IDebugClient mediator, BreakpointEvent @event)
        {
            mediator.SendNotification(@event);
        }
    }
}
