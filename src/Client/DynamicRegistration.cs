using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageProtocolShared;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using DynamicData;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    class WorkspaceFoldersHandler : IWorkspaceFoldersHandler
    {
        private readonly ISourceList<WorkspaceFolder> _workspaceFolders;

        public WorkspaceFoldersHandler(List<WorkspaceFolder> workspaceFolders)
        {
            _workspaceFolders = new SourceList<WorkspaceFolder>();
            _workspaceFolders.AddRange(workspaceFolders);
        }

        async Task<Container<WorkspaceFolder>> IRequestHandler<WorkspaceFolderParams, Container<WorkspaceFolder>>.
            Handle(WorkspaceFolderParams request, CancellationToken cancellationToken)
        {
            return new Container<WorkspaceFolder>(_workspaceFolders.Items);
        }

        public void AddFolder(DocumentUri uri, string name)
        {
            AddFolder(new WorkspaceFolder() {Name = name, Uri = uri});
        }

        public void AddFolder(WorkspaceFolder workspaceFolder)
        {
            _workspaceFolders.Add(workspaceFolder);
        }

        public void RemoveFolder(DocumentUri uri)
        {
            var folder = _workspaceFolders.Items.Where(x => x.Uri == uri);
            _workspaceFolders.RemoveMany(folder);
        }

        public void RemoveFolder(string name)
        {
            var folder = _workspaceFolders.Items.Where(x => x.Name == name);
            _workspaceFolders.RemoveMany(folder);
        }

        public void RemoveFolder(WorkspaceFolder workspaceFolder)
        {
            _workspaceFolders.Remove(workspaceFolder);
        }

        public ISourceList<WorkspaceFolder> WorkspaceFolders => _workspaceFolders;
    }

    class RegistrationHandler : IRegisterCapabilityHandler, IUnregisterCapabilityHandler
    {
        private readonly ISerializer _serializer;
        private readonly ISourceCache<Registration, string> _registrations;

        public RegistrationHandler(ISerializer serializer)
        {
            _serializer = serializer;
            _registrations = new SourceCache<Registration, string>(x => x.Id);
        }

        Task<Unit> IRequestHandler<RegistrationParams, Unit>.Handle(RegistrationParams request,
            CancellationToken cancellationToken)
        {
            Register(request.Registrations.ToArray());

            return Unit.Task;
        }

        // TODO:

        Task<Unit> IRequestHandler<UnregistrationParams, Unit>.Handle(UnregistrationParams request,
            CancellationToken cancellationToken)
        {
            _registrations.Edit(updater => {
                foreach (var item in request.Unregisterations ?? new UnregistrationContainer())
                {
                    updater.RemoveKey(item.Id);
                }
            });

            return Unit.Task;
        }

        public void Register(params Registration[] registrations)
        {
            _registrations.Edit(updater => {
                foreach (var registration in registrations ?? new RegistrationContainer())
                {
                    Register(registration, updater);
                }
            });
        }

        private void Register(Registration registration, ISourceUpdater<Registration, string> updater)
        {
            var typeDescriptor = HandlerTypeHelper.GetHandlerTypeDescriptor(registration.Method);
            if (typeDescriptor == null)
            {
                updater.AddOrUpdate(registration);
                return;
            }

            var deserializedRegistration = new Registration() {
                Id = registration.Id,
                Method = registration.Method,
                RegisterOptions = registration.RegisterOptions is JToken token
                    ? token.ToObject(typeDescriptor.RegistrationType, _serializer.JsonSerializer)
                    : registration.RegisterOptions
            };
            updater.AddOrUpdate(deserializedRegistration);
        }

        public IObservableList<Registration> Registrations => _registrations
            .Connect()
            .RemoveKey()
            .AsObservableList();

        public IObservableList<Registration> GetRegistrationsForMethod(string method) => _registrations
            .Connect()
            .RemoveKey()
            .Filter(x => x.Method == method)
            .AsObservableList();

        public IObservableList<Registration> GetRegistrationsMatchingSelector(DocumentSelector documentSelector) =>
            _registrations
                .Connect()
                .RemoveKey()
                .Filter(x => x.RegisterOptions is ITextDocumentRegistrationOptions ro && ro.DocumentSelector
                    .Join(documentSelector,
                        z => z.HasLanguage ? z.Language :
                            z.HasScheme ? z.Scheme :
                            z.HasPattern ? z.Pattern : string.Empty,
                        z => z.HasLanguage ? z.Language :
                            z.HasScheme ? z.Scheme :
                            z.HasPattern ? z.Pattern : string.Empty, (a, b) => a)
                    .Any(x => x.HasLanguage || x.HasPattern || x.HasScheme)
                )
                .AsObservableList();
    }
}
