mkdir build/deploy/services/urlshortener/client/
cp -Rf common/ASC.UrlShortener/* build/deploy/services/urlshortener/client/
yarn install --cwd common/ASC.UrlShortener --frozen-lockfile