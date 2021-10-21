mkdir build/deploy/services/ssoauth/client/
cp -Rf common/ASC.SsoAuth/* build/deploy/services/ssoauth/client/
yarn install --cwd common/ASC.SsoAuth --frozen-lockfile