// Original file: ../dotnet/src/MorganStanley.ComposeUI.ProcessExplorer.Abstractions/Infrastructure/Protos/ProcessExplorerMessages.proto


export interface Connection {
  'id'?: (string);
  'name'?: (string);
  'localEndpoint'?: (string);
  'remoteEndpoint'?: (string);
  'remoteApplication'?: (string);
  'remoteHostname'?: (string);
  'connectionInformation'?: ({[key: string]: string});
  'status'?: (string);
  '_id'?: "id";
  '_name'?: "name";
  '_localEndpoint'?: "localEndpoint";
  '_remoteEndpoint'?: "remoteEndpoint";
  '_remoteApplication'?: "remoteApplication";
  '_remoteHostname'?: "remoteHostname";
  '_status'?: "status";
}

export interface Connection__Output {
  'id'?: (string);
  'name'?: (string);
  'localEndpoint'?: (string);
  'remoteEndpoint'?: (string);
  'remoteApplication'?: (string);
  'remoteHostname'?: (string);
  'connectionInformation': ({[key: string]: string});
  'status'?: (string);
  '_id': "id";
  '_name': "name";
  '_localEndpoint': "localEndpoint";
  '_remoteEndpoint': "remoteEndpoint";
  '_remoteApplication': "remoteApplication";
  '_remoteHostname': "remoteHostname";
  '_status': "status";
}
