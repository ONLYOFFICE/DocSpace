import React from "react";
import CreateAvatarEditorPage from "./createAvatarEditorPage";
import AvatarEditorPage from "./avatarEditorPage";
import CreateUserForm from "./createUserForm";
import UpdateUserForm from "./updateUserForm";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";

import withPeopleLoader from "../../../../HOCs/withPeopleLoader";
import Loaders from "@docspace/common/components/Loaders";

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
    withPeopleLoader(observer(SectionUserBody))(
      <Loaders.ProfileView isEdit={false} />
    )
  )
);
