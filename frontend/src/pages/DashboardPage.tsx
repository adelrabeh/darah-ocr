import { useEffect, useState } from 'react'
import api from '../services/api'

interface Stats {
  total: number
  pending: number
  processing: number
  completed: number
  failed: number
}

export default function DashboardPage() {
  const [stats, setStats] = useState<Stats | null>(null)
  const [recentJobs, setRecentJobs] = useState<any[]>([])

  useEffect(() => {
    api.get('/jobs').then(r => {
      const jobs = r.data
      setRecentJobs(jobs.slice(0, 5))
      setStats({
        total: jobs.length,
        pending: jobs.filter((j: any) => j.status === 'pending').length,
        processing: jobs.filter((j: any) => j.status === 'processing').length,
        completed: jobs.filter((j: any) => j.status === 'completed').length,
        failed: jobs.filter((j: any) => j.status === 'failed').length,
      })
    })
  }, [])

  const cards = [
    { label: 'إجمالي المهام', value: stats?.total ?? 0, color: 'var(--darah-dark)' },
    { label: 'مكتملة', value: stats?.completed ?? 0, color: '#28a745' },
    { label: 'قيد المعالجة', value: stats?.processing ?? 0, color: '#007bff' },
    { label: 'فاشلة', value: stats?.failed ?? 0, color: '#dc3545' },
  ]

  return (
    <div>
      <h1 style={{ fontSize: 24, fontWeight: 700, marginBottom: 24, color: 'var(--darah-dark)' }}>
        لوحة التحكم
      </h1>

      {/* Stats */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: 16, marginBottom: 32 }}>
        {cards.map(card => (
          <div key={card.label} className="card" style={{ textAlign: 'center' }}>
            <div style={{ fontSize: 36, fontWeight: 700, color: card.color }}>{card.value}</div>
            <div style={{ fontSize: 14, color: '#666', marginTop: 4 }}>{card.label}</div>
          </div>
        ))}
      </div>

      {/* Recent Jobs */}
      <div className="card">
        <h2 style={{ fontSize: 16, fontWeight: 600, marginBottom: 16 }}>آخر المهام</h2>
        <table>
          <thead>
            <tr>
              <th>الملف</th>
              <th>الحالة</th>
              <th>الكلمات</th>
              <th>التاريخ</th>
            </tr>
          </thead>
          <tbody>
            {recentJobs.map(job => (
              <tr key={job.id}>
                <td>{job.originalFilename}</td>
                <td><span className={`badge badge-${job.status}`}>{translateStatus(job.status)}</span></td>
                <td>{job.wordCount?.toLocaleString() ?? '-'}</td>
                <td>{new Date(job.createdAt).toLocaleDateString('ar-SA')}</td>
              </tr>
            ))}
            {recentJobs.length === 0 && (
              <tr><td colSpan={4} style={{ textAlign: 'center', color: '#999', padding: 32 }}>لا توجد مهام بعد</td></tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}

function translateStatus(s: string) {
  return { pending: 'معلّقة', processing: 'جارٍ المعالجة', completed: 'مكتملة', failed: 'فاشلة' }[s] ?? s
}
