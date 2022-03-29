import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { Trans, withTranslation } from "react-i18next";
import styled from "styled-components";
import Button from "@appserver/components/button";
import TextInput from "@appserver/components/text-input";
import FieldContainer from "@appserver/components/field-container";
import Text from "@appserver/components/text";
import Section from "@appserver/common/components/Section";
import { inject, observer } from "mobx-react";
import Box from "@appserver/components/box";
import withLoader from "../withLoader";
import toastr from "studio/toastr";
import ErrorContainer from "@appserver/common/components/ErrorContainer";
import { mobile, tablet } from "@appserver/components/utils/device";
import Link from "@appserver/components/link";

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
    flex: 1fr;
    flex-direction: column;
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

  .set-app-title {
    margin-bottom: 14px;
  }

  .set-app-text {
    margin-top: 14px;
  }

  @media ${tablet} {
    #qrcode {
      display: none;
    }
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
    } catch (e) {
      setError(e);
      toastr.error(e);
    }
    setIsLoading(false);
  };

  const onKeyPress = (target) => {
    if (target.code === "Enter" || target.code === "NumpadEnter") onSubmit();
  };

  const width = window.innerWidth;

  return (
    <Section>
      <Section.SectionBody>
        <StyledForm className="set-app-container">
          <div>
            <Box className="set-app-description" marginProp="0 0 32px 0">
              <Text isBold fontSize="14px" className="set-app-title">
                {t("SetAppTitle")}
              </Text>

              <Trans t={t} i18nKey="SetAppDescription" ns="Confirm">
                The two-factor authentication is enabled to provide additional
                portal security. Configure your authenticator application to
                continue work on the portal. For example you could use Google
                Authenticator for
                <Link isHovered href={props.tfaAndroidAppUrl} target="_blank">
                  Android
                </Link>
                and{" "}
                <Link isHovered href={props.tfaIosAppUrl} target="_blank">
                  iOS
                </Link>{" "}
                or Authenticator for{" "}
                <Link isHovered href={props.tfaWinAppUrl} target="_blank">
                  Windows Phone
                </Link>{" "}
                .
              </Trans>

              <Text className="set-app-text">
                <Trans
                  t={t}
                  i18nKey="SetAppInstallDescription"
                  ns="Confirm"
                  key={secretKey}
                >
                  To connect your apllication scan the QR code or manually enter
                  your secret key <strong>{{ secretKey }}</strong> then enter
                  6-digit code from your application in the field below.
                </Trans>
              </Text>
            </Box>
            <Box displayProp="flex" className="app-code-wrapper">
              <Box className="app-code-input">
                <FieldContainer
                  labelVisible={false}
                  hasError={error ? true : false}
                  errorMessage={error}
                >
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
                </FieldContainer>
              </Box>
              <Box className="app-code-continue-btn" marginProp="0 0 0 8px">
                <Button
                  scale
                  primary
                  size={width <= 1024 ? "medium" : "normal"}
                  tabIndex={3}
                  label={
                    isLoading
                      ? t("Common:LoadingProcessing")
                      : t("SetAppButton")
                  }
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
      </Section.SectionBody>
    </Section>
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
  tfaAndroidAppUrl: auth.tfaStore.tfaAndroidAppUrl,
  tfaIosAppUrl: auth.tfaStore.tfaIosAppUrl,
  tfaWinAppUrl: auth.tfaStore.tfaWinAppUrl,
}))(
  withRouter(
    withTranslation(["Confirm", "Common"])(observer(TfaActivationWrapper))
  )
);
