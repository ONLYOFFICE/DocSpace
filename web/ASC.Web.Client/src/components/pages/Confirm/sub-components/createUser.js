import React, { useEffect, useState, useCallback } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import PropTypes from "prop-types";
import { createUser, signupOAuth } from "@appserver/common/api/people";
import { inject, observer } from "mobx-react";
import Avatar from "@appserver/components/avatar";
import Button from "@appserver/components/button";
import TextInput from "@appserver/components/text-input";
import Box from "@appserver/components/box";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import PasswordInput from "@appserver/components/password-input";
import FieldContainer from "@appserver/components/field-container";
import toastr from "@appserver/components/toast/toastr";
import SocialButton from "@appserver/components/social-button";
import FacebookButton from "@appserver/components/facebook-button";
import EmailInput from "@appserver/components/email-input";
import {
  getAuthProviders,
  getCapabilities,
} from "@appserver/common/api/settings";
import PageLayout from "@appserver/common/components/PageLayout";
import {
  createPasswordHash,
  getProviderTranslation,
} from "@appserver/common/utils";
import {
  providersData,
  PasswordLimitSpecialCharacters,
} from "@appserver/common/constants";
import { isMobile } from "react-device-detect";
import { desktop } from "@appserver/components/utils/device";
import withLoader from "../withLoader";
import MoreLoginModal from "login/moreLogin";

export const ButtonsWrapper = styled.div`
  display: table;
  margin: auto;

  .buttonWrapper {
    margin-bottom: 8px;
    width: 320px;

    @media (max-width: 768px) {
      width: 480px;
    }

    @media (max-width: 414px) {
      width: 311px;
    }
  }
`;

const ConfirmContainer = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 70px;
  align-items: center;
  margin: 80px auto 0 auto;
  max-width: 960px;

  @media (max-width: 768px) {
    grid-template-columns: 1fr;
  }
`;

const GreetingContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: left;
  height: 100%;
  padding-bottom: 32px;

  .greeting-title {
    width: 100%;
    padding-bottom: 32px;
  }

  .greeting-block {
    display: flex;
    flex-direction: row;

    .user-info {
      margin-left: 12px;
    }
  }

  .tooltip {
    position: relative;
    display: inline-block;
    margin-top: 15px;
  }

  .tooltip .tooltiptext {
    background: #ffffff;
    border: 1px solid #eceef1;
    box-sizing: border-box;
    border-radius: 6px;
    position: absolute;
    padding: 16px;
    width: 100%;
  }
`;

const RegisterContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  height: 100%;

  .or-label {
    margin: 0 8px;
  }

  .more-label {
    padding-top: 18px;
  }

  .line {
    display: flex;
    width: 320px;
    align-items: center;
    color: #eceef1;
    padding-top: 35px;

    @media (max-width: 768px) {
      width: 480px;
    }

    @media (max-width: 414px) {
      width: 311px;
    }
  }

  .line:before,
  .line:after {
    content: "";
    flex-grow: 1;
    background: #eceef1;
    height: 1px;
    font-size: 0px;
    line-height: 0px;
    margin: 0px;
  }

  .auth-form-container {
    margin-top: 32px;
    width: 320px;

    @media (max-width: 768px) {
      margin: 32px 0 0 0;
      width: 100%;
    }
    @media (max-width: 375px) {
      margin: 32px 0 0 0;
      width: 100%;
    }
  }
`;

const Confirm = (props) => {
  const { settings, t, greetingTitle, providers } = props;
  const [moreAuthVisible, setMoreAuthVisible] = useState(false);
  const [ssoLabel, setSsoLabel] = useState("");
  const [ssoUrl, setSsoUrl] = useState("");
  const [email, setEmail] = useState("");
  const [emailValid, setEmailValid] = useState(true);
  const [password, setPassword] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);

  const [fname, setFname] = useState("");
  const [sname, setSname] = useState("");

  const [isLoading, setIsLoading] = useState(false);

  const [errorText, setErrorText] = useState("");

  const getSso = async () => {
    const data = await getCapabilities();
    console.log(data);
    setSsoLabel(data.ssoLabel);
    setSsoUrl(data.ssoUrl);
  };

  useEffect(() => {
    getSso();
  }, []);

  useEffect(async () => {
    window.authCallback = authCallback;
    await setProviders();
  }, []);

  const authCallback = (profile) => {
    oAuthLogin(profile);
  };

  const setProviders = async () => {
    const { setProviders } = props;

    try {
      await getAuthProviders().then((providers) => {
        setProviders(providers);
      });
    } catch (e) {
      console.error(e);
    }
  };

  const moreAuthOpen = () => {
    setMoreAuthVisible(true);
  };

  const moreAuthClose = () => {
    setMoreAuthVisible(false);
  };

  const onChangeEmail = (e) => {
    setEmail(e.target.value);
  };

  const onChangeFname = (e) => {
    setFname(e.target.value);
  };

  const onChangeSname = (e) => {
    setSname(e.target.value);
  };

  const onChangePassword = (e) => {
    setPassword(e.target.value);
  };

  const onSubmit = () => {
    console.log("onSubmit");
  };

  const onSocialButtonClick = useCallback((e) => {
    const providerName = e.target.dataset.providername;
    const url = e.target.dataset.url;

    const { getOAuthToken, getLoginLink } = props;

    try {
      const tokenGetterWin = isDesktop
        ? (window.location.href = url)
        : window.open(
            url,
            "login",
            "width=800,height=500,status=no,toolbar=no,menubar=no,resizable=yes,scrollbars=no"
          );

      getOAuthToken(tokenGetterWin).then((code) => {
        const token = window.btoa(
          JSON.stringify({
            auth: providerName,
            mode: "popup",
            callback: "authCallback",
          })
        );

        tokenGetterWin.location.href = getLoginLink(token, code);
      });
    } catch (err) {
      console.log(err);
    }
  }, []);

  const providerButtons = () => {
    const providerButtons =
      providers &&
      providers.map((item, index) => {
        if (!providersData[item.provider]) return;
        if (index > 1) return;

        const { icon, label, iconOptions, className } = providersData[
          item.provider
        ];

        return (
          <div className="buttonWrapper" key={`${item.provider}ProviderItem`}>
            <SocialButton
              iconName={icon}
              label={getProviderTranslation(label, t)}
              className={`socialButton ${className ? className : ""}`}
              $iconOptions={iconOptions}
              data-url={item.url}
              data-providername={item.provider}
              onClick={onSocialButtonClick}
            />
          </div>
        );
      });

    return providerButtons;
  };

  const ssoButton = () => {
    return (
      <div className="buttonWrapper">
        <SocialButton
          iconName="/static/images/sso.react.svg"
          className="socialButton"
          label={ssoLabel || getProviderTranslation("sso", t)}
          onClick={() => (window.location.href = ssoUrl)}
        />
      </div>
    );
  };

  const oauthDataExists = () => {
    let existProviders = 0;
    providers && providers.length > 0;
    providers.map((item) => {
      if (!providersData[item.provider]) return;
      existProviders++;
    });

    return !!existProviders;
  };

  const ssoExists = () => {
    if (ssoUrl) return true;
    else return false;
  };

  return (
    <ConfirmContainer>
      <GreetingContainer>
        <Text
          fontSize="23px"
          fontWeight={700}
          textAlign="left"
          className="greeting-title"
        >
          {greetingTitle}
        </Text>

        {/*TODO: get user info from api */}
        <div className="greeting-block">
          <Avatar role="user" size="medium" source="" />
          <div className="user-info">
            <Text fontSize="15px" fontWeight={600}>
              John Doe
            </Text>
            <Text fontSize="12px" fontWeight={600} color="#A3A9AE">
              Head of department
            </Text>
          </div>
        </div>

        <div class="tooltip">
          <span class="tooltiptext">Welcome to join our portal!</span>
        </div>
      </GreetingContainer>

      <RegisterContainer>
        {ssoExists() && <ButtonsWrapper>{ssoButton()}</ButtonsWrapper>}

        {oauthDataExists() && (
          <>
            <ButtonsWrapper>{providerButtons()}</ButtonsWrapper>
            {providers && providers.length > 2 && (
              <Link
                isHovered
                type="action"
                fontSize="13px"
                fontWeight="600"
                color="#3B72A7"
                className="more-label"
                onClick={moreAuthOpen}
              >
                {t("ShowMore")}
              </Link>
            )}
          </>
        )}

        {(oauthDataExists() || ssoExists()) && (
          <div className="line">
            <Text color="#A3A9AE" className="or-label">
              {t("Or")}
            </Text>
          </div>
        )}

        <form className="auth-form-container">
          <FieldContainer
            isVertical={true}
            labelVisible={false}
            hasError={!emailValid}
            errorMessage={errorText ? errorText : t("Common:RequiredField")}
          >
            <TextInput
              id="login"
              name="login"
              type="email"
              hasError={!emailValid}
              value={email}
              placeholder={t("Common:Email")}
              size="large"
              scale={true}
              isAutoFocussed={true}
              tabIndex={1}
              isDisabled={isLoading}
              autoComplete="username"
              onChange={onChangeEmail}
            />
          </FieldContainer>

          <FieldContainer
            isVertical={true}
            labelVisible={false}
            errorMessage={errorText ? errorText : t("Common:RequiredField")}
          >
            <TextInput
              id="first-name"
              name="first-name"
              type="text"
              value={fname}
              placeholder={t("FirstName")}
              size="large"
              scale={true}
              isAutoFocussed={true}
              tabIndex={1}
              isDisabled={isLoading}
              onChange={onChangeFname}
            />
          </FieldContainer>

          <FieldContainer
            isVertical={true}
            labelVisible={false}
            errorMessage={errorText ? errorText : t("Common:RequiredField")}
          >
            <TextInput
              id="last-name"
              name="last-name"
              type="text"
              value={sname}
              placeholder={t("Common:LastName")}
              size="large"
              scale={true}
              isAutoFocussed={true}
              tabIndex={1}
              isDisabled={isLoading}
              onChange={onChangeSname}
            />
          </FieldContainer>

          <FieldContainer
            isVertical={true}
            labelVisible={false}
            hasError={!passwordValid}
            errorMessage={errorText ? "" : t("Common:RequiredField")}
          >
            <PasswordInput
              simpleView={true}
              passwordSettings={settings}
              id="password"
              inputName="password"
              placeholder={t("Common:Password")}
              type="password"
              hasError={!passwordValid}
              inputValue={password}
              size="large"
              scale={true}
              tabIndex={1}
              isDisabled={isLoading}
              autoComplete="current-password"
              onChange={onChangePassword}
            />
          </FieldContainer>

          <Button
            id="submit"
            className="login-button"
            primary
            size="large"
            scale={true}
            label={
              isLoading
                ? t("Common:LoadingProcessing")
                : t("LoginRegistryButton")
            }
            tabIndex={1}
            isDisabled={isLoading}
            isLoading={isLoading}
            onClick={onSubmit}
          />
        </form>

        <MoreLoginModal
          t={t}
          visible={moreAuthVisible}
          onClose={moreAuthClose}
          providers={providers}
          onSocialLoginClick={onSocialButtonClick}
          ssoLabel={ssoLabel}
          ssoUrl={ssoUrl}
        />
      </RegisterContainer>
    </ConfirmContainer>
  );
};

Confirm.propTypes = {
  location: PropTypes.object.isRequired,
  history: PropTypes.object.isRequired,
};
const CreateUserForm = (props) => (
  <PageLayout>
    <PageLayout.SectionBody>
      <Confirm {...props} />
    </PageLayout.SectionBody>
  </PageLayout>
);

export default inject(({ auth }) => {
  const {
    login,
    logout,
    isAuthenticated,
    settingsStore,
    setProviders,
    providers,
    thirdPartyLogin,
  } = auth;
  const {
    passwordSettings,
    greetingSettings,
    hashSettings,
    defaultPage,
    getSettings,
    getPortalPasswordSettings,
    getOAuthToken,
    getLoginLink,
  } = settingsStore;

  return {
    settings: passwordSettings,
    greetingTitle: greetingSettings,
    hashSettings,
    defaultPage,
    isAuthenticated,
    login,
    logout,
    getSettings,
    getPortalPasswordSettings,
    thirdPartyLogin,
    getOAuthToken,
    getLoginLink,
    setProviders,
    providers,
  };
})(
  withRouter(
    withTranslation(["Confirm", "Common"])(withLoader(observer(CreateUserForm)))
  )
);
