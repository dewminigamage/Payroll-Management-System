import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Plus, CheckCircle, XCircle, Umbrella } from 'lucide-react'
import api from '../api/client'
import { useAuth } from '../context/AuthContext'
import { PageHeader, FormField, Select, Textarea, StatusBadge, TableSkeleton, EmptyState, FilterChips, Modal } from '../components/ui'
import type { LeaveRequest, LeaveType, Employee } from '../types'

type StatusFilter = 'Pending' | 'Approved' | 'Rejected' | ''

export default function Leave() {
  const { isAdmin } = useAuth()
  const qc = useQueryClient()
  const [filter,   setFilter]   = useState<StatusFilter>('Pending')
  const [showForm, setShowForm] = useState(false)
  const [form, setForm] = useState({
    employeeID: 0, leaveTypeID: 0, startDate: '', endDate: '', totalDays: 1, reason: '',
  })

  const { data: employees = [] } = useQuery<Employee[]>({
    queryKey: ['employees-active'],
    queryFn: () => api.get('/employees', { params: { activeOnly: true } }).then(r => r.data),
  })
  const { data: leaveTypes = [] } = useQuery<LeaveType[]>({
    queryKey: ['leave-types'],
    queryFn: () => api.get('/leave/types').then(r => r.data),
  })
  const { data = [], isLoading } = useQuery<LeaveRequest[]>({
    queryKey: ['leave-requests', filter],
    queryFn: () => api.get('/leave/requests', { params: { status: filter || undefined } }).then(r => r.data),
  })

  const createMut = useMutation({
    mutationFn: () => api.post('/leave/requests', form),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['leave-requests'] }); setShowForm(false) },
  })

  const approveMut = useMutation({
    mutationFn: ({ id, status }: { id: number; status: string }) =>
      api.put(`/leave/requests/${id}/approve`, { status }),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['leave-requests'] }),
  })

  return (
    <div>
      <PageHeader
        title="Leave Management"
        subtitle={`${data.length} ${filter || 'total'} requests`}
        action={
          <button onClick={() => setShowForm(true)} className="btn-primary">
            <Plus size={15} /> New Request
          </button>
        }
      />

      <div className="page-body space-y-5">
        <FilterChips
          value={filter}
          onChange={(v) => setFilter(v as StatusFilter)}
          options={[
            { label: 'Pending',  value: 'Pending' },
            { label: 'Approved', value: 'Approved' },
            { label: 'Rejected', value: 'Rejected' },
            { label: 'All',      value: '' },
          ]}
        />

        <div className="card overflow-hidden">
          {isLoading ? <TableSkeleton cols={7} rows={5} /> : data.length === 0 ? (
            <EmptyState icon={Umbrella}
              title={filter === 'Pending' ? 'No pending leave requests' : 'No leave requests found'}
              subtitle="All requests are up to date" />
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr>
                    {['Employee','Leave Type','Start','End','Days','Reason','Status', isAdmin ? 'Actions' : ''].map(h =>
                      <th key={h} className="table-th">{h}</th>)}
                  </tr>
                </thead>
                <tbody>
                  {data.map(r => (
                    <tr key={r.requestID} className="table-row">
                      <td className="table-td font-medium text-slate-800">{r.employeeName}</td>
                      <td className="table-td">
                        <span className="bg-blue-50 text-blue-700 text-xs px-2 py-0.5 rounded-md font-medium">{r.typeName}</span>
                      </td>
                      <td className="table-td text-slate-500 tabular-nums text-xs">{r.startDate}</td>
                      <td className="table-td text-slate-500 tabular-nums text-xs">{r.endDate}</td>
                      <td className="table-td">
                        <span className="font-semibold text-slate-700">{r.totalDays}d</span>
                      </td>
                      <td className="table-td text-slate-500 italic text-xs max-w-36 truncate">{r.reason ?? '—'}</td>
                      <td className="table-td"><StatusBadge status={r.status} /></td>
                      <td className="table-td">
                        {isAdmin && r.status === 'Pending' && (
                          <div className="flex items-center gap-1">
                            <button onClick={() => approveMut.mutate({ id: r.requestID, status: 'Approved' })}
                              className="btn-ghost p-1.5 text-emerald-500 hover:text-emerald-700 hover:bg-emerald-50"
                              title="Approve">
                              <CheckCircle size={16} />
                            </button>
                            <button onClick={() => approveMut.mutate({ id: r.requestID, status: 'Rejected' })}
                              className="btn-ghost p-1.5 text-red-400 hover:text-red-600 hover:bg-red-50"
                              title="Reject">
                              <XCircle size={16} />
                            </button>
                          </div>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>

      {showForm && (
        <Modal title="Submit Leave Request" onClose={() => setShowForm(false)} width="max-w-md">
          <div className="space-y-4">
            <FormField label="Employee">
              <Select value={form.employeeID} onChange={e => setForm(f => ({ ...f, employeeID: +e.target.value }))}>
                <option value={0}>Select employee…</option>
                {employees.map(e => <option key={e.employeeID} value={e.employeeID}>{e.fullName}</option>)}
              </Select>
            </FormField>
            <FormField label="Leave Type">
              <Select value={form.leaveTypeID} onChange={e => setForm(f => ({ ...f, leaveTypeID: +e.target.value }))}>
                <option value={0}>Select type…</option>
                {leaveTypes.map(t => <option key={t.leaveTypeID} value={t.leaveTypeID}>{t.typeName} ({t.defaultDaysPerYear} days/yr)</option>)}
              </Select>
            </FormField>
            <div className="grid grid-cols-2 gap-4">
              <FormField label="Start Date">
                <input type="date" value={form.startDate} onChange={e => setForm(f => ({ ...f, startDate: e.target.value }))} className="input" />
              </FormField>
              <FormField label="End Date">
                <input type="date" value={form.endDate} onChange={e => setForm(f => ({ ...f, endDate: e.target.value }))} className="input" />
              </FormField>
            </div>
            <FormField label="Total Days">
              <input type="number" min={1} value={form.totalDays} onChange={e => setForm(f => ({ ...f, totalDays: +e.target.value }))} className="input" />
            </FormField>
            <FormField label="Reason">
              <Textarea value={form.reason} onChange={e => setForm(f => ({ ...f, reason: e.target.value }))}
                rows={3} placeholder="Brief reason for leave…" />
            </FormField>
          </div>
          <div className="flex gap-3 mt-6 justify-end border-t border-slate-100 pt-5">
            <button onClick={() => setShowForm(false)} className="btn-secondary">Cancel</button>
            <button onClick={() => createMut.mutate()}
              disabled={!form.employeeID || !form.leaveTypeID || !form.startDate || createMut.isPending}
              className="btn-primary">
              {createMut.isPending ? 'Submitting…' : 'Submit Request'}
            </button>
          </div>
        </Modal>
      )}
    </div>
  )
}
