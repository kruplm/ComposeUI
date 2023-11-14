// Original file: ../dotnet/src/MorganStanley.ComposeUI.ProcessExplorer.Abstractions/Infrastructure/Protos/ProcessExplorerMessages.proto

export const ActionType = {
  AddProcessListAction: 'AddProcessListAction',
  AddProcessAction: 'AddProcessAction',
  RemoveProcessByIdAction: 'RemoveProcessByIdAction',
  UpdateProcessAction: 'UpdateProcessAction',
  UpdateProcessStatusAction: 'UpdateProcessStatusAction',
  AddRuntimeInfoAction: 'AddRuntimeInfoAction',
  AddConnectionListAction: 'AddConnectionListAction',
  UpdateConnectionAction: 'UpdateConnectionAction',
  UpdateConnectionStatusAction: 'UpdateConnectionStatusAction',
  UpdateEnvironmentVariablesAction: 'UpdateEnvironmentVariablesAction',
  UpdateModulesAction: 'UpdateModulesAction',
  UpdateRegistrationsAction: 'UpdateRegistrationsAction',
  UpdateSubsystemAction: 'UpdateSubsystemAction',
  InitSubsystemsAction: 'InitSubsystemsAction',
  ModifySubsystemAction: 'ModifySubsystemAction',
  RemoveSubsystemsAction: 'RemoveSubsystemsAction',
  AddSubsystemAction: 'AddSubsystemAction',
  AddSubsystemsAction: 'AddSubsystemsAction',
  RestartSubsystemsAction: 'RestartSubsystemsAction',
  TerminateSubsystemsAction: 'TerminateSubsystemsAction',
  LaunchSubsystemsAction: 'LaunchSubsystemsAction',
  LaunchSubsystemsWithDelayAction: 'LaunchSubsystemsWithDelayAction',
  SubscriptionAliveAction: 'SubscriptionAliveAction',
} as const;

export type ActionType =
  | 'AddProcessListAction'
  | 0
  | 'AddProcessAction'
  | 1
  | 'RemoveProcessByIdAction'
  | 2
  | 'UpdateProcessAction'
  | 3
  | 'UpdateProcessStatusAction'
  | 4
  | 'AddRuntimeInfoAction'
  | 5
  | 'AddConnectionListAction'
  | 6
  | 'UpdateConnectionAction'
  | 7
  | 'UpdateConnectionStatusAction'
  | 8
  | 'UpdateEnvironmentVariablesAction'
  | 9
  | 'UpdateModulesAction'
  | 10
  | 'UpdateRegistrationsAction'
  | 11
  | 'UpdateSubsystemAction'
  | 12
  | 'InitSubsystemsAction'
  | 13
  | 'ModifySubsystemAction'
  | 14
  | 'RemoveSubsystemsAction'
  | 15
  | 'AddSubsystemAction'
  | 16
  | 'AddSubsystemsAction'
  | 17
  | 'RestartSubsystemsAction'
  | 18
  | 'TerminateSubsystemsAction'
  | 19
  | 'LaunchSubsystemsAction'
  | 20
  | 'LaunchSubsystemsWithDelayAction'
  | 21
  | 'SubscriptionAliveAction'
  | 22

export type ActionType__Output = typeof ActionType[keyof typeof ActionType]
