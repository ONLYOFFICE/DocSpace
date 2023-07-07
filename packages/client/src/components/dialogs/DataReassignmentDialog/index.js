import { useState } from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import PeopleSelector from "@docspace/client/src/components/PeopleSelector";

import Button from "@docspace/components/button";
import Avatar from "@docspace/components/avatar";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import CatalogSpamIcon from "PUBLIC_DIR/images/catalog.spam.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";

import SelectorAddButton from "@docspace/components/selector-add-button";
import { Base } from "@docspace/components/themes";

import {
  StyledOwnerInfo,
  StyledPeopleSelectorInfo,
  StyledPeopleSelector,
  StyledAvailableList,
  StyledFooterWrapper,
  StyledSelectedOwnerContainer,
  StyledSelectedOwner,
} from "../ChangePortalOwnerDialog/StyledDialog";

const StyledModalDialog = styled(ModalDialog)`
  .avatar-name {
    display: flex;
    align-items: center;
  }
`;

StyledModalDialog.defaultProps = { theme: Base };

const StyledCatalogSpamIcon = styled(CatalogSpamIcon)`
  ${commonIconsStyles}
  path {
    fill: #f21c0e;
  }

  padding-left: 8px;
`;

const DataReassignmentDialog = ({
  visible,
  user,
  setDataReassignmentDialogVisible,
  currentColorScheme,
  t,
}) => {
  const [selectorVisible, setSelectorVisible] = useState(false);
  const [selectedUser, setSelectedUser] = useState(null);

  const { avatar, displayName, statusType } = user;

  const onTogglePeopleSelector = () => {
    setSelectorVisible((show) => !show);
  };

  const onClose = () => {
    setDataReassignmentDialogVisible(false);
  };

  const onAccept = (item) => {
    setSelectorVisible(false);

    setSelectedUser({ ...item[0] });
  };

  return (
    <StyledModalDialog
      displayType="aside"
      visible={visible}
      onClose={onClose}
      containerVisible={selectorVisible}
      withFooterBorder
      withBodyScroll
    >
      {selectorVisible && (
        <ModalDialog.Container>
          <PeopleSelector
            acceptButtonLabel={t("Common:SelectAction")}
            onAccept={onAccept}
            onCancel={onClose}
            onBackClick={onTogglePeopleSelector}
            withCancelButton
            withAbilityCreateRoomUsers
          />
        </ModalDialog.Container>
      )}

      <ModalDialog.Header>Data reassignment</ModalDialog.Header>

      <ModalDialog.Body>
        <StyledOwnerInfo>
          <Avatar
            className="avatar"
            role="user"
            source={avatar}
            size={"big"}
            hideRoleIcon
          />
          <div className="info">
            <div className="avatar-name">
              <Text className="display-name" noSelect title={displayName}>
                {displayName}
              </Text>
              {statusType === "disabled" && (
                <StyledCatalogSpamIcon size="small" />
              )}
            </div>

            <Text className="status" noSelect>
              {statusType}
            </Text>
          </div>
        </StyledOwnerInfo>

        <StyledPeopleSelectorInfo>
          <Text className="new-owner" noSelect>
            New data owner
          </Text>
          <Text className="description" noSelect>
            User to whom the data will be transferred
          </Text>
        </StyledPeopleSelectorInfo>

        {selectedUser ? (
          <StyledSelectedOwnerContainer>
            <StyledSelectedOwner currentColorScheme={currentColorScheme}>
              <Text className="text">{selectedUser.label}</Text>
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
          <Text className="list-item" noSelect>
            We will transfer rooms created by user and documents stored in
            user’s rooms.
          </Text>
          <Text className="list-item" noSelect>
            Note: this action cannot be undone.
          </Text>
          <Text className="list-item" noSelect>
            More about data transfer
          </Text>
        </StyledAvailableList>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <StyledFooterWrapper>
          <Text className="info" noSelect>
            Delete profile when reassignment is finished
          </Text>
          <div className="button-wrapper">
            <Button
              tabIndex={5}
              label="Reassign"
              size="normal"
              primary
              scale
              isDisabled={!selectedUser}
              // onClick={onChangeAction}
              // isLoading={isLoading}
            />
            <Button
              tabIndex={5}
              label={t("Common:CancelButton")}
              size="normal"
              scale
              // onClick={onCloseAction}
              // isDisabled={isLoading}
            />
          </div>
        </StyledFooterWrapper>
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ auth, peopleStore }) => {
  const { dataReassignmentDialogVisible, setDataReassignmentDialogVisible } =
    peopleStore.dialogStore;
  const { currentColorScheme } = auth.settingsStore;
  return {
    dataReassignmentDialogVisible,
    setDataReassignmentDialogVisible,
    theme: auth.settingsStore.theme,
    currentColorScheme,
  };
})(observer(withTranslation(["Common,Translations"])(DataReassignmentDialog)));