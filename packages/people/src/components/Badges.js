import React from "react";
import styled from "styled-components";

import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import SendClockIcon from "../../public/images/send.clock.react.svg";
import CatalogSpamIcon from "../../public/images/catalog.spam.react.svg";

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
