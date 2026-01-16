import { BaseAuditable } from './base-auditable';

export class Machine extends BaseAuditable {
  id: number;
  machineCode: string;
  machineName: string;
  machineSetupTime: string;
  machineMaintenanceTime: string;
  machineWorkingHoursPerDay: number;
  remarks: string;
  userIds: number[] = [];
}
export class MachineUser extends BaseAuditable {
  id: number;

  // Machine info
  machineId: number;
  machineCode: string;
  machineName: string;

  // User info
  userId: number;
  userName: string;
}