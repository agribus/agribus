#!/usr/bin/env bash

# Usage:
#   ./migrate.sh --env dev
#   ./migrate.sh --env preprod
#   ./migrate.sh --env prod

ENV_CHOICE=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --env)
      ENV_CHOICE="${2:-}"
      shift 2;;
    -h|--help)
      echo "Usage: $0 --env dev|preprod|prod"
      return;;
    *)
      echo "Unknown arg: $1" >&2
      return;;
  esac
done

if [[ -z "${ENV_CHOICE}" ]]; then
  echo "Missing required argument: --env dev|preprod|prod" >&2
  return
fi

case "$ENV_CHOICE" in
  dev)     ENV_FILE=".env" ;;
  preprod) ENV_FILE=".env.preprod" ;;
  prod)    ENV_FILE=".env.prod" ;;
  *)
    echo "Invalid --env value: ${ENV_CHOICE} (use dev|preprod|prod)" >&2
    return;;
esac

if [[ ! -f "${ENV_FILE}" ]]; then
  echo "Env file not found: ${ENV_FILE}" >&2
  return
fi

echo "Using env file: ${ENV_FILE}"

# Extract ConnectionStrings__Postgres
ConnectionStrings__Postgres="$(
  grep -E '^[[:space:]]*ConnectionStrings__Postgres[[:space:]]*=' "${ENV_FILE}" \
  | sed -E 's/^[^=]+=[[:space:]]*//; s/^["'\''"]//; s/["'\''"]$//'
)"

if [[ -z "${ConnectionStrings__Postgres}" ]]; then
  echo "ConnectionStrings__Postgres not found in ${ENV_FILE}" >&2
  return
fi

export ConnectionStrings__Postgres

# Build and run
IMAGE="agribus.migration:latest"
DOCKERFILE="apps/tomato_backend/Dockerfile.migration"
BUILD_CTX="apps/tomato_backend"

echo "Building image: ${IMAGE}"
docker buildx build -f "${DOCKERFILE}" -t "${IMAGE}" "${BUILD_CTX}"

echo "Running migrations..."
docker run --rm --network agribus-network \
  -e "ConnectionStrings__Postgres=${ConnectionStrings__Postgres}" \
  "${IMAGE}"
