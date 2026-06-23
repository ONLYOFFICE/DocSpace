# ONLYOFFICE DocSpace

<p align="center">
  <a href="https://www.onlyoffice.com/docspace?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace">
    <img width="840" src="https://static-site.onlyoffice.com/public/images/templates/docspace/hero/yt-cover.png" alt="ONLYOFFICE DocSpace">
  </a>
</p>

[![Release Notes](https://img.shields.io/github/release/ONLYOFFICE/DocSpace?style=flat-square)](https://github.com/ONLYOFFICE/DocSpace/releases)
[![License](https://img.shields.io/badge/license-AGPLv3-orange)](https://opensource.org/license/agpl-v3)
[![GitHub stars](https://img.shields.io/github/stars/ONLYOFFICE/DocSpace?style=flat-square)](https://star-history.com/#ONLYOFFICE/DocSpace)
[![Open Issues](https://img.shields.io/github/issues-raw/ONLYOFFICE/DocSpace?style=flat-square)](https://github.com/ONLYOFFICE/DocSpace/issues)
[![DocSpace Forum](https://img.shields.io/badge/DocSpace%20Forum-Discuss-white?style=social&logo=onlyoffice&color=white)](https://forum.onlyoffice.com/c/docspace/46)
[![Twitter](https://img.shields.io/twitter/url/https/twitter.com/ONLYOFFICE.svg?style=social&label=Follow%20%40ONLYOFFICE)](https://x.com/only_office)
[![YouTube Channel](https://img.shields.io/youtube/channel/subscribers/UCNxeQm7vVujR8eFPtpVAVNg?label=Subscribe)](https://www.youtube.com/@Onlyoffice_Community)

## Table of Contents

- [What is DocSpace?](#-what-is-docspace)
- [Key Features](#-what-makes-docspace-a-must-try)
- [Security](#-how-secure-is-my-data)
- [Get DocSpace](#-how-can-i-get-docspace)
- [Integrations](#-using-docspace-inside-your-platform)
- [Repository Structure](#-repository-structure)
- [Technology Stack](#️-technology-stack)
- [Building from Source](#-building-from-source)
  - [Prerequisites](#prerequisites)
  - [System Requirements](#minimum-system-requirements)
  - [Quick Start](#quick-start)
  - [Development](#development)
- [Community & Support](#-need-help-or-have-an-idea)

## **💡 What is DocSpace?**

Welcome to the official GitHub repository for [**ONLYOFFICE DocSpace**](https://www.onlyoffice.com/docspace?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace)!

DocSpace is a secure, collaborative platform that allows you to create flexible, room-based environments to store, share, and co-author documents of any kind.

Whether you're working with your team, clients, or partners, DocSpace provides the tools you need to get work done efficiently and securely.

**[Start with a free account ↗](https://www.onlyoffice.com/docspace-registration?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace)**

This repository contains the source code for the backend and frontend components of DocSpace.

## **🚀 What makes DocSpace a must try!**

### 1. 👥 Flexible, room-based collaboration
Create customizable rooms for any purpose. Each room comes with preset permissions and roles to streamline your workflows.

* **[Collaboration rooms](https://www.onlyoffice.com/collaboration-rooms?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace):** Invite users to co-author and edit documents in real-time.
* **[Public rooms](https://www.onlyoffice.com/public-rooms?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace):** Share documents for view-only access with external users via a public link.
* **[Custom rooms](https://www.onlyoffice.com/custom-rooms?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace):** Define your own access permissions, whether it's for review, commenting, or form filling.
* **[Form filling rooms](https://www.onlyoffice.com/form-filling-rooms?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace):** Upload PDF forms from your device or template library. Invite users or share a public link to collect responses — automatically organized in a spreadsheet.
* **[Virtual data rooms](https://www.onlyoffice.com/virtual-data-rooms?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace):** Automatically index and track all content. You can enable watermarks, set file lifetime, restrict downloading and copying.

### 2. 📝 Work with any content you have
DocSpace is integrated with our complete online office suite, allowing you to work with dozens of formats.

- [Document Editor](https://www.onlyoffice.com/document-editor?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace) 📝
- [Spreadsheet Editor](https://www.onlyoffice.com/spreadsheet-editor?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace) 📊
- [Presentation Editor](https://www.onlyoffice.com/presentation-editor?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace) 📽️
- [PDF Editor](https://www.onlyoffice.com/pdf-editor?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace) ✂️
- [Form Creator](https://www.onlyoffice.com/form-creator?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace) 📋
- [Diagram Viewer](https://www.onlyoffice.com/diagram-viewer?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace) 🖼️
- [E-books reader](https://www.onlyoffice.com/e-book?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace) 📖
- [Markdown files](https://www.onlyoffice.com/app-directory/markdown?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace) 📁
- Multimedia 🎵
- Programming files and archives 🧑‍💻

### 3. 🤖 Work faster with AI agents

* Chat and search with AI
* Analyze files from your DocSpace
* Manage files and rooms
* Invite teammates for AI-powered collaboration
* Do even more with [MCP server](https://www.onlyoffice.com/mcp-server?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace) integration
* Enable AI assistants in your docs

<p align="center">
  <a href="https://www.onlyoffice.com/ai-assistants?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace">
    <img width="840" src="https://static-site.onlyoffice.com/public/images/templates/docspace/ai/en/manage_files@2x.png" alt="AI agents in ONLYOFFICE DocSpace">
  </a>
</p>

Learn more about the [AI tools ↗](https://www.onlyoffice.com/ai-assistants?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace).

### 4. 🧩 Make use of extra features
Easily extend DocSpace's functionality.

* Connect third-party services like Dropbox, Google Drive, Zoom, etc.
* Activate system plugins or add your own.

Discover plugins in the [App Directory ↗](https://www.onlyoffice.com/app-directory?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace).

## **🔒 How secure is my data?**

Collaborate with peace of mind knowing your data is protected by multiple layers of security.

| Feature | Description |
| :---- | :---- |
| **Compliance with standards** | Our data security policy is fully compliant with GDPR standards. |
| **Open source** | We affirm transparency and reliability by opening the source code of all functional modules and tools. |
| **Encryption** | We use industry-leading AES-256 encryption for data at rest and HTTPS/TLS for data in transit. |
| **Secure access & monitoring** | Flexible access rights and JWT let you entirely control document access. Activity tracking and audit reporting provide traceability. |

📚 Learn more about all [ONLYOFFICE security features ↗](https://www.onlyoffice.com/security?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace).

## **🌐 How can I get DocSpace?**

Get started with DocSpace by choosing the solution that best suits your needs.

| Option | Description | Get started |
| :---- | :---- | :---- |
| 🏢 **[DocSpace Enterprise](https://www.onlyoffice.com/docspace-enterprise?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace)** | Deploy DocSpace on your own server for total control over your documents. Highly scalable to grow with your business. | [Get it now](https://www.onlyoffice.com/download?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace#docspace-enterprise) |
| ⚙️ **[DocSpace Developer](https://www.onlyoffice.com/docspace-developer?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace)** | Bring your web app users a secure space for content storing and online document collaboration. | [Get it now](https://www.onlyoffice.com/download-developer?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace#docspace-developer) |
| ☁️ **Business Cloud** | A hassle-free cloud solution with bigger storage, enhanced security, and professional support. Pay for admins only and invite users for free. | [Check prices](https://www.onlyoffice.com/docspace-prices?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace) |
| 🚀 **Free cloud for startups** | Create a free cloud space for your documents. Invite up to 3 admins and collaborate on docs anywhere. | [Create now](https://www.onlyoffice.com/docspace-registration?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace) |
| 🎓 **Free cloud for schools and non-profits** | Create a free cloud space for your school or non-profit with 2 GB of storage and up to 20 admin accounts. | [Submit request](https://www.onlyoffice.com/free-cloud?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace) |

## 🔗 Using DocSpace inside your platform

Connect DocSpace to collaborate on office documents directly from your business platform: [Zoom](https://www.onlyoffice.com/office-for-zoom?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace), [Pipedrive](https://www.onlyoffice.com/office-for-pipedrive?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace), [Drupal](https://www.onlyoffice.com/office-for-drupal?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace), [WordPress](https://www.onlyoffice.com/office-for-wordpress?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace), [Moodle](https://www.onlyoffice.com/office-for-moodle?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace#materials), [monday](https://www.onlyoffice.com/monday?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace).

Make use of [Zapier integration](https://www.onlyoffice.com/office-for-zapier?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace) to connect DocSpace with thousands of tools your team uses.

Discover all [integrations ↗](https://www.onlyoffice.com/all-connectors?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace)

## **🧱 Repository Structure**

This repository is organized as a set of Git submodules:

| Folder | Description | README |
| :---- | :---- | :---- |
| [`server`](server/) | Backend: C# 14.0, .NET 10.0, ASP.NET Core, .NET Aspire, MySQL, Redis, RabbitMQ, OpenSearch | [Server README](https://github.com/ONLYOFFICE/DocSpace-server#readme) |
| [`client`](client/) | Frontend: TypeScript 5.9, React 19, MobX 6, Webpack 5, Next.js, pnpm monorepo with Nx | [Client README](https://github.com/ONLYOFFICE/DocSpace-client#readme) |
| [`buildtools`](buildtools/) | Build scripts, Docker configurations, CI/CD pipelines, installation packages, app configuration | [Build Tools README](https://github.com/ONLYOFFICE/DocSpace-buildtools#readme) |
| `.github/workflows` | GitHub Actions workflows for CI/CD and issue templates | |

## **🛠️ Technology Stack**

**Backend:** C# 14.0, .NET 10.0 / ASP.NET Core, .NET Aspire, MySQL 9.5, Redis, RabbitMQ, OpenSearch, OpenTelemetry

**Frontend:** TypeScript 5.9, React 19, MobX 6, Styled-Components, CSS/SASS, i18next, Webpack 5, Next.js

**Infrastructure:** Docker 28.5+, Nginx (OpenResty), pnpm 10.28+, Nx

## **🚀 Building from Source**

> **Note:** This creates a **development/testing environment** not suitable for production use.
> For production deployment, see [How can I get DocSpace?](#-how-can-i-get-docspace)

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

This will launch minimal Docker-based setup

> **More details:** [Server](https://github.com/ONLYOFFICE/DocSpace-server#readme) (launch profiles, backend architecture) · [Client](https://github.com/ONLYOFFICE/DocSpace-client#readme) (frontend modes, packages) · [Build Tools](https://github.com/ONLYOFFICE/DocSpace-buildtools#readme) (Docker, CI/CD, installation)

**4. Open the application:**

| URL | Description |
|-----|-------------|
| http://localhost:8092 | DocSpace Application |
| http://localhost:15208 | Aspire Dashboard (backend services monitoring) |
| http://localhost:8092/scalar/#ascfiles | API Documentation (Scalar) |
| http://localhost:56161 | DB Gate (database management) |
| http://localhost:56162 | Mailpit (email testing) |

### Development

- **Frontend (TypeScript/React):** VSCode workspace with task buttons for one-click start — see [Client README](https://github.com/ONLYOFFICE/DocSpace-client#development-with-vscode)
- **Backend (C#/.NET):** VSCode with C# Dev Kit and .NET Aspire extensions — see [Server README](https://github.com/ONLYOFFICE/DocSpace-server#development-with-vscode)

### Stopping Services

- Press `Ctrl+C` in the backend terminal to stop all Aspire-managed services
- Press `Ctrl+C` in the frontend terminal to stop the dev server

To completely remove Docker artifacts, see the [Server README](https://github.com/ONLYOFFICE/DocSpace-server#clear-aspire-docker-artifacts).

## **💡 Need help or have an idea?**

We ❤️ community contributions!

* **🐞 Found a bug?** Please report it by creating an [issue](https://github.com/ONLYOFFICE/DocSpace/issues).
* **❓ Have a question?** Ask our community and developers on the [ONLYOFFICE Forum](https://community.onlyoffice.com/).
* **🥷 Want to be a power user?** Visit our [YouTube channel](https://www.youtube.com/onlyofficeTV) and [Help Center](https://helpcenter.onlyoffice.com/docspace?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace).
* **👨‍💻 Need help for developers?** Check our [API documentation](https://api.onlyoffice.com/?utm_source=github&utm_medium=cpc&utm_campaign=GitHubDocSpace).
* **💡 Want to suggest a feature?** Share your ideas on our [feedback platform](https://feedback.onlyoffice.com/forums/966080-your-voice-matters).

---
<p align="center"> Made with ❤️ by the ONLYOFFICE Team </p>
