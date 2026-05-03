import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Plus, Key, UserX, UserCog } from 'lucide-react'
import api from '../api/client'
import { PageHeader, FormField, Select, StatusBadge, TableSkeleton, EmptyState, Modal } from '../components/ui'
import type { User } from '../types'

export default function UsersPage() {
  const qc = useQueryClient()
  const [showCreate, setShowCreate] = useState(false)
  const [resetUser,  setResetUser]  = useState<User | null>(null)
  const [form,   setForm]   = useState({ username: '', password: '', fullName: '', role: 'Viewer' })
  const [newPwd, setNewPwd] = useState('')

  const { data = [], isLoading } = useQuery<User[]>({
    queryKey: ['users'],
    queryFn: () => api.get('/users').then(r => r.data),
  })

  const createMut = useMutation({
    mutationFn: () => api.post('/users', form),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['users'] })
      setShowCreate(false)
      setForm({ username: '', password: '', fullName: '', role: 'Viewer' })
    },
  })

  const deactivateMut = useMutation({
    mutationFn: (id: number) => api.delete(`/users/${id}`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['users'] }),
  })

  const resetPwdMut = useMutation({
    mutationFn: (id: number) => api.put(`/users/${id}/password`, { newPassword: newPwd }),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['users'] }); setResetUser(null); setNewPwd('') },
  })

  return (
    <div>
      <PageHeader
        title="User Management"
        subtitle={`${data.filter(u => u.isActive).length} active users`}
        action={
          <button onClick={() => setShowCreate(true)} className="btn-primary">
            <Plus size={15} /> Add User
          </button>
        }
      />

      <div className="page-body">
        <div className="card overflow-hidden">
          {isLoading ? <TableSkeleton cols={6} rows={4} /> : data.length === 0 ? (
            <EmptyState icon={UserCog} title="No users found" />
          ) : (
            <table className="w-full">
              <thead>
                <tr>{['User','Role','Status','Created','Actions'].map(h => <th key={h} className="table-th">{h}</th>)}</tr>
              </thead>
              <tbody>
                {data.map(u => (
                  <tr key={u.userID} className="table-row">
                    <td className="table-td">
                      <div className="flex items-center gap-3">
                        <div className={`w-8 h-8 rounded-full flex items-center justify-center flex-shrink-0 text-xs font-bold text-white
                          ${u.role === 'Admin' ? 'bg-gradient-to-br from-violet-500 to-purple-600' : 'bg-gradient-to-br from-blue-400 to-cyan-500'}`}>
                          {u.fullName.split(' ').map((w: string) => w[0]).slice(0, 2).join('')}
                        </div>
                        <div>
                          <p className="font-medium text-slate-800">{u.fullName}</p>
                          <p className="text-xs text-slate-400">@{u.username}</p>
                        </div>
                      </div>
                    </td>
                    <td className="table-td"><StatusBadge status={u.role} /></td>
                    <td className="table-td"><StatusBadge status={u.isActive ? 'Active' : 'Inactive'} /></td>
                    <td className="table-td text-slate-500 text-xs">{u.createdDate.slice(0, 10)}</td>
                    <td className="table-td">
                      <div className="flex items-center gap-1">
                        <button onClick={() => setResetUser(u)}
                          className="btn-ghost p-1.5 text-blue-400 hover:text-blue-600 hover:bg-blue-50"
                          title="Reset password">
                          <Key size={14} />
                        </button>
                        {u.isActive && (
                          <button onClick={() => { if (confirm(`Deactivate ${u.username}?`)) deactivateMut.mutate(u.userID) }}
                            className="btn-ghost p-1.5 text-red-400 hover:text-red-600 hover:bg-red-50"
                            title="Deactivate user">
                            <UserX size={14} />
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>

      {showCreate && (
        <Modal title="Add New User" onClose={() => setShowCreate(false)} width="max-w-sm">
          <div className="space-y-4">
            <FormField label="Full Name">
              <input value={form.fullName} onChange={e => setForm(f => ({ ...f, fullName: e.target.value }))}
                className="input" placeholder="e.g. Kasun Perera" />
            </FormField>
            <FormField label="Username">
              <input value={form.username} onChange={e => setForm(f => ({ ...f, username: e.target.value }))}
                className="input" placeholder="e.g. kasun" />
            </FormField>
            <FormField label="Password">
              <input type="password" value={form.password} onChange={e => setForm(f => ({ ...f, password: e.target.value }))}
                className="input" placeholder="Min. 8 characters" />
            </FormField>
            <FormField label="Role">
              <Select value={form.role} onChange={e => setForm(f => ({ ...f, role: e.target.value }))}>
                <option value="Viewer">Viewer</option>
                <option value="Admin">Admin</option>
              </Select>
            </FormField>
          </div>
          <div className="flex gap-3 mt-6 justify-end border-t border-slate-100 pt-5">
            <button onClick={() => setShowCreate(false)} className="btn-secondary">Cancel</button>
            <button onClick={() => createMut.mutate()}
              disabled={!form.username || !form.password || !form.fullName || createMut.isPending}
              className="btn-primary">
              {createMut.isPending ? 'Creating…' : 'Create User'}
            </button>
          </div>
        </Modal>
      )}

      {resetUser && (
        <Modal title="Reset Password" onClose={() => { setResetUser(null); setNewPwd('') }} width="max-w-sm">
          <div className="bg-slate-50 rounded-xl p-4 mb-5 border border-slate-100">
            <p className="text-xs text-slate-500 font-semibold uppercase tracking-wide">User</p>
            <p className="font-semibold text-slate-800 mt-0.5">{resetUser.fullName}</p>
            <p className="text-xs text-slate-400">@{resetUser.username}</p>
          </div>
          <FormField label="New Password">
            <input type="password" value={newPwd} onChange={e => setNewPwd(e.target.value)}
              className="input" placeholder="Enter new password…" autoFocus />
          </FormField>
          <div className="flex gap-3 mt-6 justify-end border-t border-slate-100 pt-5">
            <button onClick={() => { setResetUser(null); setNewPwd('') }} className="btn-secondary">Cancel</button>
            <button onClick={() => resetPwdMut.mutate(resetUser.userID)}
              disabled={!newPwd || resetPwdMut.isPending} className="btn-primary">
              {resetPwdMut.isPending ? 'Resetting…' : 'Reset Password'}
            </button>
          </div>
        </Modal>
      )}
    </div>
  )
}
