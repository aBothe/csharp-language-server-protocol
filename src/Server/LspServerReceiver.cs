using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageProtocolShared;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server.Messages;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class LspServerReceiver : Receiver, ILspServerReceiver
    {
        private bool _initialized;

        public override (IEnumerable<Renor> results, bool hasResponse) GetRequests(JToken container)
        {
            if (_initialized) return base.GetRequests(container);

            var newResults = new List<Renor>();

            // Based on https://github.com/Microsoft/language-server-protocol/blob/master/protocol.md#initialize-request
            var (results, hasResponse) = base.GetRequests(container);
            foreach (var item in results)
            {
                if (item.IsRequest && HandlerTypeHelper.IsMethodName(item.Request.Method, typeof(IInitializeHandler)))
                {
                    newResults.Add(item);
                }
                else if (item.IsRequest)
                {
                    newResults.Add(new ServerNotInitialized());
                }
                else if (item.IsResponse)
                {
                    newResults.Add(item);
                }
                else if (item.IsNotification)
                {
                    // drop notifications
                    // newResults.Add(item);
                }
            }

            return (newResults, hasResponse);
        }

        public void Initialized()
        {
            _initialized = true;
        }
    }
}
