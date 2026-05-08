<div align="center">



&#x20; <img src="docs/screenshots/logo.png" alt="WikiUnach Logo" width="180"/>



&#x20; # WikiUnach — Biblioteca Digital Universitaria



&#x20; \*\*Collaborative academic platform for the UNACH university community\*\*



&#x20; \[!\[License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

&#x20; \[!\[Platform](https://img.shields.io/badge/Platform-Windows-lightgrey.svg)]()

&#x20; \[!\[.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-purple.svg)]()

&#x20; \[!\[C#](https://img.shields.io/badge/Language-C%23-239120.svg)]()

&#x20; \[!\[AWS](https://img.shields.io/badge/Cloud-AWS%20S3%20%2B%20RDS-orange.svg)]()

&#x20; \[!\[MySQL](https://img.shields.io/badge/Database-MySQL%208.0-blue.svg)]()



&#x20; \[📖 User Manual](docs/Manual\_de\_Usuario.pdf) •

&#x20; \[🏗️ Technical Docs](docs/Documentacion\_Tecnica.pdf) •

&#x20; \[🎬 Demo Video](#-demo) •

&#x20; \[⚙️ Setup Guide](#-installation--setup)



</div>



\---



\## 📌 Table of Contents



\- \[About the Project](#-about-the-project)

\- \[Key Features](#-key-features)

\- \[Architecture](#-architecture)

\- \[Tech Stack](#-tech-stack)

\- \[Screenshots](#-screenshots)

\- \[Demo](#-demo)

\- \[Installation \& Setup](#-installation--setup)

\- \[Configuration](#-configuration)

\- \[Database Schema](#-database-schema)

\- \[Project Structure](#-project-structure)

\- \[Team](#-team)

\- \[License](#-license)



\---



\## 📚 About the Project



\*\*WikiUnach\*\* (WUNACH) is a collaborative desktop application built for the student and faculty community of the \*\*Universidad Autónoma de Chiapas (UNACH)\*\*. It solves a critical academic problem: valuable student work — assignments, essays, research projects, and lab reports — is created once, submitted, and then lost forever.



WikiUnach turns that work into a \*\*living, searchable, version-controlled digital library\*\*, organized by Faculty → Degree → Semester → Subject.



\### The Problem It Solves

> Students spend hours recreating work that already exists. Senior students' knowledge never reaches juniors. Academic material is fragmented across personal drives, WhatsApp groups, and email threads.



\### The Solution

> A structured, role-based platform where students and teachers upload, discover, vote on, and build upon each other's academic work — permanently and collaboratively.



\---



\## ✨ Key Features



| Feature | Description |

|---|---|

| 🔐 \*\*Role-Based Access\*\* | Four roles: Visitor, Student, Teacher, Administrator |

| 📄 \*\*Wiki Pages\*\* | Create and edit academic content with full version history |

| 🔄 \*\*Version Control\*\* | Git-style commit history for every wiki page edit |

| ☁️ \*\*Cloud File Storage\*\* | Upload and download files via AWS S3 with pre-signed URLs |

| 💬 \*\*Interaction System\*\* | Comments, upvotes, downvotes, and bookmarks per page |

| 🔍 \*\*Smart Search\*\* | Search by title or tag across all published content |

| 🔔 \*\*Notifications\*\* | Real-time alerts for comments, likes, and admin actions |

| 📧 \*\*Email Verification\*\* | 6-digit code verification via Gmail SMTP on registration |

| 🔒 \*\*Secure Admin Access\*\* | Hidden admin panel protected by native C++/x86 ASM validator |

| 🎨 \*\*Dynamic Themes\*\* | Light/Dark themes with persistent user preferences |



\---



\## 🏗️ Architecture

┌─────────────────────────────────────────────────────────┐

│                   CLIENT (Windows Desktop)               │

│                                                          │

│   C# WinForms (.NET Framework 4.7.2)                    │

│   ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐  │

│   │ FrmAcceso│ │FrmPrinc. │ │FrmSubir  │ │FrmDetall.│  │

│   └──────────┘ └──────────┘ └──────────┘ └──────────┘  │

│   ┌────────────────────────────────────────────────────┐ │

│   │  Services: DBConexion │ S3Service │ EmailService   │ │

│   └────────────────────────────────────────────────────┘ │

│   ┌──────────────────────┐                               │

│   │  VerificadorAdmin.dll│  ← Native C++/x86 ASM        │

│   └──────────────────────┘                               │

└──────────────┬──────────────────────┬───────────────────┘

│                      │

▼                      ▼

┌──────────────────────┐  ┌───────────────────────────────┐

│  AWS RDS MySQL 8.0   │  │         AWS S3 Bucket          │

│  (us-east-2)         │  │  (Private + Pre-signed URLs)   │

│  - Users             │  │  - PDFs, DOCX, images, ZIP     │

│  - Wiki Pages        │  │  - Audio, video, code files    │

│  - Revisions         │  │                                │

│  - Comments/Votes    │  └───────────────────────────────┘

│  - Notifications     │

└──────────────────────┘

│

▼

┌──────────────────────┐

│    Gmail SMTP        │

│  (Email Verification │

│   \& Notifications)   │

└──────────────────────┘

---



\## 🛠️ Tech Stack



\### Frontend

| Technology | Purpose |

|---|---|

| C# / WinForms | Desktop UI with dynamic UserControls |

| .NET Framework 4.7.2 | Application runtime |

| Visual Studio 2022 | IDE and build system |



\### Backend / Cloud

| Technology | Purpose |

|---|---|

| AWS S3 | Binary file storage (private bucket, pre-signed URLs) |

| AWS RDS MySQL 8.0 | Relational database (us-east-2) |

| Gmail SMTP | Email verification codes |



\### Security

| Technology | Purpose |

|---|---|

| SHA-256 | Password hashing (hex, stored in DB) |

| VerificadorAdmin.dll | Admin access validation (C++ / x86 Assembly) |

| Pre-signed URLs | Temporary, expiring S3 download links (15 min) |



\### Libraries (NuGet)

| Package | Purpose |

|---|---|

| `MySql.Data` | MySQL client connector |

| `AWSSDK.S3` | AWS S3 operations |

| `AWSSDK.Core` | AWS SDK core utilities |

| `System.Net.Mail` | SMTP email sending (built-in) |



\---



\## 📸 Screenshots



<div align="center">



\### Login Screen

!\[Login](docs/screenshots/login.png)



\### Main Feed

!\[Main](docs/screenshots/principal.png)



\### Wiki Page Detail

!\[Detail](docs/screenshots/detalles.png)



\### Upload Task

!\[Upload](docs/screenshots/subir.png)



</div>



\---



\## 🎬 Demo



> 📽️ \*\*Full application walkthrough video:\*\*

> 👉 \[Watch on YouTube](#) \*(link coming soon)\*



The demo covers:

\- User registration with email verification

\- Navigating by Faculty → Degree → Semester → Subject

\- Uploading a file to AWS S3

\- Version history and commit comparison

\- Admin panel access via native DLL validator



\---



\## 🚀 Installation \& Setup



\### Prerequisites



Before running the application, make sure you have:



\- ✅ \*\*Windows 10 or later\*\* (x86 or x64)

\- ✅ \*\*.NET Framework 4.7.2\*\* — \[Download here](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472)

\- ✅ \*\*Visual Studio 2022\*\* (with .NET desktop development workload)

\- ✅ \*\*AWS Account\*\* with S3 bucket and RDS MySQL instance

\- ✅ \*\*Gmail Account\*\* with App Password enabled



\---



\### Step 1 — Clone the Repository



```bash

git clone https://github.com/axelramos54/WikiUnach.git

cd WikiUnach

```



\### Step 2 — Set Up Configuration



Copy the template and fill in your credentials:



```bash

cp App.config.template src/App.config

```



Open `src/App.config` and replace all placeholder values:



```xml

<add key="AWS\_AccessKey"  value="YOUR\_AWS\_ACCESS\_KEY" />

<add key="AWS\_SecretKey"  value="YOUR\_AWS\_SECRET\_KEY" />

<add key="AWS\_BucketName" value="your-s3-bucket-name" />

<add key="AWS\_Region"     value="us-east-2" />

<add key="SMTP\_User"      value="your-email@gmail.com" />

<add key="SMTP\_Pass"      value="your-gmail-app-password" />

```



\### Step 3 — Restore NuGet Packages



Open the solution in Visual Studio → right-click the solution → \*\*"Restore NuGet Packages"\*\*



Or via command line:

```bash

nuget restore src/WUNACH.slnx

```



\### Step 4 — Set Build Target to x86



> ⚠️ \*\*Critical:\*\* The native `VerificadorAdmin.dll` is compiled for x86.

> The project \*\*must\*\* be built as x86 or it will crash at runtime.



In Visual Studio:

1\. Go to \*\*Build → Configuration Manager\*\*

2\. Set \*\*Platform\*\* to `x86` for all configurations



\### Step 5 — Set Up the Database



Run the SQL script to create all tables and relationships:



```bash

mysql -u YOUR\_DB\_USER -p YOUR\_DATABASE < src/Database/Migracion\_AdminFeatures.sql

```



\### Step 6 — Build and Run



Press \*\*`F5`\*\* in Visual Studio or build via: Build → Build Solution (Ctrl+Shift+B)



The compiled output will be in `src/bin/Debug/`. Make sure `VerificadorAdmin.dll`

is in the same folder as `WUNACH.exe`.



\---



\## ⚙️ Configuration



All configuration is managed through `App.config` (not committed — see `App.config.template`).



| Key | Description | Where to get it |

|---|---|---|

| `AWS\_AccessKey` | AWS IAM access key | AWS Console → IAM → Users |

| `AWS\_SecretKey` | AWS IAM secret key | AWS Console → IAM → Users |

| `AWS\_BucketName` | Your S3 bucket name | AWS Console → S3 |

| `AWS\_Region` | AWS region | Match your RDS region |

| `SMTP\_User` | Gmail address | Your Gmail account |

| `SMTP\_Pass` | Gmail App Password | \[myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords) |



> ⚠️ \*\*Never commit `App.config` with real credentials.\*\*

> Use `App.config.template` as the base and keep your real config local only.



\---



\## 🗄️ Database Schema



The database follows \*\*3rd Normal Form (3NF)\*\* with 11 tables:

Facultades ──< Licenciaturas ──< Materias ──< PaginasWiki

│

┌──────────┼──────────┐

▼          ▼          ▼

Revisiones  Comentarios  Archivos

│

┌────────────────┼────────────┐

▼                     ▼            ▼

Usuarios             Votos     Bookmarks

│

┌─────────┴─────────┐

▼                        ▼

Notificaciones       Etiquetas



Full schema documentation: 📄 \[Technical Documentation](docs/Documentacion\_Tecnica.pdf)



\---



\## 📁 Project Structure

WikiUnach/

├── README.md

├── LICENSE

├── App.config.template          ← Safe config template (no credentials)

├── .gitignore

│

├── src/                         ← All source code

│   ├── WUNACH.slnx              ← Visual Studio solution

│   ├── Forms/                   ← WinForms screens

│   ├── Services/                ← Cloud \& DB services

│   ├── Helpers/                 ← Utility classes

│   ├── Native/                  ← VerificadorAdmin.dll

│   ├── Assets/                  ← Icons and images

│   └── Database/                ← SQL migration scripts

│

├── docs/                        ← Documentation

│   ├── Manual\_de\_Usuario.pdf

│   ├── Documentacion\_Tecnica.pdf

│   ├── Revision\_de\_Proyecto.pdf

│   └── screenshots/             ← UI screenshots

│

└── database/                    ← DB schema reference

└── schema.md

---



\## 👥 Team



| Name | Role |

|---|---|

| \*\*Axel Ramos Monroy\*\* | Lead Developer — Architecture, AWS, WinForms |

| \*\*Carlos Adolfo Ortiz Ortiz\*\* | Database Design \& SQL |

| \*\*Ruth Carolina Moscoso Villatoro\*\* | UI/UX \& Documentation |

| \*\*Luis Roberto Miranda De La Cruz\*\* | Backend Services \& Testing |



\*\*Academic Context:\*\*

\- 🏫 Universidad Autónoma de Chiapas (UNACH)

\- 🎓 Licenciatura en Sistemas Computacionales — 4to Semestre, Grupo K

\- 📚 Subject: Lenguajes de Consulta

\- 👨‍🏫 Professor: Dr. Luis Gutiérrez Alfaro



\---



\## 📄 License



This project is licensed under the \*\*MIT License\*\* — see the \[LICENSE](LICENSE) file for details.



\---



<div align="center">



\*\*Built with ❤️ for the UNACH student community\*\*



⭐ If you found this project useful, please consider starring the repository!



</div>





