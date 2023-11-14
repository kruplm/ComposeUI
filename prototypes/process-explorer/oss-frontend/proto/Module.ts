// Original file: ../dotnet/src/MorganStanley.ComposeUI.ProcessExplorer.Abstractions/Infrastructure/Protos/ProcessExplorerMessages.proto


export interface Module {
  'name'?: (string);
  'version'?: (string);
  'versionRedirectedFrom'?: (string);
  'publicKeyToken'?: (Buffer | Uint8Array | string);
  'location'?: (string);
  '_name'?: "name";
  '_version'?: "version";
  '_versionRedirectedFrom'?: "versionRedirectedFrom";
  '_publicKeyToken'?: "publicKeyToken";
  '_location'?: "location";
}

export interface Module__Output {
  'name'?: (string);
  'version'?: (string);
  'versionRedirectedFrom'?: (string);
  'publicKeyToken'?: (Buffer);
  'location'?: (string);
  '_name': "name";
  '_version': "version";
  '_versionRedirectedFrom': "versionRedirectedFrom";
  '_publicKeyToken': "publicKeyToken";
  '_location': "location";
}
