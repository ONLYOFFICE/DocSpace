import React, { useRef } from "react";
import TableHeader from "@docspace/components/table-container/TableHeader";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

const HistoryTableHeader = (props) => {
  const { sectionWidth, tableRef } = props;
  const { t } = useTranslation(["Webhooks", "People"]);

  const columns = useRef([
    {
      key: "Event ID",
      title: t("EventID"),
      resizable: true,
      enable: true,
      default: true,
      active: true,
      minWidth: 150,
    },
    {
      key: "Status",
      title: t("People:UserStatus"),
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
      setHideColumns={() => {}}
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
