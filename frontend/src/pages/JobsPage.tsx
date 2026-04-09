import { useEffect, useState, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../services/api'

export default function JobsPage() {
  const navigate = useNavigate()
  const [jobs, setJobs] = useState<any[]>([])
  const [uploading, setUploading] = useState(false)
  const [error, setError] = useState('')
  const fileRef = useRef<HTMLInputElement>(null)

  const load = () => api.get('/jobs').then(r => setJobs(r.data))

  useEffect(() => {
    load()
    const interval = setInterval(load, 5000)
    return () => clearInterval(interval)
  }, [])

  const upload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return
    setError('')
    setUploading(true)
    const form = new FormData()
    form.append('file', file)
    try {
      await api.post('/jobs/upload', form)
      load()
    } catch (err: any) {
      setError(err.response?.data?.message ?? 'فشل رفع الملف')
    } finally {
      setUploading(false)
      if (fileRef.current) fileRef.current.value = ''
    }
  }

  const deleteJob = async (id: number) => {
    if (!confirm('هل تريد حذف هذه المهمة؟')) return
    await api.delete(`/jobs/${id}`)
    load()
  }

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24 }}>
        <h1 style={{ fontSize: 24, fontWeight: 700, color: 'var(--darah-dark)' }}>المهام</h1>
        <div>
          <input ref={fileRef} type="file" accept=".pdf,.png,.jpg,.jpeg,.tiff,.tif" style={{ display: 'none' }} onChange={upload} />
          <button className="btn btn-primary" onClick={() => fileRef.current?.click()} disabled={uploading}>
            {uploading ? '⏳ جارٍ الرفع...' : '📤 رفع ملف جديد'}
          </button>
        </div>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      <div className="card">
        <table>
          <thead>
            <tr>
              <th>#</th>
              <th>الملف</th>
              <th>النوع</th>
              <th>الحجم</th>
              <th>الحالة</th>
              <th>الكلمات</th>
              <th>الجودة</th>
              <th>التاريخ</th>
              <th>إجراءات</th>
            </tr>
          </thead>
          <tbody>
            {jobs.map(job => (
              <tr key={job.id}>
                <td>{job.id}</td>
                <td style={{ maxWidth: 200, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                  {job.originalFilename}
                </td>
                <td>{job.fileType}</td>
                <td>{formatSize(job.fileSize)}</td>
                <td><span className={`badge badge-${job.status}`}>{translateStatus(job.status)}</span></td>
                <td>{job.wordCount?.toLocaleString() ?? '-'}</td>
                <td>{translateQuality(job.quality)}</td>
                <td>{new Date(job.createdAt).toLocaleDateString('ar-SA')}</td>
                <td style={{ display: 'flex', gap: 6 }}>
                  {job.status === 'completed' && (
                    <button className="btn btn-outline" style={{ padding: '4px 10px', fontSize: 12 }}
                      onClick={() => navigate(`/jobs/${job.id}`)}>
                      عرض
                    </button>
                  )}
                  <button className="btn btn-danger" style={{ padding: '4px 10px', fontSize: 12 }}
                    onClick={() => deleteJob(job.id)}>
                    حذف
                  </button>
                </td>
              </tr>
            ))}
            {jobs.length === 0 && (
              <tr><td colSpan={9} style={{ textAlign: 'center', color: '#999', padding: 40 }}>
                لا توجد مهام — ارفع ملفاً للبدء
              </td></tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}

function formatSize(bytes: number) {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / 1024 / 1024).toFixed(1)} MB`
}

function translateStatus(s: string) {
  return { pending: 'معلّقة', processing: 'جارٍ المعالجة', completed: 'مكتملة', failed: 'فاشلة' }[s] ?? s
}

function translateQuality(q: string) {
  return { high: '🟢 عالية', medium: '🟡 متوسطة', low: '🔴 منخفضة' }[q] ?? '-'
}
