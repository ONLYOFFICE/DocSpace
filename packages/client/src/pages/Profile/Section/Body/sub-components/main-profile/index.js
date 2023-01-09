import SendClockReactSvgUrl from "ASSETS_DIR/images/send.clock.react.svg?url";
import PencilOutlineReactSvgUrl from "PUBLIC_DIR/images/pencil.outline.react.svg?url";
import React from "react";
import { ReactSVG } from "react-svg";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";

import { getUserRole } from "@docspace/common/utils";

import LanguagesCombo from "./languagesCombo";
import TimezoneCombo from "./timezoneCombo";

import {
  AvatarEditorDialog,
  ChangeEmailDialog,
  ChangePasswordDialog,
  ChangeNameDialog,
} from "SRC_DIR/components/dialogs";

import { StyledWrapper, StyledInfo } from "./styled-main-profile";
import { HelpButton } from "@docspace/components";

const MainProfile = (props) => {
  const { t } = useTranslation(["Profile", "Common"]);

  const {
    profile,
    changeEmailVisible,
    setChangeEmailVisible,
    changePasswordVisible,
    setChangePasswordVisible,
    changeNameVisible,
    setChangeNameVisible,
    changeAvatarVisible,
    setChangeAvatarVisible,
    withActivationBar,
    sendActivationLink,
  } = props;

  const role = getUserRole(profile);

  const sendActivationLinkAction = () => {
    sendActivationLink && sendActivationLink(t);
  };

  return (
    <StyledWrapper>
      <Avatar
        className={"avatar"}
        size="max"
        role={role}
        source={profile.avatarMax}
        userName={profile.displayName}
        editing={true}
        editAction={() => setChangeAvatarVisible(true)}
      />
      <StyledInfo>
        <div className="rows-container">
          <div className="row">
            <div className="field">
              <Text as="div" color="#A3A9AE" className="label">
                {t("Common:Name")}
              </Text>
              <Text fontWeight={600}>{profile.displayName}</Text>
            </div>
            <IconButton
              className="edit-button"
              iconName={PencilOutlineReactSvgUrl}
              size="12"
              onClick={() => setChangeNameVisible(true)}
            />
          </div>
          <div className="row">
            <div className="field">
              <Text as="div" color="#A3A9AE" className="label">
                {t("Common:Email")}
              </Text>
              <Text
                as="div"
                className={"email-text-container"}
                fontWeight={600}
              >
                {profile.email}
                {withActivationBar && (
                  <HelpButton
                    className="send-again-icon"
                    color={"#316daa"}
                    tooltipContent={t("EmailNotVerified")}
                    iconName={SendClockReactSvgUrl}
                  />
                )}
              </Text>

              {withActivationBar && (
                <Text
                  className="send-again-text"
                  fontWeight={600}
                  noSelect
                  truncate
                  onClick={sendActivationLinkAction}
                >
                  {t("SendAgain")}
                </Text>
              )}
            </div>
            <IconButton
              className="edit-button"
              iconName={PencilOutlineReactSvgUrl}
              size="12"
              onClick={() => setChangeEmailVisible(true)}
            />
            {withActivationBar && (
              <div className="send-again-container">
                <HelpButton
                  className="send-again-icon"
                  color={"#316daa"}
                  tooltipContent={t("EmailNotVerified")}
                  iconName={SendClockReactSvgUrl}
                />
                <Text
                  className="send-again-text"
                  fontWeight={600}
                  noSelect
                  truncate
                  onClick={sendActivationLinkAction}
                >
                  {t("SendAgain")}
                </Text>
              </div>
            )}
          </div>
          <div className="row">
            <div className="field">
              <Text as="div" color="#A3A9AE" className="label">
                {t("Common:Password")}
              </Text>
              <Text fontWeight={600}>********</Text>
            </div>
            <IconButton
              className="edit-button"
              iconName={PencilOutlineReactSvgUrl}
              size="12"
              onClick={() => setChangePasswordVisible(true)}
            />
          </div>
        </div>
        <LanguagesCombo t={t} />
        <TimezoneCombo title={t("Common:ComingSoon")} />
      </StyledInfo>

      {changeEmailVisible && (
        <ChangeEmailDialog
          visible={changeEmailVisible}
          onClose={() => setChangeEmailVisible(false)}
          user={profile}
        />
      )}

      {changePasswordVisible && (
        <ChangePasswordDialog
          visible={changePasswordVisible}
          onClose={() => setChangePasswordVisible(false)}
          email={profile.email}
        />
      )}

      {changeNameVisible && (
        <ChangeNameDialog
          visible={changeNameVisible}
          onClose={() => setChangeNameVisible(false)}
          profile={profile}
        />
      )}

      {changeAvatarVisible && (
        <AvatarEditorDialog
          t={t}
          visible={changeAvatarVisible}
          onClose={() => setChangeAvatarVisible(false)}
        />
      )}
    </StyledWrapper>
  );
};

export default inject(({ auth, peopleStore }) => {
  const { targetUserStore } = peopleStore;

  const { withActivationBar, sendActivationLink } = auth.userStore;

  const {
    targetUser: profile,
    changeEmailVisible,
    setChangeEmailVisible,
    changePasswordVisible,
    setChangePasswordVisible,
    changeNameVisible,
    setChangeNameVisible,
    changeAvatarVisible,
    setChangeAvatarVisible,
  } = targetUserStore;

  return {
    profile,
    changeEmailVisible,
    setChangeEmailVisible,
    changePasswordVisible,
    setChangePasswordVisible,
    changeNameVisible,
    setChangeNameVisible,
    changeAvatarVisible,
    setChangeAvatarVisible,
    withActivationBar,
    sendActivationLink,
  };
})(observer(MainProfile));
