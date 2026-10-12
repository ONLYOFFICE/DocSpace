## Project Overview

ONLYOFFICE DocSpace — umbrella/meta repository for the DocSpace product (rooms-based document collaboration platform). Contains no source code of its own: the client, server, and buildtools live in git submodules pointing at sibling repositories.

## Tech Stack

Git submodules, GitHub Actions (release workflow). Actual product stacks: TypeScript/React (client), C#/.NET (server), Docker/Bash (buildtools).

## Project Structure

```
client/             — Submodule → DocSpace-client (frontend, pnpm/Nx monorepo)
server/             — Submodule → DocSpace-server (ASP.NET Core microservices)
buildtools/         — Submodule → DocSpace-buildtools (Docker, packaging, CI/CD)
tests/e2e-tests/    — Submodule → DocSpace-e2e-tests (branch main)
tests/stress-tests/ — Submodule → DocSpace-stress-tests (branch develop)
tests/api-tests/    — Submodule → DocSpace-api-tests (branch master)
.github/workflows/  — release.yml (release automation)
CHANGELOG.md        — Product changelog
```

## Build & Run

```bash
# Clone with submodules
git clone --recursive <repo-url>

# Initialize/update submodules after clone or branch switch
git submodule update --init

# Building and running is done from the buildtools submodule
# (see buildtools/CLAUDE.md)
```

## Key Patterns

- `client`, `server`, `buildtools` submodules track the superproject branch (`branch = .`, `update = merge`) — keep all three on the same release branch as this repo
- `tests/*` submodules track fixed branches (`update = checkout`)
- Release branches here pin the matching submodule commits for a product release
- All real development happens in the sibling repos (`DocSpace-client`, `DocSpace-server`, `DocSpace-buildtools`); this repo only aggregates

## Review Focus

**Submodules**: Correct submodule commit pins on release branches; all three product submodules on the same branch
**CI/CD**: `release.yml` workflow logic

## Git Workflow

- **Main branch**: `master`
- **Integration branch**: `develop`
- **Branch naming**: `feature/*`, `release/*`
- **Submodules**: run `git submodule update --init` after clone or branch switch
