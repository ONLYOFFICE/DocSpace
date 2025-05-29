# ONLYOFFICE DocSpace

> :warning: **Important Note**  
> **This README contains a development version of ONLYOFFICE DocSpace.**  
> The build instructions below produce a non-production environment suitable for **testing and development purposes only**.  
> **DO NOT USE THIS VERSION IN PRODUCTION ENVIRONMENTS.**

## Overview

ONLYOFFICE DocSpace is a document hub where you can connect users and documents in one place to boost collaboration. 

## Functionality

* Various room types with different access levels: view-only, review, collaboration, filling forms, custom rooms. 
* Ability to work with multiple file formats: text documents, spreadsheets, presentations, digital forms, PDFs, e-books.
* Document collaboration: two co-editing modes, Track Changes, comments, built-in chat, plugins for making audio and video calls.
* Connecting 3rd party clouds and storages.

## Technology stack

Backend: С# 13.0, .NET 9.0/.NET Core/ASP.NET Core/MySQL 8.3/Rabbit/Redis/OpenSearch

Frontend: ES6, TypeScript, React, Mobx, Styled-Components, CSS/SAAS, i18next, Webpack 5

## Recommended System Requirements

* RAM: 8 GB or more
* CPU: dual-core 2 GHz or higher
* Swap: at least 2 GB
* HDD: at least 40 GB of free space
* Docker: version 25.2.0 or later
* Docker Compose: version 2.28.0 or later

## Simple Build and Running Test Example in Docker 

> **Note**  
> The following instructions create a **development/testing environment**  
> not suitable for production use. For production deployment, see:  
> [Production Version of ONLYOFFICE DocSpace](https://github.com/ONLYOFFICE/DocSpace-buildtools/tree/master/install/OneClickInstall)

1. Clone DocSpace repository with submodules:

   ```bash
   git clone --recurse-submodules https://github.com/ONLYOFFICE/DocSpace && \
   cd "$(basename "$_" .git)" && \
   git submodule foreach "git checkout master" 
   ```
> **Note** 
> Customizing branches:
> To use specific branches (hotfix/releases) for submodules:

   ```bash
   # Checkout desired branch for main repository
   git checkout <branch-name>

   # Update submodules to specific branches
   git submodule update --init --remote
   git submodule foreach "git checkout <submodule-branch-name>"
   ```
2. Run Docker Images:
 
   ```bash
   # Change directory to the docker directory
   cd ./docker
   # Ensure that your current directory is: ./DocSpace/docker/
   docker compose -f dev.docspace.yml up -d
   ```
3. Open your web browser (Chrome, Opera, etc) and run: http://localhost


## Production Version
For production deployments, use the official enterprise-ready solutions:

ONLYOFFICE DocSpace Enterprise Edition - Commercial version with enterprise support

ONLYOFFICE DocSpace Developer Edition - Free version for small teams (up to 10 users)

[Official Production Documentation](https://www.onlyoffice.com/download.aspx#docspace-enterprise) 


## Licensing 

ONLYOFFICE DocSpace is released under AGPLv3 license. See the LICENSE file for more information.

## Project info

Official website: [https://www.onlyoffice.com](https://www.onlyoffice.com/?utm_source=github&utm_medium=cpc&utm_campaign=DocSpace "https://www.onlyoffice.com/?utm_source=github&utm_medium=cpc&utm_campaign=DocSpace")

Code repository: [https://github.com/ONLYOFFICE/DocSpace](https://github.com/ONLYOFFICE/DocSpace)

## User feedback and support

If you face any issues or have questions about ONLYOFFICE DocSpace, use the Issues section in this repository or visit our [official forum](https://forum.onlyoffice.com/).
