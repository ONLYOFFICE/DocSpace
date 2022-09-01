import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import RowContainer from "@docspace/components/row-container";
import { isMobile } from "react-device-detect";
import { AuditUserRow } from "./AuditUserRow";

const AuditRowContainer = ({
  viewAs,
  setViewAs,
  auditTrailUsers,
  theme,
  sectionWidth,
}) => {
  useEffect(() => {
    if (viewAs !== "table" && viewAs !== "row") return;

    if (sectionWidth < 1025 || isMobile) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  return (
    <RowContainer className="history-row-container" useReactWindow={false}>
      {auditTrailUsers.map((item) => (
        <AuditUserRow
          key={item.id}
          theme={theme}
          item={item}
          sectionWidth={sectionWidth}
        />
      ))}
    </RowContainer>
  );
};

export default inject(({ setup, auth }) => {
  const { viewAs, setViewAs, security } = setup;
  const { theme } = auth.settingsStore;

  return {
    viewAs,
    setViewAs,
    auditTrailUsers: security.auditTrail.users,
    theme,
  };
})(observer(AuditRowContainer));
