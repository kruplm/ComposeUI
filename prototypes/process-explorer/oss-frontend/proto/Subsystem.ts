// Original file: ../dotnet/src/MorganStanley.ComposeUI.ProcessExplorer.Abstractions/Infrastructure/Protos/ProcessExplorerMessages.proto


export interface Subsystem {
  'name'?: (string);
  'startupType'?: (string);
  'uiType'?: (string);
  'path'?: (string);
  'url'?: (string);
  'arguments'?: (string)[];
  'port'?: (number);
  'state'?: (string);
  'description'?: (string);
  'automatedStart'?: (boolean);
  '_startupType'?: "startupType";
  '_uiType'?: "uiType";
  '_path'?: "path";
  '_url'?: "url";
  '_port'?: "port";
  '_description'?: "description";
}

export interface Subsystem__Output {
  'name': (string);
  'startupType'?: (string);
  'uiType'?: (string);
  'path'?: (string);
  'url'?: (string);
  'arguments': (string)[];
  'port'?: (number);
  'state': (string);
  'description'?: (string);
  'automatedStart': (boolean);
  '_startupType': "startupType";
  '_uiType': "uiType";
  '_path': "path";
  '_url': "url";
  '_port': "port";
  '_description': "description";
}
