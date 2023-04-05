import React, { useState } from "react";

import { useHistory } from "react-router-dom";

import Row from "@docspace/components/row";
import { HistoryRowContent } from "./HistoryRowContent";

import RetryIcon from "PUBLIC_DIR/images/refresh.react.svg?url";
import InfoIcon from "PUBLIC_DIR/images/info.outline.react.svg?url";

export const HistoryRow = ({ historyItem, sectionWidth }) => {
  const history = useHistory();

  const contextOptions = [
    {
      key: "Webhook details dropdownItem",
      label: "Webhook details",
      icon: InfoIcon,
    },
    {
      key: "Retry dropdownItem",
      label: "Retry",
      icon: RetryIcon,
    },
  ];

  return (
    <>
      <Row sectionWidth={sectionWidth} key={historyItem.id} contextOptions={contextOptions}>
        <HistoryRowContent sectionWidth={sectionWidth} historyItem={historyItem} />
      </Row>
    </>
  );
};
