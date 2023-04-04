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
          content: <RequestDetails />,
        },
      ]}
      startSelect={0}
    />
  );
};
