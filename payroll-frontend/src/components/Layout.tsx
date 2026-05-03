import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import {
  LayoutDashboard, Users, DollarSign, Calendar, Clock,
  CreditCard, Umbrella, BarChart2, Settings, UserCog, LogOut,
  ChevronRight,
} from 'lucide-react'

const navGroups = [
  {
    label: 'Overview',
    items: [
      { to: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
    ],
  },
  {
    label: 'People',
    items: [
      { to: '/employees',  label: 'Employees',  icon: Users },
      { to: '/attendance', label: 'Attendance',  icon: Calendar },
      { to: '/leave',      label: 'Leave',       icon: Umbrella },
    ],
  },
  {
    label: 'Payroll',
    items: [
      { to: '/payroll',    label: 'Payroll',   icon: DollarSign },
      { to: '/overtime',   label: 'Overtime',  icon: Clock },
      { to: '/loans',      label: 'Loans',     icon: CreditCard },
    ],
  },
  {
    label: 'Admin',
    items: [
      { to: '/reports',  label: 'Reports',  icon: BarChart2 },
      { to: '/settings', label: 'Settings', icon: Settings },
      { to: '/users',    label: 'Users',    icon: UserCog, adminOnly: true },
    ],
  },
]

export default function Layout() {
  const { user, logout, isAdmin } = useAuth()
  const navigate = useNavigate()

  const initials = (user?.fullName ?? 'U')
    .split(' ').map(w => w[0]).slice(0, 2).join('').toUpperCase()

  const handleLogout = () => { logout(); navigate('/login') }

  return (
    <div className="flex h-screen bg-slate-100 overflow-hidden">
      {/* Sidebar */}
      <aside className="w-60 bg-slate-900 flex flex-col flex-shrink-0 select-none">
        {/* Logo */}
        <div className="px-5 py-5 border-b border-slate-800">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 bg-gradient-to-br from-blue-500 to-blue-700 rounded-lg flex items-center justify-center flex-shrink-0 shadow">
              <DollarSign size={16} className="text-white" />
            </div>
            <div className="leading-tight">
              <p className="text-white font-semibold text-sm">PayrollPro</p>
              <p className="text-slate-500 text-xs">Management System</p>
            </div>
          </div>
        </div>

        {/* Nav */}
        <nav className="flex-1 py-3 overflow-y-auto">
          {navGroups.map(group => {
            const visibleItems = group.items.filter(i => !i.adminOnly || isAdmin)
            if (visibleItems.length === 0) return null
            return (
              <div key={group.label} className="mb-1">
                <p className="px-4 py-2 text-[10px] font-bold text-slate-500 uppercase tracking-widest">
                  {group.label}
                </p>
                {visibleItems.map(({ to, label, icon: Icon }) => (
                  <NavLink
                    key={to}
                    to={to}
                    className={({ isActive }) =>
                      `group flex items-center gap-3 mx-2 px-3 py-2.5 rounded-lg text-sm transition-all ${
                        isActive
                          ? 'bg-blue-600 text-white shadow-sm'
                          : 'text-slate-400 hover:text-white hover:bg-slate-800'
                      }`
                    }
                  >
                    {({ isActive }) => (
                      <>
                        <Icon size={16} className={isActive ? 'text-white' : 'text-slate-500 group-hover:text-white transition-colors'} />
                        <span className="flex-1 font-medium">{label}</span>
                        {isActive && <ChevronRight size={14} className="text-blue-300" />}
                      </>
                    )}
                  </NavLink>
                ))}
              </div>
            )
          })}
        </nav>

        {/* User */}
        <div className="border-t border-slate-800 p-3">
          <div className="flex items-center gap-3 px-2 py-2 rounded-lg">
            <div className="w-8 h-8 bg-gradient-to-br from-blue-400 to-violet-500 rounded-full flex items-center justify-center flex-shrink-0">
              <span className="text-white text-xs font-bold">{initials}</span>
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-white text-xs font-medium truncate">{user?.fullName}</p>
              <p className="text-slate-500 text-xs truncate">{user?.role}</p>
            </div>
            <button onClick={handleLogout}
              className="text-slate-500 hover:text-red-400 transition-colors p-1 rounded"
              title="Sign out">
              <LogOut size={15} />
            </button>
          </div>
        </div>
      </aside>

      {/* Main */}
      <main className="flex-1 overflow-y-auto flex flex-col">
        <Outlet />
      </main>
    </div>
  )
}
