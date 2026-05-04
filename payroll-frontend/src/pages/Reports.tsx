import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { BarChart2 } from 'lucide-react'
import api from '../api/client'
import { PageHeader, PeriodFilter, TabBar, TableSkeleton, EmptyState } from '../components/ui'

type Tab = 'payroll' | 'department' | 'attendance' | 'overtime'

export default function Reports() {
  const now = new Date()
  const [tab,   setTab]   = useState<Tab>('payroll')
  const [month, setMonth] = useState(now.getMonth() + 1)
  const [year,  setYear]  = useState(now.getFullYear())

  const tabs: { key: Tab; label: string }[] = [
    { key: 'payroll',    label: 'Payroll Summary' },
    { key: 'department', label: 'By Department' },
    { key: 'attendance', label: 'Attendance' },
    { key: 'overtime',   label: 'Overtime' },
  ]

  const { data: plData, isLoading: plLoading } = useQuery({
    queryKey: ['report-payroll', month, year],
    queryFn: () => api.get('/reports/payroll-summary', { params: { month, year } }).then(r => r.data),
    enabled: tab === 'payroll',
  })
  const { data: deptData, isLoading: deptLoading } = useQuery({
    queryKey: ['report-dept', month, year],
    queryFn: () => api.get('/reports/department-summary', { params: { month, year } }).then(r => r.data),
    enabled: tab === 'department',
  })
  const { data: attData, isLoading: attLoading } = useQuery({
    queryKey: ['report-att', month, year],
    queryFn: () => api.get('/reports/attendance-summary', { params: { month, year } }).then(r => r.data),
    enabled: tab === 'attendance',
  })
  const { data: otData, isLoading: otLoading } = useQuery({
    queryKey: ['report-ot', month, year],
    queryFn: () => api.get('/reports/overtime-summary', { params: { month, year } }).then(r => r.data),
    enabled: tab === 'overtime',
  })

  return (
    <div>
      <PageHeader title="Reports" subtitle="Payroll, attendance, and overtime summaries" />

      <div className="page-body space-y-5">
        <div className="card p-5">
          <PeriodFilter month={month} year={year} onMonth={setMonth} onYear={setYear} />
        </div>

        <div className="card overflow-hidden">
          <div className="px-5 pt-4 pb-0">
            <TabBar tabs={tabs} active={tab} onChange={setTab} />
          </div>

          <div className="p-5">
            {/* Payroll */}
            {tab === 'payroll' && (
              plLoading ? <TableSkeleton cols={7} /> :
              !plData?.records?.length ? <EmptyState icon={BarChart2} title="No payroll data" subtitle="Run bulk payroll to generate data" /> : (
                <>
                  <div className="grid grid-cols-3 gap-4 mb-5">
                    {[
                      { label: 'Total Gross', v: plData.totals.grossSalary, cls: 'text-slate-800' },
                      { label: 'Total EPF',   v: plData.totals.epf,         cls: 'text-red-600' },
                      { label: 'Total Net',   v: plData.totals.netSalary,   cls: 'text-emerald-700' },
                    ].map(s => (
                      <div key={s.label} className="bg-slate-50 rounded-xl p-4 border border-slate-100">
                        <p className="text-xs font-semibold text-slate-500 uppercase tracking-wide">{s.label}</p>
                        <p className={`text-2xl font-bold mt-1 ${s.cls}`}>LKR {(s.v as number).toLocaleString()}</p>
                      </div>
                    ))}
                  </div>
                  <div className="overflow-x-auto">
                    <table className="w-full">
                      <thead><tr>
                        {['Employee','Dept','Basic','Allowances','Gross','EPF','Net'].map(h => <th key={h} className="table-th">{h}</th>)}
                      </tr></thead>
                      <tbody>
                        {plData.records.map((r: Record<string,unknown>, i: number) => (
                          <tr key={i} className="table-row">
                            <td className="table-td font-medium">{r.employeeName as string}</td>
                            <td className="table-td"><span className="bg-slate-100 text-slate-600 text-xs px-2 py-0.5 rounded-md">{(r.department as string) ?? '—'}</span></td>
                            <td className="table-td tabular-nums">{(r.basicSalary as number).toLocaleString()}</td>
                            <td className="table-td tabular-nums text-blue-600">{(r.allowances as number) > 0 ? `+${(r.allowances as number).toLocaleString()}` : '—'}</td>
                            <td className="table-td tabular-nums">{(r.grossSalary as number).toLocaleString()}</td>
                            <td className="table-td tabular-nums text-red-500">{(r.epf as number).toLocaleString()}</td>
                            <td className="table-td font-bold text-emerald-700 tabular-nums">LKR {(r.netSalary as number).toLocaleString()}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </>
              )
            )}

            {/* Department */}
            {tab === 'department' && (
              deptLoading ? <TableSkeleton cols={6} /> :
              !deptData?.length ? <EmptyState icon={BarChart2} title="No department data" subtitle="Process payroll to see department summaries" /> : (
                <table className="w-full">
                  <thead><tr>
                    {['Department','Head Count','Total Basic','Total Gross','Total EPF','Total Net'].map(h => <th key={h} className="table-th">{h}</th>)}
                  </tr></thead>
                  <tbody>
                    {(deptData ?? []).map((r: Record<string,unknown>, i: number) => (
                      <tr key={i} className="table-row">
                        <td className="table-td font-medium">{r.department as string}</td>
                        <td className="table-td font-semibold">{r.headCount as number}</td>
                        <td className="table-td tabular-nums">{(r.totalBasic as number).toLocaleString()}</td>
                        <td className="table-td tabular-nums">{(r.totalGross as number).toLocaleString()}</td>
                        <td className="table-td tabular-nums text-red-500">{(r.totalEPF as number).toLocaleString()}</td>
                        <td className="table-td font-bold text-emerald-700 tabular-nums">LKR {(r.totalNet as number).toLocaleString()}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )
            )}

            {/* Attendance */}
            {tab === 'attendance' && (
              attLoading ? <TableSkeleton cols={7} /> :
              !attData?.length ? <EmptyState icon={BarChart2} title="No attendance data" subtitle="Mark attendance to see summaries" /> : (
                <table className="w-full">
                  <thead><tr>
                    {['Employee','Dept','Present','Absent','Late','Half Day','On Leave'].map(h => <th key={h} className="table-th">{h}</th>)}
                  </tr></thead>
                  <tbody>
                    {(attData ?? []).map((r: Record<string,unknown>, i: number) => (
                      <tr key={i} className="table-row">
                        <td className="table-td font-medium">{r.employeeName as string}</td>
                        <td className="table-td text-slate-500">{(r.department as string) ?? '—'}</td>
                        <td className="table-td text-emerald-700 font-semibold">{r.present as number}</td>
                        <td className="table-td text-red-500 font-semibold">{r.absent as number}</td>
                        <td className="table-td text-amber-600">{r.late as number}</td>
                        <td className="table-td text-orange-500">{r.halfDay as number}</td>
                        <td className="table-td text-blue-500">{r.onLeave as number}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )
            )}

            {/* Overtime */}
            {tab === 'overtime' && (
              otLoading ? <TableSkeleton cols={5} /> :
              !otData?.length ? <EmptyState icon={BarChart2} title="No overtime data" subtitle="No OT records for this period" /> : (
                <table className="w-full">
                  <thead><tr>
                    {['Employee','Department','OT Hours','Rate','OT Amount (LKR)'].map(h => <th key={h} className="table-th">{h}</th>)}
                  </tr></thead>
                  <tbody>
                    {(otData ?? []).map((r: Record<string,unknown>, i: number) => (
                      <tr key={i} className="table-row">
                        <td className="table-td font-medium">{r.employeeName as string}</td>
                        <td className="table-td text-slate-500">{(r.department as string) ?? '—'}</td>
                        <td className="table-td"><span className="bg-blue-50 text-blue-700 text-xs px-2.5 py-1 rounded-lg font-semibold">{r.otHours as number} hrs</span></td>
                        <td className="table-td text-slate-500">{r.otRateMultiplier as number}×</td>
                        <td className="table-td font-bold text-blue-700 tabular-nums">LKR {(r.otAmount as number).toLocaleString()}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
