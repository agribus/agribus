# Script de migration

Ce dépôt fournit un script (`migrate.sh`) pour exécuter les migrations de base de données dans un conteneur Docker.

---

## Utilisation

Choisis l’environnement avec l’option `--env` :

```bash
# Développement local
source migrate.sh --env dev

# Pré-production
source migrate.sh --env preprod

# Production
./migrate.sh --env prod
````

Options possibles :

* `dev` → utilise `.env`
* `preprod` → utilise `.env.preprod`
* `prod` → utilise `.env.prod`

---

## Fonctionnement

1. Lit le fichier `.env` correspondant et récupère la variable `ConnectionStrings__Postgres`.
2. Construit l’image Docker de migration :

   ```
   agribus.migration:latest
   ```
3. Lance le conteneur sur le réseau Docker `agribus-network` en lui passant la chaîne de connexion comme variable d’environnement.
4. Nettoie automatiquement le conteneur après exécution (`--rm`).

---

## Notes

* Le fichier `.env` approprié (`.env`, `.env.preprod`, `.env.prod`) doit exister et contenir la variable :

  ```
  ConnectionStrings__Postgres=postgres://...
  ```
* Prérequis :

    * Docker avec Buildx activé
    * Réseau Docker `agribus-network` existant

## Troubleshooting

* **"Env file not found"** → Vérifie que le fichier `.env.*` existe.
* **"ConnectionStrings\_\_Postgres not found"** → Vérifie que la variable est bien définie dans le fichier `.env`.
* **Erreur de réseau Docker** → Assure-toi que `agribus-network` existe (`docker network ls`).