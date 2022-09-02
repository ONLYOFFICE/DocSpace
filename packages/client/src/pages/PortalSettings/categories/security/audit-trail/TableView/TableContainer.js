import React, { useEffect, useRef } from "react";
import TableContainer from "@docspace/components/table-container";
import { inject, observer } from "mobx-react";
import TableRow from "./TableRow";
import TableHeader from "./TableHeader";
import TableBody from "@docspace/components/table-container/TableBody";
import { isMobile } from "react-device-detect";

const Table = ({ auditTrailUsers, sectionWidth, viewAs, setViewAs, theme }) => {
  const ref = useRef(null);
  useEffect(() => {
    if (!sectionWidth) return;
    if (sectionWidth < 1025 || isMobile) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  return auditTrailUsers && auditTrailUsers.length > 0 ? (
    <TableContainer forwardedRef={ref}>
      <TableHeader sectionWidth={sectionWidth} containerRef={ref} />
      <TableBody>
        {auditTrailUsers.map((item) => (
          <TableRow theme={theme} key={item.id} item={item} />
        ))}
      </TableBody>
    </TableContainer>
  ) : (
    <div></div>
  );
};

export default inject(({ auth, setup }) => {
  const { security, viewAs, setViewAs } = setup;
  const { theme } = auth.settingsStore;

  return {
    auditTrailUsers: security.auditTrail.users,
    theme,
    viewAs,
    setViewAs,
  };
})(observer(Table));
