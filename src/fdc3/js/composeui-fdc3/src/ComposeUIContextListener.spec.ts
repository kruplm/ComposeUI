/* 
 *  Morgan Stanley makes this available to you under the Apache License,
 *  Version 2.0 (the "License"). You may obtain a copy of the License at
 *       http://www.apache.org/licenses/LICENSE-2.0.
 *  See the NOTICE file distributed with this work for additional information
 *  regarding copyright ownership. Unless required by applicable law or agreed
 *  to in writing, software distributed under the License is distributed on an
 *  "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
 *  or implied. See the License for the specific language governing permissions
 *  and limitations under the License.
 *  
 */

import { jest } from '@jest/globals';
import { MessageRouter } from '@morgan-stanley/composeui-messaging-client';
import { ComposeUIContextListener } from './infrastructure/ComposeUIContextListener';
import { Fdc3AddContextListenerResponse } from './infrastructure/messages/Fdc3AddContextListenerResponse';
import { Fdc3RemoveContextListenerResponse } from './infrastructure/messages/Fdc3RemoveContextListenerResponse';

const dummyContext = { type: "dummyContextType" };
const dummyChannelId = "dummyId";
let messageRouterClient: MessageRouter;
let testListener: ComposeUIContextListener;

const testInstrument = {
    type: 'fdc3.instrument',
    id: {
        ticker: 'AAPL'
    }
};

const wrongContext = {
    type: 'dummy'
}

const contextMessageHandlerMock = jest.fn((something) => {
    return "dummy";
});

describe('Tests for ComposeUIContextListener implementation API', () => {
    beforeEach(async () => {
        window.composeui = {
            fdc3: {
                config: {
                    appId: "testAppId",
                    instanceId: "testInstanceId"
                },
                channelId : "test",
                openAppIdentifier: {
                    openedAppContextId: "test"
                }
            }
        };

        const response: Fdc3AddContextListenerResponse = {
            success: true,
            id: "testListenerId"
        };

        messageRouterClient = {
            clientId: "dummy",
            subscribe: jest.fn(() => {
                return Promise.resolve({ unsubscribe: () => { } });
            }),

            publish: jest.fn(() => { return Promise.resolve() }),
            connect: jest.fn(() => { return Promise.resolve() }),
            registerEndpoint: jest.fn(() => { return Promise.resolve() }),
            unregisterEndpoint: jest.fn(() => { return Promise.resolve() }),
            registerService: jest.fn(() => { return Promise.resolve() }),
            unregisterService: jest.fn(() => { return Promise.resolve() }),
            invoke: jest.fn(() => { return Promise.resolve(`${JSON.stringify(undefined)}`) })
                .mockImplementationOnce(() => Promise.resolve(`${JSON.stringify(response)}`))
                .mockImplementationOnce(() => Promise.resolve(JSON.stringify({ context: "", payload: `${JSON.stringify(dummyContext)}` })))
        };

        testListener = new ComposeUIContextListener(true, messageRouterClient, contextMessageHandlerMock, "fdc3.instrument");
        await testListener.subscribe(dummyChannelId, "user");
    });

    it('subscribe will call messagerouter subscribe method', async () => {
        expect(messageRouterClient.subscribe).toHaveBeenCalledTimes(1);
    });

    it('handleContextMessage will trigger the handler', async () => {
        await testListener.handleContextMessage(testInstrument);
        expect(contextMessageHandlerMock).toHaveBeenCalledWith(testInstrument);
    });

    it('handleContextMessage will be rejected with Error if unsubscribed', async () => {
        testListener = new ComposeUIContextListener(true, messageRouterClient, contextMessageHandlerMock, "fdc3.instrument");
        testListener.unsubscribe();
        await expect(testListener.handleContextMessage(testInstrument))
            .rejects
            .toThrow("The current listener is not subscribed.");
    });

    it('handleContextMessage will be rejected with Error if called with wrong context type', async () => {
        await expect(testListener.handleContextMessage(wrongContext))
            .rejects
            .toThrow(Error);
    });

    it('handleContextMessage wont call the handler as the context from the open call was not handled yet', async() => {
        testListener.setOpenHandled(false);
        await testListener.handleContextMessage(testInstrument);
        expect(contextMessageHandlerMock).toHaveBeenCalledTimes(0);
    });
});