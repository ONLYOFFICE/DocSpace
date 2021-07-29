import React, { useRef } from "react";
import TableContainer from "@appserver/components/table-container";
import { inject, observer } from "mobx-react";
import TableRow from "./TableRow";
import TableHeader from "./TableHeader";
import TableBody from "@appserver/components/table-container/TableBody";

const Table = ({ peopleList }) => {
  const ref = useRef(null);

  return (
    <TableContainer forwardedRef={ref}>
      <TableHeader containerRef={ref} />
      <TableBody>
        {peopleList.map((item) => (
          <TableRow key={item.id} item={item} />
        ))}
      </TableBody>
    </TableContainer>
  );
};

export default inject(({ peopleStore }) => {
  const { peopleList } = peopleStore.usersStore;

  return {
    peopleList,
  };
})(observer(Table));
