# Agribus.Core – Project Structure Overview

This folder contains the core domain logic of the Agribus system, structured as a clean, modular .NET class library.

---

## 📁 Project Structure

```txt

src/
└── Agribus.Core/
├── Agribus.Core.csproj       # Main project file
├── Domain/                   # Domain Layer
│   ├── AggregatesModels/             # Core business models (e.g., Greenhouse, Widgets)
│   ├── ValueObjects/         # Immutable value types (e.g., Coordinates)
│   └── Exceptions/           # Domain-specific exceptions
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

---

## 🔧 Next Steps

Once this core library stabilizes, new projects can reference it:

- `Agribus.Api` – ASP.NET Core API (DONE)
- `Agribus.PostgreSQL`– (planned)
- `Agribus.InfluxDB`– (DONE)
