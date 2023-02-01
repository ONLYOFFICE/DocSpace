import Text from "@docspace/components/text";
import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import SelectTotalSizeContainer from "./SelectTotalSizeContainer";

const StyledCurrentUsersContainer = styled.div`
  height: fit-content;
`;

const CurrentUsersCountContainer = (props) => {
  const { isNeedPlusSign, maxCountManagersByQuota, t } = props;
  return (
    <StyledCurrentUsersContainer>
      <Text fontSize="16px" fontWeight={600} textAlign="center">
        {t("NumberOfAdmins")}
      </Text>
      <Text fontSize="44px" fontWeight={700} textAlign="center" noSelect>
        {maxCountManagersByQuota}
      </Text>
      <SelectTotalSizeContainer isNeedPlusSign={isNeedPlusSign} />
    </StyledCurrentUsersContainer>
  );
};

export default inject(({ auth, payments }) => {
  const { paymentQuotasStore, currentQuotaStore } = auth;
  const { maxCountManagersByQuota } = currentQuotaStore;

  return {
    maxCountManagersByQuota,
  };
})(observer(CurrentUsersCountContainer));
