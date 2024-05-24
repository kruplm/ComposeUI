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



/*
import { ComposeUIDesktopAgent } from "./ComposeUIDesktopAgent";
import { createMessageRouter } from "@morgan-stanley/composeui-messaging-client";


let fdc3 = new ComposeUIDesktopAgent("default", createMessageRouter());

export default fdc3;

*/

import { createAgent } from "@connectifi/agent-web";

//coreWebView.PostWebMessageAsJson("{ topic: 'connect', appId: '*'}");
console.log("before event");
(window as any).chrome.webview.addEventListener('message', async (event: MessageEvent) => {
    console.log("event", event);
    console.log("event.data.appId", event.data.appId);
    var appId = event.data.appId;

//document.addEventListener('DOMContentLoaded', async () => {
    // if not in Connectifi's tunnel, start the agent 
    const docPath = document.location.pathname.toLowerCase();
    if (docPath !== 'blank' && !docPath.startsWith('/api/tunnel/'))
    {
        const fdc3 = await createAgent("https://dev.connectifi-interop.com",  appId, {props: {position: "tr", }, headless: true}) as any; //headless also here
        window.fdc3 = fdc3;
    }
    return;
});
