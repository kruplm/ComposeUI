// Original file: ../dotnet/src/MorganStanley.ComposeUI.ProcessExplorer.Abstractions/Infrastructure/Protos/ProcessExplorerMessages.proto

import type { Registration as _Registration, Registration__Output as _Registration__Output } from './Registration';
import type { Connection as _Connection, Connection__Output as _Connection__Output } from './Connection';
import type { Module as _Module, Module__Output as _Module__Output } from './Module';

export interface ProcessInfoCollectorData {
  'id'?: (number);
  'registrations'?: (_Registration)[];
  'environmentVariables'?: ({[key: string]: string});
  'connections'?: (_Connection)[];
  'modules'?: (_Module)[];
}

export interface ProcessInfoCollectorData__Output {
  'id': (number);
  'registrations': (_Registration__Output)[];
  'environmentVariables': ({[key: string]: string});
  'connections': (_Connection__Output)[];
  'modules': (_Module__Output)[];
}
