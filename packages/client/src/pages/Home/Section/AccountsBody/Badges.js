import React from "react";
import { inject, observer } from "mobx-react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";

import { PaymentsType, AccountLoginType } from "@docspace/common/constants";

import Badge from "@docspace/components/badge";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";

import SendClockIcon from "PUBLIC_DIR/images/send.clock.react.svg";
import CatalogSpamIcon from "PUBLIC_DIR/images/catalog.spam.react.svg";

import { SSO_LABEL } from "SRC_DIR/helpers/constants";

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

const Badges = ({
  t,
  statusType,
  withoutPaid,
  isPaid = false,
  filter,
  getUsersList,
  isSSO = false,
}) => {
  const onClickPaid = () => {
    if (filter.payments === PaymentsType.Paid) return;
    const newFilter = filter.clone();
    newFilter.payments = PaymentsType.Paid;
    getUsersList(newFilter, true);
  };

  const onClickSSO = () => {
    if (filter.accountLoginType === AccountLoginType.SSO) return;
    const newFilter = filter.clone();
    newFilter.accountLoginType = AccountLoginType.SSO;
    getUsersList(newFilter, true);
  };

  return (
    <StyledBadgesContainer className="badges additional-badges">
      {isSSO && (
        <Badge
          label={SSO_LABEL}
          color={"#FFFFFF"}
          backgroundColor="#22C386"
          fontSize={"9px"}
          fontWeight={800}
          noHover
          lineHeight={"13px"}
          onClick={onClickSSO}
        />
      )}
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
          onClick={onClickPaid}
        />
      )}
      {statusType === "pending" && (
        <StyledSendClockIcon className="pending-badge" size="small" />
      )}
      {statusType === "disabled" && (
        <StyledCatalogSpamIcon className="disabled-badge" size="small" />
      )}
    </StyledBadgesContainer>
  );
};

export default inject(({ peopleStore }) => {
  const { filterStore, usersStore } = peopleStore;

  const { filter } = filterStore;

  const { getUsersList } = usersStore;
  return { filter, getUsersList };
})(withTranslation(["Common"])(observer(Badges)));
