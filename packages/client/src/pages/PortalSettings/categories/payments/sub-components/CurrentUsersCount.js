import Text from "@docspace/components/text";
import React from "react";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";
import SelectTotalSizeContainer from "./SelectTotalSizeContainer";

const StyledCurrentUsersContainer = styled.div`
  height: fit-content;
  .current-admins-number {
    ${(props) =>
      props.isDisabled &&
      css`
        color: ${props.theme.client.settings.payment.priceContainer
          .disableColor};
      `}
  }
`;

const CurrentUsersCountContainer = (props) => {
  const {
    isNeedPlusSign,
    maxCountManagersByQuota,
    t,
    isDisabled,
    theme,
  } = props;
  return (
    <StyledCurrentUsersContainer isDisabled={isDisabled} theme={theme}>
      <Text
        fontSize="16px"
        fontWeight={600}
        textAlign="center"
        className="current-admins-number"
      >
        {t("NumberOfAdmins")}
      </Text>
      <Text
        fontSize="44px"
        fontWeight={700}
        textAlign="center"
        noSelect
        className="current-admins-number"
      >
        {maxCountManagersByQuota}
      </Text>
      <SelectTotalSizeContainer isNeedPlusSign={isNeedPlusSign} />
    </StyledCurrentUsersContainer>
  );
};

export default inject(({ auth }) => {
  const { settingsStore, currentQuotaStore } = auth;
  const { maxCountManagersByQuota } = currentQuotaStore;
  const { theme } = settingsStore;
  return {
    theme,
    maxCountManagersByQuota,
  };
})(observer(CurrentUsersCountContainer));
