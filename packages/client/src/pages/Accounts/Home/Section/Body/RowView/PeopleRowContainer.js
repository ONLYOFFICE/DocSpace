import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";

import RowContainer from "@docspace/components/row-container";

import EmptyScreen from "../EmptyScreen";

import SimpleUserRow from "./SimpleUserRow";

const PeopleRowContainer = ({
  peopleList,
  sectionWidth,
  viewAs,
  setViewAs,
  theme,
  infoPanelVisible,
}) => {
  useEffect(() => {
    if ((viewAs !== "table" && viewAs !== "row") || !sectionWidth) return;
    // 400 - it is desktop info panel width
    if (
      (sectionWidth < 1025 && !infoPanelVisible) ||
      ((sectionWidth < 625 || (viewAs === "row" && sectionWidth < 1025)) &&
        infoPanelVisible) ||
      isMobile
    ) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  return peopleList.length > 0 ? (
    <RowContainer className="people-row-container" useReactWindow={false}>
      {peopleList.map((item) => (
        <SimpleUserRow
          theme={theme}
          key={item.id}
          item={item}
          sectionWidth={sectionWidth}
        />
      ))}
    </RowContainer>
  ) : (
    <EmptyScreen />
  );
};

export default inject(({ peopleStore, auth }) => {
  const { usersStore, viewAs, setViewAs } = peopleStore;
  const { theme } = auth.settingsStore;
  const { peopleList } = usersStore;

  const { isVisible: infoPanelVisible } = auth.infoPanelStore;

  return {
    peopleList,
    viewAs,
    setViewAs,
    theme,
    infoPanelVisible,
  };
})(observer(PeopleRowContainer));
