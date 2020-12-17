import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import {
  ModalDialog,
  Link,
  Checkbox,
  Button,
  Textarea,
  Text,
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";
import copy from "copy-to-clipboard";
import { api, utils } from "asc-web-common";
import { createI18N } from "../../../helpers/i18n";
import { getPortalInviteLinks } from "../../../store/portal/actions";
const i18n = createI18N({
  page: "InviteDialog",
  localesPath: "dialogs/InviteDialog",
});
const { getShortenedLink } = api.portal;
const { changeLanguage } = utils;

const textAreaName = "link-textarea";

class InviteDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    const { userInvitationLink, guestInvitationLink } = props;
    this.state = {
      isGuest: false,
      userInvitationLink,
      guestInvitationLink,
      isLoading: false,
      isLinkShort: false,
      visible: false,
      LinkCopySuccess: false,
      ChangeTextAnim: false,
    };
  }

  onCopyLinkToClipboard = () => {
    // console.log("COPY", this.props);
    copy(
      this.state.isGuest
        ? this.state.guestInvitationLink
        : this.state.userInvitationLink
    );

    this.ShowCopySuccessText();
  };

  ShowCopySuccessText = async () => {
    await this.StartChangeTextAnimation();

    this.setState({
      LinkCopySuccess: true,
    });

    setTimeout(async () => {
      await this.StartChangeTextAnimation();
      this.setState({ LinkCopySuccess: false });
    }, 1500);
  };

  StartChangeTextAnimation = () => {
    this.setState({ ChangeTextAnim: true });

    return new Promise((resolve) =>
      setTimeout(() => {
        this.setState({ ChangeTextAnim: false });
        resolve(true);
      }, 200)
    );
  };

  onCheckedGuest = () => this.setState({ isGuest: !this.state.isGuest });

  onGetShortenedLink = () => {
    this.setState({ isLoading: true });
    const { userInvitationLink, guestInvitationLink } = this.props;

    getShortenedLink(userInvitationLink)
      .then((link) => this.setState({ userInvitationLink: link }))
      .catch((e) => {
        console.error("getShortInvitationLink error", e);
        this.setState({ isLoading: false });
      });

    getShortenedLink(guestInvitationLink)
      .then((link) =>
        this.setState({
          guestInvitationLink: link,
          isLoading: false,
          isLinkShort: true,
        })
      )
      .catch((e) => {
        console.error("getShortInvitationLink error", e);
      });
  };

  componentDidMount() {
    const {
      getPortalInviteLinks,
      userInvitationLink,
      guestInvitationLink,
    } = this.props;

    changeLanguage(i18n).then(() => {
      if (!userInvitationLink || !guestInvitationLink) {
        getPortalInviteLinks().then(() => {
          this.setState({
            visible: true,
            userInvitationLink: this.props.userInvitationLink,
            guestInvitationLink: this.props.guestInvitationLink,
          });
        });
      } else {
        this.setState({ visible: true });
      }
    });
  }

  onClickToCloseButton = () =>
    this.props.onCloseButton && this.props.onCloseButton();
  onClose = () => this.props.onClose && this.props.onClose();

  render() {
    console.log("InviteDialog render");
    const { t, visible, settings, guestsCaption } = this.props;
    const { LinkCopySuccess, ChangeTextAnim } = this.state;

    return (
      this.state.visible && (
        <ModalDialogContainer ChangeTextAnim={ChangeTextAnim}>
          <ModalDialog visible={visible} onClose={this.onClose}>
            <ModalDialog.Header>{t("InviteLinkTitle")}</ModalDialog.Header>
            <ModalDialog.Body>
              <Text as="p">{t("HelpAnswerLinkInviteSettings")}</Text>
              <Text className="text-dialog" as="p">
                {t("InviteLinkValidInterval", { count: 7 })}
              </Text>
              <div className="flex">
                <div>
                  <Link
                    className="link-dialog"
                    type="action"
                    isHovered={LinkCopySuccess ? false : true}
                    noHover={LinkCopySuccess}
                    onClick={
                      LinkCopySuccess ? undefined : this.onCopyLinkToClipboard
                    }
                  >
                    {LinkCopySuccess
                      ? t("LinkCopySuccess")
                      : t("CopyToClipboard")}
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
                  label={t("InviteUsersAsCollaborators", { guestsCaption })}
                  isChecked={this.state.isGuest}
                  onChange={this.onCheckedGuest}
                  isDisabled={this.state.isLoading}
                />
              </div>
              <Textarea
                className="textarea-dialog"
                isReadOnly={true}
                isDisabled={this.state.isLoading}
                name={textAreaName}
                value={
                  this.state.isGuest
                    ? this.state.guestInvitationLink
                    : this.state.userInvitationLink
                }
              />
            </ModalDialog.Body>
            <ModalDialog.Footer>
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
            </ModalDialog.Footer>
          </ModalDialog>
        </ModalDialogContainer>
      )
    );
  }
}

const mapStateToProps = (state) => {
  return {
    settings: state.auth.settings.hasShortenService,
    userInvitationLink: state.portal.inviteLinks.userLink,
    guestInvitationLink: state.portal.inviteLinks.guestLink,
    guestsCaption: state.auth.settings.customNames.guestsCaption,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    getPortalInviteLinks: () => dispatch(getPortalInviteLinks()),
  };
};

const InviteDialogTranslated = withTranslation()(InviteDialogComponent);

const InviteDialog = (props) => (
  <InviteDialogTranslated i18n={i18n} {...props} />
);

InviteDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  onCloseButton: PropTypes.func.isRequired,
};

export default connect(mapStateToProps, mapDispatchToProps)(InviteDialog);
