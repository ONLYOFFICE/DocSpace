import React from "react";

import Submenu from "@docspace/components/submenu";

import { RequestDetails } from "./RequestDetails";

export const MessagesDetails = () => {
  return (
    <Submenu
      data={[
        {
          id: "webhookRequest",
          name: "Request",
          content: <RequestDetails />,
        },
        {
          id: "webhookResponse",
          name: "Response",
          content: <div>hi2</div>,
        },
      ]}
      startSelect={0}
    />
  );
};
