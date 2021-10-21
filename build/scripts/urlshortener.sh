mkdir build/deploy/services/urlshortener/client/
cp -Rf common/ASC.UrlShortener/* build/deploy/services/urlshortener/client/
yarn install --cwd build/deploy/services/urlshortener/client/ --frozen-lockfile