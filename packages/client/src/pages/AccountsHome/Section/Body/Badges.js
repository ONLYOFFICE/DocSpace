import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";

import Badge from "@docspace/components/badge";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";

import SendClockIcon from "PUBLIC_DIR/images/send.clock.react.svg";
import CatalogSpamIcon from "PUBLIC_DIR/images/catalog.spam.react.svg";

const StyledBadgesContainer = styled.div`
  height: 100%;

  display: flex;

  align-items: center;
`;

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

//TODO: need backed isPaid
const Badges = ({ t, statusType, withoutPaid, isPaid = false }) => {
  return (
    <StyledBadgesContainer className="badges additional-badges">
      {!withoutPaid && isPaid && (
        <StyledPaidBadge
          className="paid-badge"
          label={t("Paid")}
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
    </StyledBadgesContainer>
  );
};

export default withTranslation(["Common"])(Badges);
