import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import styled from "styled-components";
//import { Container, Row, Col, Card, CardTitle, CardImg } from "reactstrap";
import { Button, PageLayout, Text, toastr } from "asc-web-components";
//import {  } from "../../../../../src/store/auth/actions";

/*const BodyStyle = styled(Container)`
  margin-top: 70px;

  .buttons-style {
    margin-top: 20px;
    min-width: 110px;
  }
  .button-style {
    margin-right: 8px;
  }
  .confirm_text {
    margin: 12px 0;
  }

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

  .row_display {
    display: flex;
  }
  .confirm-text {
    margin-top: 32px;
  }
`;*/

const BodyStyle = styled.div`
  .wrapper {
    display: grid;
    grid-template-columns: repeat(10, 1fr);
    grid-gap: 10px;
    grid-auto-rows: minmax(90px, auto);
  }
  .one {
    margin-left: 10px;

    grid-column: 4 / 10;
    grid-row: 2 / 3;

    .card-img {
      max-width: 216px;
      max-height: 35px;
    }
  }
  .two {
    grid-column: 1 / 4;
    grid-row: 1 / 3;
  }
  .three {
    grid-column: 1;
    grid-row: 2 / 5;
  }
  .four {
    grid-column: 3;
    grid-row: 3;
  }
  .five {
    grid-column: 2;
    grid-row: 4;
  }
  .six {
    grid-column: 3;
    grid-row: 4;
  }
`;

class Form extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = { showButtons: true };
  }

  onAcceptClick = () => {
    this.setState({ showButtons: false });
    toastr.success("Accept click");
    setTimeout(this.onRedirect, 10000);
  };

  onRedirect = () => {
    this.props.history.push("/");
  };

  onCancelClick = () => {
    this.props.history.push("/");
  };

  render() {
    const { t, greetingTitle } = this.props;
    const mdOptions = { size: 6, offset: 2 };

    return (
      <BodyStyle>
        <div class="wrapper">
          <div class="one">
            <img
              class="card-img"
              src="images/dark_general.png"
              alt="Logo"
            ></img>
          </div>

          <div class="two">Two</div>
          <div class="three">Three</div>
          <div class="four">Four</div>
          <div class="five">Five</div>
          <div class="six">Six</div>
        </div>
      </BodyStyle>
    );
  }
}

Form.propTypes = {};

Form.defaultProps = {};

const ChangePasswordForm = props => (
  <PageLayout sectionBodyContent={<Form {...props} />} />
);

function mapStateToProps(state) {
  return { greetingTitle: state.auth.settings.greetingSettings };
}

export default connect(
  mapStateToProps,
  {}
)(withRouter(withTranslation()(ChangePasswordForm)));
/*
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
              <CardTitle className="card-title">{greetingTitle}</CardTitle>
            </Card>
          </Col>
        </Row>
        <Row>
          <Col sm="12" md={{ size: 12, offset: 2 }}>
            <Text.Body className="confirm_text" fontSize={18}>
              {t("ConfirmOwnerPortalTitle", { newOwner: "NEW OWNER" })}
            </Text.Body>
          </Col>
        </Row>
        {this.state.showButtons ? (
          <Row>
            <Col className="row_display" sm="12" md={mdOptions}>
              <Button
                className="button-style buttons-style"
                primary
                size="big"
                label={t("SaveButton")}
                tabIndex={2}
                isDisabled={false}
                onClick={this.onAcceptClick}
              />
              <Button
                className="buttons-style"
                size="big"
                label={t("CancelButton")}
                tabIndex={2}
                isDisabled={false}
                onClick={this.onCancelClick}
              />
            </Col>
          </Row>
        ) : (
          <Row>
            <Col sm="12" md={{ size: 12, offset: 2 }}>
              <Text.Body className="confirm-text" fontSize={12}>
                {t("ConfirmOwnerPortalSuccessMessage")}
              </Text.Body>
            </Col>
          </Row>
        )}
      </BodyStyle>
*/
