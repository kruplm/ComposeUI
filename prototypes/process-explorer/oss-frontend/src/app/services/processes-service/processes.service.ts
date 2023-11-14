/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */
import { Injectable } from '@angular/core';
//import { Observable, of } from 'rxjs';
//import { MockProcesses } from './mock-processes';
//import { ProcessTable } from '../../DTOs/ProcessInfo';
//import { Component } from '@angular/core';
//import { grpc } from '@improbable-eng/grpc-web';
//import { Request } from '@improbable-eng/grpc-web/dist/typings/invoke';
//import { Message } from '../../generated-protos-files/ProcessExplorerMessages_pb';
//import { ProcessExplorerMessageHandler } from '../../generated-protos-files/ProcessExplorerMessages_pb_service';
//import * as google_protobuf_empty_pb from "google-protobuf/google/protobuf/empty_pb";

//import * as grpc from '@grpc/grpc-js';
//import * as protoLoader from '@grpc/proto-loader';
//import { ProtoGrpcType } from './proto/example';
//import { ExampleHandlers } from './proto/example_package/Example';

//import {} from '../../../../../dotnet/src/MorganStanley.ComposeUI.ProcessExplorer.Abstractions/Infrastructure/Protos/ProcessExplorerMessages.proto'


import * as grpc from '@grpc/grpc-js';
import * as protoLoader from '@grpc/proto-loader';
import { ProtoGrpcType } from '../../../../proto/ProcessExplorerMessages';
import { ProcessExplorerMessageHandlerHandlers } from '../../../../proto/ProcessExplorerMessageHandler';


export class ProcessesService {
  @Injectable({
    providedIn: 'root'
  })
  
public exampleServer: ProcessExplorerMessageHandlerHandlers /*ExampleHandlers*/;

public packageDefinition = protoLoader.loadSync('./proto/example.proto');
public proto = (grpc.loadPackageDefinition(   this.packageDefinition ) as unknown) as ProtoGrpcType;

public server = new grpc.Server();
//server.addService(proto.example_package.Example.service, exampleServer);
//




  public getProcess(){
    //this.server.addService(this.proto.example_package.Example.service, this.exampleServer);
    //this.server.addService(this.proto.Process.fileDescriptorProtos., this.exampleServer);
  }
}  


  //grpcClient!: Request;

  /*
  headerDict = {
    'Content-Type': 'application/grpc',
  }
  **/
/*
  public getProcesses(tableName: string): Observable<ProcessTable[]> {
    return of(MockProcesses[tableName]);
  }*/

  /*
  public getProcess(){
    const request = new google_protobuf_empty_pb.Empty;

    this.grpcClient = grpc.invoke(ProcessExplorerMessageHandler.Subscribe, {
    host: 'http://localhost:5056',
    request: request,
    onMessage: (message: Message) => {
      console.log('message ====>', message.toObject()); 
    },
    metadata: new Headers(this.headerDict),
    onEnd: (code: grpc.Code, msg: string | undefined, trailers: grpc.Metadata) => {
      // This section works when server close connection.
      if (code == grpc.Code.OK) {
        console.log('request finished wihtout any error');
      } else {
        console.log('an error occured', code, msg, trailers);
      }
    },
  });

      

  }*/
    
  

