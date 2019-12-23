import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { connect } from "react-redux";
import styled from "styled-components";
import { Button, Text, toastr } from "asc-web-components";
import { PageLayout } from "asc-web-common";

const BodyStyle = styled.div`
  margin-top: 70px;

  .owner-container {
    display: grid;

    .owner-wrapper {
      align-self: center;
      justify-self: center;
      .owner-img {
        max-width: 216px;
        max-height: 35px;
      }
      .owner-title {
        word-wrap: break-word;
        margin: 8px 0;
        text-align: left;
        font-size: 24px;
        color: #116d9d;
      }
      .owner-confirm_text {
        margin: 20px 0 12px 0;
      }
      .owner-buttons {
        margin-top: 20px;
        min-width: 110px;
      }
      .owner-button {
        margin-right: 8px;
      }

      .owner-confirm-message {
        margin-top: 32px;
      }
    }
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

    return (
      <BodyStyle>
        <div className="owner-container">
          <div className="owner-wrapper">
            <img
              className="owner-img"
              src="images/dark_general.png"
              alt="Logo"
            />
            <Text className="owner-title">{greetingTitle}</Text>
            <Text className="owner-confirm_text" fontSize='18px'>
              {t("ConfirmOwnerPortalTitle", { newOwner: "NEW OWNER" })}
            </Text>
            {this.state.showButtons ? (
              <>
                <Button
                  className="owner-button owner-buttons"
                  primary
                  size="big"
                  label={t("SaveButton")}
                  tabIndex={2}
                  isDisabled={false}
                  onClick={this.onAcceptClick}
                />
                <Button
                  className="owner-buttons"
                  size="big"
                  label={t("CancelButton")}
                  tabIndex={2}
                  isDisabled={false}
                  onClick={this.onCancelClick}
                />
              </>
            ) : (
              <Text className="owner-confirm-message" fontSize='12px'>
                {t("ConfirmOwnerPortalSuccessMessage")}
              </Text>
            )}
          </div>
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
