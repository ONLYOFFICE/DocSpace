import React from "react";
import styled from "styled-components";

import commonIconsStyles from "@docspace/components/utils/common-icons-style";

import SendClockIcon from "PUBLIC_DIR/images/send.clock.react.svg";
import CatalogSpamIcon from "PUBLIC_DIR/images/catalog.spam.react.svg";

const StyledSendClockIcon = styled(SendClockIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.peopleTableRow.fill};
  }
`;
const StyledCatalogSpamIcon = styled(CatalogSpamIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.peopleTableRow.fill};
  }
`;

const Badges = ({ statusType }) => {
  return (
    <>
      {statusType === "pending" && <StyledSendClockIcon size="small" />}
      {statusType === "disabled" && <StyledCatalogSpamIcon size="small" />}
    </>
  );
};

export default Badges;
