import React, { useEffect, useRef } from "react";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";

import TableContainer from "@docspace/components/table-container";
import TableBody from "@docspace/components/table-container/TableBody";

import EmptyScreen from "../EmptyScreen";

import TableRow from "./TableRow";
import TableHeader from "./TableHeader";

const Table = ({
  peopleList,
  sectionWidth,
  viewAs,
  setViewAs,
  theme,
  isAdmin,
  isOwner,
  changeType,
  userId,
}) => {
  const ref = useRef(null);

  useEffect(() => {
    if (!sectionWidth) return;
    if (sectionWidth < 1025 || isMobile) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  return peopleList.length > 0 ? (
    <TableContainer forwardedRef={ref}>
      <TableHeader sectionWidth={sectionWidth} containerRef={ref} />
      <TableBody>
        {peopleList.map((item) => (
          <TableRow
            theme={theme}
            key={item.id}
            item={item}
            isAdmin={isAdmin}
            isOwner={isOwner}
            changeUserType={changeType}
            userId={userId}
          />
        ))}
      </TableBody>
    </TableContainer>
  ) : (
    <EmptyScreen />
  );
};

export default inject(({ peopleStore, auth }) => {
  const { usersStore, viewAs, setViewAs, changeType } = peopleStore;
  const { theme } = auth.settingsStore;
  const { peopleList } = usersStore;

  const { isAdmin, isOwner, id: userId } = auth.userStore.user;

  return {
    peopleList,
    viewAs,
    setViewAs,
    theme,
    isAdmin,
    isOwner,
    changeType,
    userId,
  };
})(observer(Table));
