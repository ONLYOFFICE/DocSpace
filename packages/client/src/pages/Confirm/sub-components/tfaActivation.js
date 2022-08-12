import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { Trans, withTranslation } from "react-i18next";
import styled from "styled-components";
import Button from "@docspace/components/button";
import TextInput from "@docspace/components/text-input";
import FieldContainer from "@docspace/components/field-container";
import Text from "@docspace/components/text";
import Section from "@docspace/common/components/Section";
import { inject, observer } from "mobx-react";
import Box from "@docspace/components/box";
import withLoader from "../withLoader";
import toastr from "client/toastr";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { hugeMobile, tablet } from "@docspace/components/utils/device";
import Link from "@docspace/components/link";

const StyledForm = styled(Box)`
  margin: 63px auto auto 216px;
  display: flex;
  flex: 1fr 1fr;
  gap: 80px;
  flex-direction: row;

  @media ${tablet} {
    margin: 120px auto;
    width: 480px;
    flex: 1fr;
    flex-direction: column;
    gap: 32px;
  }

  @media ${hugeMobile} {
    margin-top: 72px;
    width: 100%;
    flex: 1fr;
    flex-direction: column;
    gap: 0px;
  }

  .app-code-wrapper {
    @media ${tablet} {
      flex-direction: column;
    }
  }

  .set-app-description {
    width: 100%;
    max-width: 500px;
  }

  .set-app-title {
    margin-bottom: 14px;
  }

  .set-app-text {
    margin-top: 14px;
  }

  .qrcode-wrapper {
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 24px 80px;
    background: #f8f9f9;
    border-radius: 6px;
    margin-bottom: 32px;

    @media ${hugeMobile} {
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
      const { linkData } = props;

      setIsLoading(true);

      if (user && hash) {
        const url = await loginWithCode(user, hash, code);
        history.push(url || "/");
      } else {
        const url = await loginWithCodeAndCookie(code, linkData.confirmHeader);
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

  return (
    <StyledForm className="set-app-container">
      <Box className="set-app-description" marginProp="0 0 32px 0">
        <Text isBold fontSize="14px" className="set-app-title">
          {t("SetAppTitle")}
        </Text>

        <Trans t={t} i18nKey="SetAppDescription" ns="Confirm">
          The two-factor authentication is enabled to provide additional portal
          security. Configure your authenticator application to continue work on
          the portal. For example you could use Google Authenticator for
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
            To connect your apllication scan the QR code or manually enter your
            secret key <strong>{{ secretKey }}</strong> then enter 6-digit code
            from your application in the field below.
          </Trans>
        </Text>
      </Box>
      <Box
        displayProp="flex"
        flexDirection="column"
        className="app-code-wrapper"
      >
        <div className="qrcode-wrapper">
          <img src={qrCode} height="180px" width="180px" alt="QR-code"></img>
        </div>
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
              size="large"
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
        <Box className="app-code-continue-btn">
          <Button
            scale
            primary
            size="medium"
            tabIndex={3}
            label={
              isLoading ? t("Common:LoadingProcessing") : t("SetAppButton")
            }
            isDisabled={!code.length || isLoading}
            isLoading={isLoading}
            onClick={onSubmit}
          />
        </Box>
      </Box>
    </StyledForm>
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
      setError(e.error);
      toastr.error(e);
    }

    setIsLoaded(true);
    setIsLoading(false);
  }, []);

  return error ? (
    <ErrorContainer bodyText={error} />
  ) : (
    <Section>
      <Section.SectionBody>
        <TfaActivationForm secretKey={secretKey} qrCode={qrCode} {...props} />
      </Section.SectionBody>
    </Section>
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
