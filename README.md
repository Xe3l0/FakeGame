# 🎮 FakeExe
> Spoof your Discord Rich Presence — make it look like you're playing any game.

![Platform](https://img.shields.io/badge/platform-Windows-blue)
![Language](https://img.shields.io/badge/language-C%23-purple)
![License](https://img.shields.io/badge/license-MIT-green)

---

## ⚡ التثبيت خطوة بخطوة

### الخطوة 1 — تثبيت .NET SDK

التطبيق يحتاج .NET عشان يشتغل، نزّله من هنا:

👉 **[تحميل .NET SDK](https://aka.ms/dotnet/download)**

اختار **Windows x64** وثبّته، العملية تاخذ دقيقتين.

---

### الخطوة 2 — تشغيل المثبّت

بعد ما تحمّل ملفات المشروع:

1. افتح مجلد المشروع
2. **ضغطتين على `install.bat`**
3. البرنامج يبني نفسه تلقائياً ويظهر على الـ Desktop

> ✅ ما تحتاج تفتح CMD أو تكتب أي أوامر

---

## 🛠 كيف تستخدمه

| الحقل | الشرح |
|-------|-------|
| **Discord Application ID** | الـ ID من Discord Developer Portal — اتركه افتراضي للاستخدام العادي |
| **Game Name** | اسم اللعبة اللي تبي تظهر عند أصحابك |
| **Details** | السطر الأول تحت اسم اللعبة — مثال: `In the main menu` |
| **State** | السطر الثاني — مثال: `Level 12 \| Ranked Match` |
| **Large Image Key** | مفتاح صورة اللعبة (اختياري) |

بعد ما تملي الحقول، اضغط **▶ Start Presence** وافتح Discord وشوف! 🎮

---

## ⚠️ ملاحظة مهمة — اسم اللعبة الرئيسي

Discord يعرض **اسم تطبيقك** من Developer Portal كعنوان رئيسي، مو الاسم اللي تكتبه في التطبيق مباشرة.

عشان يطلع اسم اللعبة الصح:

1. روح 👉 [discord.com/developers/applications](https://discord.com/developers/applications)
2. افتح تطبيقك أو أنشئ واحد جديد مجاناً
3. غير **App Name** لاسم اللعبة اللي تبيها
4. احفظ وأعد تشغيل FakeExe

> مثال: تبي تظهر تلعب `Minecraft`؟ غير App Name لـ `Minecraft`

---

## 💾 الإعدادات

التطبيق يحفظ إعداداتك تلقائياً في:
```
C:\Users\[اسمك]\AppData\Roaming\FakeExe\settings.txt
```
كل ما تفتح التطبيق يرجع كل شي كما تركته ✅

---

## 🔧 بناء المشروع يدوياً

إذا تبي تبنيه بنفسك:

```bash
git clone https://github.com/YourUsername/FakeExe
cd FakeExe
dotnet publish -c Release
```

الـ `.exe` يطلع في:
```
bin\Release\net6.0-windows\win-x64\publish\FakeExe.exe
```

---

## 📋 المتطلبات

- Windows 10 أو أحدث
- [.NET 6 SDK](https://aka.ms/dotnet/download)
- Discord مثبّت وشغّال

---

## 📄 License

MIT — مجاني للاستخدام والتعديل والنشر.
