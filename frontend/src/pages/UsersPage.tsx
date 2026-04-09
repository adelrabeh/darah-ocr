import { useEffect, useState } from 'react'
import api from '../services/api'

export default function UsersPage() {
  const [users, setUsers] = useState<any[]>([])
  const [form, setForm] = useState({ username: '', email: '', password: '', role: 'user' })
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')

  const load = () => api.get('/users').then(r => setUsers(r.data))
  useEffect(() => { load() }, [])

  const createUser = async (e: React.FormEvent) => {
    e.preventDefault()
    setError(''); setSuccess('')
    try {
      await api.post('/users', form)
      setSuccess('تم إنشاء المستخدم بنجاح')
      setForm({ username: '', email: '', password: '', role: 'user' })
      load()
    } catch (err: any) {
      setError(err.response?.data?.message ?? 'فشل إنشاء المستخدم')
    }
  }

  const toggleStatus = async (id: number) => {
    await api.put(`/users/${id}/status`)
    load()
  }

  const deleteUser = async (id: number) => {
    if (!confirm('هل تريد حذف هذا المستخدم؟')) return
    await api.delete(`/users/${id}`)
    load()
  }

  return (
    <div>
      <h1 style={{ fontSize: 24, fontWeight: 700, marginBottom: 24, color: 'var(--darah-dark)' }}>
        إدارة المستخدمين
      </h1>

      {/* إضافة مستخدم */}
      <div className="card" style={{ marginBottom: 24 }}>
        <h2 style={{ fontSize: 16, fontWeight: 600, marginBottom: 16 }}>إضافة مستخدم جديد</h2>
        {error && <div className="alert alert-error">{error}</div>}
        {success && <div className="alert alert-success">{success}</div>}
        <form onSubmit={createUser} style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr 1fr auto', gap: 12, alignItems: 'end' }}>
          <div className="form-group" style={{ marginBottom: 0 }}>
            <label>اسم المستخدم</label>
            <input value={form.username} onChange={e => setForm(f => ({ ...f, username: e.target.value }))} required />
          </div>
          <div className="form-group" style={{ marginBottom: 0 }}>
            <label>البريد الإلكتروني</label>
            <input type="email" value={form.email} onChange={e => setForm(f => ({ ...f, email: e.target.value }))} required />
          </div>
          <div className="form-group" style={{ marginBottom: 0 }}>
            <label>كلمة المرور</label>
            <input type="password" value={form.password} onChange={e => setForm(f => ({ ...f, password: e.target.value }))} required />
          </div>
          <div className="form-group" style={{ marginBottom: 0 }}>
            <label>الدور</label>
            <select value={form.role} onChange={e => setForm(f => ({ ...f, role: e.target.value }))}>
              <option value="user">مستخدم</option>
              <option value="operator">مشغّل</option>
              <option value="admin">مدير</option>
            </select>
          </div>
          <button type="submit" className="btn btn-primary">إضافة</button>
        </form>
      </div>

      {/* قائمة المستخدمين */}
      <div className="card">
        <table>
          <thead>
            <tr>
              <th>اسم المستخدم</th>
              <th>البريد</th>
              <th>الدور</th>
              <th>الحالة</th>
              <th>آخر دخول</th>
              <th>إجراءات</th>
            </tr>
          </thead>
          <tbody>
            {users.map(u => (
              <tr key={u.id}>
                <td>{u.username}</td>
                <td>{u.email}</td>
                <td>{{ admin: 'مدير', operator: 'مشغّل', user: 'مستخدم' }[u.role as string] ?? u.role}</td>
                <td>
                  <span className={`badge ${u.isActive ? 'badge-completed' : 'badge-failed'}`}>
                    {u.isActive ? 'نشط' : 'موقوف'}
                  </span>
                </td>
                <td>{u.lastLoginAt ? new Date(u.lastLoginAt).toLocaleDateString('ar-SA') : 'لم يدخل بعد'}</td>
                <td style={{ display: 'flex', gap: 6 }}>
                  <button className="btn btn-outline" style={{ padding: '4px 10px', fontSize: 12 }}
                    onClick={() => toggleStatus(u.id)}>
                    {u.isActive ? 'إيقاف' : 'تفعيل'}
                  </button>
                  {u.username !== 'admin' && (
                    <button className="btn btn-danger" style={{ padding: '4px 10px', fontSize: 12 }}
                      onClick={() => deleteUser(u.id)}>
                      حذف
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
