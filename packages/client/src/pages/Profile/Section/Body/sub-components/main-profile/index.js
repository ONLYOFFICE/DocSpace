import React from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";

import { smallTablet } from "@docspace/components/utils/device";

import { getUserRole } from "../../../../../../helpers/people-helpers";

import LanguagesCombo from "./languagesCombo";
import TimezoneCombo from "./timezoneCombo";

import {
  AvatarEditorDialog,
  ChangeEmailDialog,
  ChangePasswordDialog,
  ChangeNameDialog,
} from "../../../../../../components/dialogs";

const StyledWrapper = styled.div`
  display: flex;
  padding: 24px;
  gap: 40px;
  background: ${(props) => props.theme.profile.main.background};
  border-radius: 12px;

  @media ${smallTablet} {
    background: none;
    flex-direction: column;
    gap: 24px;
    align-items: center;
    padding: 0;
  }
`;

const StyledInfo = styled.div`
  display: flex;
  flex-direction: column;
  gap: 16px;

  @media ${smallTablet} {
    width: 100%;
  }

  .row {
    display: flex;
    align-items: center;
    gap: 8px;

    .field {
      display: flex;
      gap: 24px;
    }

    .label {
      min-width: 75px;
      max-width: 75px;
      white-space: nowrap;
    }

    @media ${smallTablet} {
      gap: 8px;
      background: ${(props) => props.theme.profile.main.background};
      padding: 12px 16px;
      border-radius: 6px;

      .field {
        flex-direction: column;
        gap: 4px;
      }

      .label {
        min-width: 100%;
        max-width: 100%;
      }

      .edit-button {
        margin-left: auto;
      }
    }
  }
`;

const MainProfile = (props) => {
  const { t, ready } = useTranslation(["Profile", "Common"]);

  const {
    profile,
    updateProfile,
    fetchProfile,
    changeEmailVisible,
    setChangeEmailVisible,
    changePasswordVisible,
    setChangePasswordVisible,
    changeNameVisible,
    setChangeNameVisible,
    changeAvatarVisible,
    setChangeAvatarVisible,
  } = props;

  const role = getUserRole(profile);

  return (
    <StyledWrapper>
      <Avatar
        size="max"
        role={role}
        source={profile.avatarMax}
        userName={profile.displayName}
        editing={true}
        editAction={() => setChangeAvatarVisible(true)}
      />
      <StyledInfo>
        <div className="row">
          <div className="field">
            <Text as="div" color="#A3A9AE" className="label">
              {t("Common:Name")}
            </Text>
            <Text fontWeight={600}>{profile.displayName}</Text>
          </div>
          <IconButton
            className="edit-button"
            iconName="/static/images/pencil.outline.react.svg"
            size="12"
            onClick={() => setChangeNameVisible(true)}
          />
        </div>
        <div className="row">
          <div className="field">
            <Text as="div" color="#A3A9AE" className="label">
              {t("Common:Email")}
            </Text>
            <Text fontWeight={600}>{profile.email}</Text>
          </div>
          <IconButton
            className="edit-button"
            iconName="/static/images/pencil.outline.react.svg"
            size="12"
            onClick={() => setChangeEmailVisible(true)}
          />
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
            iconName="/static/images/pencil.outline.react.svg"
            size="12"
            onClick={() => setChangePasswordVisible(true)}
          />
        </div>

        <LanguagesCombo t={t} />
        <TimezoneCombo />
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
          t={t}
          tReady={ready}
          visible={changeNameVisible}
          onClose={() => setChangeNameVisible(false)}
          profile={profile}
          onSave={updateProfile}
        />
      )}

      {changeAvatarVisible && (
        <AvatarEditorDialog
          t={t}
          visible={changeAvatarVisible}
          onClose={() => setChangeAvatarVisible(false)}
          profile={profile}
          fetchProfile={fetchProfile}
        />
      )}
    </StyledWrapper>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;

  const {
    targetUser: profile,
    updateProfile,
    getTargetUser: fetchProfile,
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
    updateProfile,
    fetchProfile,
    changeEmailVisible,
    setChangeEmailVisible,
    changePasswordVisible,
    setChangePasswordVisible,
    changeNameVisible,
    setChangeNameVisible,
    changeAvatarVisible,
    setChangeAvatarVisible,
  };
})(observer(MainProfile));
