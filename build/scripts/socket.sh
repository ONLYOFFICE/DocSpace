mkdir build/deploy/services/socket/client/
cp -Rf common/ASC.Socket.IO/* build/deploy/services/socket/client/
yarn install --cwd build/deploy/services/socket/client/ --frozen-lockfile