// Original file: ../dotnet/src/MorganStanley.ComposeUI.ProcessExplorer.Abstractions/Infrastructure/Protos/ProcessExplorerMessages.proto

import type * as grpc from '@grpc/grpc-js'
import type { MethodDefinition } from '@grpc/proto-loader'
import type { Empty as _google_protobuf_Empty, Empty__Output as _google_protobuf_Empty__Output } from './google/protobuf/Empty';
import type { Message as _Message, Message__Output as _Message__Output } from './Message';

export interface ProcessExplorerMessageHandlerClient extends grpc.Client {
  Send(argument: _Message, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_google_protobuf_Empty__Output>): grpc.ClientUnaryCall;
  Send(argument: _Message, metadata: grpc.Metadata, callback: grpc.requestCallback<_google_protobuf_Empty__Output>): grpc.ClientUnaryCall;
  Send(argument: _Message, options: grpc.CallOptions, callback: grpc.requestCallback<_google_protobuf_Empty__Output>): grpc.ClientUnaryCall;
  Send(argument: _Message, callback: grpc.requestCallback<_google_protobuf_Empty__Output>): grpc.ClientUnaryCall;
  send(argument: _Message, metadata: grpc.Metadata, options: grpc.CallOptions, callback: grpc.requestCallback<_google_protobuf_Empty__Output>): grpc.ClientUnaryCall;
  send(argument: _Message, metadata: grpc.Metadata, callback: grpc.requestCallback<_google_protobuf_Empty__Output>): grpc.ClientUnaryCall;
  send(argument: _Message, options: grpc.CallOptions, callback: grpc.requestCallback<_google_protobuf_Empty__Output>): grpc.ClientUnaryCall;
  send(argument: _Message, callback: grpc.requestCallback<_google_protobuf_Empty__Output>): grpc.ClientUnaryCall;
  
  Subscribe(argument: _google_protobuf_Empty, metadata: grpc.Metadata, options?: grpc.CallOptions): grpc.ClientReadableStream<_Message__Output>;
  Subscribe(argument: _google_protobuf_Empty, options?: grpc.CallOptions): grpc.ClientReadableStream<_Message__Output>;
  subscribe(argument: _google_protobuf_Empty, metadata: grpc.Metadata, options?: grpc.CallOptions): grpc.ClientReadableStream<_Message__Output>;
  subscribe(argument: _google_protobuf_Empty, options?: grpc.CallOptions): grpc.ClientReadableStream<_Message__Output>;
  
}

export interface ProcessExplorerMessageHandlerHandlers extends grpc.UntypedServiceImplementation {
  Send: grpc.handleUnaryCall<_Message__Output, _google_protobuf_Empty>;
  
  Subscribe: grpc.handleServerStreamingCall<_google_protobuf_Empty__Output, _Message>;
  
}

export interface ProcessExplorerMessageHandlerDefinition extends grpc.ServiceDefinition {
  Send: MethodDefinition<_Message, _google_protobuf_Empty, _Message__Output, _google_protobuf_Empty__Output>
  Subscribe: MethodDefinition<_google_protobuf_Empty, _Message, _google_protobuf_Empty__Output, _Message__Output>
}
