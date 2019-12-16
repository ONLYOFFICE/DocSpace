import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import {
  toastr,
  ModalDialog,
  Link,
  Checkbox,
  Button,
  Textarea,
  Text
} from "asc-web-components";
import { withTranslation, I18nextProvider } from "react-i18next";
import i18n from "./i18n";
import { typeGuests } from "./../../../helpers/customNames";
import styled from "styled-components";
import copy from "copy-to-clipboard";
import { api } from "asc-web-common";
const { getShortenedLink } = api.portal;

const ModalDialogContainer = styled.div`
  .margin-text {
    margin: 12px 0;
  }

  .margin-link {
    margin-right: 12px;
  }

  .margin-textarea {
    margin-top: 12px;
  }

  .flex {
    display: flex;
    justify-content: space-between;
  }
`;

const textAreaName = "link-textarea";

class PureInviteDialog extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isGuest: false,
      userInvitationLink: this.props.userInvitationLink,
      guestInvitationLink: this.props.guestInvitationLink,
      isLoading: false,
      isLinkShort: false,
      visible: false
    };
  }

  onCopyLinkToClipboard = () => {
    // console.log("COPY");
    const { t } = this.props;
    copy(
      this.state.isGuest
        ? this.state.guestInvitationLink
        : this.state.userInvitationLink
    );
    toastr.success(t("LinkCopySuccess"));
  };

  onCheckedGuest = () => this.setState({ isGuest: !this.state.isGuest });

  onGetShortenedLink = () => {
    this.setState({ isLoading: true });
    const {
      userInvitationLink,
      guestInvitationLink
    } = this.props;

    getShortenedLink(userInvitationLink)
      .then(link => this.setState({ userInvitationLink: link }))
      .catch(e => {
        console.error("getShortInvitationLink error", e);
        this.setState({ isLoading: false });
      });

    getShortenedLink(guestInvitationLink)
      .then(link =>
        this.setState({
          guestInvitationLink: link,
          isLoading: false,
          isLinkShort: true
        })
      )
      .catch(e => {
        console.error("getShortInvitationLink error", e);
      });
  };

  componentDidMount() {
      this.onCopyLinkToClipboard();
  }

  onClickToCloseButton = () =>
    this.props.onCloseButton && this.props.onCloseButton();
  onClose = () => this.props.onClose && this.props.onClose();

  render() {
    console.log("InviteDialog render");
    const { t, visible, settings } = this.props;

    return (
      <ModalDialogContainer>
        <ModalDialog
          visible={visible}
          onClose={this.onClose}
          headerContent={t("InviteLinkTitle")}
          bodyContent={
            <>
              <Text className="margin-text" as="p">
                {t("HelpAnswerLinkInviteSettings")}
              </Text>
              <Text className="margin-text" as="p">
                {t("InviteLinkValidInterval", { count: 7 })}
              </Text>
              <div className="flex">
                <div>
                  <Link
                    className="margin-link"
                    type="action"
                    isHovered={true}
                    onClick={this.onCopyLinkToClipboard}
                  >
                    {t("CopyToClipboard")}
                  </Link>
                  {settings && !this.state.isLinkShort && (
                    <Link
                      type="action"
                      isHovered={true}
                      onClick={this.onGetShortenedLink}
                    >
                      {t("GetShortenLink")}
                    </Link>
                  )}
                </div>
                <Checkbox
                  label={t("InviteUsersAsCollaborators", { typeGuests })}
                  isChecked={this.state.isGuest}
                  onChange={this.onCheckedGuest}
                  isDisabled={this.state.isLoading}
                />
              </div>
              <Textarea
                className="margin-textarea"
                isReadOnly={true}
                isDisabled={this.state.isLoading}
                name={textAreaName}
                value={
                  this.state.isGuest
                    ? this.state.guestInvitationLink
                    : this.state.userInvitationLink
                }
              />
            </>
          }
          footerContent={
            <>
              <Button
                key="CloseBtn"
                label={
                  this.state.isLoading
                    ? t("LoadingProcessing")
                    : t("CloseButton")
                }
                size="medium"
                primary={true}
                onClick={this.onClickToCloseButton}
                isLoading={this.state.isLoading}
              />
            </>
          }
        />
      </ModalDialogContainer>
    );
  }
}

const mapStateToProps = state => {
  return {
    settings: state.auth.settings.hasShortenService,
    userInvitationLink: state.portal.inviteLinks.userLink,
    guestInvitationLink: state.portal.inviteLinks.guestLink
  };
};

const InviteDialogContainer = withTranslation()(PureInviteDialog);

const InviteDialog = props => (
  <I18nextProvider i18n={i18n}>
    <InviteDialogContainer {...props} />
  </I18nextProvider>
);

InviteDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  onCloseButton: PropTypes.func.isRequired
};

export default connect(mapStateToProps)(withRouter(InviteDialog));
