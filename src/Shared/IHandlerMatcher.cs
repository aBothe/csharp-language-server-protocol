using System.Collections.Generic;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    public interface IHandlerMatcher
    {
        /// <summary>
        /// Finds the first handler that matches the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="descriptors">The descriptors.</param>
        /// <returns></returns>
        IEnumerable<ILspHandlerDescriptor> FindHandler(object parameters, IEnumerable<ILspHandlerDescriptor> descriptors);
    }
}
