import React from "react";
import styled from "styled-components";
import Submenu from "@docspace/components/submenu";

import RequestDetails from "./RequestDetails";
import ResponseDetails from "./ResponseDetails";
import { useTranslation } from "react-i18next";
import { isMobileOnly } from "react-device-detect";
import { inject, observer } from "mobx-react";

const SubmenuWrapper = styled.div`
  .sticky {
    z-index: 3;

    top: ${() => (isMobileOnly ? "68px" : "0px")};
  }
`;

const MessagesDetails = ({ eventDetails }) => {
  const { t } = useTranslation(["Webhooks"]);
  const menuData = [
    {
      id: "webhookRequest",
      name: t("Request"),
      content: <RequestDetails />,
    },
  ];

  if (eventDetails.status >= 200 && eventDetails.status < 500) {
    menuData.push({
      id: "webhookResponse",
      name: t("Response"),
      content: <ResponseDetails />,
    });
  }

  return (
    <SubmenuWrapper>
      <Submenu data={menuData} startSelect={0} />
    </SubmenuWrapper>
  );
};

export default inject(({ webhooksStore }) => {
  const { eventDetails } = webhooksStore;

  return { eventDetails };
})(observer(MessagesDetails));
