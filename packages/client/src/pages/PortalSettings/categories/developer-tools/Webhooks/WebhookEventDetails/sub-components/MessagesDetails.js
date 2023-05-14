import React from "react";
import styled from "styled-components";
import Submenu from "@docspace/components/submenu";

import { RequestDetails } from "./RequestDetails";
import { ResponseDetails } from "./ResponseDetails";

const SubmenuWrapper = styled.div`
  .sticky {
    z-index: 3;
    top: 68px;
  }
`;

export const MessagesDetails = ({ webhookDetails }) => {
  const menuData = [
    {
      id: "webhookRequest",
      name: "Request",
      content: <RequestDetails webhookDetails={webhookDetails} />,
    },
  ];

  webhookDetails.status >= 200 &&
    webhookDetails.status < 500 &&
    menuData.push({
      id: "webhookResponse",
      name: "Response",
      content: <ResponseDetails webhookDetails={webhookDetails} />,
    });

  return (
    <SubmenuWrapper>
      <Submenu data={menuData} startSelect={0} />
    </SubmenuWrapper>
  );
};
