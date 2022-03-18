import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import toastr from "@appserver/components/toast/toastr";
import Section from "@appserver/common/components/Section";
import { tryRedirectTo } from "@appserver/common/utils";
import { inject, observer } from "mobx-react";
import withLoader from "../withLoader";
import { Base } from "@appserver/components/themes";

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
        color: ${(props) => props.theme.studio.confirm.change.titleColor};
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

BodyStyle.defaultProps = { theme: Base };

class Form extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = { showButtons: true };
  }

  onAcceptClick = () => {
    this.setState({ showButtons: false });
    toastr.success(t("ConfirmOwnerPortalSuccessMessage"));
    setTimeout(this.onRedirect, 10000);
  };

  onRedirect = () => {
    tryRedirectTo(this.props.defaultPage);
  };

  onCancelClick = () => {
    tryRedirectTo(this.props.defaultPage);
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
            <Text className="owner-confirm_text" fontSize="18px">
              {t("ConfirmOwnerPortalTitle", { newOwner: "NEW OWNER" })}
            </Text>
            {this.state.showButtons ? (
              <>
                <Button
                  className="owner-button owner-buttons"
                  primary
                  size="normal"
                  label={t("Common:SaveButton")}
                  tabIndex={2}
                  isDisabled={false}
                  onClick={this.onAcceptClick}
                />
                <Button
                  className="owner-buttons"
                  size="normal"
                  label={t("Common:CancelButton")}
                  tabIndex={2}
                  isDisabled={false}
                  onClick={this.onCancelClick}
                />
              </>
            ) : (
              <Text className="owner-confirm-message" fontSize="12px">
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

const ChangeOwnerForm = (props) => (
  <Section>
    <Section.SectionBody>
      <Form {...props} />
    </Section.SectionBody>
  </Section>
);

export default inject(({ auth }) => ({
  greetingTitle: auth.settingsStore.greetingSettings,
  defaultPage: auth.settingsStore.defaultPage,
}))(
  withRouter(
    withTranslation(["Confirm", "Common"])(
      withLoader(observer(ChangeOwnerForm))
    )
  )
);
