// Original file: ../dotnet/src/MorganStanley.ComposeUI.ProcessExplorer.Abstractions/Infrastructure/Protos/ProcessExplorerMessages.proto

import type { ActionType as _ActionType, ActionType__Output as _ActionType__Output } from './ActionType';
import type { Process as _Process, Process__Output as _Process__Output } from './Process';
import type { ProcessInfoCollectorData as _ProcessInfoCollectorData, ProcessInfoCollectorData__Output as _ProcessInfoCollectorData__Output } from './ProcessInfoCollectorData';
import type { Connection as _Connection, Connection__Output as _Connection__Output } from './Connection';
import type { Registration as _Registration, Registration__Output as _Registration__Output } from './Registration';
import type { Module as _Module, Module__Output as _Module__Output } from './Module';
import type { Subsystem as _Subsystem, Subsystem__Output as _Subsystem__Output } from './Subsystem';

export interface Message {
  'action'?: (_ActionType);
  'description'?: (string);
  'assemblyId'?: (string);
  'processId'?: (number);
  'periodOfDelay'?: (number);
  'processes'?: (_Process)[];
  'processStatusChanges'?: ({[key: number]: string});
  'runtimeInfo'?: (_ProcessInfoCollectorData | null);
  'multipleRuntimeInfo'?: ({[key: string]: _ProcessInfoCollectorData});
  'connections'?: (_Connection)[];
  'connectionStatusChanges'?: ({[key: string]: string});
  'registrations'?: (_Registration)[];
  'modules'?: (_Module)[];
  'environmentVariables'?: ({[key: string]: string});
  'subsystems'?: ({[key: string]: _Subsystem});
  'action1'?: "action";
  '_description'?: "description";
  '_assemblyId'?: "assemblyId";
  '_processId'?: "processId";
}

export interface Message__Output {
  'action'?: (_ActionType__Output);
  'description'?: (string);
  'assemblyId'?: (string);
  'processId'?: (number);
  'periodOfDelay': (number);
  'processes': (_Process__Output)[];
  'processStatusChanges': ({[key: number]: string});
  'runtimeInfo': (_ProcessInfoCollectorData__Output | null);
  'multipleRuntimeInfo': ({[key: string]: _ProcessInfoCollectorData__Output});
  'connections': (_Connection__Output)[];
  'connectionStatusChanges': ({[key: string]: string});
  'registrations': (_Registration__Output)[];
  'modules': (_Module__Output)[];
  'environmentVariables': ({[key: string]: string});
  'subsystems': ({[key: string]: _Subsystem__Output});
  'action1': "action";
  '_description': "description";
  '_assemblyId': "assemblyId";
  '_processId': "processId";
}
