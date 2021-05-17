import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { Trans, withTranslation } from "react-i18next";
import styled from "styled-components";
import Button from "@appserver/components/button";
import TextInput from "@appserver/components/text-input";
import Text from "@appserver/components/text";
import PageLayout from "@appserver/common/components/PageLayout";
import { inject, observer } from "mobx-react";
import Box from "@appserver/components/box";
import Link from "@appserver/components/link";
import withLoader from "../withLoader";
import toastr from "studio/toastr";
import ErrorContainer from "@appserver/common/components/ErrorContainer";

const StyledForm = styled(Box)`
  margin: 63px auto auto 216px;
  width: 570px;
  display: flex;
  flex: 1fr 1fr;
  gap: 50px;
  flex-direction: row;
  .set-app-text {
    margin-bottom: 14px;
  }
`;
const TfaActivationForm = withLoader((props) => {
  const { t, secretKey, qrCode, loginWithCode, user, hash, history } = props;

  const [code, setCode] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const onSubmit = async () => {
    try {
      const url = await loginWithCode(user, hash, code);
      history.push(url || "/");
    } catch (e) {
      setError(e);
      toastr.error(e);
    }
  };

  const onKeyPress = (target) => {
    if (target.code === "Enter") onSubmit();
  };

  return (
    <PageLayout>
      <PageLayout.SectionBody>
        <StyledForm className="set-app-container">
          <div>
            <Box className="set-app-description" marginProp="0 0 32px 0">
              <Text isBold fontSize="14px" className="set-app-text">
                {t("SetAppTitle")}
              </Text>
              <Text className="set-app-text">{t("SetAppDescription")}</Text>
              <Trans
                t={t}
                i18nKey="SetAppInstallDescription"
                ns="Confirm"
                key={secretKey}
              >
                <Text>
                  To connect your apllication scan the QR code or manually enter
                  your secret key <strong>{{ key: secretKey }}</strong> then
                  enter 6-digit code from your application in the field below.
                </Text>
              </Trans>
            </Box>
            <Box displayProp="flex">
              <Box className="app-code-input">
                <TextInput
                  id="code"
                  name="code"
                  type="text"
                  size="base"
                  scale
                  isAutoFocussed
                  tabIndex={1}
                  placeholder={t("EnterCodePlaceholder")}
                  isDisabled={isLoading}
                  onChange={(e) => setCode(e.target.value)}
                  value={code}
                />
              </Box>
              <Box className="app-code-continue-btn" marginProp="0 0 0 8px">
                <Button
                  primary
                  size="medium"
                  tabIndex={3}
                  label={isLoading ? t("LoadingProcessing") : t("SetAppButton")}
                  isDisabled={!code.length || isLoading}
                  isLoading={isLoading}
                  onClick={onSubmit}
                />
              </Box>
            </Box>
          </div>
          <div id="qrcode">
            <img src={qrCode} height="180px" width="180px" alt="QR-code"></img>
          </div>
        </StyledForm>
      </PageLayout.SectionBody>
    </PageLayout>
  );
});

const TfaActivationWrapper = (props) => {
  const {
    t,
    getSecretKeyAndQR,
    linkData,
    setIsLoaded,
    setIsLoading,
    loginWithCode,
    location,
    history,
  } = props;

  const { user, hash } = location.state;

  const [secretKey, setSecretKey] = useState("");
  const [qrCode, setQrCode] = useState("");
  const [error, setError] = useState(null);

  useEffect(async () => {
    try {
      setIsLoading(true);
      const confirmKey = linkData.confirmHeader;
      const res = await getSecretKeyAndQR(confirmKey);

      setSecretKey(res.manualEntryKey);
      setQrCode(res.qrCodeSetupImageUrl);
    } catch (e) {
      setError(e);
      toastr.error(e);
    }

    setIsLoaded(true);
    setIsLoading(false);
  }, []);

  return error ? (
    <ErrorContainer bodyText={error} />
  ) : (
    <TfaActivationForm
      secretKey={secretKey}
      qrCode={qrCode}
      user={user}
      hash={hash}
      {...props}
    />
  );
};

export default inject(({ auth, confirm }) => ({
  setIsLoaded: confirm.setIsLoaded,
  setIsLoading: confirm.setIsLoading,
  getSecretKeyAndQR: auth.tfaStore.getSecretKeyAndQR,
  loginWithCode: auth.loginWithCode,
}))(withRouter(withTranslation("Confirm")(observer(TfaActivationWrapper))));
