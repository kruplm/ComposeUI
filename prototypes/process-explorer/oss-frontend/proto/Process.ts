// Original file: ../dotnet/src/MorganStanley.ComposeUI.ProcessExplorer.Abstractions/Infrastructure/Protos/ProcessExplorerMessages.proto

import type { Duration as _google_protobuf_Duration, Duration__Output as _google_protobuf_Duration__Output } from './google/protobuf/Duration';
import type { ProcessThreadInfo as _ProcessThreadInfo, ProcessThreadInfo__Output as _ProcessThreadInfo__Output } from './ProcessThreadInfo';
import type { Long } from '@grpc/proto-loader';

export interface Process {
  'startTime'?: (string);
  'processorUsageTime'?: (_google_protobuf_Duration | null);
  'physicalMemoryUsageBit'?: (number | string | Long);
  'processName'?: (string);
  'processId'?: (number);
  'processPriorityClass'?: (string);
  'threads'?: (_ProcessThreadInfo)[];
  'virtualMemorySize'?: (number | string | Long);
  'parentId'?: (number);
  'privateMemoryUsage'?: (number | string | Long);
  'processStatus'?: (string);
  'memoryUsage'?: (number | string);
  'processorUsage'?: (number | string);
  '_startTime'?: "startTime";
  '_processorUsageTime'?: "processorUsageTime";
  '_physicalMemoryUsageBit'?: "physicalMemoryUsageBit";
  '_processName'?: "processName";
  '_processId'?: "processId";
  '_processPriorityClass'?: "processPriorityClass";
  '_virtualMemorySize'?: "virtualMemorySize";
  '_parentId'?: "parentId";
  '_privateMemoryUsage'?: "privateMemoryUsage";
  '_processStatus'?: "processStatus";
  '_memoryUsage'?: "memoryUsage";
  '_processorUsage'?: "processorUsage";
}

export interface Process__Output {
  'startTime'?: (string);
  'processorUsageTime'?: (_google_protobuf_Duration__Output | null);
  'physicalMemoryUsageBit'?: (string);
  'processName'?: (string);
  'processId'?: (number);
  'processPriorityClass'?: (string);
  'threads': (_ProcessThreadInfo__Output)[];
  'virtualMemorySize'?: (string);
  'parentId'?: (number);
  'privateMemoryUsage'?: (string);
  'processStatus'?: (string);
  'memoryUsage'?: (number);
  'processorUsage'?: (number);
  '_startTime': "startTime";
  '_processorUsageTime': "processorUsageTime";
  '_physicalMemoryUsageBit': "physicalMemoryUsageBit";
  '_processName': "processName";
  '_processId': "processId";
  '_processPriorityClass': "processPriorityClass";
  '_virtualMemorySize': "virtualMemorySize";
  '_parentId': "parentId";
  '_privateMemoryUsage': "privateMemoryUsage";
  '_processStatus': "processStatus";
  '_memoryUsage': "memoryUsage";
  '_processorUsage': "processorUsage";
}
