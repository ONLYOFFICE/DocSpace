﻿import CrossReactSvgUrl from "PUBLIC_DIR/images/cross.react.svg?url";
import React from "react";
import { inject, observer } from "mobx-react";
import { ReactSVG } from "react-svg";
import { withTranslation } from "react-i18next";

import PeopleSelector from "SRC_DIR/components/PeopleSelector";

import Filter from "@docspace/common/api/people/filter";
import ModalDialog from "@docspace/components/modal-dialog";
import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import SelectorAddButton from "@docspace/components/selector-add-button";
import Button from "@docspace/components/button";
import Link from "@docspace/components/link";
import toastr from "@docspace/components/toast/toastr";

import {
  StyledOwnerInfo,
  StyledPeopleSelectorInfo,
  StyledPeopleSelector,
  StyledAvailableList,
  StyledFooterWrapper,
  StyledSelectedOwnerContainer,
  StyledSelectedOwner,
} from "./StyledDialog";

const filter = new Filter();

filter.employeeStatus = 1;

const ChangePortalOwnerDialog = ({
  t,
  visible,
  onClose,

  sendOwnerChange,

  displayName,
  avatar,
  id,
  currentColorScheme,
}) => {
  const [selectorVisible, setSelectorVisible] = React.useState(false);
  const [isLoading, setIsLoading] = React.useState(false);
  const [selectedUser, setSelectedUser] = React.useState(null);

  const onBackClick = () => {
    setSelectorVisible(false);
  };

  const onTogglePeopleSelector = () => {
    if (isLoading) return;
    setSelectedUser(null);
    setSelectorVisible((val) => !val);
  };

  const onAccept = (item) => {
    setSelectorVisible(false);
    setSelectedUser({ ...item[0] });
  };

  const onChangeAction = () => {
    setIsLoading(true);
    sendOwnerChange(selectedUser.id)
      .then(() => {
        onClose && onClose();
        toastr.success(
          t("Settings:ConfirmEmailSended", {
            ownerName: selectedUser.label,
          })
        );
      })
      .catch((error) => {
        toastr.error(error?.response?.data?.error?.message);
        onClose && onClose();
      });
  };

  const onCloseAction = () => {
    if (isLoading) return;
    onClose && onClose();
  };

  const onClearSelectedItem = () => {
    if (isLoading) return;
    setSelectedUser(null);
  };

  const ownerRights = [
    t("DoTheSame"),
    t("AppointAdmin"),
    t("SetAccessRights"),
    t("ManagePortal"),
    t("ManageUser"),
    t("ChangePortalOwner:ChangeOwner"),
    t("BackupPortal"),
    t("DeactivateOrDeletePortal"),
  ];

  return (
    <ModalDialog
      displayType={"aside"}
      visible={visible}
      onClose={onCloseAction}
      withBodyScroll
      withFooterBorder
      containerVisible={selectorVisible}
    >
      {selectorVisible && (
        <ModalDialog.Container>
          <PeopleSelector
            withCancelButton
            filter={filter}
            excludeItems={[id]}
            acceptButtonLabel={t("Common:SelectAction")}
            onAccept={onAccept}
            onCancel={onBackClick}
            onBackClick={onBackClick}
          />
        </ModalDialog.Container>
      )}
      <ModalDialog.Header>{t("Translations:OwnerChange")}</ModalDialog.Header>
      <ModalDialog.Body>
        <StyledOwnerInfo>
          <Avatar
            className="avatar"
            role={"owner"}
            source={avatar}
            size={"big"}
          />
          <div className="info">
            <Text className="display-name" noSelect title={displayName}>
              {displayName}
            </Text>
            <Text className="status" noSelect title={t("Common:Owner")}>
              {t("Common:Owner")}
            </Text>
          </div>
        </StyledOwnerInfo>

        <StyledPeopleSelectorInfo>
          <Text className="new-owner" noSelect title={t("NewPortalOwner")}>
            {t("NewPortalOwner")}
          </Text>
          <Text className="description" noSelect title={t("ChangeInstruction")}>
            {t("ChangeInstruction")}
          </Text>
        </StyledPeopleSelectorInfo>

        {selectedUser ? (
          <StyledSelectedOwnerContainer>
            <StyledSelectedOwner currentColorScheme={currentColorScheme}>
              <Text className="text">{selectedUser.label}</Text>
              <ReactSVG
                className="cross-icon"
                onClick={onClearSelectedItem}
                src={CrossReactSvgUrl}
              />
            </StyledSelectedOwner>

            <Link
              type={"action"}
              isHovered
              fontWeight={600}
              onClick={onTogglePeopleSelector}
            >
              {t("ChangeUser")}
            </Link>
          </StyledSelectedOwnerContainer>
        ) : (
          <StyledPeopleSelector>
            <SelectorAddButton
              className="selector-add-button"
              onClick={onTogglePeopleSelector}
            />
            <Text
              className="label"
              noSelect
              title={t("Translations:ChooseFromList")}
            >
              {t("Translations:ChooseFromList")}
            </Text>
          </StyledPeopleSelector>
        )}

        <StyledAvailableList>
          <Text className="list-header" noSelect title={t("PortalOwnerCan")}>
            {t("PortalOwnerCan")}
          </Text>

          {ownerRights?.map((item) => (
            <Text key={item} className="list-item" noSelect title={item}>
              — {item};
            </Text>
          ))}
        </StyledAvailableList>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <StyledFooterWrapper>
          <Text className="info" noSelect>
            {t("Settings:AccessRightsChangeOwnerConfirmText")}
          </Text>
          <div className="button-wrapper">
            <Button
              tabIndex={5}
              label={t("Common:ChangeButton")}
              size="normal"
              primary
              scale
              isDisabled={!selectedUser}
              onClick={onChangeAction}
              isLoading={isLoading}
            />
            <Button
              tabIndex={5}
              label={t("Common:CancelButton")}
              size="normal"
              scale
              onClick={onCloseAction}
              isDisabled={isLoading}
            />
          </div>
        </StyledFooterWrapper>
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default inject(({ auth, setup }) => {
  const { displayName, avatar, id } = auth.userStore.user;
  const { currentColorScheme } = auth.settingsStore;
  const { sendOwnerChange } = setup;

  return { displayName, avatar, id, sendOwnerChange, currentColorScheme };
})(
  withTranslation([
    "ChangePortalOwner",
    "Common",
    "Translations",
    "ProfileAction",
    "Settings",
  ])(observer(ChangePortalOwnerDialog))
);
