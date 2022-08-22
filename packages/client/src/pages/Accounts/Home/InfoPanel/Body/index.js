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

const InfoPanelBodyContent = ({ t, selection }) => {
  return (
    <StyledInfoBody>
      {selection.length === 0 ? (
        <EmptyScreen t={t} />
      ) : selection.length === 1 ? (
        <SingleItem t={t} selection={selection} />
      ) : null}
    </StyledInfoBody>
  );
};

InfoPanelBodyContent.defaultProps = { theme: Base };

export default inject(({ auth, peopleStore }) => {
  const { selection } = peopleStore.selectionStore;

  return { selection };
})(
  withRouter(
    withTranslation([
      "InfoPanel",
      "ConnectDialog",
      "Common",
      "People",
      "PeopleTranslations",
      "Settings",
    ])(
      withLoader(observer(InfoPanelBodyContent))(
        <Loaders.InfoPanelBodyLoader />
      )
    )
  )
);
