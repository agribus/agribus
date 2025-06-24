# Agribus.Core – Project Structure Overview

This folder contains the core domain logic of the Agribus system, structured as a clean, modular .NET class library.

---

## 📁 Project Structure

```

src/
└── Agribus.Core/
├── Agribus.Core.csproj       # Main project file
├── Domain/                   # Domain Layer
│   ├── Entities/             # Core business models (e.g., Farm, Sensor)
│   ├── ValueObjects/         # Immutable value types (e.g., Coordinates)
│   └── Exceptions/           # Domain-specific exceptions
├── Features/                 # Business use cases (currently placeholder)
├── Ports/                    # Abstractions for infrastructure (Hexagonal/Onion)
│   ├── Api/                  # API-facing interfaces
│   └── Spi/                  # SPI (Service Provider Interface) for adapters
├── bin/                      # Build outputs (gitignored)
└── obj/                      # Intermediate build files (gitignored)

```

---

## 🧠 Key Concepts

- **Domain**: Pure business logic — the heart of the system.
- **Ports**: Interface contracts, for use with adapters (DB, messaging, etc.).
- **Features**: Use case entrypoints.
- **Classlib only**: this project is intended to be consumed by APIs, workers.

---

## 🐳 Docker (TODO)

---

## 🔧 Next Steps

Once this core library stabilizes, new projects can reference it:

- `Agribus.Api` – ASP.NET Core API (planned)
- `Agribus.PostgreSQL`– (planned)
- `Agribus.InfluxDB`– (planned)
