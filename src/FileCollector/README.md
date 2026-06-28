# File Collector

برنامه‌ی ویندوز فرم برای جمع‌آوری خودکار فایل‌ها از چندین پوشه با تنظیمات کاملاً داینامیک.

ساخته‌شده با .NET Framework 4.8 و Windows Forms با رابط کاربری فارسی و راست‌چین (RTL).

## ✨ امکانات

### 📁 مدیریت پوشه‌ها
- افزودن چندین پوشه به‌صورت همزمان
- پشتیبانی از زیرپوشه‌ها (Subfolder Recursion)
- فیلتر بر اساس پسوند، حداقل/حداکثر حجم
- ۳ حالت نظارت: Realtime (همان لحظه) / Interval (هر N ثانیه) / Scheduled
- Pause/Resume/Stop برای هر پوشه به‌صورت مستقل

### 🎬 اکشن‌ها (تا ۵ مرحله در زنجیره)
| اکشن | توضیح |
|------|-------|
| Copy | کپی فایل به مسیر جدید |
| Move | انتقال فایل |
| Rename | تغییر نام با الگو |
| Delete | حذف دائمی |
| Recycle | ارسال به سطل بازیافت |
| Zip | فشرده‌سازی به .zip |
| ZipAndMove | فشرده‌سازی و سپس حذف اصلی |
| Extract | باز کردن فایل ZIP |
| CustomCommand | اجرای فایل اجرایی دلخواه با آرگومان |
| TextProcessing | پردازش متن (Find/Replace, Header/Footer, ...) |
| DatabaseStore | ذخیره در SQL Server ریموت |

### 🔤 پردازش متن
- Find & Replace با پشتیبانی از Regex
- درج Header / Footer
- Append / Prepend متن
- تشخیص خودکار فایل متنی (نه باینری)
- پشتیبانی از Encoding های مختلف: UTF-8, UTF-16, ASCII, Windows-1256
- حفظ BOM در صورت وجود

### 🗄️ ذخیره‌سازی در دیتابیس ریموت
- پشتیبانی از SQL Server
- ۳ روش ذخیره:
  - **BLOB Direct** — فایل به‌صورت `VARBINARY(MAX)` (مناسب فایل کوچک)
  - **Hybrid** ⭐ — metadata در DB، فایل در File Share (پیشنهادی)
  - **FILESTREAM** — استفاده از قابلیت FILESTREAM (نیاز به تنظیمات سرور)
- جلوگیری از فایل تکراری با MD5
- فشرده‌سازی GZIP قبل از ذخیره (اختیاری)
- Threshold حجم قابل تنظیم
- دکمه «تست اتصال و ساخت جدول»

### 📊 ۴ نوار پیشرفت
1. **پیشرفت کلی** — کل عملیات
2. **پیشرفت هر پوشه** — به‌صورت مستقل و موازی
3. **پیشرفت فایل جاری** — با نمایش مرحله Chain
4. **صف انتظار** — تعداد فایل‌های در انتظار

### 📈 آمار زنده
- تعداد فایل‌های پردازش‌شده/رد‌شده/خطا
- سرعت پردازش (فایل/ثانیه)
- زمان سپری‌شده و تخمین زمان باقی‌مانده
- نرخ موفقیت
- تعداد ورکرهای فعال

### 🔧 قابلیت‌های دیگر
- ذخیره تنظیمات در `config.json` با Hot-Reload
- Import/Export تنظیمات (برای بکاپ یا انتقال)
- SQLite محلی برای تاریخچه و لاگ
- جلوگیری از پردازش فایل تکراری (Deduplication با MD5)
- سیستم‌تری (System Tray) با Minimize to Tray
- لاگ روزانه با چرخش خودکار (Log Rotation)
- ۲۲ متغیر قابل استفاده در الگوها: `{date}`, `{time}`, `{filename}`, `{md5}`, `{size}`, ...

## 🚀 شروع سریع

### پیش‌نیازها
- Windows 7 SP1 یا بالاتر
- .NET Framework 4.8
- (اختیاری) SQL Server برای ذخیره‌سازی ریموت

### ساخت پروژه
1. پروژه را در Visual Studio 2019/2022 باز کنید
2. NuGet پکیج‌های زیر را نصب کنید:
   ```
   Install-Package Newtonsoft.Json
   Install-Package System.Data.SQLite.Core
   ```
3. Build پروژه را بزنید
4. اجرا کنید

### استفاده
1. روی «+ افزودن پوشه» کلیک کنید
2. نام و مسیر منبع را وارد کنید
3. در تب «اکشن‌ها» اکشن‌های مورد نظر را اضافه کنید (مثلاً Copy → TextProcessing → DatabaseStore)
4. در تب «پردازش متن» پردازش متن را فعال کنید (اختیاری)
5. در تب «پایگاه‌داده» ذخیره‌سازی ریموت را فعال کنید (اختیاری)
6. ذخیره کنید
7. روی «شروع همه» کلیک کنید

## 📂 ساختار پروژه

```
FileCollector/
├── FileCollector.sln
└── src/
    └── FileCollector/
        ├── FileCollector.csproj
        ├── App.config
        ├── Program.cs
        ├── Models/
        │   ├── AppConfig.cs
        │   ├── FolderConfig.cs
        │   ├── ActionConfig.cs
        │   ├── TextProcessingConfig.cs
        │   ├── DatabaseConfig.cs
        │   └── ProgressInfo.cs
        ├── Core/
        │   ├── ConfigManager.cs       # بارگذاری/ذخیره config با hot-reload
        │   ├── LogManager.cs          # لاگ روزانه
        │   ├── VariableResolver.cs    # جایگزینی {variable} در الگوها
        │   ├── TextProcessor.cs       # پردازش متن
        │   ├── DatabaseManager.cs     # SQLite محلی + SQL Server ریموت
        │   ├── ActionExecutor.cs      # اجرای اکشن‌ها
        │   ├── FolderWatcher.cs       # FileSystemWatcher + Interval
        │   └── CollectorEngine.cs     # موتور اصلی
        └── Forms/
            ├── MainForm.cs            # فرم اصلی با ۴ نوار پیشرفت
            ├── MainForm.Designer.cs
            ├── FolderConfigForm.cs    # تنظیمات هر پوشه
            └── FolderConfigForm.Designer.cs
```

## 🎯 مثال‌های کاربردی

### سناریو ۱: جمع‌آوری لاگ‌های سرور
- نظارت روی `C:\Logs\` با زیرپوشه‌ها
- فیلتر: `*.log`
- اکشن: TextProcessing → Move to `\\server\archive\{year}\{month}\`
- پردازش متن: افزودن Header با تاریخ و منبع

### سناریو ۲: ذخیره فاکتورها در دیتابیس
- نظارت روی `C:\Invoices\`
- فیلتر: `*.pdf;*.xml`
- اکشن: DatabaseStore (روش Hybrid)
- مسیر File Share: `\\server\invoices\`
- جدول SQL Server: `dbo.collected_files`
- Threshold حجم: 50 MB

### سناریو ۳: فشرده‌سازی و آپلود
- نظارت روی `D:\Uploads\`
- اکشن‌ها به ترتیب:
  1. ZipAndMove → `C:\Temp\`
  2. CustomCommand → `curl -F "file=@{filepath}" https://api.example.com/upload`
  3. Delete (پاک‌سازی موقت)

## ⚙️ متغیرهای الگو

در هر الگوی متنی (مسیر مقصد، نام فایل، Header، Find/Replace و ...) قابل استفاده:

| متغیر | توضیح | نمونه |
|-------|-------|-------|
| `{date}` | تاریخ امروز | `2026-06-28` |
| `{time}` | ساعت فعلی | `14:30:15` |
| `{datetime}` | تاریخ و زمان | `2026-06-28 14:30:15` |
| `{year}` | سال | `2026` |
| `{month}` | ماه | `06` |
| `{day}` | روز | `28` |
| `{hour}` | ساعت | `14` |
| `{minute}` | دقیقه | `30` |
| `{second}` | ثانیه | `15` |
| `{filename}` | نام فایل با پسوند | `report.pdf` |
| `{filename_noext}` | نام فایل بدون پسوند | `report` |
| `{ext}` | پسوند فایل | `.pdf` |
| `{size}` | حجم فایل (بایت) | `102400` |
| `{size_kb}` | حجم فایل (KB) | `100.00` |
| `{size_mb}` | حجم فایل (MB) | `0.10` |
| `{md5}` | هش MD5 فایل | `d41d8cd98f00b204...` |
| `{source_folder}` | مسیر پوشه منبع | `C:\Input\Reports` |
| `{source_folder_name}` | نام پوشه منبع | `Reports` |
| `{username}` | نام کاربر فعلی | `admin` |
| `{machine_name}` | نام ماشین | `PC-001` |
| `{guid}` | GUID جدید | `a1b2c3d4...` |
| `{counter}` | شماره ترتیبی (۶ رقمی) | `000001` |

## 📝 License

MIT License — آزاد برای استفاده تجاری و غیرتجاری.

## 👤 Author

**Masoud Sarafraz**  
GitHub: [MasoudSarafraz](https://github.com/MasoudSarafraz)
