import React from "react";
import CreateAvatarEditorPage from "./createAvatarEditorPage";
import AvatarEditorPage from "./avatarEditorPage";
import CreateUserForm from "./createUserForm";
import UpdateUserForm from "./updateUserForm";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";

const SectionUserBody = ({ avatarEditorIsOpen, match, isMy }) => {
  const { type } = match.params;
  return type ? (
    avatarEditorIsOpen ? (
      <CreateAvatarEditorPage />
    ) : (
      <CreateUserForm />
    )
  ) : avatarEditorIsOpen ? (
    <AvatarEditorPage />
  ) : (
    <UpdateUserForm isMy={isMy} />
  );
};

export default withRouter(
  inject(({ peopleStore }) => ({
    avatarEditorIsOpen: peopleStore.avatarEditorStore.visible,
  }))(observer(SectionUserBody))
);
