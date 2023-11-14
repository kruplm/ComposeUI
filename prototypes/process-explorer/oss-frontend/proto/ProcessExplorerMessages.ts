import type * as grpc from '@grpc/grpc-js';
import type { EnumTypeDefinition, MessageTypeDefinition } from '@grpc/proto-loader';

import type { ProcessExplorerMessageHandlerClient as _ProcessExplorerMessageHandlerClient, ProcessExplorerMessageHandlerDefinition as _ProcessExplorerMessageHandlerDefinition } from './ProcessExplorerMessageHandler';

type SubtypeConstructor<Constructor extends new (...args: any) => any, Subtype> = {
  new(...args: ConstructorParameters<Constructor>): Subtype;
};

export interface ProtoGrpcType {
  ActionType: EnumTypeDefinition
  Connection: MessageTypeDefinition
  Message: MessageTypeDefinition
  Module: MessageTypeDefinition
  Process: MessageTypeDefinition
  ProcessExplorerMessageHandler: SubtypeConstructor<typeof grpc.Client, _ProcessExplorerMessageHandlerClient> & { service: _ProcessExplorerMessageHandlerDefinition }
  ProcessInfoCollectorData: MessageTypeDefinition
  ProcessThreadInfo: MessageTypeDefinition
  Registration: MessageTypeDefinition
  Subsystem: MessageTypeDefinition
  google: {
    protobuf: {
      Duration: MessageTypeDefinition
      Empty: MessageTypeDefinition
    }
  }
}

