import React from "react";

import { Consumer } from "@docspace/components/utils/context";

import { inject, observer } from "mobx-react";
import HistoryTableView from "./HistoryTableView";
import HistoryRowView from "./HistoryRowView";

const WebhookHistoryTable = (props) => {
  const { viewAs } = props;

  return (
    <Consumer>
      {(context) =>
        viewAs === "table" ? (
          <HistoryTableView sectionWidth={context.sectionWidth} />
        ) : (
          <HistoryRowView sectionWidth={context.sectionWidth} />
        )
      }
    </Consumer>
  );
};

export default inject(({ setup }) => {
  const { viewAs } = setup;

  return {
    viewAs,
  };
})(observer(WebhookHistoryTable));
