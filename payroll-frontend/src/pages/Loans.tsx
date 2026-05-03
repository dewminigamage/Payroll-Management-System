import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Plus, CreditCard } from 'lucide-react'
import api from '../api/client'
import { useAuth } from '../context/AuthContext'
import { PageHeader, FormField, Select, StatusBadge, TableSkeleton, EmptyState, FilterChips, Modal } from '../components/ui'
import type { EmployeeLoan, LoanType, Employee } from '../types'

type StatusFilter = 'Active' | 'Completed' | 'Cancelled' | ''

export default function Loans() {
  const { isAdmin } = useAuth()
  const qc = useQueryClient()
  const [filter,     setFilter]     = useState<StatusFilter>('Active')
  const [showCreate, setShowCreate] = useState(false)
  const [showRepay,  setShowRepay]  = useState<EmployeeLoan | null>(null)
  const [form, setForm] = useState({
    employeeID: 0, loanTypeID: 0, loanAmount: 0, monthlyInstallment: 0,
    startMonth: new Date().getMonth() + 1, startYear: new Date().getFullYear(), notes: '',
  })
  const [repay, setRepay] = useState({ payMonth: new Date().getMonth() + 1, payYear: new Date().getFullYear(), amountPaid: 0, notes: '' })

  const { data: employees = [] } = useQuery<Employee[]>({
    queryKey: ['employees-active'],
    queryFn: () => api.get('/employees', { params: { activeOnly: true } }).then(r => r.data),
  })
  const { data: loanTypes = [] } = useQuery<LoanType[]>({
    queryKey: ['loan-types'],
    queryFn: () => api.get('/loans/types').then(r => r.data),
  })
  const { data = [], isLoading } = useQuery<EmployeeLoan[]>({
    queryKey: ['loans', filter],
    queryFn: () => api.get('/loans', { params: { status: filter || undefined } }).then(r => r.data),
  })

  const createMut = useMutation({
    mutationFn: () => api.post('/loans', form),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['loans'] }); setShowCreate(false) },
  })

  const repayMut = useMutation({
    mutationFn: (id: number) => api.post(`/loans/${id}/repayments`, repay),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['loans'] }); setShowRepay(null) },
  })

  return (
    <div>
      <PageHeader
        title="Loans"
        subtitle={`${data.length} ${filter || 'total'} loans`}
        action={isAdmin && (
          <button onClick={() => setShowCreate(true)} className="btn-primary">
            <Plus size={15} /> New Loan
          </button>
        )}
      />

      <div className="page-body space-y-5">
        <FilterChips
          value={filter}
          onChange={(v) => setFilter(v as StatusFilter)}
          options={[
            { label: 'Active',    value: 'Active' },
            { label: 'Completed', value: 'Completed' },
            { label: 'Cancelled', value: 'Cancelled' },
            { label: 'All',       value: '' },
          ]}
        />

        <div className="card overflow-hidden">
          {isLoading ? <TableSkeleton cols={7} rows={5} /> : data.length === 0 ? (
            <EmptyState icon={CreditCard} title="No loans found" subtitle="Create a new loan to get started" />
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr>{['Employee','Type','Loan Amount','Remaining','Installment/Mo','Started','Status',''].map(h =>
                    <th key={h} className="table-th">{h}</th>)}
                  </tr>
                </thead>
                <tbody>
                  {data.map(l => {
                    const pct = l.loanAmount > 0 ? ((l.loanAmount - l.remainingBalance) / l.loanAmount) * 100 : 0
                    return (
                      <tr key={l.loanID} className="table-row">
                        <td className="table-td font-medium text-slate-800">{l.employeeName}</td>
                        <td className="table-td">
                          <span className="bg-slate-100 text-slate-600 text-xs px-2 py-0.5 rounded-md font-medium">{l.loanTypeName}</span>
                        </td>
                        <td className="table-td tabular-nums">{l.loanAmount.toLocaleString()}</td>
                        <td className="table-td">
                          <div>
                            <p className="font-semibold tabular-nums text-slate-800">{l.remainingBalance.toLocaleString()}</p>
                            <div className="w-20 h-1.5 bg-slate-100 rounded-full mt-1">
                              <div className="h-full bg-blue-400 rounded-full" style={{ width: `${pct}%` }} />
                            </div>
                          </div>
                        </td>
                        <td className="table-td tabular-nums text-slate-600">{l.monthlyInstallment.toLocaleString()}</td>
                        <td className="table-td text-slate-500 text-xs">{l.startMonth}/{l.startYear}</td>
                        <td className="table-td"><StatusBadge status={l.status} /></td>
                        <td className="table-td">
                          {isAdmin && l.status === 'Active' && (
                            <button onClick={() => { setRepay(r => ({ ...r, amountPaid: l.monthlyInstallment })); setShowRepay(l) }}
                              className="btn-ghost text-xs text-blue-600 hover:bg-blue-50 px-2.5 py-1.5 gap-1.5">
                              <CreditCard size={13} /> Repay
                            </button>
                          )}
                        </td>
                      </tr>
                    )
                  })}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>

      {/* Create Loan */}
      {showCreate && (
        <Modal title="Create New Loan" onClose={() => setShowCreate(false)} width="max-w-md">
          <div className="space-y-4">
            <FormField label="Employee">
              <Select value={form.employeeID} onChange={e => setForm(x => ({ ...x, employeeID: +e.target.value }))}>
                <option value={0}>Select employee…</option>
                {employees.map(e => <option key={e.employeeID} value={e.employeeID}>{e.fullName}</option>)}
              </Select>
            </FormField>
            <FormField label="Loan Type">
              <Select value={form.loanTypeID} onChange={e => setForm(x => ({ ...x, loanTypeID: +e.target.value }))}>
                <option value={0}>Select type…</option>
                {loanTypes.map(t => <option key={t.loanTypeID} value={t.loanTypeID}>{t.typeName}</option>)}
              </Select>
            </FormField>
            <div className="grid grid-cols-2 gap-4">
              <FormField label="Loan Amount (LKR)">
                <input type="number" value={form.loanAmount} onChange={e => setForm(x => ({ ...x, loanAmount: +e.target.value }))} className="input" />
              </FormField>
              <FormField label="Monthly Installment">
                <input type="number" value={form.monthlyInstallment} onChange={e => setForm(x => ({ ...x, monthlyInstallment: +e.target.value }))} className="input" />
              </FormField>
              <FormField label="Start Month">
                <input type="number" min={1} max={12} value={form.startMonth} onChange={e => setForm(x => ({ ...x, startMonth: +e.target.value }))} className="input" />
              </FormField>
              <FormField label="Start Year">
                <input type="number" value={form.startYear} onChange={e => setForm(x => ({ ...x, startYear: +e.target.value }))} className="input" />
              </FormField>
            </div>
          </div>
          <div className="flex gap-3 mt-6 justify-end border-t border-slate-100 pt-5">
            <button onClick={() => setShowCreate(false)} className="btn-secondary">Cancel</button>
            <button onClick={() => createMut.mutate()} disabled={!form.employeeID || createMut.isPending} className="btn-primary">
              {createMut.isPending ? 'Creating…' : 'Create Loan'}
            </button>
          </div>
        </Modal>
      )}

      {/* Record Repayment */}
      {showRepay && (
        <Modal title="Record Repayment" onClose={() => setShowRepay(null)} width="max-w-sm">
          <div className="bg-blue-50 rounded-xl p-4 mb-5 border border-blue-100">
            <p className="text-xs text-blue-600 font-semibold uppercase tracking-wide">Loan</p>
            <p className="font-semibold text-slate-800 mt-0.5">{showRepay.employeeName} — {showRepay.loanTypeName}</p>
            <p className="text-sm text-blue-700 mt-1">Remaining: <strong>LKR {showRepay.remainingBalance.toLocaleString()}</strong></p>
          </div>
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <FormField label="Pay Month">
                <input type="number" min={1} max={12} value={repay.payMonth}
                  onChange={e => setRepay(r => ({ ...r, payMonth: +e.target.value }))} className="input" />
              </FormField>
              <FormField label="Pay Year">
                <input type="number" value={repay.payYear}
                  onChange={e => setRepay(r => ({ ...r, payYear: +e.target.value }))} className="input" />
              </FormField>
            </div>
            <FormField label="Amount Paid (LKR)">
              <input type="number" value={repay.amountPaid}
                onChange={e => setRepay(r => ({ ...r, amountPaid: +e.target.value }))} className="input" />
            </FormField>
          </div>
          <div className="flex gap-3 mt-6 justify-end border-t border-slate-100 pt-5">
            <button onClick={() => setShowRepay(null)} className="btn-secondary">Cancel</button>
            <button onClick={() => repayMut.mutate(showRepay.loanID)} disabled={repayMut.isPending} className="btn-primary">
              {repayMut.isPending ? 'Recording…' : 'Record Payment'}
            </button>
          </div>
        </Modal>
      )}
    </div>
  )
}
