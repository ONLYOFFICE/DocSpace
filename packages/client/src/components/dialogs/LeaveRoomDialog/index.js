import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { ShareAccessRights } from "@docspace/common/constants";
import toastr from "@docspace/components/toast/toastr";
import RoomsFilter from "@docspace/common/api/rooms/filter";

const LeaveRoomDialog = (props) => {
  const {
    t,
    tReady,
    visible,
    setIsVisible,
    setChangeRoomOwnerIsVisible,
    isOwner,
    updateRoomMemberRole,
    roomId,
    userId,
    removeFiles,
    isAdmin,
    setSelected,
    isRoot,
    folders,
    setFolders,
  } = props;

  const navigate = useNavigate();

  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    document.addEventListener("keyup", onKeyUp, false);

    return () => {
      document.removeEventListener("keyup", onKeyUp, false);
    };
  }, []);

  const onKeyUp = (e) => {
    if (e.keyCode === 27) onClose();
    if (e.keyCode === 13 || e.which === 13) onLeaveRoom();
  };

  const onLeaveRoom = () => {
    if (isOwner) {
      setChangeRoomOwnerIsVisible(true);
      onClose();
    } else {
      setIsLoading(true);
      updateRoomMemberRole(roomId, {
        invitations: [{ id: userId, access: ShareAccessRights.None }],
      })
        .then(() => {
          if (!isAdmin) {
            if (isRoot) {
              const filter = RoomsFilter.getDefault();
              navigate(`rooms/shared/filter?${filter.toUrlParams()}`);
            } else {
              removeFiles(null, [roomId]);
            }
          } else {
            const newFolders = folders;
            const folderIndex = newFolders.findIndex((r) => r.id === roomId);
            newFolders[folderIndex].inRoom = false;
            setFolders(newFolders);
          }

          toastr.success(t("Files:YouLeftTheRoom"));
        })

        .finally(() => {
          onClose();
          setIsLoading(false);
          setSelected("none");
        });
    }
  };

  const onClose = () => setIsVisible(false);

  return (
    <ModalDialog isLoading={!tReady} visible={visible} onClose={onClose}>
      <ModalDialog.Header>{t("Files:LeaveTheRoom")}</ModalDialog.Header>
      <ModalDialog.Body>
        <div className="modal-dialog-content-body">
          <Text noSelect>
            {isOwner
              ? t("Files:LeaveRoomDescription")
              : t("Files:WantLeaveRoom")}
          </Text>
        </div>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="OkButton"
          label={isOwner ? t("Files:AssignOwner") : t("Common:OKButton")}
          size="normal"
          primary
          scale
          onClick={onLeaveRoom}
          isDisabled={isLoading}
        />
        <Button
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
          isDisabled={isLoading}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default inject(
  ({ auth, dialogsStore, filesStore, selectedFolderStore }) => {
    const {
      leaveRoomDialogVisible: visible,
      setLeaveRoomDialogVisible: setIsVisible,
      setChangeRoomOwnerIsVisible,
    } = dialogsStore;
    const { user } = auth.userStore;
    const {
      selection,
      bufferSelection,
      updateRoomMemberRole,
      removeFiles,
      setSelected,
      folders,
      setFolders,
    } = filesStore;

    const roomId = selection.length
      ? selection[0].id
      : bufferSelection
      ? bufferSelection.id
      : selectedFolderStore.id;

    const selections = selection.length ? selection : [bufferSelection];
    const folderItem = selections[0] ? selections[0] : selectedFolderStore;

    const isRoomOwner = folderItem?.createdBy?.id === user.id;
    const isRoot = selection.length || bufferSelection ? false : true;

    return {
      visible,
      setIsVisible,
      setChangeRoomOwnerIsVisible,
      isOwner: isRoomOwner,
      updateRoomMemberRole,
      roomId,
      userId: user.id,
      removeFiles,
      isAdmin: user.isOwner || user.isAdmin,
      setSelected,
      isRoot,
      folders,
      setFolders,
    };
  }
)(observer(withTranslation(["Common", "Files"])(LeaveRoomDialog)));
