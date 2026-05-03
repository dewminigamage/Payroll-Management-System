import React, { type ReactNode } from 'react'
import { X } from 'lucide-react'

// ── Badge ────────────────────────────────────────────────────────────────────
type BadgeVariant = 'green' | 'red' | 'yellow' | 'blue' | 'purple' | 'gray' | 'orange'
const badgeStyles: Record<BadgeVariant, string> = {
  green:  'bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200',
  red:    'bg-red-50 text-red-600 ring-1 ring-red-200',
  yellow: 'bg-amber-50 text-amber-700 ring-1 ring-amber-200',
  blue:   'bg-blue-50 text-blue-700 ring-1 ring-blue-200',
  purple: 'bg-violet-50 text-violet-700 ring-1 ring-violet-200',
  gray:   'bg-slate-100 text-slate-500 ring-1 ring-slate-200',
  orange: 'bg-orange-50 text-orange-700 ring-1 ring-orange-200',
}

export function Badge({ label, variant = 'gray' }: { label: string; variant?: BadgeVariant }) {
  return (
    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${badgeStyles[variant]}`}>
      {label}
    </span>
  )
}

export function StatusBadge({ status }: { status: string }) {
  const map: Record<string, BadgeVariant> = {
    Active: 'green', Completed: 'gray', Cancelled: 'red',
    Present: 'green', Absent: 'red', Late: 'yellow', 'Half Day': 'orange', Leave: 'blue',
    Pending: 'yellow', Approved: 'green', Rejected: 'red',
    Admin: 'purple', Viewer: 'blue',
    Inactive: 'gray',
  }
  return <Badge label={status} variant={map[status] ?? 'gray'} />
}

// ── Modal ────────────────────────────────────────────────────────────────────
export function Modal({ title, onClose, children, width = 'max-w-lg' }: {
  title: string; onClose: () => void; children: ReactNode; width?: string
}) {
  return (
    <div className="modal-overlay" onClick={e => { if (e.target === e.currentTarget) onClose() }}>
      <div className={`modal-box ${width}`}>
        <div className="flex items-center justify-between px-6 py-5 border-b border-slate-100">
          <h2 className="text-base font-semibold text-slate-800">{title}</h2>
          <button onClick={onClose} className="btn-ghost p-1.5">
            <X size={18} />
          </button>
        </div>
        <div className="px-6 py-5">{children}</div>
      </div>
    </div>
  )
}

// ── FormField ────────────────────────────────────────────────────────────────
export function FormField({ label, children }: { label: string; children: ReactNode }) {
  return (
    <div>
      <label className="label">{label}</label>
      {children}
    </div>
  )
}

export function Input(props: React.InputHTMLAttributes<HTMLInputElement>) {
  return <input {...props} className={`input ${props.className ?? ''}`} />
}

export function Select(props: React.SelectHTMLAttributes<HTMLSelectElement> & { children: ReactNode }) {
  return (
    <select {...props} className={`input cursor-pointer ${props.className ?? ''}`}>
      {props.children}
    </select>
  )
}

export function Textarea(props: React.TextareaHTMLAttributes<HTMLTextAreaElement>) {
  return <textarea {...props} className={`input resize-none ${props.className ?? ''}`} />
}

// ── PageHeader ────────────────────────────────────────────────────────────────
export function PageHeader({ title, subtitle, action }: {
  title: string; subtitle?: string; action?: ReactNode
}) {
  return (
    <div className="page-header flex items-center justify-between">
      <div>
        <h1 className="text-xl font-semibold text-slate-800">{title}</h1>
        {subtitle && <p className="text-sm text-slate-500 mt-0.5">{subtitle}</p>}
      </div>
      {action && <div>{action}</div>}
    </div>
  )
}

// ── Empty State ────────────────────────────────────────────────────────────────
export function EmptyState({ icon: Icon, title, subtitle }: {
  icon: React.ElementType; title: string; subtitle?: string
}) {
  return (
    <div className="flex flex-col items-center justify-center py-16 text-center">
      <div className="w-14 h-14 bg-slate-100 rounded-2xl flex items-center justify-center mb-4">
        <Icon size={24} className="text-slate-400" />
      </div>
      <p className="text-sm font-medium text-slate-600">{title}</p>
      {subtitle && <p className="text-xs text-slate-400 mt-1">{subtitle}</p>}
    </div>
  )
}

// ── Skeleton ────────────────────────────────────────────────────────────────
export function Skeleton({ className = '' }: { className?: string }) {
  return <div className={`bg-slate-200 animate-pulse rounded-lg ${className}`} />
}

export function TableSkeleton({ cols = 5, rows = 6 }: { cols?: number; rows?: number }) {
  return (
    <div className="p-4 space-y-3">
      {Array.from({ length: rows }).map((_, i) => (
        <div key={i} className="flex gap-4">
          {Array.from({ length: cols }).map((_, j) => (
            <Skeleton key={j} className={`h-5 ${j === 0 ? 'w-36' : 'flex-1'}`} />
          ))}
        </div>
      ))}
    </div>
  )
}

// ── Month/Year Filter Bar ────────────────────────────────────────────────────
const MONTHS = ['January','February','March','April','May','June',
                'July','August','September','October','November','December']

export function PeriodFilter({
  month, year, onMonth, onYear, extra
}: {
  month: number; year: number;
  onMonth: (m: number) => void; onYear: (y: number) => void;
  extra?: ReactNode
}) {
  return (
    <div className="flex flex-wrap items-center gap-3">
      <select value={month} onChange={e => onMonth(+e.target.value)}
        className="input w-36">
        {MONTHS.map((m, i) => <option key={i} value={i + 1}>{m}</option>)}
      </select>
      <input type="number" value={year} onChange={e => onYear(+e.target.value)}
        className="input w-24" />
      {extra}
    </div>
  )
}

// ── Tab bar ────────────────────────────────────────────────────────────────
export function TabBar<T extends string>({ tabs, active, onChange }: {
  tabs: { key: T; label: string }[];
  active: T;
  onChange: (t: T) => void;
}) {
  return (
    <div className="flex gap-1 border-b border-slate-200">
      {tabs.map(t => (
        <button key={t.key} onClick={() => onChange(t.key)}
          className={`px-4 py-2.5 text-sm font-medium border-b-2 transition-all -mb-px ${
            active === t.key
              ? 'border-blue-600 text-blue-600'
              : 'border-transparent text-slate-500 hover:text-slate-700 hover:border-slate-300'
          }`}>
          {t.label}
        </button>
      ))}
    </div>
  )
}

// ── Stat Card ────────────────────────────────────────────────────────────────
export function StatCard({ label, value, sub, icon: Icon, gradient }: {
  label: string; value: string | number; sub?: string;
  icon: React.ElementType; gradient: string
}) {
  return (
    <div className="card p-6 flex items-start gap-4 hover:shadow-md transition-shadow">
      <div className={`w-12 h-12 ${gradient} rounded-2xl flex items-center justify-center flex-shrink-0 shadow-sm`}>
        <Icon size={22} className="text-white" />
      </div>
      <div className="min-w-0 flex-1">
        <p className="text-xs font-semibold text-slate-500 uppercase tracking-wide">{label}</p>
        <p className="text-2xl font-bold text-slate-800 mt-1 truncate">{value}</p>
        {sub && <p className="text-xs text-slate-400 mt-0.5">{sub}</p>}
      </div>
    </div>
  )
}

// ── Filter Chip ────────────────────────────────────────────────────────────
export function FilterChips<T extends string>({
  options, value, onChange
}: { options: { label: string; value: T }[]; value: T; onChange: (v: T) => void }) {
  return (
    <div className="flex flex-wrap gap-2">
      {options.map(o => (
        <button key={o.value} onClick={() => onChange(o.value)}
          className={`px-3.5 py-1.5 rounded-full text-xs font-medium border transition-all ${
            value === o.value
              ? 'bg-blue-600 text-white border-blue-600 shadow-sm'
              : 'bg-white text-slate-600 border-slate-200 hover:border-blue-300 hover:text-blue-600'
          }`}>
          {o.label}
        </button>
      ))}
    </div>
  )
}
