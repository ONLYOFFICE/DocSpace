import React from "react";
import styled, { css } from "styled-components";
import { useTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import Slider from "@docspace/components/slider";
import PlusIcon from "../../../../../../public/images/plus.react.svg";
import MinusIcon from "../../../../../../public/images/minus.react.svg";
import { smallTablet } from "@docspace/components/utils/device";
import TextInput from "@docspace/components/text-input";
import { inject, observer } from "mobx-react";
import { getConvertedSize } from "@docspace/common/utils";
const StyledBody = styled.div`
  .select-total-size_title {
    margin-bottom: 12px;
    margin-left: auto;
    margin-right: auto;
    width: max-content;
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
        noSelect
        fontWeight={600}
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
