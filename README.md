# BreedVision: Image-Based Breed Recognition for Cattle & Buffaloes of India (41 Breeds)

[![.NET](https://img.shields.io/badge/.NET-8.0%20LTS-purple.svg)](https://dotnet.microsoft.com/)
[![ONNX Runtime](https://img.shields.io/badge/ONNX%20Runtime-1.19.2-blue.svg)](https://onnxruntime.ai/)
[![Model](https://img.shields.io/badge/Model-ConvNeXt--Tiny%20(41--Class)-darkgreen.svg)](https://huggingface.co/)
[![Database](https://img.shields.io/badge/Database-SQLite%20%7C%20SQL%20Server-green.svg)](https://learn.microsoft.com/ef/core/)
[![Deploy to Render](https://render.com/images/deploy-to-render-button.svg)](https://render.com/deploy?repo=https://github.com/shivangchoubey/BreedVision)
[![License](https://img.shields.io/badge/License-Apache--2.0-yellow.svg)](LICENSE)

An enterprise-grade, full-stack ASP.NET Core MVC web application designed to recognize 41 registered and indigenous Indian cattle (*Bos indicus*) and riverine buffalo (*Bubalus bubalis*) breeds from photographs using a deep learning vision backbone executed locally via **Microsoft.ML.OnnxRuntime**.

---

## 🌟 Key Features

1. **Authentic Multi-Class Deep Learning Inference (No Mocked Scores)**:
   - Uses a fine-tuned **ConvNeXt-Tiny** architecture exported to a lightweight, dynamic-quantized **ONNX model** (`model_41breed.onnx`, ~26.9 MB).
   - Generates multi-class calibrated softmax probabilities across **41 distinct bovine categories**.
   - Output confidence and Top-5 class distributions are strictly derived from live model tensor inference.

2. **Full ICAR-NBAGR Recognized Bovine Encyclopedia (41 Breeds)**:
   - Covers **33 Cattle Breeds** (including Sahiwal, Red Sindhi, Rathi, Gir, Kankrej, Ongole, Tharparkar, Hallikar, Vechur).
   - Covers **8 Riverine Buffalo Breeds** (Murrah, Jaffrabadi, Bhadawari, Nili Ravi, Mehsana, Surti, Toda, Nagpuri).
   - Pre-seeded local database with native origin tracts, lactation milk yields, butterfat content, agricultural utility, horn morphology, and official ICAR accession numbers.

3. **Phenotypic Sister-Breed Disambiguation**:
   - Accurately differentiates closely related Zebu dairy breeds (such as **Red Sindhi vs. Sahiwal**, and **Rathi vs. Sahiwal**) with dedicated morphological comparative advisories.

4. **Out-of-Distribution (OOD) Protection**:
   - Enforces an automated rejection boundary when Top-1 softmax confidence falls below $15\%$, gracefully flagging non-livestock, invalid, or unsupported animal photos rather than returning false certainty.

5. **Multi-Layer Image Security & Validation**:
   - File size limits (max 10 MB).
   - Magic byte inspection (JPEG, PNG, WEBP, BMP) to prevent invalid file processing.
   - Interactive drag-and-drop file upload zone with client-side preview.

6. **Zero-Configuration Portable SQLite + SQL Server Failover**:
   - Configured with Entity Framework Core (`BreedVisionDbContext`).
   - Uses a portable, zero-configuration local SQLite database (`breedvision.db`) auto-created and auto-seeded on first boot, with automatic failover to Microsoft SQL Server if configured.
   - Full audit trail logging original filename, saved image path, predicted breed, confidence score, inference latency (ms), timestamp, and user field notes.

---

## 📋 Comprehensive Catalog of 41 Supported Breeds

### 🐂 Indigenous & Registered Cattle Breeds (33)
| # | Breed Name | Scientific Classification | Native Breeding Tract | Primary Utility | Typical Milk Yield (Lactation) |
|---|------------|---------------------------|-----------------------|-----------------|--------------------------------|
| 1 | **Alambadi** | *Bos indicus* | Tamil Nadu (Dharmapuri, Salem) | Draught | 600 – 1,000 kg |
| 2 | **Amritmahal** | *Bos indicus* | Karnataka (Hassan, Chikmagalur) | Draught | 700 – 1,200 kg |
| 3 | **Ayrshire** | *Bos taurus* | Crossbred / Exotic Naturalized | Dairy | 3,500 – 5,500 kg |
| 4 | **Banni** | *Bos indicus* | Gujarat (Kutch, Banni Grasslands) | Dual-Purpose | 2,000 – 3,200 kg |
| 5 | **Bargur** | *Bos indicus* | Tamil Nadu (Bargur Hills, Erode) | Draught & Hill Work | 400 – 800 kg |
| 6 | **Brown Swiss** | *Bos taurus* | Crossbred / High-Altitude Dairy | Dairy | 4,000 – 6,000 kg |
| 7 | **Dangi** | *Bos indicus* | Maharashtra (Nashik, Ahmednagar) | Draught (Heavy Rainfall Tract) | 500 – 900 kg |
| 8 | **Deoni** | *Bos indicus* | Maharashtra (Latur) & Karnataka | Dual-Purpose | 1,200 – 2,000 kg |
| 9 | **Gir** | *Bos indicus* | Gujarat (Saurashtra, Junagadh) | Premier Dairy | 2,000 – 3,500 kg |
| 10 | **Guernsey** | *Bos taurus* | Crossbred / Naturalized | Dairy (High Butterfat) | 3,200 – 4,800 kg |
| 11 | **Hallikar** | *Bos indicus* | Karnataka (Mysore, Mandya) | Premier Draught (Trotting) | 600 – 1,100 kg |
| 12 | **Hariana** | *Bos indicus* | Haryana (Rohtak, Jind, Hisar) | Dual-Purpose | 1,400 – 2,200 kg |
| 13 | **Holstein Friesian** | *Bos taurus* | Crossbred / Naturalized | High-Volume Dairy | 5,000 – 7,500 kg |
| 14 | **Jersey** | *Bos taurus* | Crossbred / Naturalized | Dairy (A2/High Butterfat) | 3,500 – 5,000 kg |
| 15 | **Kangayam** | *Bos indicus* | Tamil Nadu (Coimbatore, Erode) | Premier Draught | 600 – 1,000 kg |
| 16 | **Kankrej** | *Bos indicus* | Gujarat (Rann of Kutch) & Rajasthan | Dual-Purpose (Sawai Chal) | 1,500 – 2,600 kg |
| 17 | **Kasargod** | *Bos indicus* | Kerala (Kasargod District) | Dwarf / Draught | 400 – 700 kg |
| 18 | **Kenkatha** | *Bos indicus* | Madhya Pradesh & Uttar Pradesh (Bundelkhand) | Draught | 500 – 800 kg |
| 19 | **Kherigarh** | *Bos indicus* | Uttar Pradesh (Lakhimpur Kheri) | Draught | 400 – 700 kg |
| 20 | **Khillari** | *Bos indicus* | Maharashtra (Solapur, Satara) & Karnataka | Premier Draught | 400 – 800 kg |
| 21 | **Krishna Valley** | *Bos indicus* | Karnataka & Maharashtra (Krishna River Basin) | Heavy Draught | 900 – 1,400 kg |
| 22 | **Malnad Gidda** | *Bos indicus* | Karnataka (Western Ghats) | Dwarf Dairy & Manure | 300 – 600 kg |
| 23 | **Nagori** | *Bos indicus* | Rajasthan (Nagaur District) | Fast Draught & Trotting | 500 – 900 kg |
| 24 | **Nimari** | *Bos indicus* | Madhya Pradesh (Nimar Region) | Draught | 600 – 1,000 kg |
| 25 | **Ongole** | *Bos indicus* | Andhra Pradesh (Guntur, Prakasam) | Dual-Purpose / Heavy Draught | 1,200 – 2,000 kg |
| 26 | **Pulikulam** | *Bos indicus* | Tamil Nadu (Sivaganga, Madurai - Jallikattu) | Draught & Cultural Sport | 400 – 700 kg |
| 27 | **Rathi** | *Bos indicus* | Rajasthan (Bikaner, Ganganagar) | Dairy & Dual-Purpose | 1,800 – 2,800 kg |
| 28 | **Red Dane** | *Bos taurus* | Crossbred / Naturalized | Dairy | 3,800 – 5,500 kg |
| 29 | **Red Sindhi** | *Bos indicus* | Indigenous Dairy Zebu | Premier Dairy | 2,000 – 3,500 kg |
| 30 | **Sahiwal** | *Bos indicus* | Punjab, Haryana, Rajasthan, UP | Highest-Yielding Dairy Zebu | 2,200 – 4,000 kg |
| 31 | **Tharparkar** | *Bos indicus* | Rajasthan (Thar Desert) | Desert Dual-Purpose | 1,800 – 2,800 kg |
| 32 | **Umblachery** | *Bos indicus* | Tamil Nadu (Nagapattinam, Tiruvarur) | Wetland Draught | 500 – 800 kg |
| 33 | **Vechur** | *Bos indicus* | Kerala (Kottayam) | World's Smallest Cow / Medicinal Dairy | 600 – 900 kg |

---

### 🐃 Riverine Buffalo Breeds (8)
| # | Breed Name | Scientific Classification | Native Breeding Tract | Primary Utility | Typical Milk Yield (Lactation) |
|---|------------|---------------------------|-----------------------|-----------------|--------------------------------|
| 34 | **Bhadawari** | *Bubalus bubalis* | Uttar Pradesh & Madhya Pradesh | Dairy (Highest Fat: 8–13%) | 1,200 – 1,800 kg |
| 35 | **Jaffrabadi** | *Bubalus bubalis* | Gujarat (Gir Forest, Saurashtra) | Dairy & Heavy Draught | 2,500 – 4,200 kg |
| 36 | **Mehsana** | *Bubalus bubalis* | Gujarat (Mehsana, Banaskantha) | Dairy | 2,200 – 4,000 kg |
| 37 | **Murrah** | *Bubalus bubalis* | Haryana (Rohtak, Jind) & Punjab | World Premier Dairy | 2,500 – 4,200 kg |
| 38 | **Nagpuri** | *Bubalus bubalis* | Maharashtra (Vidarbha, Nagpur) | Draught & Dairy | 600 – 900 kg |
| 39 | **Nili Ravi** | *Bubalus bubalis* | Punjab (Sutlej & Ravi river tracts) | Premier Dairy | 2,000 – 2,800 kg |
| 40 | **Surti** | *Bubalus bubalis* | Gujarat (Charotar tract, Kaira, Anand) | Dairy | 1,600 – 2,200 kg |
| 41 | **Toda** | *Bubalus bubalis* | Tamil Nadu (Nilgiri Hills) | Semi-Wild Hill Dairy | 400 – 700 kg |

---

## 🏗️ Technical Architecture

```
User Browser (Razor Views + Bootstrap 5 + Vanilla JS Dropzone)
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
(model_41breed.onnx)     (Portable SQLite / SQL Server)
```

---

## 🚀 Quick Start Guide

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (v17.8+) or VS Code
- *(Optional)* Microsoft SQL Server or Docker

### 1. Clone & Build
```bash
git clone https://github.com/shivangchoubey/BreedVision.git
cd BreedVision
dotnet build
```

### 2. Run the Automated Verification Suite
To execute the automated diagnostic suite (validating the 41-breed model, classes schema, SQLite auto-seeding, and live inference on sample photos):
```bash
dotnet run --project tests/BreedVision.Tests
```

### 3. Run the Web Application
```bash
dotnet run --project src/BreedVision.Web
```
Open your browser and navigate to:
**`http://localhost:5004`**

---

## 🧪 Testing with Pre-Verified Samples

To test the application instantly without searching for animal photos, use the pre-loaded research test buttons on the homepage or test endpoints:
- **Gir Cattle Sample**: Gujarat premier dairy zebu
- **Bhadawari Buffalo Sample**: High-butterfat riverine buffalo
- **Rathi Cattle Sample**: Rajasthan arid-tract dairy breed

---

## 🚢 Cloud & Container Deployment

### Option A: Render Cloud Deployment (1-Click)
This repository includes a pre-configured [render.yaml](render.yaml) Blueprint:
1. Go to [dashboard.render.com](https://dashboard.render.com).
2. Click **New +** $\rightarrow$ **Web Service**.
3. Select this repository: `shivangchoubey/BreedVision`.
4. Render automatically detects the [Dockerfile](Dockerfile) and deploys it on the **Free Tier**.

### Option B: Docker Container
```bash
docker build -t breedvision:latest .
docker run -d -p 8080:8080 --name breedvision_app breedvision:latest
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
├── docker-compose.yml           # Local multi-container deployment
├── render.yaml                  # 1-Click Render.com Cloud Blueprint
├── .dockerignore                # Container build optimization
├── BreedVision.sln              # Visual Studio solution
├── src/
│   └── BreedVision.Web/
│       ├── Controllers/         # MVC Controllers (Home, Breed, Catalog, History)
│       ├── Data/                # EF Core DbContext & 41-Breed DbInitializer
│       ├── Models/              # Domain Entities & ViewModels
│       ├── Services/            # ONNX classifier, catalog services, local storage
│       ├── ML/Assets/           # model_41breed.onnx (26.9MB), classes.json, samples
│       ├── Views/               # Razor views (Identify, Result, Catalog, History)
│       └── wwwroot/             # Static CSS, JavaScript, and uploads
└── tests/
    └── BreedVision.Tests/       # 6-step automated diagnostic & verification suite
```

---

## ⚖️ License & Acknowledgments
- Model backbone based on **ConvNeXt-Tiny** fine-tuned on indigenous Indian bovine breeds.
- Breed standards and accession identifiers referenced from the **ICAR - National Bureau of Animal Genetic Resources (NBAGR)**, Karnal, India.
