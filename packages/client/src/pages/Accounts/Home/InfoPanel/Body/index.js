import { inject, observer } from "mobx-react";
import React from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import SeveralItems from "./SeveralItems";
import SingleItem from "./SingleItem";

import { StyledInfoBody } from "./StyledBody";
import { Base } from "@docspace/components/themes";
import EmptyScreen from "./EmptyScreen";
import withLoader from "SRC_DIR/HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders";

const InfoPanelBodyContent = ({
  t,
  selection,
  isOwner,
  isAdmin,
  changeUserType,
  userId,
}) => {
  console.log(selection);
  return (
    <StyledInfoBody>
      {selection.length === 0 ? (
        <EmptyScreen t={t} />
      ) : selection.length === 1 ? (
        <SingleItem
          t={t}
          selection={selection}
          isOwner={isOwner}
          isAdmin={isAdmin}
          changeUserType={changeUserType}
          userId={userId}
        />
      ) : (
        <SeveralItems count={selection.length} />
      )}
    </StyledInfoBody>
  );
};

InfoPanelBodyContent.defaultProps = { theme: Base };

export default inject(({ auth, peopleStore }) => {
  const { selection, bufferSelection } = peopleStore.selectionStore;
  const { changeType: changeUserType } = peopleStore;

  const { isOwner, isAdmin, id: userId } = auth.userStore.user;

  const selectedUsers = selection.length
    ? [...selection]
    : bufferSelection
    ? [bufferSelection]
    : [];

  return { selection: selectedUsers, isOwner, isAdmin, changeUserType, userId };
})(
  withRouter(
    withTranslation([
      "InfoPanel",
      "ConnectDialog",
      "Common",
      "People",
      "PeopleTranslations",
      "Settings",
      "SmartBanner",
    ])(
      withLoader(observer(InfoPanelBodyContent))(
        <Loaders.InfoPanelBodyLoader />
      )
    )
  )
);
