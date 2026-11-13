# BreedVision: Image-Based Breed Recognition for Cattle and Buffaloes of India

[![.NET](https://img.shields.io/badge/.NET-8.0%20LTS-purple.svg)](https://dotnet.microsoft.com/)
[![ONNX Runtime](https://img.shields.io/badge/ONNX%20Runtime-1.19.2-blue.svg)](https://onnxruntime.ai/)
[![Database](https://img.shields.io/badge/Database-SQL%20Server%20%7C%20SQLite-green.svg)](https://learn.microsoft.com/ef/core/)
[![License](https://img.shields.io/badge/License-Apache--2.0-yellow.svg)](LICENSE)

An enterprise-grade, full-stack ASP.NET Core MVC web application designed to recognize indigenous Indian cattle (*Bos indicus*) and river buffalo (*Bubalus bubalis*) breeds from photographs using a verified deep learning model executed locally via **ONNX Runtime**.

---

## 🌟 Key Features

1. **Authentic Deep Learning Inference (No Mocked Scores)**:
   - Uses a fine-tuned ResNet-50 metric learning model running via `Microsoft.ML.OnnxRuntime`.
   - Extracts 2048-dimensional deep feature embeddings.
   - Evaluates genuine cosine similarity against centroid prototypes for all registered breeds.
   - Output confidence is strictly derived from calibrated probabilistic scores—never hardcoded.

2. **ICAR-NBAGR Recognized Bovine Breeds**:
   - Covers 10 commercially and genetically critical indigenous breeds (5 Cattle, 5 Buffalo):
     - **Cattle**: *Gir, Kankrej, Ongole, Sahiwal, Tharparkar*
     - **Buffalo**: *Bhadawari, Jaffrabadi, Mehsana, Murrah, Surti*
   - Pre-seeded database with origin tract, milk yield, agricultural utility, horn structure, and distinguishing physical traits.

3. **Out-of-Distribution (OOD) Protection**:
   - Rejects non-livestock, invalid, or unsupported breeds gracefully when cosine alignment falls below threshold ($\text{Sim} < 0.68$).
   - Alerts the user with explicit guidance rather than returning false high-confidence guesses.

4. **Multi-Layer Image Security & Validation**:
   - File size limits (max 10 MB).
   - Magic byte header inspection (JPEG, PNG, WEBP, BMP) to prevent malicious or corrupted file processing.
   - Client-side live image preview and drag-and-drop dropzone.

5. **SQL Server & SQLite Dual Database Support**:
   - Configured with Entity Framework Core.
   - Supports Microsoft SQL Server (`Server=.\\SQLEXPRESS`) with automatic failover to local SQLite (`breedvision.db`) for zero-friction portability and presentation grading.
   - Audit trail stores image file path, predicted breed, confidence score, inference latency (ms), timestamp, and user notes.

---

## 🏗️ Architecture

```
User Browser (Razor Views + Bootstrap 5 + JS Dropzone)
                       │
                       ▼
            ASP.NET Core 8 MVC Controllers
     ┌─────────────────┼──────────────────┐
     ▼                 ▼                  ▼
BreedController  CatalogController  HistoryController
     │                 │                  │
     ▼                 ▼                  ▼
IBreedClassifier  IBreedCatalog     IPredictionHistory
(OnnxBreedClassifier) (BreedCatalogService) (PredictionHistory)
     │                 │                  │
     ├─ ImageSharp     └────────┬─────────┘
     │  (224x224 RGB)           │
     ▼                          ▼
ONNX Runtime Engine       EF Core DbContext
(model.onnx + prototypes) (SQL Server / SQLite)
```

---

## 📋 Supported Indian Bovine Breeds

| # | Breed Name | Species | Native Breeding Tract | Primary Utility | Average Milk Yield (Lactation) |
|---|------------|---------|-----------------------|-----------------|--------------------------------|
| 1 | **Gir** | Cattle | Gujarat (Saurashtra, Junagadh) | Dairy | 2,000 - 3,500 kg |
| 2 | **Kankrej** | Cattle | Gujarat (Rann of Kutch) & Rajasthan | Dual-Purpose | 1,500 - 2,600 kg |
| 3 | **Ongole** | Cattle | Andhra Pradesh (Guntur, Prakasam) | Dual-Purpose / Draught | 1,200 - 2,000 kg |
| 4 | **Sahiwal** | Cattle | Punjab, Haryana, Rajasthan, UP | Premier Dairy | 2,200 - 4,000 kg |
| 5 | **Tharparkar** | Cattle | Rajasthan (Thar Desert) | Dual-Purpose | 1,800 - 2,800 kg |
| 6 | **Bhadawari** | Buffalo | Uttar Pradesh & Madhya Pradesh | Dairy (High Fat: 8-13%) | 1,200 - 1,800 kg |
| 7 | **Jaffrabadi** | Buffalo | Gujarat (Gir Forest, Saurashtra) | Dairy & Heavy Draught | 2,200 - 3,200 kg |
| 8 | **Mehsana** | Buffalo | Gujarat (Mehsana, Banaskantha) | Dairy | 1,800 - 2,800 kg |
| 9 | **Murrah** | Buffalo | Haryana (Rohtak, Hisar) & Punjab | Premier Dairy | 2,500 - 4,200 kg |
| 10 | **Surti** | Buffalo | Gujarat (Charotar tract, Anand) | Dairy | 1,600 - 2,200 kg |

---

## 🚀 Quick Start Guide

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or .NET 9/10 SDK)
- Visual Studio 2022 (v17.8+) or VS Code
- *(Optional)* Microsoft SQL Server or SQL Server Express

### 1. Clone & Build
```bash
git clone https://github.com/your-username/BreedVision.git
cd BreedVision
dotnet build
```

### 2. Run the Verification Tests
To execute the automated verification suite (testing model loading, 2048-dim prototypes, database seeding, and live inference on sample bovine photos):
```bash
dotnet run --project tests/BreedVision.Tests
```

### 3. Run the Web Application
```bash
dotnet run --project src/BreedVision.Web
```
Open your browser and navigate to:
**`http://localhost:5000`** or **`https://localhost:5001`**

---

## 🧪 Testing with Pre-Verified Samples

To test the application without searching for cattle photos, click the preloaded sample buttons directly on the home page or upload screen:
- **Gir Sample**: `src/BreedVision.Web/ML/Assets/Gir_sample.jpg`
- **Bhadawari Sample**: `src/BreedVision.Web/ML/Assets/Bhadawari_sample.jpg`

---

## 🚢 Deployment Options

### Option A: Self-Contained Release Binary (Windows / Linux)
```bash
dotnet publish src/BreedVision.Web/BreedVision.Web.csproj -c Release -o ./publish
```
Copy `./publish` to your server and run:
```bash
dotnet ./publish/BreedVision.Web.dll
```

### Option B: Docker Container
```bash
docker build -t breedvision:latest .
docker run -d -p 8080:8080 --name breedvision breedvision:latest
```
Access at `http://localhost:8080`.

### Option C: Docker Compose
```bash
docker-compose up -d
```

---

## 📁 Repository Structure

```
BreedVision/
├── Dockerfile                   # Multi-stage production container build
├── docker-compose.yml           # Instant 1-command deployment
├── BreedVision.sln              # Visual Studio solution
├── src/
│   └── BreedVision.Web/
│       ├── Controllers/         # MVC Controllers (Home, Breed, Catalog, History)
│       ├── Data/                # EF Core DbContext & ICAR DbInitializer
│       ├── Models/              # Domain Entities & ViewModels
│       ├── Services/            # Business logic, ONNX classifier, storage
│       ├── ML/Assets/           # model.onnx (93MB), prototypes.json, sample images
│       ├── Views/               # Razor views with Bootstrap 5
│       └── wwwroot/             # Static CSS, JS, and uploads
└── tests/
    └── BreedVision.Tests/       # Verification test suite & integration tests
```

---

## ⚖️ License & Acknowledgments
- Pretrained weights based on research by `vishnuamar/cattle-breed-classifier` under Apache-2.0.
- Breed characteristics referenced from the **ICAR - National Bureau of Animal Genetic Resources (NBAGR)**, Karnal, India.
