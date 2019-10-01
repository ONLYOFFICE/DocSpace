import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import styled from "styled-components";
import {
  Container,
  Collapse,
  Row,
  Col,
  Card,
  CardTitle,
  CardImg
} from "reactstrap";
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
  getPasswordSettings
} from "../../../../../src/store/auth/actions";

const BodyStyle = styled(Container)`
  margin-top: 70px;
  p {
    margin-bottom: 5px;
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

    const { match, location } = props;

    const str = location.search.split("&");
    const userId = str[2].slice(4);
    const indexOfSlash = match.path.lastIndexOf("/");
    const typeLink = match.path.slice(indexOfSlash + 1);
    const queryString = `type=${typeLink}&${location.search.slice(1)}`;

    this.state = {
      password: "",
      passwordValid: true,
      isValidConfirmLink: false,
      errorText: "",
      isLoading: false,
      passwordEmpty: false,
      queryString: queryString,
      type: typeLink,
      userId: userId
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
    this.state.errorText && this.setState({ errorText: "" });
    this.onKeyPress(event);
  };

  onSubmit = e => {
    this.setState({ isLoading: true }, function() {
      const { userId, password, queryString } = this.state;
      const { history, changePassword } = this.props;
      this.setState({ errorText: "" });
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

      changePassword(userId, { password }, queryString)
        .then(() => {
          console.log("UPDATE PASSWORD");
          history.push("/");
        })
        .catch(e => {
          history.push("/");
          console.log("ERROR UPDATE PASSWORD:", e.message);
          this.setState({ errorText: e.message });
          this.setState({ isLoading: false });
        });
    });
  };

  componentDidMount() {
    const { getPasswordSettings, history } = this.props;
    getPasswordSettings(this.state.queryString)
      .then(() => {
        console.log("GET PASSWORD SETTINGS SUCCESS");
      })
      .catch(e => {
        console.log("ERROR GET PASSWORD SETTINGS", e);
        history.push(`/login/error=${e}`);
      });

    window.addEventListener("keydown", this.onKeyPress);
    window.addEventListener("keyup", this.onKeyPress);
  }

  componentWillUnmount() {
    window.removeEventListener("keydown", this.onKeyPress);
    window.removeEventListener("keyup", this.onKeyPress);
  }

  onCopyToClipboard = () =>
    toastr.success(this.props.t("EmailAndPasswordCopiedToClipboard"));
    
  validatePassword = value => this.setState({ passwordValid: value });

  render() {
    const { settings, isConfirmLoaded, t } = this.props;
    const { isLoading, password, passwordEmpty, errorText } = this.state;
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
          </Col>
        </Row>

        <Row className="login-row">
          <Col sm="12" md={mdOptions}>
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
              onCopyToClipboard={this.onCopyToClipboard}
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
          </Col>
        </Row>
        <Row className="password-row">
          <Col sm="12" md={mdOptions}>
            <Button
              primary
              size="big"
              tabIndex={3}
              label={
                isLoading ? t("LoadingProcessing") : t("ImportContactsOkButton")
              }
              isDisabled={isLoading}
              isLoading={isLoading}
              onClick={this.onSubmit}
            />
          </Col>
        </Row>
        <Collapse isOpen={!!errorText}>
          <Row className="login-row">
            <Col sm="12" md={mdOptions}>
              <div className="alert alert-danger">{errorText}</div>
            </Col>
          </Row>
        </Collapse>
      </BodyStyle>
    );
  }
}

Form.propTypes = {
  match: PropTypes.object.isRequired,
  location: PropTypes.object.isRequired,
  history: PropTypes.object.isRequired,
  changePassword: PropTypes.func.isRequired
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
    isConfirmLoaded: state.auth.isConfirmLoaded,
    settings: state.auth.password
  };
}

export default connect(
  mapStateToProps,
  { changePassword, getPasswordSettings }
)(withRouter(withTranslation()(ChangePasswordForm)));
