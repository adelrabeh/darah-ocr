import { Outlet, NavLink, useNavigate } from 'react-router-dom'
import { useState, useEffect } from 'react'
import api from '../services/api'

interface User { id: number; username: string; role: string }

export default function Layout() {
  const navigate = useNavigate()
  const [user, setUser] = useState<User | null>(null)

  useEffect(() => {
    api.get('/auth/me').then(r => setUser(r.data.user)).catch(() => navigate('/login'))
  }, [])

  const logout = () => {
    localStorage.removeItem('token')
    navigate('/login')
  }

  return (
    <div style={{ display: 'flex', minHeight: '100vh' }}>
      {/* Sidebar */}
      <aside style={{
        width: 240, background: 'var(--darah-dark)', color: '#fff',
        display: 'flex', flexDirection: 'column', padding: '0'
      }}>
        {/* Logo */}
        <div style={{ padding: '24px 20px', borderBottom: '1px solid rgba(255,255,255,0.1)' }}>
          <div style={{ fontSize: 13, opacity: 0.8 }}>دارة الملك عبدالعزيز</div>
          <div style={{ fontSize: 16, fontWeight: 700, marginTop: 4 }}>منظومة رقمنة الوثائق</div>
        </div>

        {/* Nav */}
        <nav style={{ flex: 1, padding: '16px 0' }}>
          {[
            { to: '/', label: '📊 لوحة التحكم' },
            { to: '/jobs', label: '📄 المهام' },
            ...(user?.role === 'admin' ? [{ to: '/users', label: '👥 المستخدمون' }] : [])
          ].map(item => (
            <NavLink
              key={item.to}
              to={item.to}
              end={item.to === '/'}
              style={({ isActive }) => ({
                display: 'block', padding: '12px 20px', color: '#fff',
                textDecoration: 'none', fontSize: 14,
                background: isActive ? 'rgba(255,255,255,0.15)' : 'transparent',
                borderRight: isActive ? '3px solid #fff' : '3px solid transparent'
              })}
            >
              {item.label}
            </NavLink>
          ))}
        </nav>

        {/* User info */}
        <div style={{ padding: '16px 20px', borderTop: '1px solid rgba(255,255,255,0.1)' }}>
          <div style={{ fontSize: 13, opacity: 0.7 }}>{user?.username}</div>
          <button onClick={logout} className="btn" style={{
            marginTop: 8, background: 'rgba(255,255,255,0.1)', color: '#fff',
            width: '100%', justifyContent: 'center', fontSize: 13
          }}>
            تسجيل الخروج
          </button>
        </div>
      </aside>

      {/* Main */}
      <main style={{ flex: 1, padding: '32px', overflow: 'auto' }}>
        <Outlet context={{ user }} />
      </main>
    </div>
  )
}
