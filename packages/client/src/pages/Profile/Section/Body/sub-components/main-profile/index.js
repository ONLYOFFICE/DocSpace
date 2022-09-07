import React, { useState } from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";

import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";

import withCultureNames from "@docspace/common/hoc/withCultureNames";
import { smallTablet } from "@docspace/components/utils/device";

import { getUserRole } from "../../../../../../helpers/people-helpers";

import LanguagesCombo from "./languagesCombo";
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

  .avatar {
    height: 124px;
    width: 124px;
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
      min-width: 60px;
      max-width: 60px;
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

  const { profile, cultureNames, culture, updateProfile } = props;
  const { cultureName, currentCulture } = profile;

  const [changeNameDialogVisible, setChangeNameDialogVisible] = useState(false);
  const [changeEmailDialogVisible, setChangeEmailDialogVisible] = useState(
    false
  );
  const [
    changePasswordDialogVisible,
    setChangePasswordDialogVisible,
  ] = useState(false);
  const [avatarDialogVisible, setAvatarDialogVisible] = useState(false);

  const role = getUserRole(profile);

  return (
    <StyledWrapper>
      <Avatar
        className="avatar"
        size="max"
        role={role}
        source={profile.avatarMax}
        userName={profile.displayName}
        editing={true}
        editAction={() => setAvatarDialogVisible(true)}
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
            onClick={() => setChangeNameDialogVisible(true)}
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
            onClick={() => setChangeEmailDialogVisible(true)}
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
            onClick={() => setChangePasswordDialogVisible(true)}
          />
        </div>

        <LanguagesCombo
          t={t}
          profile={profile}
          cultureName={cultureName}
          currentCulture={currentCulture}
          culture={culture}
          cultureNames={cultureNames}
        />
      </StyledInfo>

      {changeEmailDialogVisible && (
        <ChangeEmailDialog
          visible={changeEmailDialogVisible}
          onClose={() => setChangeEmailDialogVisible(false)}
          user={profile}
        />
      )}

      {changePasswordDialogVisible && (
        <ChangePasswordDialog
          visible={changePasswordDialogVisible}
          onClose={() => setChangePasswordDialogVisible(false)}
          email={profile.email}
        />
      )}

      {changeNameDialogVisible && (
        <ChangeNameDialog
          t={t}
          tReady={ready}
          visible={changeNameDialogVisible}
          onClose={() => setChangeNameDialogVisible(false)}
          profile={profile}
          onSave={updateProfile}
        />
      )}

      {avatarDialogVisible && (
        <AvatarEditorDialog
          t={t}
          visible={avatarDialogVisible}
          onClose={() => setAvatarDialogVisible(false)}
          profile={profile}
        />
      )}
    </StyledWrapper>
  );
};

export default withCultureNames(MainProfile);
