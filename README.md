# منظومة رقمنة الوثائق العربية — دارة الملك عبدالعزيز

نظام OCR متكامل لاستخراج النصوص العربية من الوثائق والملفات.

## المتطلبات

- .NET 8 SDK
- PostgreSQL 14+
- Node.js 18+ (لبناء الـ Frontend)
- Tesseract OCR مع بيانات اللغة العربية

## تثبيت Tesseract على Windows

```powershell
# تنزيل Tesseract
# https://github.com/UB-Mannheim/tesseract/wiki
# اختر النسخة التي تشمل اللغة العربية (Arabic)

# بعد التثبيت، انسخ مجلد tessdata إلى مجلد التطبيق
# مثال: C:\Program Files\Tesseract-OCR\tessdata
```

## تثبيت Tesseract على Linux/Ubuntu

```bash
apt-get install tesseract-ocr tesseract-ocr-ara tesseract-ocr-eng
# انسخ tessdata من /usr/share/tesseract-ocr/5/tessdata/
```

## إعداد قاعدة البيانات

```sql
CREATE DATABASE darah_ocr;
CREATE USER darah_user WITH PASSWORD 'your_password';
GRANT ALL PRIVILEGES ON DATABASE darah_ocr TO darah_user;
```

## إعداد الإعدادات

عدّل `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=darah_ocr;Username=darah_user;Password=your_password"
  },
  "Jwt": {
    "Secret": "YOUR_VERY_LONG_RANDOM_SECRET_KEY_HERE_MIN_32_CHARS"
  },
  "Storage": {
    "UploadsPath": "C:\\DarahOcr\\uploads"
  },
  "Tesseract": {
    "DataPath": "C:\\DarahOcr\\tessdata"
  }
}
```

## بناء الـ Frontend

```bash
cd frontend
npm install
npm run build
# سيتم نسخ الملفات تلقائياً إلى src/DarahOcr.Api/wwwroot
```

## تشغيل النظام

```bash
cd src/DarahOcr.Api
dotnet run
# أو للإنتاج:
dotnet publish -c Release -o publish
dotnet publish/DarahOcr.Api.dll
```

## النشر على IIS (Windows Server)

1. نشر التطبيق:
```powershell
dotnet publish -c Release -o C:\inetpub\wwwroot\darah-ocr
```

2. إنشاء Application Pool في IIS:
   - Name: `DarahOcr`
   - .NET CLR Version: `No Managed Code`
   - Managed Pipeline Mode: `Integrated`

3. إنشاء Website في IIS:
   - Physical Path: `C:\inetpub\wwwroot\darah-ocr`
   - Application Pool: `DarahOcr`

4. تثبيت ASP.NET Core Hosting Bundle:
   - https://dotnet.microsoft.com/download/dotnet/8.0

## بيانات الدخول الافتراضية

- **المستخدم:** admin
- **كلمة المرور:** Admin@1234

⚠️ **غيّر كلمة المرور فور تسجيل الدخول الأول**

## هيكل المشروع

```
darah-ocr/
├── src/
│   └── DarahOcr.Api/          # ASP.NET Core Backend
│       ├── Controllers/        # Auth, Jobs, Users
│       ├── Data/              # AppDbContext
│       ├── Migrations/        # قاعدة البيانات
│       ├── Models/            # User, OcrJob, OcrResult
│       ├── Services/          # OcrService (Tesseract)
│       └── wwwroot/           # Frontend build (مولّد تلقائياً)
└── frontend/                  # React/Vite Frontend
    └── src/
        ├── pages/             # Login, Dashboard, Jobs, Users
        ├── components/        # Layout
        └── services/          # API client
```
