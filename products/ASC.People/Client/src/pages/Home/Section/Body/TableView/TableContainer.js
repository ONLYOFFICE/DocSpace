import React, { useEffect, useRef } from "react";
import TableContainer from "@appserver/components/table-container";
import { inject, observer } from "mobx-react";
import TableRow from "./TableRow";
import TableHeader from "./TableHeader";
import TableBody from "@appserver/components/table-container/TableBody";
import EmptyScreen from "../EmptyScreen";
import { isMobile } from "react-device-detect";

const Table = ({ peopleList, sectionWidth, viewAs, setViewAs }) => {
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
          <TableRow key={item.id} item={item} />
        ))}
      </TableBody>
    </TableContainer>
  ) : (
    <EmptyScreen />
  );
};

export default inject(({ peopleStore }) => {
  const { usersStore, viewAs, setViewAs } = peopleStore;
  const { peopleList } = usersStore;

  return {
    peopleList,
    viewAs,
    setViewAs,
  };
})(observer(Table));
