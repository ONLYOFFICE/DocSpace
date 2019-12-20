import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import styled from "styled-components";
import {
  Button,
  Text,
  PasswordInput,
  Loader,
  toastr,
  Heading
} from "asc-web-components";
import { PageLayout } from "asc-web-common";
import { store } from "asc-web-common";
import { getConfirmationInfo, changePassword } from '../../../../store/confirm/actions';
const { logout } = store.auth.actions;

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
      password: "",
      passwordValid: true,
      isValidConfirmLink: false,
      isLoading: false,
      passwordEmpty: false,
      key: linkData.confirmHeader,
      userId: linkData.uid
    };
  }

  onKeyPress = target => {
    if (target.key === "Enter") {
      this.onSubmit();
    }
  };

  onChange = event => {
    this.setState({ password: event.target.value });
    !this.state.passwordValid && this.setState({ passwordValid: true });
    event.target.value.trim() && this.setState({ passwordEmpty: false });
    this.onKeyPress(event);
  };

  onSubmit = e => {
    this.setState({ isLoading: true }, function() {
      const { userId, password, key } = this.state;
      const { history, changePassword } = this.props;
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

      changePassword(userId, password, key)
        .then(() => this.props.logout())
        .then(() => {
          history.push("/");
          toastr.success(this.props.t("ChangePasswordSuccess"));
        })
        .catch(error => {
          toastr.error(this.props.t(`${error}`));
          this.setState({ isLoading: false });
        });
    });
  };

  componentDidMount() {
    const { getConfirmationInfo, history } = this.props;
    getConfirmationInfo(this.state.key).catch(error => {
      toastr.error(this.props.t(`${error}`));
      history.push("/");
    });

    window.addEventListener("keydown", this.onKeyPress);
    window.addEventListener("keyup", this.onKeyPress);
  }

  componentWillUnmount() {
    window.removeEventListener("keydown", this.onKeyPress);
    window.removeEventListener("keyup", this.onKeyPress);
  }

  validatePassword = value => this.setState({ passwordValid: value });

  render() {
    const { settings, isConfirmLoaded, t, greetingTitle } = this.props;
    const { isLoading, password, passwordEmpty } = this.state;

    return !isConfirmLoaded ? (
      <Loader className="pageLoader" type="rombs" size='40px' />
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
        <Text className="password-text" fontSize='14px'>
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
            fromNumber: 6,
            toNumber: 30
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
  linkData: PropTypes.object.isRequired
};

Form.defaultProps = {
  password: ""
};

const ChangePasswordForm = props => (
  <PageLayout sectionBodyContent={<Form {...props} />} />
);

function mapStateToProps(state) {
  return {
    isValidConfirmLink: state.auth.isValidConfirmLink,
    isConfirmLoaded: state.confirm.isConfirmLoaded,
    settings: state.auth.settings.passwordSettings,
    isAuthenticated: state.auth.isAuthenticated,
    greetingTitle: state.auth.settings.greetingSettings
  };
}

export default connect(mapStateToProps, {
  changePassword,
  getConfirmationInfo,
  logout
})(withRouter(withTranslation()(ChangePasswordForm)));
