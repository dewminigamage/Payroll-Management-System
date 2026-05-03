export interface Employee {
  employeeID: number
  fullName: string
  nic: string
  department: string | null
  position: string | null
  basicSalary: number
  joinDate: string
  contactNumber: string | null
  email: string | null
  isActive: boolean
}

export interface PayrollRecord {
  payrollID: number
  employeeID: number
  employeeName: string | null
  department: string | null
  payMonth: number
  payYear: number
  basicSalary: number
  allowances: number
  grossSalary: number
  epf: number
  etf: number
  tax: number
  otherDeductions: number
  netSalary: number
  remarks: string | null
  createdDate: string
}

export interface AttendanceRecord {
  attendanceID: number
  employeeID: number
  employeeName: string | null
  attendanceDate: string
  status: string
  remarks: string | null
}

export interface OvertimeRecord {
  overtimeID: number
  employeeID: number
  employeeName: string | null
  payMonth: number
  payYear: number
  otHours: number
  otRateMultiplier: number
  otAmount: number
  notes: string | null
  createdDate: string
}

export interface LoanType {
  loanTypeID: number
  typeName: string
  isActive: boolean
}

export interface EmployeeLoan {
  loanID: number
  employeeID: number
  employeeName: string | null
  loanTypeID: number
  loanTypeName: string | null
  loanAmount: number
  remainingBalance: number
  monthlyInstallment: number
  startMonth: number
  startYear: number
  status: string
  notes: string | null
  createdDate: string
}

export interface LeaveType {
  leaveTypeID: number
  typeName: string
  defaultDaysPerYear: number
  description: string | null
}

export interface LeaveBalance {
  balanceID: number
  employeeID: number
  employeeName: string | null
  leaveTypeID: number
  typeName: string | null
  year: number
  totalDays: number
  usedDays: number
  remainingDays: number
}

export interface LeaveRequest {
  requestID: number
  employeeID: number
  employeeName: string | null
  leaveTypeID: number
  typeName: string | null
  startDate: string
  endDate: string
  totalDays: number
  reason: string | null
  status: string
  approvedBy: string | null
  approvedDate: string | null
  remarks: string | null
  createdDate: string
}

export interface User {
  userID: number
  username: string
  fullName: string
  role: string
  isActive: boolean
  createdDate: string
}

export interface DashboardSummary {
  totalEmployees: number
  activeEmployees: number
  monthlyPayroll: number
  pendingLeave: number
  activeLoans: number
  totalLoanBalance: number
  payrollMonth: number
  payrollYear: number
  byDepartment: { department: string; headCount: number; totalSalary: number }[]
  recentActivity: { description: string; date: string }[]
}

export interface AuthUser {
  token: string
  username: string
  fullName: string
  role: string
  expiresAt: string
}
