import React, { useRef } from "react";
import TableHeader from "@docspace/components/table-container/TableHeader";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

const HistoryTableHeader = (props) => {
  const { sectionWidth, tableRef } = props;
  const { t } = useTranslation(["Webhooks"]);

  const columns = useRef([
    {
      key: "Event ID",
      title: t("EventID"),
      resizable: true,
      enable: true,
      default: true,
      active: true,
    },
    {
      key: "Status",
      title: t("Status"),
      enable: true,
      resizable: true,
    },
    {
      key: "Delivery",
      title: t("Delivery"),
      enable: true,
      resizable: true,
    },
  ]);
  return (
    <TableHeader
      columns={columns.current}
      checkboxSize="32px"
      containerRef={tableRef}
      sectionWidth={sectionWidth}
      showSettings={false}
      style={{ position: "absolute" }}
      useReactWindow
      setHideColumns={true}
      infoPanelVisible={true}
    />
  );
};

export default inject(({ settingsStore }) => {
  const { withPaging } = settingsStore;

  return {
    withPaging,
  };
})(observer(HistoryTableHeader));
