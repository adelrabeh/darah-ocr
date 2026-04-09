import { useEffect, useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import api from '../services/api'

export default function JobDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [job, setJob] = useState<any>(null)
  const [copied, setCopied] = useState(false)

  useEffect(() => {
    api.get(`/jobs/${id}`).then(r => setJob(r.data))
  }, [id])

  const copyText = () => {
    navigator.clipboard.writeText(job.result.rawText)
    setCopied(true)
    setTimeout(() => setCopied(false), 2000)
  }

  const downloadText = () => {
    const blob = new Blob([job.result.rawText], { type: 'text/plain;charset=utf-8' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `${job.originalFilename}.txt`
    a.click()
    URL.revokeObjectURL(url)
  }

  if (!job) return <div style={{ padding: 40, textAlign: 'center' }}>جارٍ التحميل...</div>

  return (
    <div>
      <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginBottom: 24 }}>
        <button className="btn btn-outline" onClick={() => navigate('/jobs')}>← رجوع</button>
        <h1 style={{ fontSize: 20, fontWeight: 700, color: 'var(--darah-dark)' }}>
          {job.originalFilename}
        </h1>
      </div>

      {/* معلومات الملف */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: 16, marginBottom: 24 }}>
        {[
          { label: 'عدد الصفحات', value: job.result?.pageCount ?? '-' },
          { label: 'عدد الكلمات', value: job.result?.wordCount?.toLocaleString() ?? '-' },
          { label: 'الثقة', value: job.result ? `${job.result.confidenceScore}%` : '-' },
          { label: 'الجودة', value: { high: '🟢 عالية', medium: '🟡 متوسطة', low: '🔴 منخفضة' }[job.result?.qualityLevel] ?? '-' },
        ].map(item => (
          <div key={item.label} className="card" style={{ textAlign: 'center' }}>
            <div style={{ fontSize: 24, fontWeight: 700, color: 'var(--darah-dark)' }}>{item.value}</div>
            <div style={{ fontSize: 13, color: '#666', marginTop: 4 }}>{item.label}</div>
          </div>
        ))}
      </div>

      {/* النص المستخرج */}
      {job.result && (
        <div className="card">
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
            <h2 style={{ fontSize: 16, fontWeight: 600 }}>النص المستخرج</h2>
            <div style={{ display: 'flex', gap: 8 }}>
              <button className="btn btn-outline" onClick={copyText}>
                {copied ? '✅ تم النسخ' : '📋 نسخ'}
              </button>
              <button className="btn btn-primary" onClick={downloadText}>
                💾 تحميل .txt
              </button>
            </div>
          </div>
          <textarea
            readOnly
            value={job.result.rawText}
            style={{
              width: '100%', height: 500, resize: 'vertical',
              background: '#fafafa', fontFamily: 'Amiri, serif',
              fontSize: 16, lineHeight: 1.8, direction: 'rtl'
            }}
          />
          {job.result.processingNotes && (
            <div className="alert alert-error" style={{ marginTop: 12 }}>
              ⚠️ {job.result.processingNotes}
            </div>
          )}
        </div>
      )}
    </div>
  )
}
