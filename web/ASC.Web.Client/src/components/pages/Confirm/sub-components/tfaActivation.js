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
import withLoader from "../withLoader";
import toastr from "studio/toastr";
import ErrorContainer from "@appserver/common/components/ErrorContainer";
import { mobile, tablet, isMobile } from "@appserver/components/utils/device";

const StyledForm = styled(Box)`
  margin: 63px auto auto 216px;
  width: 570px;
  display: flex;
  flex: 1fr 1fr;
  gap: 50px;
  flex-direction: row;

  @media ${tablet} {
    margin: 120px auto;
    width: 480px;
  }

  @media ${mobile} {
    margin: 72px 16px auto 8px;
    width: 311px;
  }

  .app-code-wrapper {
    @media ${tablet} {
      flex-direction: column;
    }
  }

  .app-code-continue-btn {
    @media ${tablet} {
      margin: 32px 0 0 0;
    }
  }

  .set-app-text {
    margin-bottom: 14px;
  }
`;
const TfaActivationForm = withLoader((props) => {
  const {
    t,
    secretKey,
    qrCode,
    loginWithCode,
    loginWithCodeAndCookie,
    history,
    location,
  } = props;

  const [code, setCode] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const onSubmit = async () => {
    try {
      const { user, hash } = (location && location.state) || {};

      setIsLoading(true);

      if (user && hash) {
        const url = await loginWithCode(user, hash, code);
        history.push(url || "/");
      } else {
        const url = await loginWithCodeAndCookie(code);
        history.push(url || "/");
      }

      setIsLoading(false);
    } catch (e) {
      setError(e);
      toastr.error(e);
    }
  };

  const onKeyPress = (target) => {
    if (target.code === "Enter") onSubmit();
  };

  const width = window.innerWidth;

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
            <Box displayProp="flex" className="app-code-wrapper">
              <Box className="app-code-input">
                <TextInput
                  id="code"
                  name="code"
                  type="text"
                  size={width <= 1024 ? "large" : "base"}
                  scale
                  isAutoFocussed
                  tabIndex={1}
                  placeholder={t("EnterCodePlaceholder")}
                  isDisabled={isLoading}
                  maxLength={6}
                  onChange={(e) => {
                    setCode(e.target.value);
                    setError("");
                  }}
                  value={code}
                  hasError={error ? true : false}
                  onKeyDown={onKeyPress}
                />
              </Box>
              <Box className="app-code-continue-btn" marginProp="0 0 0 8px">
                <Button
                  scale
                  primary
                  size={width <= 1024 ? "large" : "medium"}
                  tabIndex={3}
                  label={isLoading ? t("LoadingProcessing") : t("SetAppButton")}
                  isDisabled={!code.length || isLoading}
                  isLoading={isLoading}
                  onClick={onSubmit}
                />
              </Box>
            </Box>
          </div>
          {window.innerWidth > 375 && (
            <div id="qrcode">
              <img
                src={qrCode}
                height="180px"
                width="180px"
                alt="QR-code"
              ></img>
            </div>
          )}
        </StyledForm>
      </PageLayout.SectionBody>
    </PageLayout>
  );
});

const TfaActivationWrapper = (props) => {
  const { getSecretKeyAndQR, linkData, setIsLoaded, setIsLoading } = props;

  const [secretKey, setSecretKey] = useState("");
  const [qrCode, setQrCode] = useState("");
  const [error, setError] = useState(null);

  useEffect(async () => {
    try {
      setIsLoading(true);
      const confirmKey = linkData.confirmHeader;
      const res = await getSecretKeyAndQR(confirmKey);
      const { manualEntryKey, qrCodeSetupImageUrl } = res;

      setSecretKey(manualEntryKey);
      setQrCode(qrCodeSetupImageUrl);
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
    <TfaActivationForm secretKey={secretKey} qrCode={qrCode} {...props} />
  );
};

export default inject(({ auth, confirm }) => ({
  setIsLoaded: confirm.setIsLoaded,
  setIsLoading: confirm.setIsLoading,
  getSecretKeyAndQR: auth.tfaStore.getSecretKeyAndQR,
  loginWithCode: auth.loginWithCode,
  loginWithCodeAndCookie: auth.tfaStore.loginWithCodeAndCookie,
}))(withRouter(withTranslation("Confirm")(observer(TfaActivationWrapper))));
