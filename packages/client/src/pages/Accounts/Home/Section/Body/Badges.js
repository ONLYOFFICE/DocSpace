import React from "react";
import styled from "styled-components";

import Badge from "@docspace/components/badge";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";

import SendClockIcon from "PUBLIC_DIR/images/send.clock.react.svg";
import CatalogSpamIcon from "PUBLIC_DIR/images/catalog.spam.react.svg";

const StyledPaidBadge = styled(Badge)`
  margin-right: 8px;
`;

const StyledSendClockIcon = styled(SendClockIcon)`
  ${commonIconsStyles}
  path {
    fill: #a3a9ae;
  }
`;
const StyledCatalogSpamIcon = styled(CatalogSpamIcon)`
  ${commonIconsStyles}
  path {
    fill: #f21c0e;
  }
`;

const Badges = ({ statusType, isPaid = true }) => {
  return (
    <>
      {isPaid && (
        <StyledPaidBadge
          label={"Paid"}
          color={"#FFFFFF"}
          backgroundColor={"#EDC409"}
          fontSize={"9px"}
          fontWeight={800}
          lineHeight={"13px"}
          noHover
        />
      )}
      {statusType === "pending" && <StyledSendClockIcon size="small" />}
      {statusType === "disabled" && <StyledCatalogSpamIcon size="small" />}
    </>
  );
};

export default Badges;
