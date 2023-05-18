import React from "react";
import styled from "styled-components";
import Submenu from "@docspace/components/submenu";

import { RequestDetails } from "./RequestDetails";
import { ResponseDetails } from "./ResponseDetails";
import { useTranslation } from "react-i18next";

const SubmenuWrapper = styled.div`
  .sticky {
    z-index: 3;
    top: 68px;
  }
`;

export const MessagesDetails = ({ webhookDetails }) => {
  const { t } = useTranslation(["Webhooks"]);
  const menuData = [
    {
      id: "webhookRequest",
      name: t("Request"),
      content: <RequestDetails webhookDetails={webhookDetails} />,
    },
  ];

  webhookDetails.status >= 200 &&
    webhookDetails.status < 500 &&
    menuData.push({
      id: "webhookResponse",
      name: t("Response"),
      content: <ResponseDetails webhookDetails={webhookDetails} />,
    });

  return (
    <SubmenuWrapper>
      <Submenu data={menuData} startSelect={0} />
    </SubmenuWrapper>
  );
};
