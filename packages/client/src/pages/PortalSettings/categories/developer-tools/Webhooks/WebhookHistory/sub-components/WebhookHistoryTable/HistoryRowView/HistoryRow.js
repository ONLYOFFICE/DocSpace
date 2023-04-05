import React from "react";

import { useHistory } from "react-router-dom";

import Row from "@docspace/components/row";
import { HistoryRowContent } from "./HistoryRowContent";

import RetryIcon from "PUBLIC_DIR/images/refresh.react.svg?url";
import InfoIcon from "PUBLIC_DIR/images/info.outline.react.svg?url";

export const HistoryRow = ({ historyItem, sectionWidth }) => {
  const history = useHistory();

  const redirectToDetails = () =>
    history.push(window.location.pathname + `/event/${historyItem.id}`);

  const contextOptions = [
    {
      key: "Webhook details dropdownItem",
      label: "Webhook details",
      icon: InfoIcon,
      onClick: redirectToDetails,
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
