import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import RowContainer from "@docspace/components/row-container";
import SimpleUserRow from "./SimpleUserRow";
import EmptyScreen from "../EmptyScreen";
import { isMobile } from "react-device-detect";

const PeopleRowContainer = ({
  peopleList,
  sectionWidth,
  viewAs,
  setViewAs,
  theme,
}) => {
  useEffect(() => {
    if (viewAs !== "table" && viewAs !== "row") return;

    if (sectionWidth < 1025 || isMobile) {
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

  return {
    peopleList,
    viewAs,
    setViewAs,
    theme,
  };
})(observer(PeopleRowContainer));
