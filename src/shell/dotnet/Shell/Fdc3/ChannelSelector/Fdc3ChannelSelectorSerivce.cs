﻿// Morgan Stanley makes this available to you under the Apache License,
// Version 2.0 (the "License"). You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0.
// 
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership. Unless required by applicable law or agreed
// to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions
// and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Finos.Fdc3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Converters;
using MorganStanley.ComposeUI.Messaging;
using MorganStanley.ComposeUI.Messaging.Abstractions;

namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector;

internal class Fdc3ChannelSelectorSerice : IHostedService
{
    private readonly object _disposeLock = new();

    private readonly List<Func<ValueTask>> _disposeTask = new();
    private readonly IHost _host;
    private IChannelSelectorCommunicator _channelSelectorComm; //= //new ChannelSelectorCommunicator();  //todo reference to the control or the model.. 
    //private Fdc3ChannelSelectorControl _channelSelector;  //todo reference to the control or the model.. 
    private Fdc3ChannelSelectorViewModel _channelSelector;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new AppMetadataJsonConverter(), new IconJsonConverter() }
    };

    public Fdc3ChannelSelectorSerice(IHost host)
    {
        _host = host;
        _channelSelector = new Fdc3ChannelSelectorViewModel(_channelSelectorComm);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await StartMessageRouterServiceAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Func<ValueTask>[]? reversedList = null;

        lock (_disposeLock)
        {
            reversedList = _disposeTask.AsEnumerable().Reverse().ToArray();
        }


        if (reversedList != null)
        {
            for (var i = 0; i < reversedList.Length; i++)
            {
                await reversedList[i].Invoke();
            }
        }
    }

    private async Task StartMessageRouterServiceAsync(CancellationToken cancellationToken)
    {
        var messageRouter = _host.Services.GetRequiredService<IMessageRouter>();

        await messageRouter.RegisterServiceAsync(
            "ComposeUI/fdc3/v2.0/channelSelector", //joinUserChannel
            async (endpoint, payload, context) =>
            {
                var request = payload?.ReadJson<JoinUserChannelRequest>(_jsonSerializerOptions); //Todo delete ChannelSelectorRequest
                if (request == null)
                {
                    return null;
                }

                var response = await JoinUserChannel(request.ChannelId); //todo

                return response is null ? null : MessageBuffer.Factory.CreateJson(response, _jsonSerializerOptions);
            },
            cancellationToken: cancellationToken);

       

        lock (_disposeLock)
        {
            _disposeTask.Add(
                async () =>
                {
                    if (messageRouter != null)
                    {
                        await messageRouter.UnregisterServiceAsync("ComposeUI/fdc3/v2.0/channelSelector", cancellationToken);
                        
                        await messageRouter.DisposeAsync();
                    }
                });
        }
    }

    private ValueTask<JoinUserChannelResponse> JoinUserChannel(string channelId)
    {
        //_channelSelector.
        
        //_channelSelector.SendChannelSelectorRequest(channelId, channelId); //todo params
        return new ValueTask<JoinUserChannelResponse>(); //todo replace fake return value
    }



}