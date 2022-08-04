import React from "react";
import PropTypes from "prop-types";

import Link from "@docspace/components/link";
import ModalDialog from "@docspace/components/modal-dialog";
import Checkbox from "@docspace/components/checkbox";
import Button from "@docspace/components/button";
import Textarea from "@docspace/components/textarea";
import Text from "@docspace/components/text";

import { withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";
import copy from "copy-to-clipboard";
import { inject, observer } from "mobx-react";

const textAreaName = "link-textarea";

class InviteDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    // const { userInvitationLink, guestInvitationLink } = props;
    this.state = {
      isGuest: false,
      // userInvitationLink: null,
      // guestInvitationLink: null,
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
        ? this.props.guestInvitationLink
        : this.props.userInvitationLink
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
    const {
      getShortenedLink,
      userInvitationLink,
      guestInvitationLink,
    } = this.props;

    const { isGuest } = this.state;
    const link = isGuest ? guestInvitationLink : userInvitationLink;

    getShortenedLink(link, !isGuest)
      .then((link) => {
        if (!isGuest) {
          this.setState({ userInvitationLink: link, isLinkShort: true });
        } else {
          this.setState({ guestInvitationLink: link, isLinkShort: true });
        }
      })
      .catch((e) => console.error("getShortInvitationLink error", e)) // TODO: add translation
      .finally(() => this.setState({ isLoading: false }));
  };

  componentDidMount() {
    const {
      getPortalInviteLinks,
      userInvitationLink,
      guestInvitationLink,
    } = this.props;

    if (!userInvitationLink || !guestInvitationLink) {
      getPortalInviteLinks().then(() => {
        this.setState({
          visible: true,
        });
      });
    } else {
      this.setState({ visible: true });
    }
  }

  onClickToCloseButton = () =>
    this.props.onCloseButton && this.props.onCloseButton();
  onClose = () => this.props.onClose && this.props.onClose();

  render() {
    console.log("InviteDialog render");
    const { t, tReady, visible, hasShortenService, guestsCaption } = this.props;
    const { LinkCopySuccess, ChangeTextAnim } = this.state;

    return (
      this.state.visible && (
        <ModalDialogContainer
          isLoading={!tReady}
          ChangeTextAnim={ChangeTextAnim}
          visible={visible}
          onClose={this.onClose}
          zIndex={400}
          autoMaxHeight
        >
          <ModalDialog.Header>
            {t("PeopleTranslations:InviteLinkTitle")}
          </ModalDialog.Header>
          <ModalDialog.Body>
            {/* <Text as="p">{t("HelpAnswerLinkInviteSettings")}</Text> */}
            <Text className="text-dialog" as="p">
              {t("InviteLinkValidInterval", { count: 7 })}
            </Text>
            {/* <Link
                className="link-dialog"
                type="action"
                isHovered={LinkCopySuccess ? false : true}
                noHover={LinkCopySuccess}
                onClick={
                  LinkCopySuccess ? undefined : this.onCopyLinkToClipboard
                }
              >
                {LinkCopySuccess ? t("LinkCopySuccess") : t("CopyToClipboard")}
              </Link> */}
            {hasShortenService && !this.state.isLinkShort && (
              <Link
                type="action"
                isHovered={true}
                onClick={this.onGetShortenedLink}
              >
                {t("GetShortenLink")}
              </Link>
            )}
            <Textarea
              className="textarea-dialog"
              isReadOnly={true}
              isDisabled={this.state.isLoading}
              name={textAreaName}
              value={
                this.state.isGuest
                  ? this.props.guestInvitationLink
                  : this.props.userInvitationLink
              }
            />
            <Checkbox
              className="checkbox-dialog"
              label={t("InviteUsersAsCollaborators", { guestsCaption })}
              isChecked={this.state.isGuest}
              onChange={this.onCheckedGuest}
              isDisabled={this.state.isLoading}
            />
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              key="CloseBtn"
              label={
                this.state.isLoading
                  ? t("Common:LoadingProcessing")
                  : t("CopyToClipboard")
              }
              size="normal"
              primary
              scale
              onClick={LinkCopySuccess ? undefined : this.onCopyLinkToClipboard}
              isLoading={this.state.isLoading}
            />
            <Button
              key="CloseBtn"
              label={t("Common:CloseButton")}
              size="normal"
              scale
              onClick={this.onClickToCloseButton}
              isLoading={this.state.isLoading}
            />
          </ModalDialog.Footer>
        </ModalDialogContainer>
      )
    );
  }
}

const InviteDialog = withTranslation([
  "InviteDialog",
  "Common",
  "PeopleTranslations",
])(InviteDialogComponent);

InviteDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  onCloseButton: PropTypes.func.isRequired,
};

export default inject(({ auth, peopleStore }) => ({
  hasShortenService: auth.settingsStore.hasShortenService,
  guestsCaption: auth.settingsStore.customNames.guestsCaption,
  getPortalInviteLinks: peopleStore.inviteLinksStore.getPortalInviteLinks,
  getShortenedLink: peopleStore.inviteLinksStore.getShortenedLink,
  userInvitationLink: peopleStore.inviteLinksStore.userLink,
  guestInvitationLink: peopleStore.inviteLinksStore.guestLink,
}))(observer(InviteDialog));
