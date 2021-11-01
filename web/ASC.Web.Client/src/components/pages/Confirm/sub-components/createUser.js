import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import PropTypes from "prop-types";
import axios from "axios";
import { createUser, signupOAuth } from "@appserver/common/api/people";
import { inject, observer } from "mobx-react";
import Button from "@appserver/components/button";
import TextInput from "@appserver/components/text-input";
import Box from "@appserver/components/box";
import Text from "@appserver/components/text";
import PasswordInput from "@appserver/components/password-input";
import toastr from "@appserver/components/toast/toastr";
import SocialButton from "@appserver/components/social-button";
import FacebookButton from "@appserver/components/facebook-button";
import EmailInput from "@appserver/components/email-input";
import { getAuthProviders } from "@appserver/common/api/settings";
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

const inputWidth = "400px";

const ButtonsWrapper = styled.div`
  display: table;
  margin: -6px;
  margin-top: 17px;
  margin-right: auto;
`;

const ConfirmContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-left: 200px;

  .buttonWrapper {
    margin: 6px;
    min-width: 225px;
  }

  @media (max-width: 830px) {
    margin-left: 40px;
  }

  .start-basis {
    align-items: flex-start;
    ${isMobile && `margin-top: 56px;`}

    @media ${desktop} {
      min-width: 604px;
    }
  }

  .margin-left {
    margin-left: 20px;
  }

  .full-width {
    width: ${inputWidth};
  }

  .confirm-row {
    margin: 23px 0 0;
  }

  .break-word {
    word-break: break-word;
  }
`;

const emailInputName = "email";
const passwordInputName = "password";

const emailRegex = "[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,}$";
const validationEmail = new RegExp(emailRegex);

class Confirm extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      email: "",
      emailValid: true,
      firstName: "",
      firstNameValid: true,
      lastName: "",
      lastNameValid: true,
      password: "",
      passwordValid: true,
      errorText: "",
      isLoading: false,
      passwordEmpty: false,
      key: props.linkData.confirmHeader,
      linkType: props.linkData.type,
    };
  }

  /*componentWillMount() {
      const { isAuthenticated, logout } = this.props;

      if(isAuthenticated)
          logout();
  }*/

  onSubmit = () => {
    this.setState({ isLoading: true }, () => {
      const { defaultPage, linkData, hashSettings } = this.props;
      const isVisitor = parseInt(linkData.emplType) === 2;

      this.setState({ errorText: "" });

      let hasError = false;

      if (!this.state.firstName.trim()) {
        hasError = true;
        this.setState({ firstNameValid: !hasError });
      }

      if (!this.state.lastName.trim()) {
        hasError = true;
        this.setState({ lastNameValid: !hasError });
      }

      if (!validationEmail.test(this.state.email.trim())) {
        hasError = true;
        this.setState({ emailValid: !hasError });
      }

      if (!this.state.passwordValid) {
        hasError = true;
        this.setState({ passwordValid: !hasError });
      }

      !this.state.password.trim() && this.setState({ passwordEmpty: true });

      if (hasError) {
        this.setState({ isLoading: false });
        return false;
      }

      const hash = createPasswordHash(this.state.password, hashSettings);

      const loginData = {
        userName: this.state.email,
        passwordHash: hash,
      };

      const personalData = {
        firstname: this.state.firstName,
        lastname: this.state.lastName,
        email: this.state.email,
      };
      const registerData = Object.assign(personalData, {
        isVisitor: isVisitor,
      });

      this.createConfirmUser(registerData, loginData, this.state.key)
        .then(() => window.location.replace(defaultPage))
        .catch((error) => {
          console.error("confirm error", error);
          this.setState({
            errorText: error,
            isLoading: false,
          });
        });
    });
  };

  addFacebookToStart = (facebookIndex, providerButtons) => {
    const { providers, t } = this.props;
    const faceBookData = providers[facebookIndex];
    const { icon, label, iconOptions } = providersData[faceBookData.provider];
    providerButtons.unshift(
      <div
        className="buttonWrapper"
        key={`${faceBookData.provider}ProviderItem`}
      >
        <FacebookButton
          iconName={icon}
          label={getProviderTranslation(label, t)}
          className="socialButton"
          $iconOptions={iconOptions}
          data-url={faceBookData.url}
          data-providername={faceBookData.provider}
          onClick={this.onSocialButtonClick}
        />
      </div>
    );
  };

  providerButtons = () => {
    const { providers, t } = this.props;

    let facebookIndex = null;
    const providerButtons =
      providers &&
      providers.map((item, index) => {
        if (!providersData[item.provider]) return;
        const { icon, label, iconOptions, className } = providersData[
          item.provider
        ];

        if (item.provider === "Facebook") {
          facebookIndex = index;
          return;
        }
        return (
          <div className="buttonWrapper" key={`${item.provider}ProviderItem`}>
            <SocialButton
              iconName={icon}
              label={getProviderTranslation(label, t)}
              className={`socialButton ${className ? className : ""}`}
              $iconOptions={iconOptions}
              data-url={item.url}
              data-providername={item.provider}
              onClick={this.onSocialButtonClick}
            />
          </div>
        );
      });

    if (facebookIndex) this.addFacebookToStart(facebookIndex, providerButtons);

    return providerButtons;
  };

  oauthDataExists = () => {
    const { providers } = this.props;

    let existProviders = 0;
    providers && providers.length > 0;
    providers.map((item) => {
      if (!providersData[item.provider]) return;
      existProviders++;
    });

    return !!existProviders;
  };

  authCallback = (profile) => {
    const { t, defaultPage } = this.props;
    const { FirstName, LastName, EMail, Serialized } = profile;

    console.log(profile);

    const signupAccount = {
      EmployeeType: null,
      FirstName: FirstName,
      LastName: LastName,
      Email: EMail,
      PasswordHash: "",
      SerializedProfile: Serialized,
    };

    signupOAuth(signupAccount)
      .then(() => {
        window.location.replace(defaultPage);
      })
      .catch((e) => {
        toastr.error(e);
      });
  };

  setProviders = async () => {
    const { setProviders } = this.props;

    try {
      await getAuthProviders().then((providers) => {
        setProviders(providers);
      });
    } catch (e) {
      console.error(e);
    }
  };

  createConfirmUser = async (registerData, loginData, key) => {
    const data = Object.assign(
      { fromInviteLink: true },
      registerData,
      loginData
    );

    const user = await createUser(data, key);

    console.log("Created user", user);

    const { login } = this.props;
    const { userName, passwordHash } = loginData;

    const response = await login(userName, passwordHash);

    console.log("Login", response);

    return user;
  };

  onSocialButtonClick = (e) => {
    const providerName = e.target.dataset.providername;
    const url = e.target.dataset.url;

    const { getOAuthToken, getLoginLink } = this.props;

    try {
      const tokenGetterWin = window.open(
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
  };

  onKeyPress = (event) => {
    if (event.key === "Enter") {
      this.onSubmit();
    }
  };

  onCopyToClipboard = () =>
    toastr.success(this.props.t("EmailAndPasswordCopiedToClipboard"));
  validatePassword = (value) => this.setState({ passwordValid: value });

  componentDidMount() {
    this.setProviders();
    window.authCallback = this.authCallback;

    window.addEventListener("keydown", this.onKeyPress);
    window.addEventListener("keyup", this.onKeyPress);
  }

  componentWillUnmount() {
    window.removeEventListener("keydown", this.onKeyPress);
    window.removeEventListener("keyup", this.onKeyPress);
  }

  onChangeName = (event) => {
    this.setState({ firstName: event.target.value });
    !this.state.firstNameValid &&
      this.setState({ firstNameValid: event.target.value });
    this.state.errorText && this.setState({ errorText: "" });
  };

  onChangeSurname = (event) => {
    this.setState({ lastName: event.target.value });
    !this.state.lastNameValid && this.setState({ lastNameValid: true });
    this.state.errorText && this.setState({ errorText: "" });
  };

  onChangeEmail = (event) => {
    this.setState({ email: event.target.value });
    // !this.state.emailValid && this.setState({ emailValid: true });
    this.state.errorText && this.setState({ errorText: "" });
  };

  onValidateEmail = (value) => this.setState({ emailValid: value.isValid });

  onChangePassword = (event) => {
    this.setState({ password: event.target.value });
    !this.state.passwordValid && this.setState({ passwordValid: true });
    event.target.value.trim() && this.setState({ passwordEmpty: false });
    this.state.errorText && this.setState({ errorText: "" });
    this.onKeyPress(event);
  };

  render() {
    //console.log("createUser render");

    const { settings, t, greetingTitle, providers } = this.props;
    const { email, password } = this.state;
    const showCopyLink = !!email.trim() || !!password.trim();

    return (
      <ConfirmContainer>
        <div className="start-basis">
          <div className="margin-left">
            <Text className="confirm-row" as="p" fontSize="18px">
              {t("InviteTitle")}
            </Text>

            <div className="confirm-row full-width break-word">
              <a href="/login">
                <img src="images/dark_general.png" alt="Logo" />
              </a>
              <Text as="p" fontSize="24px" color="#116d9d">
                {greetingTitle}
              </Text>
            </div>
          </div>

          <div>
            <div className="full-width">
              <TextInput
                className="confirm-row"
                id="name"
                name="name"
                value={this.state.firstName}
                placeholder={t("FirstName")}
                size="huge"
                scale={true}
                tabIndex={1}
                isAutoFocussed={true}
                autoComplete="given-name"
                isDisabled={this.state.isLoading}
                hasError={!this.state.firstNameValid}
                onChange={this.onChangeName}
                onKeyDown={this.onKeyPress}
              />

              <TextInput
                className="confirm-row"
                id="surname"
                name="surname"
                value={this.state.lastName}
                placeholder={t("Common:LastName")}
                size="huge"
                scale={true}
                tabIndex={2}
                autoComplete="family-name"
                isDisabled={this.state.isLoading}
                hasError={!this.state.lastNameValid}
                onChange={this.onChangeSurname}
                onKeyDown={this.onKeyPress}
              />

              <EmailInput
                className="confirm-row"
                id="email"
                name={emailInputName}
                value={this.state.email}
                placeholder={t("Common:Email")}
                size="huge"
                scale={true}
                tabIndex={3}
                autoComplete="email"
                isDisabled={this.state.isLoading}
                // hasError={!this.state.emailValid}
                onChange={this.onChangeEmail}
                onKeyDown={this.onKeyPress}
                onValidateInput={this.onValidateEmail}
              />
            </div>

            <PasswordInput
              className="confirm-row"
              id="password"
              inputName={passwordInputName}
              emailInputName={emailInputName}
              inputValue={this.state.password}
              placeholder={t("Common:Password")}
              size="huge"
              scale={true}
              tabIndex={4}
              maxLength={30}
              inputWidth={inputWidth}
              hasError={this.state.passwordEmpty}
              onChange={this.onChangePassword}
              onCopyToClipboard={this.onCopyToClipboard}
              onValidateInput={this.validatePassword}
              clipActionResource={t("Common:CopyEmailAndPassword")}
              clipEmailResource={`${t("Common:Email")}: `}
              clipPasswordResource={`${t("Common:Password")}: `}
              tooltipPasswordTitle={`${t("Common:PasswordLimitMessage")}:`}
              tooltipPasswordLength={`${t("Common:PasswordLimitLength", {
                fromNumber: settings ? settings.minLength : 8,
                toNumber: 30,
              })}:`}
              tooltipPasswordDigits={t("Common:PasswordLimitDigits")}
              tooltipPasswordCapital={t("Common:PasswordLimitUpperCase")}
              tooltipPasswordSpecial={`${t(
                "Common:PasswordLimitSpecialSymbols"
              )} (${PasswordLimitSpecialCharacters})`}
              generatorSpecial={PasswordLimitSpecialCharacters}
              passwordSettings={settings}
              isDisabled={this.state.isLoading}
              onKeyDown={this.onKeyPress}
              showCopyLink={showCopyLink}
            />

            <Button
              className="confirm-row"
              primary
              size="big"
              label={t("LoginRegistryButton")}
              tabIndex={5}
              isLoading={this.state.isLoading}
              onClick={this.onSubmit}
            />
          </div>
          {this.oauthDataExists && (
            <Box>
              <ButtonsWrapper>{this.providerButtons()}</ButtonsWrapper>
            </Box>
          )}

          {/*             <Row className='confirm-row'>

                    <Text as='p' fontSize='14px'>{t('LoginWithAccount')}</Text>

            </Row>
 */}
          <Text className="confirm-row" fontSize="14px" color="#c30">
            {this.state.errorText}
          </Text>
        </div>
      </ConfirmContainer>
    );
  }
}

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
