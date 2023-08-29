import React from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import { inject, observer } from "mobx-react";
import { getConvertedSize } from "@docspace/common/utils";
const StyledBody = styled.div`
  .select-total-size_title {
    margin-bottom: 8px;
    margin-left: auto;
    margin-right: auto;

    color: ${(props) =>
      props.isDisabled
        ? props.theme.client.settings.payment.priceContainer.disableColor
        : props.theme.client.settings.payment.priceContainer.featureTextColor};
  }
`;

const SelectTotalSizeContainer = ({
  allowedStorageSizeByQuota,
  usedTotalStorageSizeTitle,
  theme,
  isNeedPlusSign,
}) => {
  const { t } = useTranslation(["Payments", "Common"]);

  const convertedSize = getConvertedSize(t, allowedStorageSizeByQuota);

  return (
    <StyledBody theme={theme}>
      <Text
        textAlign={"center"}
        noSelect
        fontWeight={600}
        fontSize="11px"
        className="select-total-size_title"
        color={theme.client.settings.payment.storageSizeTitle}
      >
        {usedTotalStorageSizeTitle}: {convertedSize} {isNeedPlusSign ? "+" : ""}
      </Text>
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { paymentQuotasStore } = auth;
  const { usedTotalStorageSizeTitle } = paymentQuotasStore;
  const { theme } = auth.settingsStore;
  const { allowedStorageSizeByQuota } = payments;

  return {
    theme,
    usedTotalStorageSizeTitle,
    allowedStorageSizeByQuota,
  };
})(observer(SelectTotalSizeContainer));
