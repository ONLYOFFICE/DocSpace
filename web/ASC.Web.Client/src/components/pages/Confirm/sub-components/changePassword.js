import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import axios from "axios";
import PropTypes from "prop-types";
import styled from "styled-components";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import PasswordInput from "@appserver/components/password-input";
import toastr from "@appserver/components/toast/toastr";
import Heading from "@appserver/components/heading";
import Section from "@appserver/common/components/Section";
import { createPasswordHash, tryRedirectTo } from "@appserver/common/utils";
import { PasswordLimitSpecialCharacters } from "@appserver/common/constants";
import { inject, observer } from "mobx-react";
import withLoader from "../withLoader";

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

  .password-input {
    .password-field-wrapper {
      width: 100%;
    }
  }
`;

class Form extends React.PureComponent {
  constructor(props) {
    super(props);
    const { linkData } = props;

    this.state = {
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
      const {
        t,
        hashSettings,
        defaultPage,
        logout,
        changePassword,
      } = this.props;
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

      changePassword(userId, hash, key)
        .then(() => logout())
        .then(() => {
          toastr.success(t("ChangePasswordSuccess"));
          tryRedirectTo(defaultPage);
        })
        .catch((error) => {
          toastr.error(t(`${error}`));
          this.setState({ isLoading: false });
        });
    });
  };

  componentDidMount() {
    window.addEventListener("keydown", this.onKeyPress);
    window.addEventListener("keyup", this.onKeyPress);
  }

  componentWillUnmount() {
    window.removeEventListener("keydown", this.onKeyPress);
    window.removeEventListener("keyup", this.onKeyPress);
  }

  validatePassword = (value) => this.setState({ passwordValid: value });

  render() {
    const { settings, t, greetingTitle, theme } = this.props;
    const { isLoading, password, passwordEmpty } = this.state;

    return (
      <BodyStyle>
        <div className="password-header">
          <img
            className="password-logo"
            src="images/dark_general.png"
            alt="Logo"
          />
          <Heading
            className="password-title"
            color={theme.studio.confirm.change.titleColor}
          >
            {greetingTitle}
          </Heading>
        </div>
        <Text className="password-text" fontSize="14px">
          {t("PassworResetTitle")}
        </Text>
        <PasswordInput
          id="password"
          className="password-input"
          name="password"
          inputName="password"
          inputValue={password}
          size="huge"
          scale={true}
          type="password"
          isDisabled={isLoading}
          hasError={passwordEmpty}
          onValidateInput={this.validatePassword}
          generatorSpecial={PasswordLimitSpecialCharacters}
          tabIndex={1}
          value={password}
          onChange={this.onChange}
          emailInputName="E-mail"
          passwordSettings={settings}
          tooltipPasswordTitle="Password must contain:"
          tooltipPasswordLength={`${t("Common:PasswordLimitLength", {
            fromNumber: settings ? settings.minLength : 8,
            toNumber: 30,
          })}:`}
          placeholder={t("Common:Password")}
          maxLength={30}
          onKeyDown={this.onKeyPress}
          isAutoFocussed={true}
          inputWidth="490px"
        />
        <Button
          id="button"
          className="password-button"
          primary
          size="normal"
          tabIndex={2}
          label={
            isLoading ? t("Common:LoadingProcessing") : t("Common:OKButton")
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
  logout: PropTypes.func.isRequired,
  linkData: PropTypes.object.isRequired,
};

Form.defaultProps = {
  password: "",
};

const ChangePasswordForm = (props) => (
  <Section>
    <Section.SectionBody>
      <Form {...props} />
    </Section.SectionBody>
  </Section>
);

export default inject(({ auth, setup }) => {
  const { settingsStore, logout, isAuthenticated } = auth;
  const {
    greetingSettings,
    hashSettings,
    defaultPage,
    passwordSettings,
    getSettings,
    getPortalPasswordSettings,
    theme,
  } = settingsStore;
  const { changePassword } = setup;

  return {
    theme,
    settings: passwordSettings,
    hashSettings,
    greetingTitle: greetingSettings,
    defaultPage,
    logout,
    isAuthenticated,
    getSettings,
    getPortalPasswordSettings,
    changePassword,
  };
})(
  withRouter(
    withTranslation(["Confirm", "Common"])(
      withLoader(observer(ChangePasswordForm))
    )
  )
);
