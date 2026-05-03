import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Plus, Clock } from 'lucide-react'
import api from '../api/client'
import { useAuth } from '../context/AuthContext'
import { PageHeader, PeriodFilter, FormField, Select, TableSkeleton, EmptyState, Modal } from '../components/ui'
import type { OvertimeRecord, Employee } from '../types'

export default function Overtime() {
  const { isAdmin } = useAuth()
  const qc = useQueryClient()
  const now = new Date()
  const [month, setMonth] = useState(now.getMonth() + 1)
  const [year,  setYear]  = useState(now.getFullYear())
  const [showForm, setShowForm] = useState(false)
  const [form, setForm] = useState({ employeeID: 0, otHours: 0, otRateMultiplier: 1.5, notes: '' })
  const [basicSalary, setBasicSalary] = useState(0)

  const { data: employees = [] } = useQuery<Employee[]>({
    queryKey: ['employees-active'],
    queryFn: () => api.get('/employees', { params: { activeOnly: true } }).then(r => r.data),
  })
  const { data = [], isLoading } = useQuery<OvertimeRecord[]>({
    queryKey: ['overtime', month, year],
    queryFn: () => api.get('/overtime', { params: { month, year } }).then(r => r.data),
  })

  const saveMut = useMutation({
    mutationFn: () => {
      const otAmount = Math.round((basicSalary / 160) * form.otHours * form.otRateMultiplier * 100) / 100
      return api.post('/overtime', { ...form, payMonth: month, payYear: year, otAmount, notes: form.notes || null })
    },
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['overtime'] }); setShowForm(false) },
  })

  const selectEmp = (id: number) => {
    setBasicSalary(employees.find(e => e.employeeID === id)?.basicSalary ?? 0)
    setForm(f => ({ ...f, employeeID: id }))
  }

  const calcOT = () => Math.round((basicSalary / 160) * form.otHours * form.otRateMultiplier * 100) / 100

  return (
    <div>
      <PageHeader
        title="Overtime"
        subtitle={`${data.length} records · Total LKR ${data.reduce((s, r) => s + r.otAmount, 0).toLocaleString()}`}
        action={isAdmin && (
          <button onClick={() => setShowForm(true)} className="btn-primary">
            <Plus size={15} /> Add OT Record
          </button>
        )}
      />

      <div className="page-body space-y-5">
        <div className="card overflow-hidden">
          <div className="px-5 py-4 border-b border-slate-100">
            <PeriodFilter month={month} year={year} onMonth={setMonth} onYear={setYear} />
          </div>

          {isLoading ? <TableSkeleton cols={5} rows={5} /> : data.length === 0 ? (
            <EmptyState icon={Clock} title="No overtime records" subtitle="Add OT records for this period" />
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr>{['Employee','OT Hours','Rate','OT Amount (LKR)','Notes'].map(h =>
                    <th key={h} className="table-th">{h}</th>)}
                  </tr>
                </thead>
                <tbody>
                  {data.map(r => (
                    <tr key={r.overtimeID} className="table-row">
                      <td className="table-td font-medium text-slate-800">{r.employeeName}</td>
                      <td className="table-td">
                        <span className="bg-blue-50 text-blue-700 text-xs px-2.5 py-1 rounded-lg font-semibold">{r.otHours} hrs</span>
                      </td>
                      <td className="table-td text-slate-500">{r.otRateMultiplier}×</td>
                      <td className="table-td font-bold text-blue-700 tabular-nums">LKR {r.otAmount.toLocaleString()}</td>
                      <td className="table-td text-slate-500 text-xs italic">{r.notes ?? '—'}</td>
                    </tr>
                  ))}
                </tbody>
                <tfoot>
                  <tr className="bg-slate-50 border-t-2 border-slate-200">
                    <td className="table-td font-bold" colSpan={3}>Total OT</td>
                    <td className="table-td font-bold text-blue-700 tabular-nums">
                      LKR {data.reduce((s, r) => s + r.otAmount, 0).toLocaleString()}
                    </td>
                    <td />
                  </tr>
                </tfoot>
              </table>
            </div>
          )}
        </div>
      </div>

      {showForm && (
        <Modal title="Add / Update OT Record" onClose={() => setShowForm(false)} width="max-w-md">
          <div className="space-y-4">
            <FormField label="Employee">
              <Select value={form.employeeID} onChange={e => selectEmp(+e.target.value)}>
                <option value={0}>Select employee…</option>
                {employees.map(e => <option key={e.employeeID} value={e.employeeID}>{e.fullName}</option>)}
              </Select>
            </FormField>
            <div className="grid grid-cols-2 gap-4">
              <FormField label="OT Hours">
                <input type="number" min={0} step={0.5} value={form.otHours}
                  onChange={e => setForm(f => ({ ...f, otHours: +e.target.value }))}
                  className="input" />
              </FormField>
              <FormField label="Rate Multiplier">
                <input type="number" min={1} step={0.25} value={form.otRateMultiplier}
                  onChange={e => setForm(f => ({ ...f, otRateMultiplier: +e.target.value }))}
                  className="input" />
              </FormField>
            </div>
            {form.employeeID > 0 && form.otHours > 0 && (
              <div className="bg-blue-50 border border-blue-100 rounded-xl p-4">
                <p className="text-xs text-blue-600 font-semibold uppercase tracking-wide mb-1">Calculated OT Amount</p>
                <p className="text-xl font-bold text-blue-700">LKR {calcOT().toLocaleString()}</p>
                <p className="text-xs text-blue-500 mt-0.5">Based on LKR {basicSalary.toLocaleString()} / 160 hrs × {form.otHours} hrs × {form.otRateMultiplier}×</p>
              </div>
            )}
            <FormField label="Notes (optional)">
              <input value={form.notes} onChange={e => setForm(f => ({ ...f, notes: e.target.value }))}
                className="input" placeholder="Optional note…" />
            </FormField>
          </div>
          <div className="flex gap-3 mt-6 justify-end border-t border-slate-100 pt-5">
            <button onClick={() => setShowForm(false)} className="btn-secondary">Cancel</button>
            <button onClick={() => saveMut.mutate()} disabled={!form.employeeID || saveMut.isPending} className="btn-primary">
              {saveMut.isPending ? 'Saving…' : 'Save Record'}
            </button>
          </div>
        </Modal>
      )}
    </div>
  )
}
