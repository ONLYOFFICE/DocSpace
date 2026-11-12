# ONLYOFFICE Apps

<p align="center">
  <a href="https://www.onlyoffice.com/signup?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=header">
    <img width="840" src="https://static-blog.onlyoffice.com/wp-content/uploads/2026/10/01171512/ONLYOFFICE-Apps.png" alt="ONLYOFFICE Apps">
  </a>
</p>

[![Release Notes](https://img.shields.io/github/release/ONLYOFFICE/DocSpace?style=flat-square)](https://github.com/ONLYOFFICE/DocSpace/releases)
[![License](https://img.shields.io/badge/license-AGPLv3-orange)](https://docspace.onlyoffice.com/s/gnC2xcxWjHhHmsM)
[![GitHub stars](https://img.shields.io/github/stars/ONLYOFFICE/DocSpace?style=flat-square)](https://star-history.com/#ONLYOFFICE/DocSpace)
[![Open Issues](https://img.shields.io/github/issues-raw/ONLYOFFICE/DocSpace?style=flat-square)](https://github.com/ONLYOFFICE/DocSpace/issues)
[![ONLYOFFICE Community](https://img.shields.io/badge/ONLYOFFICE%20Community-Discuss-white?style=social&logo=onlyoffice&color=white)](https://community.onlyoffice.com/c/apps/46)
[![Twitter](https://img.shields.io/twitter/url/https/twitter.com/ONLYOFFICE.svg?style=social&label=Follow%20%40ONLYOFFICE)](https://x.com/only_office)
[![YouTube Channel](https://img.shields.io/youtube/channel/subscribers/UCNxeQm7vVujR8eFPtpVAVNg?label=Subscribe)](https://www.youtube.com/@Onlyoffice_Community)

## Table of Contents

- [What are ONLYOFFICE Apps?](#-what-are-onlyoffice-apps)
- [What's new in 4.0](#-whats-new-in-40)
- [Key Features](#-what-makes-onlyoffice-apps-a-must-try)
- [Security](#-how-secure-is-my-data)
- [Get ONLYOFFICE Apps](#-how-can-i-get-onlyoffice-apps)
- [Integrations](#-using-onlyoffice-apps-inside-your-platform)
- [Repository Structure](#-repository-structure)
- [Technology Stack](#️-technology-stack)
- [Single-container installation](#single-container-installation)
- [Building from Source](#-building-from-source)
  - [Prerequisites](#prerequisites)
  - [System Requirements](#minimum-system-requirements)
  - [Quick Start](#quick-start)
  - [Development](#development)
- [Community & Support](#-need-help-or-have-an-idea)

## **💡 What are ONLYOFFICE Apps?**

Welcome to the official GitHub repository for [**ONLYOFFICE Apps**](https://www.onlyoffice.com/blog/2026/10/onlyoffice-docspace-apps?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=intro)!

ONLYOFFICE Apps are modular building blocks that extend [ONLYOFFICE Docs](https://github.com/ONLYOFFICE/DocumentServer): **AI agents, Forms, Rooms, and Files**. Each app solves an everyday problem, and you turn on only what you need. Apps and editors work as one flexible system: install the editors, extend them with apps, and integrate the package into your own solution. No technical skills required, so you can get going in minutes. Starting with version 4.0, released alongside ONLYOFFICE Docs 10.0, ONLYOFFICE Apps replace DocSpace.

**[Start with a free account ↗](https://www.onlyoffice.com/signup?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=intro)**

This repository contains the source code for the backend and frontend components of ONLYOFFICE Apps.

## **🆕 What's new in 4.0**

* **From one fixed space to modular apps:** AI agents, Forms, Rooms, and Files are now separate apps you can mix and match. In the cloud, all apps are activated by default; on server builds, a quick installation gets you set up. Existing DocSpace accounts move to the new structure automatically, with no action needed.
* **AI chat in every app:** work with rooms, files, and forms faster, with workspace automation, quick file generation, intelligent search, and a personal knowledge base.
* **Bigger knowledge base for AI agents:** now supports TXT, MD, JSON, CSV, Word, Excel, and PowerPoint formats (including macro-enabled files and templates), and PDF.
* **Forms app:** start from auto-generated forms or ready-made templates, collect responses in a database or spreadsheet, and let AI analyze the results.
* **Files app:** one home for all your documents, with quick search and filters, folders and tags, and precise access settings.
* **AI in the cloud, ready in one click:** no API keys, no AI subscription, and a choice of leading models. Note: custom API keys set up in the cloud before this release are no longer used; you can keep using them in the free Desktop Editors.
* **Works with ONLYOFFICE Docs 10.0:** AI tools in every editor, a smarter PDF Editor, faster spreadsheets, and 40+ security fixes.

👉 [See what's changed](https://www.onlyoffice.com/blog/2026/10/onlyoffice-docspace-apps?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=whats_new) · [Meet ONLYOFFICE Docs 10.0](https://www.onlyoffice.com/blog/2026/10/onlyoffice-docs-10?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=whats_new)

## **🚀 What makes ONLYOFFICE Apps a must try!**

### 1. 🤖 AI agents: your smart helpers

**AI Chat across all apps.** Work faster with rooms, files, and forms: open the chat as a side panel in any room, folder, or document. Your conversations follow you across the workspace.

* **Create ready-to-use files:** ask for a report, a fillable PDF form, a presentation, or an image, and get the file right in your workspace.
* **Work with files and images:** attach documents, scans, receipts, or screenshots and ask AI to extract, summarize, or analyze them.
* **Search the web:** get up-to-date answers with clickable sources.

**AI agents for your team.** Create custom agents in their own dedicated section for a wide range of tasks. Give an agent instructions and a knowledge base built from your own documents, then invite colleagues with different roles. Call any agent from the AI Chat in any room or folder.

**Extend AI with MCP.** With the built-in [ONLYOFFICE MCP Server](https://www.onlyoffice.com/mcp-server?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=ai), AI can create rooms, organize files, and manage users right from the chat. Connect external MCP servers too, and choose how much freedom AI gets with tool approval modes.

**AI in the cloud.** Start in one click, with no AI provider sign-up or API keys. There is no AI subscription: you pay only for what you use from your workspace Wallet. Choose from a hand-picked selection of models from OpenAI, Anthropic, Google, xAI, DeepSeek, and more, and assign a different model to each task, such as chat, summarization, translation, OCR, or image generation.

<p align="center">
  <a href="https://www.onlyoffice.com/ai-assistants?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=ai">
    <img width="840" src="https://static-blog.onlyoffice.com/wp-content/uploads/2026/10/12140753/AI-agents.png" alt="AI agents in ONLYOFFICE Apps">
  </a>
</p>

💡 **Want your own API keys or local models?** Connect your cloud to the free [ONLYOFFICE Desktop Editors](https://www.onlyoffice.com/download-desktop?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=ai) and use any AI provider, or run models locally with Ollama or LM Studio.

👉 [10 things to know about AI in ONLYOFFICE Cloud](https://www.onlyoffice.com/blog/2026/10/cloud_ai_10?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=ai) · [Learn more about AI tools](https://www.onlyoffice.com/ai-assistants?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=ai)

### 2. 📋 Forms: collect and understand data with ease
* Never start from a blank page: pick an auto-generated form or a ready-made template, tweak it, and share. You can also ask AI to build a fillable PDF form from a short description.
* Responses land neatly in a database or spreadsheet, all organized for you.
* AI analyzes the results and helps you understand every answer.

👉 [Explore Forms](https://www.onlyoffice.com/forms?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features)

### 3. 👥 Rooms: teamwork, your way
Rooms are built for working together, with a room type for every situation. Each room comes with preset permissions and roles.

* **[Collaboration rooms](https://www.onlyoffice.com/collaboration-rooms?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features):** Invite users to co-author and edit documents in real-time.
* **[Public rooms](https://www.onlyoffice.com/public-rooms?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features):** Share documents for view-only access with external users via a public link.
* **[Custom rooms](https://www.onlyoffice.com/custom-rooms?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features):** Define your own access permissions, whether it's for review, commenting, or form filling.
* **[Virtual data rooms](https://www.onlyoffice.com/virtual-data-rooms?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features):** Automatically index and track all content. You can enable watermarks, set file lifetime, restrict downloading and copying.

AI helps you find, manage, and keep everything tidy.

👉 [Explore Rooms](https://www.onlyoffice.com/rooms?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features)

### 4. 🗂 Files: everything in one place
* Store and manage all your documents in a single spot.
* Find what you need in seconds with quick search and filters.
* Organize with folders and tags, and decide exactly who sees what.
* No more digging through endless folders: AI helps you track down the file you need.

👉 [Explore Files](https://www.onlyoffice.com/files?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features)

### 5. 📝 Work with any content you have
ONLYOFFICE Apps are integrated with our complete online office suite, so you can work with dozens of formats.

- [Text documents](https://www.onlyoffice.com/word-processor?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features) 📝
- [Spreadsheets](https://www.onlyoffice.com/sheets?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features) 📊
- [Presentations](https://www.onlyoffice.com/slides?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features) 📽️
- [PDFs](https://www.onlyoffice.com/pdf-editor?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features) ✂️
- [Forms](https://www.onlyoffice.com/form-creator?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features) 📋
- [Diagrams](https://www.onlyoffice.com/diagram-viewer?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features) 🖼️
- [eBooks](https://www.onlyoffice.com/ebook?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features) 📖
- Markdown files 📁
- Multimedia 🎵
- Programming files and archives 🧑‍💻

### 6. 📱 Work from anywhere
Access your workspace from the free desktop apps for Windows, macOS, and Linux, and from the mobile apps for iOS and Android, now with an AI assistant, AI AutoFill for forms, and voice input.

👉 [Get desktop and mobile apps](https://www.onlyoffice.com/download-desktop?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features)

### 7. 🧩 Make use of extra features
Easily extend what the apps can do.

* Connect third-party services like Dropbox, Google Drive, Zoom, etc.
* Activate system plugins or add your own.

Discover plugins in the [Marketplace ↗](https://www.onlyoffice.com/marketplace?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=features).

## **🔒 How secure is my data?**

Collaborate with peace of mind knowing your data is protected by multiple layers of security.

| Feature | Description |
| :---- | :---- |
| **Compliance with standards** | Our data security policy is fully compliant with GDPR standards. |
| **Open source** | We affirm transparency and reliability by opening the source code of all functional modules and tools. |
| **Encryption** | We use industry-leading AES-256 encryption for data at rest and HTTPS/TLS for data in transit. |
| **Secure access & monitoring** | Flexible access rights and JWT let you entirely control document access. Activity tracking and audit reporting provide traceability. |

📚 Learn more about all [ONLYOFFICE security features ↗](https://www.onlyoffice.com/security?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=security).

## **🌐 How can I get ONLYOFFICE Apps?**

On your own server, install ONLYOFFICE Docs first and then extend it with the apps you need. In the cloud, everything is ready to go. Choose the option that best suits your needs.

| Option | Description | Get started |
| :---- | :---- | :---- |
| 🏢 **[ONLYOFFICE Docs Enterprise + Apps](https://www.onlyoffice.com/docs-enterprise?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=get)** | Install ONLYOFFICE Docs Enterprise on your own server and extend it with apps for total control over your documents. Highly scalable to grow with your business. | [Get it now](https://www.onlyoffice.com/download?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=get#for-enterprises) |
| ⚙️ **[ONLYOFFICE Docs Developer + Apps](https://www.onlyoffice.com/for-developers?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=get)** | Integrate ONLYOFFICE Docs Developer into your web app and extend it with apps to give your users a secure space for storing content and collaborating on documents. | [Get it now](https://www.onlyoffice.com/download?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=get#for-developers) |
| ☁️ **Business Cloud** | A hassle-free cloud solution with bigger storage, enhanced security, and professional support. Pay for admins only and invite users for free. | [Check prices](https://www.onlyoffice.com/pricing?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=get#cloud) |
| 🚀 **Free cloud for startups** | Create a free cloud space for your documents. Invite up to 3 admins and collaborate on docs anywhere. | [Create now](https://www.onlyoffice.com/signup?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=get) |
| 🎓 **Free cloud for schools and non-profits** | Create a free cloud space for your school or non-profit with 2 GB of storage and up to 20 admin accounts. | [Submit request](https://www.onlyoffice.com/free-cloud?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=get) |

## 🔗 Using ONLYOFFICE Apps inside your platform

Connect ONLYOFFICE Apps to collaborate on office documents directly from your business platform: [Zoom](https://www.onlyoffice.com/office-for-zoom?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=integrations), [Pipedrive](https://www.onlyoffice.com/office-for-pipedrive?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=integrations), [Drupal](https://www.onlyoffice.com/office-for-drupal?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=integrations), [WordPress](https://www.onlyoffice.com/office-for-wordpress?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=integrations), [Moodle](https://www.onlyoffice.com/office-for-moodle?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=integrations#materials), [monday](https://www.onlyoffice.com/monday?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=integrations), etc.

Make use of [Zapier integration](https://www.onlyoffice.com/office-for-zapier?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=integrations) to connect ONLYOFFICE Apps with thousands of tools your team uses.

**Building on ONLYOFFICE Docs?** The modular apps plug into the editors, so you can enable only what your users need. See [ONLYOFFICE 10.0 for developers ↗](https://www.onlyoffice.com/blog/2026/10/onlyoffice-10-developers?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=integrations).

Discover all [integrations ↗](https://www.onlyoffice.com/all-connectors?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=integrations)

## **🧱 Repository Structure**

This repository is organized as a set of Git submodules:

| Folder | Description | README |
| :---- | :---- | :---- |
| [`server`](server/) | Backend: C# 14.0, .NET 10.0, ASP.NET Core, .NET Aspire, MySQL, Redis, RabbitMQ, OpenSearch | [Server README](https://github.com/ONLYOFFICE/DocSpace-server#readme) |
| [`client`](client/) | Frontend: TypeScript 5.9, React 19, MobX 6, Vite 6, Webpack 5, Next.js, pnpm monorepo with Nx | [Client README](https://github.com/ONLYOFFICE/DocSpace-client#readme) |
| [`buildtools`](buildtools/) | Build scripts, Docker configurations, CI/CD pipelines, installation packages, app configuration | [Build Tools README](https://github.com/ONLYOFFICE/DocSpace-buildtools#readme) |
| `.github/workflows` | GitHub Actions workflows for CI/CD and issue templates | |

## **🛠️ Technology Stack**

**Backend:** C# 14.0, .NET 10.0 / ASP.NET Core, .NET Aspire 13.1, MySQL 9.5, Redis, RabbitMQ, OpenSearch, OpenTelemetry

**Frontend:** TypeScript 5.9, React 19, MobX 6, CSS/SASS (SCSS modules), i18next, Vite 6, Webpack 5, Next.js

**Infrastructure:** Docker 28.5+, Nginx (OpenResty), pnpm 10.28+, Nx

## Single-container installation

The standalone deployment runs all ONLYOFFICE Apps services in one container, alongside optional containers for the editors, MySQL, and OpenSearch. It is the default topology of the installation script for every edition: Community, Enterprise, and Developer. If you need to scale out horizontally, use the microservices or stack topology instead. [Check the instructions ➡️](https://github.com/ONLYOFFICE/DocSpace-buildtools/tree/master/install/docker/standalone)

## **🚀 Building from Source**

> **Note:** This creates a **development/testing environment** not suitable for production use.
> For production deployment, see [How can I get ONLYOFFICE Apps?](#-how-can-i-get-onlyoffice-apps)

### Prerequisites

| Tool | Version | Verification Command |
|------|---------|---------------------|
| [Node.js](https://nodejs.org/) | >= 24 | `node --version` |
| [pnpm](https://pnpm.io/) | >= 10.28.0 | `pnpm --version` |
| [.NET SDK](https://dotnet.microsoft.com/download) | 10.0 | `dotnet --version` |
| [Docker](https://www.docker.com/) | >= 28.5.0 | `docker --version` |

### Minimum System Requirements

- **CPU:** Dual-core 2 GHz or higher
- **RAM:** 8 GB or more
- **SSD:** At least 40 GB of free space
- **Swap file:** At least 2 GB
- **OS:** macOS, Linux, or Windows with WSL2

### Quick Start

**1. Clone the repository with all submodules:**

```bash
git clone --recurse-submodules https://github.com/ONLYOFFICE/DocSpace && \
cd DocSpace && \
git submodule foreach "git checkout master"
```

**2. Start the preview**:

```bash
cd server/common/ASC.AppHost
dotnet run --launch-profile preview
```

This will launch a minimal Docker-based setup.

> **More details:** [Server](https://github.com/ONLYOFFICE/DocSpace-server#readme) (launch profiles, backend architecture) · [Client](https://github.com/ONLYOFFICE/DocSpace-client#readme) (frontend modes, packages) · [Build Tools](https://github.com/ONLYOFFICE/DocSpace-buildtools#readme) (Docker, CI/CD, installation)

**3. Open the application:**

| URL | Description |
|-----|-------------|
| http://localhost:8092 | ONLYOFFICE Apps |
| http://localhost:15208 | Aspire Dashboard (backend services monitoring) |
| http://localhost:8092/scalar/#ascfiles | API Documentation (Scalar) |
| http://localhost:56161 | DB Gate (database management) |
| http://localhost:56162 | Mailpit (email testing) |

### Development

To work on the frontend, run the backend and the frontend in two terminals, starting from the repository root:

```bash
# Terminal 1: all backend services, without the client build
cd server/common/ASC.AppHost
dotnet run --launch-profile frontend-dev
```

```bash
# Terminal 2: the frontend dev server
cd client
pnpm install && pnpm start
```

The application is available at http://localhost:8092. For the full backend setup with all services, use `dotnet run --launch-profile development` instead.

- **Frontend (TypeScript/React):** VSCode workspace with task buttons for one-click start — see [Client README](https://github.com/ONLYOFFICE/DocSpace-client#development-with-vscode)
- **Backend (C#/.NET):** VSCode with C# Dev Kit and .NET Aspire extensions — see [Server README](https://github.com/ONLYOFFICE/DocSpace-server#development-with-vscode)

### Stopping Services

- Press `Ctrl+C` in the backend terminal to stop all Aspire-managed services
- Press `Ctrl+C` in the frontend terminal to stop the dev server

To completely remove Docker artifacts, see the [Server README](https://github.com/ONLYOFFICE/DocSpace-server#clear-aspire-docker-artifacts).

## 💡 Need help or have an idea?

We ❤️ community contributions!

* **🐞 Found a bug?** Please report it by creating an [issue](https://github.com/ONLYOFFICE/DocSpace/issues).
* **❓ Have a question?** Ask other users and our developers in the [ONLYOFFICE Community](https://community.onlyoffice.com/).
* **🥷 Want to be a power user?** Visit our [YouTube channel](https://www.youtube.com/onlyofficeTV) and [Help Center](https://helpcenter.onlyoffice.com/docspace/getting-started?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=support).
* **👨‍💻 Need help for developers?** Check our [API documentation](https://api.onlyoffice.com/?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=support).
* **💡 Want to suggest a feature?** Share your ideas [here](https://community.onlyoffice.com/c/suggestions/40).

**Ready to try the new ONLYOFFICE Apps?** 👉 [Create your free account](https://www.onlyoffice.com/signup?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace&utm_content=footer_cta) ✨

---
<p align="center"> Made with ❤️ by the ONLYOFFICE Team </p>