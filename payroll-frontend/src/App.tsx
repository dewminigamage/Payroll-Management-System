import { Routes, Route, Navigate } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext'
import { ProtectedRoute } from './components/ProtectedRoute'
import Layout from './components/Layout'
import Login from './pages/Login'
import Dashboard from './pages/Dashboard'
import Employees from './pages/Employees'
import Payroll from './pages/Payroll'
import Attendance from './pages/Attendance'
import Overtime from './pages/Overtime'
import Loans from './pages/Loans'
import Leave from './pages/Leave'
import Reports from './pages/Reports'
import SettingsPage from './pages/Settings'
import UsersPage from './pages/Users'

export default function App() {
  return (
    <AuthProvider>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route
          path="/"
          element={
            <ProtectedRoute>
              <Layout />
            </ProtectedRoute>
          }
        >
          <Route index element={<Navigate to="/dashboard" replace />} />
          <Route path="dashboard"  element={<Dashboard />} />
          <Route path="employees"  element={<Employees />} />
          <Route path="payroll"    element={<Payroll />} />
          <Route path="attendance" element={<Attendance />} />
          <Route path="overtime"   element={<Overtime />} />
          <Route path="loans"      element={<Loans />} />
          <Route path="leave"      element={<Leave />} />
          <Route path="reports"    element={<Reports />} />
          <Route path="settings"   element={<SettingsPage />} />
          <Route path="users"      element={<ProtectedRoute adminOnly><UsersPage /></ProtectedRoute>} />
        </Route>
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </AuthProvider>
  )
}
