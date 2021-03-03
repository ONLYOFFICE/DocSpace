import React from "react";
import CreateAvatarEditorPage from "./createAvatarEditorPage";
import AvatarEditorPage from "./avatarEditorPage";
import CreateUserForm from "./createUserForm";
import UpdateUserForm from "./updateUserForm";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";

const SectionUserBody = ({ avatarEditorIsOpen, match }) => {
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
    <UpdateUserForm />
  );
};

export default inject(({ peopleStore }) => ({
  avatarEditorIsOpen: peopleStore.avatarEditorStore.visible,
}))(withRouter(observer(SectionUserBody)));
