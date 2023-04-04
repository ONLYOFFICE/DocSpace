import React from "react";
import styled from "styled-components";
import Submenu from "@docspace/components/submenu";

import { RequestDetails } from "./RequestDetails";

const SubmenuWrapper = styled.div`
  .sticky {
    z-index: 3;
  }
`;

export const MessagesDetails = () => {
  return (
    <SubmenuWrapper>
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
    </SubmenuWrapper>
  );
};
