\# Contributing to WikiUnach



Thank you for your interest in contributing to WikiUnach!



\## Branch Strategy



This project follows a simplified Git Flow:



| Branch | Purpose |

|---|---|

| `main` | Stable, production-ready code |

| `develop` | Integration branch — all features merge here first |

| `feature/\*` | Individual features or improvements |

| `hotfix/\*` | Critical bug fixes applied directly from main |



\## How to Contribute



\### 1. Fork and Clone

```bash

git clone https://github.com/axelramos54/WikiUnach.git

cd WikiUnach

```



\### 2. Create a Feature Branch

```bash

git checkout develop

git checkout -b feature/your-feature-name

```



\### 3. Make Your Changes

\- Follow the existing code style (C# naming conventions)

\- Never commit `App.config` with real credentials

\- Test your changes with a real AWS connection before submitting



\### 4. Commit With Clear Messages

```bash

git commit -m "feat: describe what your feature does"

```



\*\*Commit message prefixes:\*\*



| Prefix | Use for |

|---|---|

| `feat:` | New feature |

| `fix:` | Bug fix |

| `docs:` | Documentation changes |

| `refactor:` | Code restructure without behavior change |

| `chore:` | Build, config, or tooling changes |



\### 5. Push and Open a Pull Request

```bash

git push origin feature/your-feature-name

```



Then open a Pull Request from your branch → `develop` on GitHub.



\## Security Rules



\- 🚫 Never commit `App.config` with real AWS or SMTP credentials

\- 🚫 Never push to `main` directly

\- ✅ Always use `App.config.template` for configuration examples

\- ✅ Always run `git status` before `git add`



\## Questions?



Open an issue on GitHub or contact the team directly.

