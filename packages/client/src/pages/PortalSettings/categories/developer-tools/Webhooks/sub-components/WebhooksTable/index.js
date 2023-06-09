import React from "react";

import { inject, observer } from "mobx-react";
import { Consumer } from "@docspace/components/utils/context";

import WebhooksTableView from "./WebhooksTableView";
import WebhooksRowView from "./WebhooksRowView";

const WebhooksTable = (props) => {
  const { viewAs, openSettingsModal, openDeleteModal } = props;

  return (
    <Consumer>
      {(context) =>
        viewAs === "table" ? (
          <WebhooksTableView
            sectionWidth={context.sectionWidth}
            openSettingsModal={openSettingsModal}
            openDeleteModal={openDeleteModal}
          />
        ) : (
          <WebhooksRowView
            sectionWidth={context.sectionWidth}
            openSettingsModal={openSettingsModal}
            openDeleteModal={openDeleteModal}
          />
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
})(observer(WebhooksTable));
