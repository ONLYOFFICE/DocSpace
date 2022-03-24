import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import PropTypes from "prop-types";
import axios from "axios";
import {
  changePassword,
  updateActivationStatus,
  updateUser,
} from "@appserver/common/api/people";
import { inject, observer } from "mobx-react";
import Button from "@appserver/components/button";
import TextInput from "@appserver/components/text-input";
import Text from "@appserver/components/text";
import PasswordInput from "@appserver/components/password-input";
import toastr from "@appserver/components/toast/toastr";
import Loader from "@appserver/components/loader";
import Section from "@appserver/common/components/Section";
import {
  AppServerConfig,
  EmployeeActivationStatus,
  PasswordLimitSpecialCharacters,
} from "@appserver/common/constants";
import { combineUrl, createPasswordHash } from "@appserver/common/utils";

const inputWidth = "320px";

const ConfirmContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-left: 200px;

  @media (max-width: 830px) {
    margin-left: 40px;
  }

  .start-basis {
    align-items: flex-start;
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

  .display-none {
    display: none;
  }
`;

const emailInputName = "email";

class Confirm extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      email: props.linkData.email,
      firstName: props.linkData.firstname,
      firstNameValid: true,
      lastName: props.linkData.lastname,
      lastNameValid: true,
      password: "",
      passwordValid: true,
      errorText: "",
      isLoading: false,
      passwordEmpty: false,
      key: props.linkData.confirmHeader,
      linkType: props.linkData.type,
      userId: props.linkData.uid,
    };
  }

  onSubmit = (e) => {
    this.setState({ isLoading: true }, function () {
      const { hashSettings, defaultPage } = this.props;

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

      if (!this.state.passwordValid) {
        hasError = true;
        this.setState({ passwordValid: !hasError });
      }

      if (!this.state.password.trim()) {
        this.setState({ passwordEmpty: true });
        hasError = true;
      }

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
      };

      this.activateConfirmUser(
        personalData,
        loginData,
        this.state.key,
        this.state.userId,
        EmployeeActivationStatus.Activated
      )
        .then(() => window.location.replace(defaultPage))
        .catch((error) => {
          console.error("activate error", error);
          this.setState({
            errorText: error,
            isLoading: false,
          });
        });
    });
  };

  activateConfirmUser = async (
    personalData,
    loginData,
    key,
    userId,
    activationStatus
  ) => {
    const changedData = {
      id: userId,
      FirstName: personalData.firstname,
      LastName: personalData.lastname,
    };

    const res1 = await changePassword(userId, loginData.passwordHash, key);

    console.log("changePassword", res1);

    const res2 = await updateActivationStatus(activationStatus, userId, key);

    console.log("updateActivationStatus", res2);

    const { login } = this.props;
    const { userName, passwordHash } = loginData;

    const res3 = await login(userName, passwordHash);

    console.log("Login", res3);

    const res4 = await updateUser(changedData);

    console.log("updateUser", res4);
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
    const { getSettings, getPortalPasswordSettings, history } = this.props;
    const requests = [getSettings(), getPortalPasswordSettings(this.state.key)];

    axios.all(requests).catch((e) => {
      console.error("get settings error", e);
      history.push(combineUrl(AppServerConfig.proxyURL, `/login/error=${e}`));
    });

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

  onChangePassword = (event) => {
    this.setState({ password: event.target.value });
    !this.state.passwordValid && this.setState({ passwordValid: true });
    event.target.value.trim() && this.setState({ passwordEmpty: false });
    this.state.errorText && this.setState({ errorText: "" });
    this.onKeyPress(event);
  };

  render() {
    console.log("ActivateUser render");
    const { settings, t, greetingTitle, theme } = this.props;
    return !settings ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
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
              <Text
                as="p"
                fontSize="24px"
                color={theme.studio.confirm.activateUser.textColor}
              >
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

              <TextInput
                className="confirm-row display-none"
                id="email"
                name={emailInputName}
                value={this.state.email}
                size="huge"
                scale={true}
                isReadOnly={true}
              />
            </div>

            <PasswordInput
              className="confirm-row"
              id="password"
              inputName="password"
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
            />

            <Button
              className="confirm-row"
              primary
              size="normal"
              label={t("LoginRegistryButton")}
              tabIndex={5}
              isLoading={this.state.isLoading}
              onClick={this.onSubmit}
            />
          </div>

          {/*             <Row className='confirm-row'>

                    <Text as='p' fontSize='14px'>{t('LoginWithAccount')}</Text>

            </Row>
 */}
          <Text
            className="confirm-row"
            fontSize="14px"
            color={theme.studio.confirm.activateUser.textColorError}
          >
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
const ActivateUserForm = (props) => (
  <Section>
    <Section.SectionBody>
      <Confirm {...props} />
    </Section.SectionBody>
  </Section>
);

export default inject(({ auth }) => {
  const {
    greetingSettings,
    hashSettings,
    defaultPage,
    passwordSettings,
    getSettings,
    getPortalPasswordSettings,
    theme,
  } = auth.settingsStore;

  return {
    theme,
    settings: passwordSettings,
    greetingTitle: greetingSettings,
    hashSettings,
    defaultPage,
    getSettings,
    getPortalPasswordSettings,
    login: auth.login,
  };
})(
  withRouter(withTranslation(["Confirm", "Common"])(observer(ActivateUserForm)))
);
