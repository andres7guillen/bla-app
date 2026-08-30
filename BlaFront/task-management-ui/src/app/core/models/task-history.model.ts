export interface TaskHistory {
  id: string;
  taskId: string;
  previousStatus: string | null;
  newStatus: string;
  changedBy: string;
  changedAt: string;
}
