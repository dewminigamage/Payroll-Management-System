import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Plus, Pencil, UserX, UserCheck, Search, Users } from 'lucide-react'
import api from '../api/client'
import { useAuth } from '../context/AuthContext'
import { PageHeader, Modal, FormField, Input, Select, StatusBadge, TableSkeleton, EmptyState } from '../components/ui'
import type { Employee } from '../types'

const empty = {
  fullName: '', nic: '', department: '', position: '',
  basicSalary: 0, joinDate: '', contactNumber: '', email: '', isActive: true,
}

export default function Employees() {
  const { isAdmin } = useAuth()
  const qc = useQueryClient()
  const [search,   setSearch]   = useState('')
  const [showForm, setShowForm] = useState(false)
  const [editing,  setEditing]  = useState<Employee | null>(null)
  const [form,     setForm]     = useState({ ...empty })

  const { data = [], isLoading } = useQuery<Employee[]>({
    queryKey: ['employees'],
    queryFn: () => api.get('/employees').then(r => r.data),
  })

  const saveMut = useMutation({
    mutationFn: (e: typeof form & { id?: number }) =>
      e.id ? api.put(`/employees/${e.id}`, e) : api.post('/employees', e),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['employees'] }); closeForm() },
  })

  const toggleMut = useMutation({
    mutationFn: (e: Employee) =>
      api.put(`/employees/${e.employeeID}`, { ...e, isActive: !e.isActive, joinDate: e.joinDate.slice(0, 10) }),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['employees'] }),
  })

  const openCreate = () => { setForm({ ...empty }); setEditing(null); setShowForm(true) }
  const openEdit   = (e: Employee) => {
    setForm({ fullName: e.fullName, nic: e.nic, department: e.department ?? '',
      position: e.position ?? '', basicSalary: e.basicSalary, joinDate: e.joinDate.slice(0, 10),
      contactNumber: e.contactNumber ?? '', email: e.email ?? '', isActive: e.isActive })
    setEditing(e); setShowForm(true)
  }
  const closeForm = () => { setShowForm(false); setEditing(null) }

  const filtered = data.filter(e =>
    e.fullName.toLowerCase().includes(search.toLowerCase()) ||
    (e.department ?? '').toLowerCase().includes(search.toLowerCase()) ||
    (e.position   ?? '').toLowerCase().includes(search.toLowerCase())
  )

  return (
    <div>
      <PageHeader
        title="Employees"
        subtitle={`${data.filter(e => e.isActive).length} active of ${data.length} total`}
        action={isAdmin && (
          <button onClick={openCreate} className="btn-primary">
            <Plus size={16} /> Add Employee
          </button>
        )}
      />

      <div className="page-body">
        <div className="card overflow-hidden">
          {/* Search bar */}
          <div className="px-5 py-4 border-b border-slate-100 flex items-center gap-3">
            <div className="relative flex-1 max-w-sm">
              <Search size={15} className="absolute left-3.5 top-3 text-slate-400" />
              <input value={search} onChange={e => setSearch(e.target.value)}
                placeholder="Search name, department, position…"
                className="input pl-9 py-2.5" />
            </div>
            <span className="text-xs text-slate-400">{filtered.length} result{filtered.length !== 1 ? 's' : ''}</span>
          </div>

          {isLoading ? <TableSkeleton cols={7} rows={6} /> : filtered.length === 0 ? (
            <EmptyState icon={Users} title="No employees found" subtitle="Try adjusting your search" />
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr>
                    {['Employee','NIC','Department','Position','Basic Salary','Join Date','Status',''].map(h => (
                      <th key={h} className="table-th">{h}</th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {filtered.map(e => (
                    <tr key={e.employeeID} className="table-row">
                      <td className="table-td">
                        <div className="flex items-center gap-3">
                          <div className="w-8 h-8 bg-gradient-to-br from-blue-400 to-violet-500 rounded-full flex items-center justify-center flex-shrink-0">
                            <span className="text-white text-xs font-semibold">
                              {e.fullName.split(' ').map(w => w[0]).slice(0, 2).join('')}
                            </span>
                          </div>
                          <div>
                            <p className="font-medium text-slate-800 text-sm">{e.fullName}</p>
                            <p className="text-xs text-slate-400">{e.email ?? '—'}</p>
                          </div>
                        </div>
                      </td>
                      <td className="table-td font-mono text-xs text-slate-500">{e.nic}</td>
                      <td className="table-td">
                        {e.department ? (
                          <span className="bg-slate-100 text-slate-600 text-xs px-2 py-0.5 rounded-md font-medium">{e.department}</span>
                        ) : '—'}
                      </td>
                      <td className="table-td text-slate-600">{e.position ?? '—'}</td>
                      <td className="table-td font-semibold text-slate-700">
                        LKR {e.basicSalary.toLocaleString()}
                      </td>
                      <td className="table-td text-slate-500 text-xs">{e.joinDate.slice(0, 10)}</td>
                      <td className="table-td"><StatusBadge status={e.isActive ? 'Active' : 'Inactive'} /></td>
                      <td className="table-td">
                        {isAdmin && (
                          <div className="flex items-center gap-1">
                            <button onClick={() => openEdit(e)} className="btn-ghost p-1.5 text-blue-500 hover:text-blue-700 hover:bg-blue-50">
                              <Pencil size={14} />
                            </button>
                            <button onClick={() => toggleMut.mutate(e)}
                              className={`btn-ghost p-1.5 ${e.isActive ? 'text-red-400 hover:text-red-600 hover:bg-red-50' : 'text-emerald-500 hover:text-emerald-700 hover:bg-emerald-50'}`}>
                              {e.isActive ? <UserX size={14} /> : <UserCheck size={14} />}
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
        <Modal title={editing ? 'Edit Employee' : 'Add Employee'} onClose={closeForm}>
          <div className="grid grid-cols-2 gap-4">
            <FormField label="Full Name">
              <Input value={form.fullName} onChange={e => setForm(f => ({ ...f, fullName: e.target.value }))} placeholder="e.g. Kasun Perera" />
            </FormField>
            <FormField label="NIC">
              <Input value={form.nic} onChange={e => setForm(f => ({ ...f, nic: e.target.value }))} placeholder="199012345671" />
            </FormField>
            <FormField label="Department">
              <Input value={form.department} onChange={e => setForm(f => ({ ...f, department: e.target.value }))} placeholder="e.g. IT" />
            </FormField>
            <FormField label="Position">
              <Input value={form.position} onChange={e => setForm(f => ({ ...f, position: e.target.value }))} placeholder="e.g. Developer" />
            </FormField>
            <FormField label="Basic Salary (LKR)">
              <Input type="number" value={form.basicSalary} onChange={e => setForm(f => ({ ...f, basicSalary: +e.target.value }))} />
            </FormField>
            <FormField label="Join Date">
              <Input type="date" value={form.joinDate} onChange={e => setForm(f => ({ ...f, joinDate: e.target.value }))} />
            </FormField>
            <FormField label="Contact Number">
              <Input value={form.contactNumber} onChange={e => setForm(f => ({ ...f, contactNumber: e.target.value }))} placeholder="077XXXXXXX" />
            </FormField>
            <FormField label="Email">
              <Input type="email" value={form.email} onChange={e => setForm(f => ({ ...f, email: e.target.value }))} placeholder="name@company.lk" />
            </FormField>
          </div>
          {saveMut.isError && (
            <p className="mt-3 text-xs text-red-600 bg-red-50 p-3 rounded-xl">Failed to save. Please check the details.</p>
          )}
          <div className="flex gap-3 mt-6 justify-end border-t border-slate-100 pt-5">
            <button onClick={closeForm} className="btn-secondary">Cancel</button>
            <button onClick={() => saveMut.mutate(editing ? { ...form, id: editing.employeeID } : form)}
              disabled={saveMut.isPending} className="btn-primary">
              {saveMut.isPending ? 'Saving…' : editing ? 'Update Employee' : 'Add Employee'}
            </button>
          </div>
        </Modal>
      )}
    </div>
  )
}
