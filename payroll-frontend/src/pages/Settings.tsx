import { useState, useEffect } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Save, Settings } from 'lucide-react'
import api from '../api/client'
import { useAuth } from '../context/AuthContext'
import { PageHeader } from '../components/ui'

export default function SettingsPage() {
  const { isAdmin } = useAuth()
  const qc = useQueryClient()
  const [local,  setLocal]  = useState<Record<string, string>>({})
  const [saved,  setSaved]  = useState(false)

  const { data = {}, isLoading } = useQuery<Record<string, string>>({
    queryKey: ['settings'],
    queryFn: () => api.get('/settings').then(r => r.data),
  })

  useEffect(() => { setLocal(data) }, [data])

  const saveMut = useMutation({
    mutationFn: () => api.put('/settings', local),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['settings'] })
      setSaved(true)
      setTimeout(() => setSaved(false), 3000)
    },
  })

  const friendlyLabel = (key: string) =>
    key.replace(/_/g, ' ').replace(/\b\w/g, c => c.toUpperCase())

  return (
    <div>
      <PageHeader
        title="Company Settings"
        subtitle="Configure payroll parameters and company information"
        action={isAdmin && (
          <button onClick={() => saveMut.mutate()} disabled={saveMut.isPending} className="btn-primary">
            <Save size={15} /> {saveMut.isPending ? 'Saving…' : 'Save Changes'}
          </button>
        )}
      />

      <div className="page-body max-w-2xl">
        {saved && (
          <div className="mb-5 bg-emerald-50 text-emerald-700 text-sm rounded-xl p-4 border border-emerald-200 flex items-center gap-2">
            <span className="w-2 h-2 bg-emerald-500 rounded-full" />
            Settings saved successfully.
          </div>
        )}

        {isLoading ? (
          <div className="card p-6 space-y-4">
            {Array.from({ length: 5 }).map((_, i) => (
              <div key={i} className="space-y-1.5">
                <div className="h-3 bg-slate-200 animate-pulse rounded w-24" />
                <div className="h-10 bg-slate-200 animate-pulse rounded-xl" />
              </div>
            ))}
          </div>
        ) : Object.keys(local).length === 0 ? (
          <div className="card p-12 text-center">
            <Settings size={32} className="text-slate-300 mx-auto mb-3" />
            <p className="text-sm text-slate-500">No settings configured yet.</p>
          </div>
        ) : (
          <div className="card p-6 space-y-5">
            {Object.entries(local).map(([key, value]) => (
              <div key={key}>
                <label className="label">{friendlyLabel(key)}</label>
                <input
                  type="text"
                  value={value ?? ''}
                  onChange={e => setLocal(s => ({ ...s, [key]: e.target.value }))}
                  disabled={!isAdmin}
                  className="input disabled:bg-slate-50 disabled:text-slate-500 disabled:cursor-not-allowed"
                />
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
