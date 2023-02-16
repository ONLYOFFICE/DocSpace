import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { Trans, withTranslation } from "react-i18next";
import styled from "styled-components";
import Button from "@docspace/components/button";
import TextInput from "@docspace/components/text-input";
import FieldContainer from "@docspace/components/field-container";
import Text from "@docspace/components/text";
import { inject, observer } from "mobx-react";
import Box from "@docspace/components/box";
import withLoader from "../withLoader";
import toastr from "@docspace/components/toast/toastr";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { hugeMobile, tablet } from "@docspace/components/utils/device";
import Link from "@docspace/components/link";
import FormWrapper from "@docspace/components/form-wrapper";
import DocspaceLogo from "../../../DocspaceLogo";
import { StyledPage, StyledContent } from "./StyledConfirm";

const StyledForm = styled(Box)`
  margin: 56px auto;
  display: flex;
  flex: 1fr 1fr;
  gap: 80px;
  flex-direction: row;
  justify-content: center;

  @media ${tablet} {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 32px;
  }

  @media ${hugeMobile} {
    margin: 0 auto;
    flex-direction: column;
    gap: 0px;
    padding-right: 8px;
  }

  .app-code-wrapper {
    width: 100%;

    @media ${tablet} {
      flex-direction: column;
    }
  }

  .docspace-logo {
    padding-bottom: 40px;

    @media ${tablet} {
      display: flex;
      align-items: center;
      justify-content: center;
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
    padding: 0px 80px;
    border-radius: 6px;
    margin-bottom: 32px;

    @media ${hugeMobile} {
      display: none;
    }
  }

  .app-code-continue-btn {
    margin-top: 8px;
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
        history.push("/");
      }
    } catch (err) {
      let errorMessage = "";
      if (typeof err === "object") {
        errorMessage =
          err?.response?.data?.error?.message ||
          err?.statusText ||
          err?.message ||
          "";
      } else {
        errorMessage = err;
      }
      setError(errorMessage);
      toastr.error(e);
    }
    setIsLoading(false);
  };

  const onKeyPress = (target) => {
    if (target.code === "Enter" || target.code === "NumpadEnter") onSubmit();
  };

  return (
    <StyledPage>
      <StyledContent>
        <StyledForm className="set-app-container">
          <Box className="set-app-description" marginProp="0 0 32px 0">
            <DocspaceLogo className="docspace-logo" />
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
          <FormWrapper>
            <Box
              displayProp="flex"
              flexDirection="column"
              className="app-code-wrapper"
            >
              <div className="qrcode-wrapper">
                <img
                  src={qrCode}
                  height="180px"
                  width="180px"
                  alt="QR-code"
                ></img>
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
          </FormWrapper>
        </StyledForm>
      </StyledContent>
    </StyledPage>
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
