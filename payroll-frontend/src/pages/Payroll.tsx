import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Play, Trash2, DollarSign } from 'lucide-react'
import api from '../api/client'
import { useAuth } from '../context/AuthContext'
import { PageHeader, PeriodFilter, TableSkeleton, EmptyState } from '../components/ui'
import type { PayrollRecord } from '../types'

export default function Payroll() {
  const { isAdmin } = useAuth()
  const qc = useQueryClient()
  const now = new Date()
  const [month, setMonth] = useState(now.getMonth() + 1)
  const [year,  setYear]  = useState(now.getFullYear())
  const [msg,   setMsg]   = useState('')

  const { data = [], isLoading } = useQuery<PayrollRecord[]>({
    queryKey: ['payroll', month, year],
    queryFn: () => api.get('/payroll', { params: { month, year } }).then(r => r.data),
  })

  const bulkMut = useMutation({
    mutationFn: () => api.post('/payroll/bulk', { payMonth: month, payYear: year }),
    onSuccess: (res) => { qc.invalidateQueries({ queryKey: ['payroll'] }); setMsg(res.data.message) },
  })

  const deleteMut = useMutation({
    mutationFn: (id: number) => api.delete(`/payroll/${id}`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['payroll'] }),
  })

  const totals = {
    gross: data.reduce((s, r) => s + r.grossSalary, 0),
    epf:   data.reduce((s, r) => s + r.epf, 0),
    net:   data.reduce((s, r) => s + r.netSalary, 0),
  }

  return (
    <div>
      <PageHeader
        title="Payroll"
        subtitle={data.length > 0 ? `${data.length} records · Net LKR ${totals.net.toLocaleString()}` : 'Manage monthly payroll records'}
        action={isAdmin && (
          <button onClick={() => bulkMut.mutate()} disabled={bulkMut.isPending} className="btn-primary">
            <Play size={15} /> {bulkMut.isPending ? 'Processing…' : 'Run Bulk Payroll'}
          </button>
        )}
      />

      <div className="page-body space-y-5">
        {msg && (
          <div className="bg-emerald-50 text-emerald-700 text-sm rounded-xl p-4 border border-emerald-200 flex items-center gap-2">
            <span className="w-2 h-2 bg-emerald-500 rounded-full" />{msg}
          </div>
        )}

        {/* Summary cards */}
        {data.length > 0 && (
          <div className="grid grid-cols-3 gap-4">
            {[
              { label: 'Total Gross',  value: totals.gross, color: 'text-slate-700' },
              { label: 'Total EPF',    value: totals.epf,   color: 'text-red-600' },
              { label: 'Total Net',    value: totals.net,   color: 'text-emerald-700' },
            ].map(s => (
              <div key={s.label} className="card px-5 py-4">
                <p className="text-xs text-slate-500 font-semibold uppercase tracking-wide">{s.label}</p>
                <p className={`text-xl font-bold mt-1 ${s.color}`}>LKR {s.value.toLocaleString()}</p>
              </div>
            ))}
          </div>
        )}

        <div className="card overflow-hidden">
          <div className="px-5 py-4 border-b border-slate-100">
            <PeriodFilter month={month} year={year} onMonth={setMonth} onYear={setYear} />
          </div>

          {isLoading ? <TableSkeleton cols={8} rows={6} /> : data.length === 0 ? (
            <EmptyState icon={DollarSign}
              title="No payroll records"
              subtitle={isAdmin ? "Click 'Run Bulk Payroll' to generate records for this period" : 'No payroll processed for this period'} />
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr>
                    {['Employee','Dept','Basic','Allowances','Gross','EPF (-)',  'ETF (-)','Net Pay',''].map(h => (
                      <th key={h} className="table-th">{h}</th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {data.map(r => (
                    <tr key={r.payrollID} className="table-row">
                      <td className="table-td font-medium text-slate-800">{r.employeeName}</td>
                      <td className="table-td">
                        {r.department ? <span className="bg-slate-100 text-slate-600 text-xs px-2 py-0.5 rounded-md font-medium">{r.department}</span> : '—'}
                      </td>
                      <td className="table-td tabular-nums">{r.basicSalary.toLocaleString()}</td>
                      <td className="table-td tabular-nums text-blue-600">{r.allowances > 0 ? `+${r.allowances.toLocaleString()}` : '—'}</td>
                      <td className="table-td tabular-nums font-medium">{r.grossSalary.toLocaleString()}</td>
                      <td className="table-td tabular-nums text-red-500">{r.epf.toLocaleString()}</td>
                      <td className="table-td tabular-nums text-red-400 text-xs">{r.etf.toLocaleString()}</td>
                      <td className="table-td">
                        <span className="font-bold text-emerald-700 tabular-nums">LKR {r.netSalary.toLocaleString()}</span>
                      </td>
                      <td className="table-td">
                        {isAdmin && (
                          <button onClick={() => { if (confirm('Delete this payroll record?')) deleteMut.mutate(r.payrollID) }}
                            className="btn-ghost p-1.5 text-red-400 hover:text-red-600 hover:bg-red-50">
                            <Trash2 size={14} />
                          </button>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
                <tfoot>
                  <tr className="bg-slate-50 border-t-2 border-slate-200">
                    <td className="table-td font-bold text-slate-700" colSpan={2}>
                      Totals — {data.length} employees
                    </td>
                    <td className="table-td tabular-nums font-semibold text-slate-600">{data.reduce((s,r) => s+r.basicSalary,0).toLocaleString()}</td>
                    <td className="table-td" />
                    <td className="table-td tabular-nums font-semibold">{totals.gross.toLocaleString()}</td>
                    <td className="table-td tabular-nums font-semibold text-red-500">{totals.epf.toLocaleString()}</td>
                    <td className="table-td" />
                    <td className="table-td tabular-nums font-bold text-emerald-700">LKR {totals.net.toLocaleString()}</td>
                    <td />
                  </tr>
                </tfoot>
              </table>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
