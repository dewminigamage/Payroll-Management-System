import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Save, Calendar } from 'lucide-react'
import api from '../api/client'
import { useAuth } from '../context/AuthContext'
import { PageHeader, PeriodFilter, FormField, Select, StatusBadge, TableSkeleton, EmptyState, Modal } from '../components/ui'
import type { AttendanceRecord, Employee } from '../types'

const STATUSES = ['Present','Absent','Late','Half Day','Leave']

export default function Attendance() {
  const { isAdmin } = useAuth()
  const qc = useQueryClient()
  const now = new Date()
  const [month, setMonth] = useState(now.getMonth() + 1)
  const [year,  setYear]  = useState(now.getFullYear())
  const [empId, setEmpId] = useState<number | ''>('')
  const [showMark, setShowMark] = useState(false)
  const [markForm, setMarkForm] = useState({
    employeeID: 0, attendanceDate: now.toISOString().slice(0, 10), status: 'Present', remarks: ''
  })

  const { data: employees = [] } = useQuery<Employee[]>({
    queryKey: ['employees-active'],
    queryFn: () => api.get('/employees', { params: { activeOnly: true } }).then(r => r.data),
  })
  const { data = [], isLoading } = useQuery<AttendanceRecord[]>({
    queryKey: ['attendance', month, year, empId],
    queryFn: () => api.get('/attendance', { params: { month, year, employeeId: empId || undefined } }).then(r => r.data),
  })

  const saveMut = useMutation({
    mutationFn: () => api.post('/attendance', {
      employeeID: markForm.employeeID, attendanceDate: markForm.attendanceDate,
      status: markForm.status, remarks: markForm.remarks || null,
    }),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['attendance'] }); setShowMark(false) },
  })

  return (
    <div>
      <PageHeader
        title="Attendance"
        subtitle={`${data.length} records for selected period`}
        action={isAdmin && (
          <button onClick={() => setShowMark(true)} className="btn-primary">
            <Save size={15} /> Mark Attendance
          </button>
        )}
      />

      <div className="page-body space-y-5">
        <div className="card overflow-hidden">
          <div className="px-5 py-4 border-b border-slate-100">
            <PeriodFilter month={month} year={year} onMonth={setMonth} onYear={setYear}
              extra={
                <Select value={empId} onChange={e => setEmpId(e.target.value ? +e.target.value : '')} className="w-48">
                  <option value="">All employees</option>
                  {employees.map(e => <option key={e.employeeID} value={e.employeeID}>{e.fullName}</option>)}
                </Select>
              }
            />
          </div>

          {isLoading ? <TableSkeleton cols={4} rows={8} /> : data.length === 0 ? (
            <EmptyState icon={Calendar} title="No attendance records" subtitle="Select a period and employee to view records" />
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr>
                    {['Employee','Date','Status','Remarks'].map(h => <th key={h} className="table-th">{h}</th>)}
                  </tr>
                </thead>
                <tbody>
                  {data.map(r => (
                    <tr key={r.attendanceID} className="table-row">
                      <td className="table-td font-medium text-slate-800">{r.employeeName}</td>
                      <td className="table-td text-slate-500 tabular-nums">{r.attendanceDate}</td>
                      <td className="table-td"><StatusBadge status={r.status} /></td>
                      <td className="table-td text-slate-500 italic text-xs">{r.remarks ?? '—'}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>

      {showMark && (
        <Modal title="Mark Attendance" onClose={() => setShowMark(false)} width="max-w-md">
          <div className="space-y-4">
            <FormField label="Employee">
              <Select value={markForm.employeeID} onChange={e => setMarkForm(f => ({ ...f, employeeID: +e.target.value }))}>
                <option value={0}>Select employee…</option>
                {employees.map(e => <option key={e.employeeID} value={e.employeeID}>{e.fullName}</option>)}
              </Select>
            </FormField>
            <div className="grid grid-cols-2 gap-4">
              <FormField label="Date">
                <input type="date" value={markForm.attendanceDate}
                  onChange={e => setMarkForm(f => ({ ...f, attendanceDate: e.target.value }))}
                  className="input" />
              </FormField>
              <FormField label="Status">
                <Select value={markForm.status} onChange={e => setMarkForm(f => ({ ...f, status: e.target.value }))}>
                  {STATUSES.map(s => <option key={s}>{s}</option>)}
                </Select>
              </FormField>
            </div>
            <FormField label="Remarks (optional)">
              <input value={markForm.remarks} onChange={e => setMarkForm(f => ({ ...f, remarks: e.target.value }))}
                className="input" placeholder="Optional note…" />
            </FormField>
          </div>
          <div className="flex gap-3 mt-6 justify-end border-t border-slate-100 pt-5">
            <button onClick={() => setShowMark(false)} className="btn-secondary">Cancel</button>
            <button onClick={() => saveMut.mutate()} disabled={!markForm.employeeID || saveMut.isPending} className="btn-primary">
              {saveMut.isPending ? 'Saving…' : 'Save Record'}
            </button>
          </div>
        </Modal>
      )}
    </div>
  )
}
