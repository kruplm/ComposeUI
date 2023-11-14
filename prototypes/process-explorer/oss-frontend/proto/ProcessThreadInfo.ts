// Original file: ../dotnet/src/MorganStanley.ComposeUI.ProcessExplorer.Abstractions/Infrastructure/Protos/ProcessExplorerMessages.proto

import type { Duration as _google_protobuf_Duration, Duration__Output as _google_protobuf_Duration__Output } from './google/protobuf/Duration';

export interface ProcessThreadInfo {
  'startTime'?: (string);
  'priorityLevel'?: (number);
  'id'?: (number);
  'status'?: (string);
  'processorUsageTime'?: (_google_protobuf_Duration | null);
  'waitReason'?: (string);
  '_startTime'?: "startTime";
  '_priorityLevel'?: "priorityLevel";
  '_id'?: "id";
  '_status'?: "status";
  '_processorUsageTime'?: "processorUsageTime";
  '_waitReason'?: "waitReason";
}

export interface ProcessThreadInfo__Output {
  'startTime'?: (string);
  'priorityLevel'?: (number);
  'id'?: (number);
  'status'?: (string);
  'processorUsageTime'?: (_google_protobuf_Duration__Output | null);
  'waitReason'?: (string);
  '_startTime': "startTime";
  '_priorityLevel': "priorityLevel";
  '_id': "id";
  '_status': "status";
  '_processorUsageTime': "processorUsageTime";
  '_waitReason': "waitReason";
}
