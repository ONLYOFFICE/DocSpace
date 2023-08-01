import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import { FolderType } from "@docspace/common/constants";
import Loaders from "@docspace/common/components/Loaders";

import PersonPlusReactSvgUrl from "PUBLIC_DIR/images/person+.react.svg?url";
import EmailPlusReactSvgUrl from "PUBLIC_DIR/images/e-mail+.react.svg?url";

import { StyledUserList, StyledUserTypeHeader } from "../../styles/members";

import { ShareAccessRights } from "@docspace/common/constants";

import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
import User from "./User";
import MembersHelper from "../../helpers/MembersHelper";

const Members = ({
  t,
  selfId,
  selection,

  setIsMobileHidden,
  updateRoomMembers,
  setUpdateRoomMembers,

  selectionParentRoom,
  setSelectionParentRoom,

  setIsScrollLocked,

  getRoomMembers,
  updateRoomMemberRole,
  setView,
  roomsView,
  resendEmailInvitations,
  setInvitePanelOptions,
  setInviteUsersWarningDialogVisible,
  changeUserType,
  isGracePeriod,
}) => {
  const membersHelper = new MembersHelper({ t });

  const [members, setMembers] = useState(null);
  const [showLoader, setShowLoader] = useState(false);

  const security = selectionParentRoom ? selectionParentRoom.security : {};

  const canInviteUserInRoomAbility = security?.EditAccess;

  const fetchMembers = async (roomId) => {
    let timerId;
    if (members) timerId = setTimeout(() => setShowLoader(true), 1000);
    let data = await getRoomMembers(roomId);

    data = data.filter((m) => m.sharedTo.email || m.sharedTo.displayName);
    clearTimeout(timerId);

    let inRoomMembers = [];
    let expectedMembers = [];
    data.map((fetchedMember) => {
      const member = {
        access: fetchedMember.access,
        canEditAccess: fetchedMember.canEditAccess,
        ...fetchedMember.sharedTo,
      };
      if (member.activationStatus !== 2) inRoomMembers.push(member);
      else expectedMembers.push(member);
    });

    setShowLoader(false);
    setUpdateRoomMembers(false);
    return {
      inRoom: inRoomMembers,
      expected: expectedMembers,
    };
  };

  useEffect(async () => {
    if (!selectionParentRoom) return;

    if (selectionParentRoom.members) {
      setMembers(selectionParentRoom.members);
      return;
    }

    const fetchedMembers = await fetchMembers(selectionParentRoom.id);
    setSelectionParentRoom({
      ...selectionParentRoom,
      members: fetchedMembers,
    });
  }, [selectionParentRoom]);

  useEffect(async () => {
    if (!selection.isRoom) return;

    const fetchedMembers = await fetchMembers(selection.id);
    setSelectionParentRoom({
      ...selection,
      members: fetchedMembers,
    });
    if (roomsView === "info_members" && !selection?.security?.Read)
      setView("info_details");
  }, [selection]);

  useEffect(async () => {
    if (!updateRoomMembers) return;

    const fetchedMembers = await fetchMembers(selection.id);

    setSelectionParentRoom({
      ...selectionParentRoom,
      members: fetchedMembers,
    });
    setMembers(fetchedMembers);
  }, [selectionParentRoom, selection?.id, updateRoomMembers]);

  const onClickInviteUsers = () => {
    setIsMobileHidden(true);
    const parentRoomId = selectionParentRoom.id;

    if (isGracePeriod) {
      setInviteUsersWarningDialogVisible(true);
      return;
    }

    setInvitePanelOptions({
      visible: true,
      roomId: parentRoomId,
      hideSelector: false,
      defaultAccess: ShareAccessRights.ReadOnly,
    });
  };

  const onRepeatInvitation = async () => {
    const userIds = members.expected.map((user) => user.id);
    resendEmailInvitations(selectionParentRoom.id, userIds)
      .then(() =>
        toastr.success(t("PeopleTranslations:SuccessSentMultipleInvitatios"))
      )
      .catch((err) => toastr.error(err));
  };

  if (showLoader) return <Loaders.InfoPanelViewLoader view="members" />;
  if (!selectionParentRoom || !members) return null;

  const [currentMember] = members.inRoom.filter(
    (member) => member.id === selfId
  );

  return (
    <>
      <StyledUserTypeHeader>
        <Text className="title">
          {t("UsersInRoom")} : {members.inRoom.length}
        </Text>
        {canInviteUserInRoomAbility && (
          <IconButton
            id="info_add-user"
            className={"icon"}
            title={t("Common:AddUsers")}
            iconName={PersonPlusReactSvgUrl}
            isFill={true}
            onClick={onClickInviteUsers}
            size={16}
          />
        )}
      </StyledUserTypeHeader>

      <StyledUserList>
        {Object.values(members.inRoom).map((user) => (
          <User
            security={security}
            key={user.id}
            t={t}
            user={user}
            membersHelper={membersHelper}
            currentMember={currentMember}
            updateRoomMemberRole={updateRoomMemberRole}
            roomId={selectionParentRoom.id}
            roomType={selectionParentRoom.roomType}
            selectionParentRoom={selectionParentRoom}
            setSelectionParentRoom={setSelectionParentRoom}
            changeUserType={changeUserType}
            setIsScrollLocked={setIsScrollLocked}
          />
        ))}
      </StyledUserList>

      {!!members.expected.length && (
        <StyledUserTypeHeader isExpect>
          <Text className="title">{t("PendingInvitations")}</Text>
          {canInviteUserInRoomAbility && (
            <IconButton
              className={"icon"}
              title={t("Common:RepeatInvitation")}
              iconName={EmailPlusReactSvgUrl}
              isFill={true}
              onClick={onRepeatInvitation}
              size={16}
            />
          )}
        </StyledUserTypeHeader>
      )}

      <StyledUserList>
        {Object.values(members.expected).map((user, i) => (
          <User
            security={security}
            isExpect
            key={i}
            t={t}
            user={user}
            membersHelper={membersHelper}
            currentMember={currentMember}
            updateRoomMemberRole={updateRoomMemberRole}
            roomId={selectionParentRoom.id}
            roomType={selectionParentRoom.roomType}
            selectionParentRoom={selectionParentRoom}
            setSelectionParentRoom={setSelectionParentRoom}
            changeUserType={changeUserType}
            setIsScrollLocked={setIsScrollLocked}
          />
        ))}
      </StyledUserList>
    </>
  );
};

export default inject(
  ({
    auth,
    filesStore,
    peopleStore,
    dialogStore,
    dialogsStore,
    accessRightsStore,
  }) => {
    const {
      setIsMobileHidden,
      selectionParentRoom,

      setSelectionParentRoom,
      setView,
      roomsView,

      updateRoomMembers,
      setUpdateRoomMembers,

      setIsScrollLocked,
    } = auth.infoPanelStore;
    const {
      getRoomMembers,
      updateRoomMemberRole,
      resendEmailInvitations,
    } = filesStore;
    const { id: selfId } = auth.userStore.user;
    const { isGracePeriod } = auth.currentTariffStatusStore;
    const {
      setInvitePanelOptions,
      setInviteUsersWarningDialogVisible,
    } = dialogsStore;

    const { changeType: changeUserType } = peopleStore;

    return {
      setView,
      roomsView,
      setIsMobileHidden,
      selectionParentRoom,
      setSelectionParentRoom,

      setIsScrollLocked,

      getRoomMembers,
      updateRoomMemberRole,

      updateRoomMembers,
      setUpdateRoomMembers,

      selfId,

      setInvitePanelOptions,
      setInviteUsersWarningDialogVisible,
      resendEmailInvitations,
      changeUserType,
      isGracePeriod,
    };
  }
)(
  withTranslation([
    "InfoPanel",
    "Common",
    "Translations",
    "People",
    "PeopleTranslations",
    "Settings",
  ])(observer(Members))
);
