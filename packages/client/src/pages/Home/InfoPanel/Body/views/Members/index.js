import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";

import Loaders from "@docspace/common/components/Loaders";

import { StyledUserList, StyledUserTypeHeader } from "../../styles/members";

import { ShareAccessRights } from "@docspace/common/constants";

import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
import User from "./User";
import MembersHelper from "../../helpers/MembersHelper";

const Members = ({
  t,
  selfId,
  isOwner,
  isAdmin,

  selection,

  selectionParentRoom,
  setSelectionParentRoom,

  getRoomMembers,
  updateRoomMemberRole,

  resendEmailInvitations,
  setInvitePanelOptions,

  changeUserType,
  canInviteUserInRoom,
}) => {
  const membersHelper = new MembersHelper({ t });

  const [members, setMembers] = useState(null);
  const [showLoader, setShowLoader] = useState(false);

  const isDisabledInvite = !canInviteUserInRoom({ access: selection.access });
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
        ...fetchedMember.sharedTo,
      };
      if (member.activationStatus !== 2) inRoomMembers.push(member);
      else expectedMembers.push(member);
    });

    setShowLoader(false);
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
  }, [selection]);

  const onClickInviteUsers = () => {
    const parentRoomId = selectionParentRoom.id;

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
        <IconButton
          className={"icon"}
          title={t("Common:AddUsers")}
          iconName="/static/images/person+.react.svg"
          isFill={true}
          onClick={onClickInviteUsers}
          size={16}
          isDisabled={isDisabledInvite}
        />
      </StyledUserTypeHeader>

      <StyledUserList>
        {Object.values(members.inRoom).map((user) => (
          <User
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
          />
        ))}
      </StyledUserList>

      {!!members.expected.length && (
        <StyledUserTypeHeader isExpect>
          <Text className="title">{t("ExpectPeople")}</Text>
          <IconButton
            className={"icon"}
            title={t("Repeat invitation")}
            iconName="/static/images/e-mail+.react.svg"
            isFill={true}
            onClick={onRepeatInvitation}
            size={16}
          />
        </StyledUserTypeHeader>
      )}

      <StyledUserList>
        {Object.values(members.expected).map((user) => (
          <User
            isExpect
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
          />
        ))}
      </StyledUserList>
    </>
  );
};

export default inject(
  ({ auth, filesStore, peopleStore, dialogsStore, accessRightsStore }) => {
    const { selectionParentRoom, setSelectionParentRoom } = auth.infoPanelStore;
    const {
      getRoomMembers,
      updateRoomMemberRole,
      resendEmailInvitations,
    } = filesStore;
    const { isOwner, isAdmin, id: selfId } = auth.userStore.user;
    const { setInvitePanelOptions } = dialogsStore;
    const { changeType: changeUserType } = peopleStore;
    const { canInviteUserInRoom } = accessRightsStore;

    return {
      selectionParentRoom,
      setSelectionParentRoom,

      getRoomMembers,
      updateRoomMemberRole,

      isOwner,
      isAdmin,
      selfId,

      setInvitePanelOptions,
      resendEmailInvitations,

      changeUserType,
      canInviteUserInRoom,
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
