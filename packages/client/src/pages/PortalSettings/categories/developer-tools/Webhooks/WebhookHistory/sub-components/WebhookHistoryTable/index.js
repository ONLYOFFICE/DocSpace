import React from "react";

import { Consumer } from "@docspace/components/utils/context";

import { inject, observer } from "mobx-react";
import HistoryTableView from "./HistoryTableView";

const WebhookHistoryTable = (props) => {
  const { viewAs, historyWebhooks } = props;

  return (
    <Consumer>
      {(context) =>
        viewAs === "table" ? (
          <HistoryTableView sectionWidth={context.sectionWidth} historyWebhooks={historyWebhooks} />
        ) : (
          <></>
        )
      }
    </Consumer>
  );
};

export default inject(({ filesStore }) => {
  const { viewAs } = filesStore;

  return {
    viewAs,
  };
})(observer(WebhookHistoryTable));
