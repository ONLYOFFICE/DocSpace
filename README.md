# ONLYOFFICE DocSpace

## Overview

ONLYOFFICE DocSpace is a document hub where you can connect users and documents in one place to boost collaboration. 

## Functionality

* Various room types with different access levels: view-only, review, collaboration, filling forms, custom rooms. 
* Ability to work with multiple file formats: text documents, spreadsheets, presentations, digital forms, PDFs, e-books.
* Document collaboration: two co-editing modes, Track Changes, comments, built-in chat, plugins for making audio and video calls.
* Connecting 3rd party clouds and storages.

## Technology stack

Backend: С# 11.0, .NET 7.0/.NET Core/ASP.NET Core/MySQL 8.0/Rabbit/Redis/OpenSearch

Frontend: ES6, TypeScript, React, Mobx, Styled-Components, CSS/SAAS, i18next, Webpack 5

## Recommended System Requirements

* RAM: 8 GB or more
* CPU: dual-core 2 GHz or higher
* Swap: at least 2 GB
* HDD: at least 30 GB of free space
* Docker: version 25.2.0 or later
* Docker Compose: version 2.28.0 or later

## Simple Build and Running Test Example in Docker 

1. Clone DocSpace repository with submodules:

   ```bash
   git clone --recurse-submodules -j8 https://github.com/ONLYOFFICE/DocSpace.git
   cd ./DocSpace/docker/

   ```
2. Run Docker Images:
 
   ```bash
    docker compose -f dev.docspace.yml up -d

   ```
3. Open your web browser (Chrome, Opera, etc) and run: http://localhost

## Licensing 

ONLYOFFICE DocSpace is released under AGPLv3 license. See the LICENSE file for more information.

## Project info

Official website: [https://www.onlyoffice.com](https://www.onlyoffice.com/?utm_source=github&utm_medium=cpc&utm_campaign=DocSpace "https://www.onlyoffice.com/?utm_source=github&utm_medium=cpc&utm_campaign=DocSpace")

Code repository: [https://github.com/ONLYOFFICE/DocSpace](https://github.com/ONLYOFFICE/DocSpace)

## User feedback and support

If you face any issues or have questions about ONLYOFFICE DocSpace, use the Issues section in this repository or visit our [official forum](https://forum.onlyoffice.com/).
