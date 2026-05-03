import { useQuery } from '@tanstack/react-query'
import { Users, DollarSign, Clock, CreditCard, TrendingUp, ArrowUpRight } from 'lucide-react'
import api from '../api/client'
import { StatCard, TableSkeleton } from '../components/ui'
import type { DashboardSummary } from '../types'

const MONTHS = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec']

export default function Dashboard() {
  const { data, isLoading } = useQuery<DashboardSummary>({
    queryKey: ['dashboard'],
    queryFn: () => api.get('/dashboard').then(r => r.data),
  })

  const monthLabel = data ? `${MONTHS[(data.payrollMonth ?? 1) - 1]} ${data.payrollYear}` : '…'

  return (
    <div className="flex-1">
      {/* Header */}
      <div className="page-header">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-xl font-semibold text-slate-800">Dashboard</h1>
            <p className="text-sm text-slate-500 mt-0.5">Overview for {monthLabel}</p>
          </div>
          <div className="flex items-center gap-2 bg-emerald-50 text-emerald-700 text-xs font-medium px-3 py-1.5 rounded-full ring-1 ring-emerald-200">
            <span className="w-1.5 h-1.5 bg-emerald-500 rounded-full animate-pulse" />
            System Online
          </div>
        </div>
      </div>

      <div className="page-body space-y-6">
        {/* Stats */}
        {isLoading ? (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-5">
            {Array.from({ length: 4 }).map((_, i) => (
              <div key={i} className="card p-6 flex items-start gap-4">
                <div className="w-12 h-12 bg-slate-200 animate-pulse rounded-2xl" />
                <div className="flex-1 space-y-2">
                  <div className="h-3 bg-slate-200 animate-pulse rounded w-20" />
                  <div className="h-7 bg-slate-200 animate-pulse rounded w-28" />
                </div>
              </div>
            ))}
          </div>
        ) : data && (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-5">
            <StatCard
              label="Active Employees"
              value={data.activeEmployees}
              sub={`${data.totalEmployees} total · All departments`}
              icon={Users}
              gradient="bg-gradient-to-br from-blue-500 to-blue-600"
            />
            <StatCard
              label="Net Payroll"
              value={data.monthlyPayroll > 0 ? `LKR ${(data.monthlyPayroll / 1000).toFixed(0)}K` : 'Not processed'}
              sub={data.monthlyPayroll > 0 ? `${monthLabel}` : 'Run bulk payroll to generate'}
              icon={DollarSign}
              gradient="bg-gradient-to-br from-emerald-500 to-emerald-600"
            />
            <StatCard
              label="Pending Leave"
              value={data.pendingLeave}
              sub="Awaiting approval"
              icon={Clock}
              gradient="bg-gradient-to-br from-amber-500 to-orange-500"
            />
            <StatCard
              label="Active Loans"
              value={data.activeLoans}
              sub={`LKR ${(data.totalLoanBalance / 1000).toFixed(0)}K outstanding`}
              icon={CreditCard}
              gradient="bg-gradient-to-br from-violet-500 to-purple-600"
            />
          </div>
        )}

        {/* Bottom row */}
        <div className="grid grid-cols-1 lg:grid-cols-5 gap-5">
          {/* Department table */}
          <div className="card lg:col-span-3 overflow-hidden">
            <div className="px-5 py-4 border-b border-slate-100 flex items-center justify-between">
              <div>
                <h2 className="text-sm font-semibold text-slate-700">Workforce by Department</h2>
                <p className="text-xs text-slate-400 mt-0.5">Active employee distribution</p>
              </div>
              <TrendingUp size={16} className="text-slate-300" />
            </div>
            {isLoading ? <TableSkeleton cols={3} rows={5} /> : (
              data && data.byDepartment.length === 0 ? (
                <p className="p-8 text-sm text-slate-400 text-center">No department data</p>
              ) : (
                <table className="w-full">
                  <thead>
                    <tr>
                      <th className="table-th">Department</th>
                      <th className="table-th text-right">Staff</th>
                      <th className="table-th text-right">Total Salary (LKR)</th>
                    </tr>
                  </thead>
                  <tbody>
                    {data?.byDepartment.map((d, i) => {
                      const maxSalary = Math.max(...(data.byDepartment.map(x => x.totalSalary)))
                      const pct = maxSalary > 0 ? (d.totalSalary / maxSalary) * 100 : 0
                      return (
                        <tr key={d.department} className={`table-row ${i % 2 === 0 ? '' : 'bg-slate-50/50'}`}>
                          <td className="table-td font-medium">
                            <div className="flex items-center gap-2.5">
                              <div className="w-2 h-2 rounded-full bg-blue-500 opacity-70" />
                              {d.department}
                            </div>
                          </td>
                          <td className="table-td text-right font-semibold text-slate-600">{d.headCount}</td>
                          <td className="table-td text-right">
                            <div className="flex items-center justify-end gap-2">
                              <div className="w-16 h-1.5 bg-slate-100 rounded-full overflow-hidden">
                                <div className="h-full bg-blue-400 rounded-full" style={{ width: `${pct}%` }} />
                              </div>
                              <span className="text-slate-700 tabular-nums">{d.totalSalary.toLocaleString()}</span>
                            </div>
                          </td>
                        </tr>
                      )
                    })}
                  </tbody>
                </table>
              )
            )}
          </div>

          {/* Recent activity */}
          <div className="card lg:col-span-2 overflow-hidden">
            <div className="px-5 py-4 border-b border-slate-100 flex items-center justify-between">
              <div>
                <h2 className="text-sm font-semibold text-slate-700">Recent Activity</h2>
                <p className="text-xs text-slate-400 mt-0.5">Latest payroll events</p>
              </div>
              <ArrowUpRight size={16} className="text-slate-300" />
            </div>
            <div className="divide-y divide-slate-100">
              {isLoading ? (
                <div className="p-5 space-y-3">
                  {Array.from({ length: 5 }).map((_, i) => (
                    <div key={i} className="h-4 bg-slate-200 animate-pulse rounded" style={{ width: `${70 + Math.random() * 25}%` }} />
                  ))}
                </div>
              ) : data?.recentActivity.length === 0 ? (
                <p className="p-8 text-sm text-slate-400 text-center">No recent activity</p>
              ) : (
                data?.recentActivity.slice(0, 6).map((a, i) => (
                  <div key={i} className="flex items-start gap-3 px-5 py-3.5 hover:bg-slate-50 transition-colors">
                    <div className="w-7 h-7 bg-blue-50 rounded-full flex items-center justify-center flex-shrink-0 mt-0.5">
                      <DollarSign size={12} className="text-blue-500" />
                    </div>
                    <div>
                      <p className="text-xs font-medium text-slate-700">{a.description}</p>
                      <p className="text-xs text-slate-400 mt-0.5">{new Date(a.date).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' })}</p>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
