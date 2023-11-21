import * as jspb from 'google-protobuf'

import * as google_protobuf_duration_pb from 'google-protobuf/google/protobuf/duration_pb';
import * as google_protobuf_empty_pb from 'google-protobuf/google/protobuf/empty_pb';


export class Message extends jspb.Message {
  getAction(): ActionType;
  setAction(value: ActionType): Message;

  getDescription(): string;
  setDescription(value: string): Message;
  hasDescription(): boolean;
  clearDescription(): Message;

  getAssemblyid(): string;
  setAssemblyid(value: string): Message;
  hasAssemblyid(): boolean;
  clearAssemblyid(): Message;

  getProcessid(): number;
  setProcessid(value: number): Message;
  hasProcessid(): boolean;
  clearProcessid(): Message;

  getPeriodofdelay(): number;
  setPeriodofdelay(value: number): Message;

  getProcessesList(): Array<Process>;
  setProcessesList(value: Array<Process>): Message;
  clearProcessesList(): Message;
  addProcesses(value?: Process, index?: number): Process;

  getProcessstatuschangesMap(): jspb.Map<number, string>;
  clearProcessstatuschangesMap(): Message;

  getRuntimeinfo(): ProcessInfoCollectorData | undefined;
  setRuntimeinfo(value?: ProcessInfoCollectorData): Message;
  hasRuntimeinfo(): boolean;
  clearRuntimeinfo(): Message;

  getMultipleruntimeinfoMap(): jspb.Map<string, ProcessInfoCollectorData>;
  clearMultipleruntimeinfoMap(): Message;

  getConnectionsList(): Array<Connection>;
  setConnectionsList(value: Array<Connection>): Message;
  clearConnectionsList(): Message;
  addConnections(value?: Connection, index?: number): Connection;

  getConnectionstatuschangesMap(): jspb.Map<string, string>;
  clearConnectionstatuschangesMap(): Message;

  getRegistrationsList(): Array<Registration>;
  setRegistrationsList(value: Array<Registration>): Message;
  clearRegistrationsList(): Message;
  addRegistrations(value?: Registration, index?: number): Registration;

  getModulesList(): Array<Module>;
  setModulesList(value: Array<Module>): Message;
  clearModulesList(): Message;
  addModules(value?: Module, index?: number): Module;

  getEnvironmentvariablesMap(): jspb.Map<string, string>;
  clearEnvironmentvariablesMap(): Message;

  getSubsystemsMap(): jspb.Map<string, Subsystem>;
  clearSubsystemsMap(): Message;

  getAction1Case(): Message.Action1Case;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Message.AsObject;
  static toObject(includeInstance: boolean, msg: Message): Message.AsObject;
  static serializeBinaryToWriter(message: Message, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Message;
  static deserializeBinaryFromReader(message: Message, reader: jspb.BinaryReader): Message;
}

export namespace Message {
  export type AsObject = {
    action: ActionType,
    description?: string,
    assemblyid?: string,
    processid?: number,
    periodofdelay: number,
    processesList: Array<Process.AsObject>,
    processstatuschangesMap: Array<[number, string]>,
    runtimeinfo?: ProcessInfoCollectorData.AsObject,
    multipleruntimeinfoMap: Array<[string, ProcessInfoCollectorData.AsObject]>,
    connectionsList: Array<Connection.AsObject>,
    connectionstatuschangesMap: Array<[string, string]>,
    registrationsList: Array<Registration.AsObject>,
    modulesList: Array<Module.AsObject>,
    environmentvariablesMap: Array<[string, string]>,
    subsystemsMap: Array<[string, Subsystem.AsObject]>,
  }

  export enum Action1Case { 
    ACTION1_NOT_SET = 0,
    ACTION = 1,
  }

  export enum DescriptionCase { 
    _DESCRIPTION_NOT_SET = 0,
    DESCRIPTION = 2,
  }

  export enum AssemblyidCase { 
    _ASSEMBLYID_NOT_SET = 0,
    ASSEMBLYID = 3,
  }

  export enum ProcessidCase { 
    _PROCESSID_NOT_SET = 0,
    PROCESSID = 4,
  }
}

export class Process extends jspb.Message {
  getStarttime(): string;
  setStarttime(value: string): Process;
  hasStarttime(): boolean;
  clearStarttime(): Process;

  getProcessorusagetime(): google_protobuf_duration_pb.Duration | undefined;
  setProcessorusagetime(value?: google_protobuf_duration_pb.Duration): Process;
  hasProcessorusagetime(): boolean;
  clearProcessorusagetime(): Process;

  getPhysicalmemoryusagebit(): number;
  setPhysicalmemoryusagebit(value: number): Process;
  hasPhysicalmemoryusagebit(): boolean;
  clearPhysicalmemoryusagebit(): Process;

  getProcessname(): string;
  setProcessname(value: string): Process;
  hasProcessname(): boolean;
  clearProcessname(): Process;

  getProcessid(): number;
  setProcessid(value: number): Process;
  hasProcessid(): boolean;
  clearProcessid(): Process;

  getProcesspriorityclass(): string;
  setProcesspriorityclass(value: string): Process;
  hasProcesspriorityclass(): boolean;
  clearProcesspriorityclass(): Process;

  getThreadsList(): Array<ProcessThreadInfo>;
  setThreadsList(value: Array<ProcessThreadInfo>): Process;
  clearThreadsList(): Process;
  addThreads(value?: ProcessThreadInfo, index?: number): ProcessThreadInfo;

  getVirtualmemorysize(): number;
  setVirtualmemorysize(value: number): Process;
  hasVirtualmemorysize(): boolean;
  clearVirtualmemorysize(): Process;

  getParentid(): number;
  setParentid(value: number): Process;
  hasParentid(): boolean;
  clearParentid(): Process;

  getPrivatememoryusage(): number;
  setPrivatememoryusage(value: number): Process;
  hasPrivatememoryusage(): boolean;
  clearPrivatememoryusage(): Process;

  getProcessstatus(): string;
  setProcessstatus(value: string): Process;
  hasProcessstatus(): boolean;
  clearProcessstatus(): Process;

  getMemoryusage(): number;
  setMemoryusage(value: number): Process;
  hasMemoryusage(): boolean;
  clearMemoryusage(): Process;

  getProcessorusage(): number;
  setProcessorusage(value: number): Process;
  hasProcessorusage(): boolean;
  clearProcessorusage(): Process;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Process.AsObject;
  static toObject(includeInstance: boolean, msg: Process): Process.AsObject;
  static serializeBinaryToWriter(message: Process, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Process;
  static deserializeBinaryFromReader(message: Process, reader: jspb.BinaryReader): Process;
}

export namespace Process {
  export type AsObject = {
    starttime?: string,
    processorusagetime?: google_protobuf_duration_pb.Duration.AsObject,
    physicalmemoryusagebit?: number,
    processname?: string,
    processid?: number,
    processpriorityclass?: string,
    threadsList: Array<ProcessThreadInfo.AsObject>,
    virtualmemorysize?: number,
    parentid?: number,
    privatememoryusage?: number,
    processstatus?: string,
    memoryusage?: number,
    processorusage?: number,
  }

  export enum StarttimeCase { 
    _STARTTIME_NOT_SET = 0,
    STARTTIME = 4,
  }

  export enum ProcessorusagetimeCase { 
    _PROCESSORUSAGETIME_NOT_SET = 0,
    PROCESSORUSAGETIME = 5,
  }

  export enum PhysicalmemoryusagebitCase { 
    _PHYSICALMEMORYUSAGEBIT_NOT_SET = 0,
    PHYSICALMEMORYUSAGEBIT = 6,
  }

  export enum ProcessnameCase { 
    _PROCESSNAME_NOT_SET = 0,
    PROCESSNAME = 7,
  }

  export enum ProcessidCase { 
    _PROCESSID_NOT_SET = 0,
    PROCESSID = 8,
  }

  export enum ProcesspriorityclassCase { 
    _PROCESSPRIORITYCLASS_NOT_SET = 0,
    PROCESSPRIORITYCLASS = 9,
  }

  export enum VirtualmemorysizeCase { 
    _VIRTUALMEMORYSIZE_NOT_SET = 0,
    VIRTUALMEMORYSIZE = 11,
  }

  export enum ParentidCase { 
    _PARENTID_NOT_SET = 0,
    PARENTID = 12,
  }

  export enum PrivatememoryusageCase { 
    _PRIVATEMEMORYUSAGE_NOT_SET = 0,
    PRIVATEMEMORYUSAGE = 13,
  }

  export enum ProcessstatusCase { 
    _PROCESSSTATUS_NOT_SET = 0,
    PROCESSSTATUS = 14,
  }

  export enum MemoryusageCase { 
    _MEMORYUSAGE_NOT_SET = 0,
    MEMORYUSAGE = 15,
  }

  export enum ProcessorusageCase { 
    _PROCESSORUSAGE_NOT_SET = 0,
    PROCESSORUSAGE = 16,
  }
}

export class ProcessThreadInfo extends jspb.Message {
  getStarttime(): string;
  setStarttime(value: string): ProcessThreadInfo;
  hasStarttime(): boolean;
  clearStarttime(): ProcessThreadInfo;

  getPrioritylevel(): number;
  setPrioritylevel(value: number): ProcessThreadInfo;
  hasPrioritylevel(): boolean;
  clearPrioritylevel(): ProcessThreadInfo;

  getId(): number;
  setId(value: number): ProcessThreadInfo;
  hasId(): boolean;
  clearId(): ProcessThreadInfo;

  getStatus(): string;
  setStatus(value: string): ProcessThreadInfo;
  hasStatus(): boolean;
  clearStatus(): ProcessThreadInfo;

  getProcessorusagetime(): google_protobuf_duration_pb.Duration | undefined;
  setProcessorusagetime(value?: google_protobuf_duration_pb.Duration): ProcessThreadInfo;
  hasProcessorusagetime(): boolean;
  clearProcessorusagetime(): ProcessThreadInfo;

  getWaitreason(): string;
  setWaitreason(value: string): ProcessThreadInfo;
  hasWaitreason(): boolean;
  clearWaitreason(): ProcessThreadInfo;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ProcessThreadInfo.AsObject;
  static toObject(includeInstance: boolean, msg: ProcessThreadInfo): ProcessThreadInfo.AsObject;
  static serializeBinaryToWriter(message: ProcessThreadInfo, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ProcessThreadInfo;
  static deserializeBinaryFromReader(message: ProcessThreadInfo, reader: jspb.BinaryReader): ProcessThreadInfo;
}

export namespace ProcessThreadInfo {
  export type AsObject = {
    starttime?: string,
    prioritylevel?: number,
    id?: number,
    status?: string,
    processorusagetime?: google_protobuf_duration_pb.Duration.AsObject,
    waitreason?: string,
  }

  export enum StarttimeCase { 
    _STARTTIME_NOT_SET = 0,
    STARTTIME = 1,
  }

  export enum PrioritylevelCase { 
    _PRIORITYLEVEL_NOT_SET = 0,
    PRIORITYLEVEL = 2,
  }

  export enum IdCase { 
    _ID_NOT_SET = 0,
    ID = 3,
  }

  export enum StatusCase { 
    _STATUS_NOT_SET = 0,
    STATUS = 4,
  }

  export enum ProcessorusagetimeCase { 
    _PROCESSORUSAGETIME_NOT_SET = 0,
    PROCESSORUSAGETIME = 5,
  }

  export enum WaitreasonCase { 
    _WAITREASON_NOT_SET = 0,
    WAITREASON = 6,
  }
}

export class ProcessInfoCollectorData extends jspb.Message {
  getId(): number;
  setId(value: number): ProcessInfoCollectorData;

  getRegistrationsList(): Array<Registration>;
  setRegistrationsList(value: Array<Registration>): ProcessInfoCollectorData;
  clearRegistrationsList(): ProcessInfoCollectorData;
  addRegistrations(value?: Registration, index?: number): Registration;

  getEnvironmentvariablesMap(): jspb.Map<string, string>;
  clearEnvironmentvariablesMap(): ProcessInfoCollectorData;

  getConnectionsList(): Array<Connection>;
  setConnectionsList(value: Array<Connection>): ProcessInfoCollectorData;
  clearConnectionsList(): ProcessInfoCollectorData;
  addConnections(value?: Connection, index?: number): Connection;

  getModulesList(): Array<Module>;
  setModulesList(value: Array<Module>): ProcessInfoCollectorData;
  clearModulesList(): ProcessInfoCollectorData;
  addModules(value?: Module, index?: number): Module;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ProcessInfoCollectorData.AsObject;
  static toObject(includeInstance: boolean, msg: ProcessInfoCollectorData): ProcessInfoCollectorData.AsObject;
  static serializeBinaryToWriter(message: ProcessInfoCollectorData, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ProcessInfoCollectorData;
  static deserializeBinaryFromReader(message: ProcessInfoCollectorData, reader: jspb.BinaryReader): ProcessInfoCollectorData;
}

export namespace ProcessInfoCollectorData {
  export type AsObject = {
    id: number,
    registrationsList: Array<Registration.AsObject>,
    environmentvariablesMap: Array<[string, string]>,
    connectionsList: Array<Connection.AsObject>,
    modulesList: Array<Module.AsObject>,
  }
}

export class Registration extends jspb.Message {
  getImplementationtype(): string;
  setImplementationtype(value: string): Registration;
  hasImplementationtype(): boolean;
  clearImplementationtype(): Registration;

  getLifetime(): string;
  setLifetime(value: string): Registration;
  hasLifetime(): boolean;
  clearLifetime(): Registration;

  getServicetype(): string;
  setServicetype(value: string): Registration;
  hasServicetype(): boolean;
  clearServicetype(): Registration;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Registration.AsObject;
  static toObject(includeInstance: boolean, msg: Registration): Registration.AsObject;
  static serializeBinaryToWriter(message: Registration, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Registration;
  static deserializeBinaryFromReader(message: Registration, reader: jspb.BinaryReader): Registration;
}

export namespace Registration {
  export type AsObject = {
    implementationtype?: string,
    lifetime?: string,
    servicetype?: string,
  }

  export enum ImplementationtypeCase { 
    _IMPLEMENTATIONTYPE_NOT_SET = 0,
    IMPLEMENTATIONTYPE = 1,
  }

  export enum LifetimeCase { 
    _LIFETIME_NOT_SET = 0,
    LIFETIME = 2,
  }

  export enum ServicetypeCase { 
    _SERVICETYPE_NOT_SET = 0,
    SERVICETYPE = 3,
  }
}

export class Connection extends jspb.Message {
  getId(): string;
  setId(value: string): Connection;
  hasId(): boolean;
  clearId(): Connection;

  getName(): string;
  setName(value: string): Connection;
  hasName(): boolean;
  clearName(): Connection;

  getLocalendpoint(): string;
  setLocalendpoint(value: string): Connection;
  hasLocalendpoint(): boolean;
  clearLocalendpoint(): Connection;

  getRemoteendpoint(): string;
  setRemoteendpoint(value: string): Connection;
  hasRemoteendpoint(): boolean;
  clearRemoteendpoint(): Connection;

  getRemoteapplication(): string;
  setRemoteapplication(value: string): Connection;
  hasRemoteapplication(): boolean;
  clearRemoteapplication(): Connection;

  getRemotehostname(): string;
  setRemotehostname(value: string): Connection;
  hasRemotehostname(): boolean;
  clearRemotehostname(): Connection;

  getConnectioninformationMap(): jspb.Map<string, string>;
  clearConnectioninformationMap(): Connection;

  getStatus(): string;
  setStatus(value: string): Connection;
  hasStatus(): boolean;
  clearStatus(): Connection;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Connection.AsObject;
  static toObject(includeInstance: boolean, msg: Connection): Connection.AsObject;
  static serializeBinaryToWriter(message: Connection, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Connection;
  static deserializeBinaryFromReader(message: Connection, reader: jspb.BinaryReader): Connection;
}

export namespace Connection {
  export type AsObject = {
    id?: string,
    name?: string,
    localendpoint?: string,
    remoteendpoint?: string,
    remoteapplication?: string,
    remotehostname?: string,
    connectioninformationMap: Array<[string, string]>,
    status?: string,
  }

  export enum IdCase { 
    _ID_NOT_SET = 0,
    ID = 1,
  }

  export enum NameCase { 
    _NAME_NOT_SET = 0,
    NAME = 2,
  }

  export enum LocalendpointCase { 
    _LOCALENDPOINT_NOT_SET = 0,
    LOCALENDPOINT = 3,
  }

  export enum RemoteendpointCase { 
    _REMOTEENDPOINT_NOT_SET = 0,
    REMOTEENDPOINT = 4,
  }

  export enum RemoteapplicationCase { 
    _REMOTEAPPLICATION_NOT_SET = 0,
    REMOTEAPPLICATION = 5,
  }

  export enum RemotehostnameCase { 
    _REMOTEHOSTNAME_NOT_SET = 0,
    REMOTEHOSTNAME = 6,
  }

  export enum StatusCase { 
    _STATUS_NOT_SET = 0,
    STATUS = 8,
  }
}

export class Module extends jspb.Message {
  getName(): string;
  setName(value: string): Module;
  hasName(): boolean;
  clearName(): Module;

  getVersion(): string;
  setVersion(value: string): Module;
  hasVersion(): boolean;
  clearVersion(): Module;

  getVersionredirectedfrom(): string;
  setVersionredirectedfrom(value: string): Module;
  hasVersionredirectedfrom(): boolean;
  clearVersionredirectedfrom(): Module;

  getPublickeytoken(): Uint8Array | string;
  getPublickeytoken_asU8(): Uint8Array;
  getPublickeytoken_asB64(): string;
  setPublickeytoken(value: Uint8Array | string): Module;
  hasPublickeytoken(): boolean;
  clearPublickeytoken(): Module;

  getLocation(): string;
  setLocation(value: string): Module;
  hasLocation(): boolean;
  clearLocation(): Module;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Module.AsObject;
  static toObject(includeInstance: boolean, msg: Module): Module.AsObject;
  static serializeBinaryToWriter(message: Module, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Module;
  static deserializeBinaryFromReader(message: Module, reader: jspb.BinaryReader): Module;
}

export namespace Module {
  export type AsObject = {
    name?: string,
    version?: string,
    versionredirectedfrom?: string,
    publickeytoken?: Uint8Array | string,
    location?: string,
  }

  export enum NameCase { 
    _NAME_NOT_SET = 0,
    NAME = 1,
  }

  export enum VersionCase { 
    _VERSION_NOT_SET = 0,
    VERSION = 2,
  }

  export enum VersionredirectedfromCase { 
    _VERSIONREDIRECTEDFROM_NOT_SET = 0,
    VERSIONREDIRECTEDFROM = 3,
  }

  export enum PublickeytokenCase { 
    _PUBLICKEYTOKEN_NOT_SET = 0,
    PUBLICKEYTOKEN = 4,
  }

  export enum LocationCase { 
    _LOCATION_NOT_SET = 0,
    LOCATION = 5,
  }
}

export class Subsystem extends jspb.Message {
  getName(): string;
  setName(value: string): Subsystem;

  getStartuptype(): string;
  setStartuptype(value: string): Subsystem;
  hasStartuptype(): boolean;
  clearStartuptype(): Subsystem;

  getUitype(): string;
  setUitype(value: string): Subsystem;
  hasUitype(): boolean;
  clearUitype(): Subsystem;

  getPath(): string;
  setPath(value: string): Subsystem;
  hasPath(): boolean;
  clearPath(): Subsystem;

  getUrl(): string;
  setUrl(value: string): Subsystem;
  hasUrl(): boolean;
  clearUrl(): Subsystem;

  getArgumentsList(): Array<string>;
  setArgumentsList(value: Array<string>): Subsystem;
  clearArgumentsList(): Subsystem;
  addArguments(value: string, index?: number): Subsystem;

  getPort(): number;
  setPort(value: number): Subsystem;
  hasPort(): boolean;
  clearPort(): Subsystem;

  getState(): string;
  setState(value: string): Subsystem;

  getDescription(): string;
  setDescription(value: string): Subsystem;
  hasDescription(): boolean;
  clearDescription(): Subsystem;

  getAutomatedstart(): boolean;
  setAutomatedstart(value: boolean): Subsystem;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Subsystem.AsObject;
  static toObject(includeInstance: boolean, msg: Subsystem): Subsystem.AsObject;
  static serializeBinaryToWriter(message: Subsystem, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Subsystem;
  static deserializeBinaryFromReader(message: Subsystem, reader: jspb.BinaryReader): Subsystem;
}

export namespace Subsystem {
  export type AsObject = {
    name: string,
    startuptype?: string,
    uitype?: string,
    path?: string,
    url?: string,
    argumentsList: Array<string>,
    port?: number,
    state: string,
    description?: string,
    automatedstart: boolean,
  }

  export enum StartuptypeCase { 
    _STARTUPTYPE_NOT_SET = 0,
    STARTUPTYPE = 2,
  }

  export enum UitypeCase { 
    _UITYPE_NOT_SET = 0,
    UITYPE = 3,
  }

  export enum PathCase { 
    _PATH_NOT_SET = 0,
    PATH = 4,
  }

  export enum UrlCase { 
    _URL_NOT_SET = 0,
    URL = 5,
  }

  export enum PortCase { 
    _PORT_NOT_SET = 0,
    PORT = 7,
  }

  export enum DescriptionCase { 
    _DESCRIPTION_NOT_SET = 0,
    DESCRIPTION = 9,
  }
}

export enum ActionType { 
  ADDPROCESSLISTACTION = 0,
  ADDPROCESSACTION = 1,
  REMOVEPROCESSBYIDACTION = 2,
  UPDATEPROCESSACTION = 3,
  UPDATEPROCESSSTATUSACTION = 4,
  ADDRUNTIMEINFOACTION = 5,
  ADDCONNECTIONLISTACTION = 6,
  UPDATECONNECTIONACTION = 7,
  UPDATECONNECTIONSTATUSACTION = 8,
  UPDATEENVIRONMENTVARIABLESACTION = 9,
  UPDATEMODULESACTION = 10,
  UPDATEREGISTRATIONSACTION = 11,
  UPDATESUBSYSTEMACTION = 12,
  INITSUBSYSTEMSACTION = 13,
  MODIFYSUBSYSTEMACTION = 14,
  REMOVESUBSYSTEMSACTION = 15,
  ADDSUBSYSTEMACTION = 16,
  ADDSUBSYSTEMSACTION = 17,
  RESTARTSUBSYSTEMSACTION = 18,
  TERMINATESUBSYSTEMSACTION = 19,
  LAUNCHSUBSYSTEMSACTION = 20,
  LAUNCHSUBSYSTEMSWITHDELAYACTION = 21,
  SUBSCRIPTIONALIVEACTION = 22,
}
