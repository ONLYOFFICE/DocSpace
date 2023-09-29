import React, { useState, useEffect, useCallback } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";

import {
  EmployeeActivationStatus,
  RoomsType,
  ShareAccessRights,
} from "@docspace/common/constants";
import Loaders from "@docspace/common/components/Loaders";
import MembersHelper from "../../helpers/MembersHelper";
import PublicRoomBlock from "./sub-components/PublicRoomBlock";
import MembersList from "./MembersList";

const Members = ({
  t,
  selfId,
  selection,

  updateRoomMembers,
  setUpdateRoomMembers,

  selectionParentRoom,
  setSelectionParentRoom,

  setIsScrollLocked,

  getRoomMembers,
  getRoomLinks,
  updateRoomMemberRole,
  setView,
  roomsView,
  resendEmailInvitations,
  changeUserType,
  isPublicRoomType,

  setExternalLinks,
  membersFilter,
  externalLinks,
  members,
  setMembersList,
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const membersHelper = new MembersHelper({ t });

  const security = selectionParentRoom ? selectionParentRoom.security : {};

  const fetchMembers = async (roomId, clearFilter = true) => {
    if (isLoading) return;
    const isPublic = selection?.roomType ?? selectionParentRoom?.roomType;
    const requests = [getRoomMembers(roomId, clearFilter)];

    if (isPublic && clearFilter) {
      requests.push(getRoomLinks(roomId));
    }

    let timerId;
    if (clearFilter) timerId = setTimeout(() => setIsLoading(true), 300);

    const [data, links] = await Promise.all(requests);
    clearFilter && setIsLoading(false);
    clearTimeout(timerId);

    links && setExternalLinks(links);

    const users = [];
    const administrators = [];
    const expectedMembers = [];
    data.map((fetchedMember) => {
      const member = {
        access: fetchedMember.access,
        canEditAccess: fetchedMember.canEditAccess,
        ...fetchedMember.sharedTo,
      };

      if (member.activationStatus === EmployeeActivationStatus.Pending) {
        member.isExpect = true;
        expectedMembers.push(member);
      } else if (
        member.access === ShareAccessRights.FullAccess ||
        member.access === ShareAccessRights.RoomManager
      ) {
        administrators.push(member);
      } else {
        users.push(member);
      }
    });

    let hasPrevAdminsTitle =
      members?.roomId === roomId
        ? getHasPrevTitle(members?.administrators, "administration")
        : false;

    if (administrators.length && !hasPrevAdminsTitle) {
      administrators.unshift({
        id: "administration",
        displayName: t("Administration"),
        isTitle: true,
      });
    }

    let hasPrevUsersTitle =
      members?.roomId === roomId
        ? getHasPrevTitle(members?.users, "user")
        : false;

    if (users.length && !hasPrevUsersTitle) {
      users.unshift({ id: "user", displayName: t("Users"), isTitle: true });
    }

    let hasPrevExpectedTitle =
      members?.roomId === roomId
        ? getHasPrevTitle(members?.expected, "expected")
        : false;

    if (expectedMembers.length && !hasPrevExpectedTitle) {
      expectedMembers.unshift({
        id: "expected",
        displayName: t("ExpectUsers"),
        isTitle: true,
        isExpect: true,
      });
    }

    setUpdateRoomMembers(false);

    return {
      users,
      administrators,
      expected: expectedMembers,
      roomId,
    };
  };

  const getHasPrevTitle = (array, type) => {
    return array.findIndex((x) => x.id === type) > -1;
  };

  const updateSelectionParentRoomActionSelection = useCallback(async () => {
    if (!selection.isRoom || selection.id === members?.roomId) return;

    const fetchedMembers = await fetchMembers(selection.id);
    setMembersList(fetchedMembers);

    setSelectionParentRoom({
      ...selection,
      members: fetchedMembers,
    });
    if (roomsView === "info_members" && !selection?.security?.Read)
      setView("info_details");
  }, [selection]);

  useEffect(() => {
    updateSelectionParentRoomActionSelection();
  }, [selection, updateSelectionParentRoomActionSelection]);

  const updateMembersAction = useCallback(async () => {
    if (!updateRoomMembers) return;

    const fetchedMembers = await fetchMembers(selection.id);

    setSelectionParentRoom({
      ...selectionParentRoom,
      members: fetchedMembers,
    });

    setMembersList(fetchedMembers);
  }, [selectionParentRoom, selection?.id, updateRoomMembers]);

  useEffect(() => {
    updateMembersAction();
  }, [
    selectionParentRoom,
    selection?.id,
    updateRoomMembers,
    updateMembersAction,
  ]);

  const onRepeatInvitation = async () => {
    resendEmailInvitations(selectionParentRoom.id, true)
      .then(() =>
        toastr.success(t("PeopleTranslations:SuccessSentMultipleInvitatios"))
      )
      .catch((err) => toastr.error(err));
  };

  const loadNextPage = async () => {
    const roomId = selectionParentRoom.id;
    const fetchedMembers = await fetchMembers(roomId, false);
    const { users, administrators, expected } = fetchedMembers;

    const newMembers = {
      administrators: [...members.administrators, ...administrators],
      users: [...members.users, ...users],
      expected: [...members.expected, ...expected],
    };

    setMembersList(newMembers);
  };

  if (isLoading) return <Loaders.InfoPanelViewLoader view="members" />;
  else if (!members) return <></>;

  const [currentMember] = members.administrators.filter(
    (member) => member.id === selfId
  );

  const { administrators, users, expected } = members;
  const membersList = [...administrators, ...users, ...expected];

  const adminsTitleCount = administrators.length ? 1 : 0;
  const usersTitleCount = users.length ? 1 : 0;
  const expectedTitleCount = expected.length ? 1 : 0;

  const headersCount = adminsTitleCount + usersTitleCount + expectedTitleCount;

  return (
    <>
      {isPublicRoomType && <PublicRoomBlock t={t} />}
      <MembersList
        loadNextPage={loadNextPage}
        t={t}
        security={security}
        members={membersList}
        membersHelper={membersHelper}
        currentMember={currentMember}
        updateRoomMemberRole={updateRoomMemberRole}
        selectionParentRoom={selectionParentRoom}
        setSelectionParentRoom={setSelectionParentRoom}
        changeUserType={changeUserType}
        setIsScrollLocked={setIsScrollLocked}
        hasNextPage={membersList.length - headersCount < membersFilter.total}
        itemCount={membersFilter.total + headersCount}
        onRepeatInvitation={onRepeatInvitation}
        isPublicRoomType={isPublicRoomType}
        withBanner={isPublicRoomType && externalLinks.length > 0}
        setMembers={setMembersList}
      />
    </>
  );
};

export default inject(
  ({ auth, filesStore, peopleStore, selectedFolderStore, publicRoomStore }) => {
    const {
      selectionParentRoom,
      setSelectionParentRoom,
      setView,
      roomsView,

      updateRoomMembers,
      setUpdateRoomMembers,

      setIsScrollLocked,
      membersList,
      setMembersList,
    } = auth.infoPanelStore;
    const {
      getRoomMembers,
      getRoomLinks,
      updateRoomMemberRole,
      resendEmailInvitations,
      membersFilter,
    } = filesStore;
    const { id: selfId } = auth.userStore.user;

    const { changeType: changeUserType } = peopleStore;
    const { roomLinks, setExternalLinks } = publicRoomStore;

    const roomType =
      selectedFolderStore.roomType ?? selectionParentRoom?.roomType;

    const isPublicRoomType =
      roomType === RoomsType.PublicRoom || roomType === RoomsType.CustomRoom;

    return {
      setView,
      roomsView,
      selectionParentRoom,
      setSelectionParentRoom,

      setIsScrollLocked,

      getRoomMembers,
      getRoomLinks,
      updateRoomMemberRole,

      updateRoomMembers,
      setUpdateRoomMembers,

      selfId,

      resendEmailInvitations,
      changeUserType,
      isPublicRoomType,
      setExternalLinks,
      membersFilter,
      externalLinks: roomLinks,
      members: membersList,
      setMembersList,
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
    "CreateEditRoomDialog",
  ])(observer(Members))
);
