# lychee_client – Frontend Monorepo (Web & Mobile)

This folder contains the **web** and **mobile** applications of the Agribus ecosystem, structured as a modular, maintainable **Angular + Capacitor** monorepo.

Built with `pnpm`, powered by `TurboRepo` for fast dev & CI workflows.

---

## 📁 Project Structure

```txt

lychee_client/
├── package.json                      # Root dependencies & scripts (shared across apps)
├── tsconfig.json                     # Shared TypeScript config
├── angular.json                      # Angular workspace config
├── src/                              # Application-level entry points
│   └── app/                          # Web frontend (Angular standalone app) 
│   │   ├── components /
│   │   ├── directives /       
│   │   ├── guards /
│   │   ├── intefaces / 
│   │   ├── pipes / 
│   │   ├── services / 
│   │   └── utils /  
│   └── environments /                  # Env config for Angular (dev, prod)
│       ├── environment.production.ts   # Production environment settings
│       └── environment.ts              # Base environment config (used for switching)
│
└── dist/                      # Build outputs (gitignored)

```

---

## 🧠 Key Principles

- **Single Codebase**: Web and mobile apps share the same code through `common/`.
- **Capacitor**: Used to wrap the Angular web app into native mobile apps (Android/iOS).
- **Modular Angular**: Structured around standalone components and feature modules.

---

## 🔧 Development Tips

- Use `apps/common` for any reusable logic or UI components.
- Keep feature modules under `common/modules/` to reduce duplication.
- Build the web app **before** syncing to Capacitor via `pnpm build --filter=lychee && pnpm capacitor copy`.

---

## 📱 Native Builds

After syncing the web app:

```bash
pnpm capacitor open android
# or
pnpm capacitor open ios
```
