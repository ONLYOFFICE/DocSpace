import React from "react";
import styled from "styled-components";
import Submenu from "@docspace/components/submenu";

import { RequestDetails } from "./RequestDetails";
import { ResponseDetails } from "./ResponseDetails";

const SubmenuWrapper = styled.div`
  .sticky {
    z-index: 3;
  }
`;

export const MessagesDetails = ({ webhookDetails }) => {
  return (
    <SubmenuWrapper>
      <Submenu
        data={[
          {
            id: "webhookRequest",
            name: "Request",
            content: <RequestDetails webhookDetails={webhookDetails} />,
          },
          {
            id: "webhookResponse",
            name: "Response",
            content: <ResponseDetails webhookDetails={webhookDetails} />,
          },
        ]}
        startSelect={0}
      />
    </SubmenuWrapper>
  );
};
