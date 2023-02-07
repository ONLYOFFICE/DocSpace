import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import RowContainer from "@docspace/components/row-container";
import { isMobile } from "react-device-detect";
import { HistoryUserRow } from "./HistoryUserRow";

const HistoryRowContainer = ({
  viewAs,
  setViewAs,
  historyUsers,
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
      {historyUsers.map((item) => (
        <HistoryUserRow
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
    historyUsers: security.loginHistory.users,
    theme,
  };
})(observer(HistoryRowContainer));
