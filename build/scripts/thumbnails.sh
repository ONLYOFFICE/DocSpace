mkdir build/deploy/services/thumb/client/
cp -Rf common/ASC.Thumbnails/* build/deploy/services/thumb/client/
yarn install --cwd common/ASC.Thumbnails --frozen-lockfile