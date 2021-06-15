import React from "react";
import CreateAvatarEditorPage from "./createAvatarEditorPage";
import AvatarEditorPage from "./avatarEditorPage";
import CreateUserForm from "./createUserForm";
import UpdateUserForm from "./updateUserForm";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";

import withLoader from "../../../../HOCs/withLoader";
import Loaders from "@appserver/common/components/Loaders";

const SectionUserBody = ({ avatarEditorIsOpen, match, isMy, loaded }) => {
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
  }))(
    withLoader(observer(SectionUserBody))(
      <Loaders.ProfileView isEdit={false} />
    )
  )
);
