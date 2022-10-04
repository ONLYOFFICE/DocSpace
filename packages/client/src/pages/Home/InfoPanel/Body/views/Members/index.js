import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";

import withLoader from "@docspace/client/src/HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders";

import { StyledUserList, StyledUserTypeHeader } from "../../styles/members";

import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
import User from "./User";

const Members = ({
  t,
  selfId,
  isOwner,
  isAdmin,

  selection,

  selectionParentRoom,
  setSelectionParentRoom,

  getRoomMembers,
  changeUserType,
}) => {
  const [members, setMembers] = useState(null);
  const [showLoader, setShowLoader] = useState(false);

  const fetchMembers = async (roomId) => {
    let timerId;
    if (members) timerId = setTimeout(() => setShowLoader(true), 1500);
    let fetchedMembers = await getRoomMembers(roomId);
    fetchedMembers = fetchedMembers.filter(
      (m) => m.sharedTo.email || m.sharedTo.displayName
    );
    clearTimeout(timerId);
    setMembers(fetchedMembers);
    setShowLoader(false);
    return fetchedMembers;
  };

  useEffect(async () => {
    if (!selectionParentRoom) return;
    if (selectionParentRoom.members) {
      setMembers(selectionParentRoom.members);
      return;
    }
    const fetchedMembers = await fetchMembers(selection.id);
    setSelectionParentRoom({
      ...selectionParentRoom,
      members: fetchedMembers,
    });
  }, [selectionParentRoom]);

  useEffect(async () => {
    if (!selection.isRoom) return;
    if (selectionParentRoom && selectionParentRoom.id === selection.id) return;
    const fetchedMembers = await fetchMembers(selection.id);
    setSelectionParentRoom({
      ...selection,
      members: fetchedMembers,
    });
  }, [selection]);

  const onAddUsers = () => {
    toastr.warning("Work in progress");
  };

  if (showLoader) return <Loaders.InfoPanelViewLoader view="members" />;
  if (!members) return null;

  return (
    <>
      <StyledUserTypeHeader>
        <Text className="title">
          {t("UsersInRoom")} : {members.length}
        </Text>
        <IconButton
          className={"icon"}
          title={t("Common:AddUsers")}
          iconName="/static/images/person+.react.svg"
          isFill={true}
          onClick={onAddUsers}
          size={16}
        />
      </StyledUserTypeHeader>

      <StyledUserList>
        {Object.values(members).map((user) => (
          <User
            key={user.sharedTo.id}
            t={t}
            user={user.sharedTo}
            isOwner={isOwner}
            isAdmin={isAdmin}
            selfId={selfId}
            changeUserType={changeUserType}
          />
        ))}
      </StyledUserList>

      {/* <StyledUserTypeHeader>
        <Text className="title">{`${t("Expect people")}:`}</Text>
        <IconButton
          className={"icon"}
          title={t("Repeat invitation")}
          iconName="/static/images/e-mail+.react.svg"
          isFill={true}
          onClick={() => {}}
          size={16}
        />
      </StyledUserTypeHeader>

      <UserList t={t} users={data.members.expect} isExpect /> */}
    </>
  );
};

// export default withTranslation([
//   "InfoPanel",
//   "Common",
//   "Translations",
//   "People",
//   "PeopleTranslations",
//   "Settings",
// ])(withLoader(Members)(<Loaders.InfoPanelViewLoader view="members" />));

export default withTranslation([
  "InfoPanel",
  "Common",
  "Translations",
  "People",
  "PeopleTranslations",
  "Settings",
])(Members);
