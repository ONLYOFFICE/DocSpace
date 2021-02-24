import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import styled from "styled-components";
import {
  Button,
  Text,
  PasswordInput,
  Loader,
  toastr,
  Heading,
} from "asc-web-components";
import { PageLayout } from "asc-web-common";
import { utils as commonUtils, api } from "asc-web-common";

import { inject, observer } from "mobx-react";

const { createPasswordHash, tryRedirectTo } = commonUtils;

const BodyStyle = styled.form`
  margin: 70px auto 0 auto;
  max-width: 500px;
  .password-header {
    margin-bottom: 24px;
    .password-logo {
      max-width: 216px;
      max-height: 35px;
    }
    .password-title {
      margin: 8px 0;
    }
  }
  .password-text {
    margin-bottom: 5px;
  }
  .password-button {
    margin-top: 20px;
  }
`;

class Form extends React.PureComponent {
  constructor(props) {
    super(props);
    const { linkData } = props;

    this.state = {
      isConfirmLoaded: false,
      password: "",
      passwordValid: true,
      // isValidConfirmLink: false,
      isLoading: false,
      passwordEmpty: false,
      key: linkData.confirmHeader,
      userId: linkData.uid,
    };
  }

  onKeyPress = (target) => {
    if (target.key === "Enter") {
      this.onSubmit();
    }
  };

  onChange = (event) => {
    this.setState({ password: event.target.value });
    !this.state.passwordValid && this.setState({ passwordValid: true });
    event.target.value.trim() && this.setState({ passwordEmpty: false });
    this.onKeyPress(event);
  };

  onSubmit = (e) => {
    this.setState({ isLoading: true }, function () {
      const { userId, password, key } = this.state;
      const { hashSettings, defaultPage } = this.props;
      let hasError = false;

      if (!this.state.passwordValid) {
        hasError = true;
        this.setState({ passwordValid: !hasError });
      }

      !this.state.password.trim() && this.setState({ passwordEmpty: true });

      if (hasError) {
        this.setState({ isLoading: false });
        return false;
      }
      const hash = createPasswordHash(password, hashSettings);

      api.people
        .changePassword(userId, hash, key)
        .then(() => this.props.logout())
        .then(() => {
          toastr.success(this.props.t("ChangePasswordSuccess"));
          tryRedirectTo(defaultPage);
        })
        .catch((error) => {
          toastr.error(this.props.t(`${error}`));
          this.setState({ isLoading: false });
        });
    });
  };

  componentDidMount() {
    const { defaultPage, getSettings, getPortalPasswordSettings } = this.props;

    const requests = [getSettings(), getPortalPasswordSettings(this.state.key)];

    Promise.all(requests)
      .then(() => {
        this.setState({ isConfirmLoaded: true });
        console.log("get settings success");
      })
      .catch((error) => {
        toastr.error(this.props.t(`${error}`));
        tryRedirectTo(defaultPage);
      });

    window.addEventListener("keydown", this.onKeyPress);
    window.addEventListener("keyup", this.onKeyPress);
  }

  componentWillUnmount() {
    window.removeEventListener("keydown", this.onKeyPress);
    window.removeEventListener("keyup", this.onKeyPress);
  }

  validatePassword = (value) => this.setState({ passwordValid: value });

  render() {
    const { settings, isConfirmLoaded, t, greetingTitle } = this.props;
    const { isLoading, password, passwordEmpty } = this.state;

    return !isConfirmLoaded ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <BodyStyle>
        <div className="password-header">
          <img
            className="password-logo"
            src="images/dark_general.png"
            alt="Logo"
          />
          <Heading className="password-title" color="#116d9d">
            {greetingTitle}
          </Heading>
        </div>
        <Text className="password-text" fontSize="14px">
          {t("PassworResetTitle")}
        </Text>
        <PasswordInput
          id="password"
          name="password"
          inputName="password"
          inputValue={password}
          size="huge"
          scale={true}
          type="password"
          isDisabled={isLoading}
          hasError={passwordEmpty}
          onValidateInput={this.validatePassword}
          generatorSpecial="!@#$%^&*"
          tabIndex={1}
          value={password}
          onChange={this.onChange}
          emailInputName="E-mail"
          passwordSettings={settings}
          tooltipPasswordTitle="Password must contain:"
          tooltipPasswordLength={`${t("ErrorPasswordLength", {
            fromNumber: settings.minLength,
            toNumber: 30,
          })}:`}
          placeholder={t("PasswordCustomMode")}
          maxLength={30}
          onKeyDown={this.onKeyPress}
          isAutoFocussed={true}
          inputWidth="490px"
        />
        <Button
          id="button"
          className="password-button"
          primary
          size="big"
          tabIndex={2}
          label={
            isLoading ? t("LoadingProcessing") : t("ImportContactsOkButton")
          }
          isDisabled={isLoading}
          isLoading={isLoading}
          onClick={this.onSubmit}
        />
      </BodyStyle>
    );
  }
}

Form.propTypes = {
  history: PropTypes.object.isRequired,
  changePassword: PropTypes.func.isRequired,
  logout: PropTypes.func.isRequired,
  linkData: PropTypes.object.isRequired,
};

Form.defaultProps = {
  password: "",
};

const ChangePasswordForm = (props) => (
  <PageLayout>
    <PageLayout.SectionBody>
      <Form {...props} />
    </PageLayout.SectionBody>
  </PageLayout>
);

export default inject(({ auth }) => {
  const { settingsStore, logout, isAuthenticated } = auth;
  const {
    greetingSettings,
    hashSettings,
    defaultPage,
    passwordSettings,
    getSettings,
    getPortalPasswordSettings,
  } = settingsStore;
  return {
    settings: passwordSettings,
    hashSettings,
    greetingTitle: greetingSettings,
    defaultPage,
    logout,
    isAuthenticated,
    getSettings,
    getPortalPasswordSettings,
  };
})(withRouter(withTranslation("Confirm")(observer(ChangePasswordForm))));
