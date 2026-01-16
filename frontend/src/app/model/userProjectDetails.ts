export interface UserProjectDetails {
    userId: number;
    userName: string;
    name: string;
    designationId: number;
    designationName: string;
    assignedProjects: ProjectAssignment[];
}

export interface ProjectAssignment {
    projectId: number;
    projectCode: string;
    projectName: string;
    effectiveDate: Date;
    isActive: boolean;
    designationId: number;
    designationName: string;
}
