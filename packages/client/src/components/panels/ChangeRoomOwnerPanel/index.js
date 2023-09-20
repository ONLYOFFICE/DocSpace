import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";
import Aside from "@docspace/components/aside";
import Backdrop from "@docspace/components/backdrop";
import PeopleSelector from "@docspace/client/src/components/PeopleSelector";
import { withTranslation } from "react-i18next";
import Filter from "@docspace/common/api/people/filter";
import { EmployeeType, ShareAccessRights } from "@docspace/common/constants";
import toastr from "@docspace/components/toast/toastr";

const StyledChangeRoomOwner = styled.div`
  display: contents;

  ${({ showBackButton }) =>
    !showBackButton &&
    css`
      .arrow-button {
        display: none;
      }

      .selector_body {
        height: calc(((100% - 16px) - 111px) - 54px);
      }

      .selector_footer {
        height: 110px;
        min-height: 110px;
        max-height: 110px;
      }

      .selector_footer-checkbox {
        padding: 17px 0 1px 0;
      }
    `}
`;

const ChangeRoomOwner = (props) => {
  const {
    t,
    visible,
    setIsVisible,
    showBackButton,
    setRoomOwner,
    roomId,
    setFolder,
    updateRoomMemberRole,
    userId,
    isAdmin,
    setRoomParams,
    removeFiles,
    folders,
    setFolders,
  } = props;

  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    document.addEventListener("keyup", onKeyUp, false);

    return () => {
      document.removeEventListener("keyup", onKeyUp, false);
    };
  }, []);

  const onKeyUp = (e) => {
    if (e.keyCode === 27) onClose();
    if (e.keyCode === 13 || e.which === 13) onChangeRoomOwner();
  };

  const onLeaveRoom = () => {
    setIsLoading(true);
    updateRoomMemberRole(roomId, {
      invitations: [{ id: userId, access: ShareAccessRights.None }],
    })
      .then(() => {
        if (!isAdmin) removeFiles(null, [roomId]);
        else {
          const newFolders = folders;
          const folderIndex = newFolders.findIndex((r) => r.id === roomId);
          newFolders[folderIndex].inRoom = false;
          setFolders(newFolders);
        }
        toastr.success(t("Files:LeftAndAppointNewOwner"));
      })

      .finally(() => {
        onClose();
        setIsLoading(false);
      });
  };

  const onChangeRoomOwner = (
    user,
    selectedAccess,
    newFooterInputValue,
    isChecked
  ) => {
    setIsLoading(true);

    setRoomOwner(user[0].id, [roomId])
      .then(async (res) => {
        setFolder(res[0]);
        if (isChecked) await onLeaveRoom();
        else toastr.success(t("Files:AppointNewOwner"));
        setRoomParams && setRoomParams(res[0].createdBy);
      })
      .finally(() => {
        setIsLoading(false);
        onClose();
      });
  };

  const onClose = () => {
    setIsVisible(false);
  };

  const onBackClick = () => {
    onClose();
  };

  const filter = new Filter();
  filter.role = [EmployeeType.Admin, EmployeeType.User]; // 1(EmployeeType.User) - RoomAdmin | 3(EmployeeType.Admin) - DocSpaceAdmin

  const backClickProp = showBackButton ? { onBackClick } : {};

  return (
    <StyledChangeRoomOwner showBackButton={showBackButton}>
      <Backdrop
        onClick={onClose}
        visible={visible}
        zIndex={320}
        isAside={true}
      />
      <Aside
        className="header_aside-panel"
        visible={visible}
        onClose={onClose}
        withoutBodyScroll
      >
        <PeopleSelector
          withCancelButton
          {...backClickProp}
          onAccept={onChangeRoomOwner}
          onCancel={onClose}
          acceptButtonLabel={t("Files:AssignOwner")}
          headerLabel={t("Files:ChangeTheRoomOwner")}
          filter={filter}
          isLoading={isLoading}
          withFooterCheckbox={!showBackButton}
          footerCheckboxLabel={t("Files:LeaveTheRoom")}
          isChecked={!showBackButton}
        />
      </Aside>
    </StyledChangeRoomOwner>
  );
};

export default inject(
  ({ auth, dialogsStore, filesStore, selectedFolderStore }) => {
    const {
      changeRoomOwnerIsVisible,
      setChangeRoomOwnerIsVisible,
      changeRoomOwnerData,
    } = dialogsStore;
    const { user } = auth.userStore;
    const {
      setRoomOwner,
      selection,
      bufferSelection,
      setFolder,
      updateRoomMemberRole,
      removeFiles,
      folders,
      setFolders,
    } = filesStore;

    const roomId = selection.length
      ? selection[0].id
      : bufferSelection
      ? bufferSelection.id
      : selectedFolderStore.id;

    return {
      visible: changeRoomOwnerIsVisible,
      setIsVisible: setChangeRoomOwnerIsVisible,
      showBackButton: changeRoomOwnerData.showBackButton,
      setRoomParams: changeRoomOwnerData.setRoomParams,
      setRoomOwner,
      userId: user.id,
      roomId,
      setFolder,
      updateRoomMemberRole,
      isAdmin: user.isOwner || user.isAdmin,
      removeFiles,
      folders,
      setFolders,
    };
  }
)(observer(withTranslation(["Files"])(ChangeRoomOwner)));
