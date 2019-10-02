import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import styled from "styled-components";
import { Container, Row, Col, Card, CardTitle, CardImg } from "reactstrap";
import {
  Button,
  PageLayout,
  Text,
  PasswordInput,
  Loader,
  toastr
} from "asc-web-components";
import { welcomePageTitle } from "../../../../helpers/customNames";
import {
  changePassword,
  getConfirmationInfo,
  logout
} from "../../../../../src/store/auth/actions";

const BodyStyle = styled(Container)`
  margin-top: 70px;
  p {
    margin-bottom: 5px;
  }
  .button-style {
    margin-top: 20px;
  }

  .password-row {
    margin: 23px 0 0;
    .password-card {
      border: none;
      .card-img {
        max-width: 216px;
        max-height: 35px;
      }
      .card-title {
        word-wrap: break-word;
        margin: 8px 0;
        text-align: left;
        font-size: 24px;
        color: #116d9d;
      }
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
      const { history, changePassword, logout } = this.props;
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

      changePassword(userId, { password }, key)
        .then(() => {
          logout();
          toastr.success(this.props.t("ChangePasswordSuccess"));
          history.push("/");
        })
        .catch(e => {
          toastr.error(this.props.t(`${e.message}`));
          this.setState({ isLoading: false });
        });
    });
  };

  componentDidMount() {
    const { getConfirmationInfo, history } = this.props;
    getConfirmationInfo(this.state.key).catch(e => {
      toastr.error(this.props.t(`${e.message}`));
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
    const { settings, isConfirmLoaded, t } = this.props;
    const { isLoading, password, passwordEmpty } = this.state;
    const mdOptions = { size: 6, offset: 3 };

    return !isConfirmLoaded ? (
      <Loader className="pageLoader" type="rombs" size={40} />
    ) : (
      <BodyStyle>
        <Row className="password-row">
          <Col sm="12" md={mdOptions}>
            <Card className="password-card">
              <CardImg
                className="card-img"
                src="images/dark_general.png"
                alt="Logo"
                top
              />
              <CardTitle className="card-title">
                {t("CustomWelcomePageTitle", { welcomePageTitle })}
              </CardTitle>
            </Card>
            <Text.Body fontSize={14}>{t("PassworResetTitle")}</Text.Body>

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
              className="button-style"
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
          </Col>
        </Row>
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

function mapStateToProps(store) {
  return {
    isValidConfirmLink: store.auth.isValidConfirmLink,
    isConfirmLoaded: store.auth.isConfirmLoaded,
    settings: store.auth.password,
    isAuthenticated: store.auth.isAuthenticated
  };
}

export default connect(
  mapStateToProps,
  { changePassword, getConfirmationInfo, logout }
)(withRouter(withTranslation()(ChangePasswordForm)));
