import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { PortalFeaturesLimitations } from "@docspace/common/constants";
import { getConvertedSize } from "@docspace/common/utils";
import { mobile } from "@docspace/components/utils/device";

const StyledCurrentTariffContainer = styled.div`
  display: flex;
  min-height: 40px;
  background: ${(props) => props.theme.client.settings.payment.backgroundColor};
  margin-bottom: 24px;
  flex-wrap: wrap;
  margin-top: 14px;
  padding: 12px 16px;
  box-sizing: border-box;
  padding-bottom: 0;

  @media ${mobile} {
    flex-direction: column;
    margin-bottom: 27px;
  }

  div {
    padding-bottom: 8px;
    margin-right: 24px;
  }

  p {
    margin-bottom: 0;
    color: ${(props) => props.theme.client.settings.payment.tariffText};
    .current-tariff_count {
      color: ${(props) => props.theme.client.settings.payment.tariffText};
      margin-left: 4px;
    }
  }
`;

const CurrentTariffContainer = ({ style, quotaCharacteristics }) => {
  const { t } = useTranslation(["Payments", "Common"]);

  return (
    <StyledCurrentTariffContainer style={style}>
      {quotaCharacteristics.map((item, index) => {
        const maxValue = item.value;
        const usedValue = item.used.value;

        if (maxValue === PortalFeaturesLimitations.Unavailable) return;

        const isExistsMaxValue =
          maxValue !== PortalFeaturesLimitations.Limitless;

        const resultingMaxValue =
          item.type === "size" && isExistsMaxValue
            ? getConvertedSize(t, maxValue)
            : isExistsMaxValue
            ? maxValue
            : null;

        const resultingUsedValue =
          item.type === "size" ? getConvertedSize(t, usedValue) : usedValue;

        return (
          <div key={index}>
            <Text isBold noSelect fontSize={"14px"}>
              {item.used.title}
              <Text
                className="current-tariff_count"
                as="span"
                isBold
                fontSize={"14px"}
              >
                {resultingUsedValue}
                {resultingMaxValue ? `/${resultingMaxValue}` : ""}
              </Text>
            </Text>
          </div>
        );
      })}
    </StyledCurrentTariffContainer>
  );
};

export default inject(({ auth }) => {
  const { currentQuotaStore } = auth;
  const { quotaCharacteristics } = currentQuotaStore;

  return {
    quotaCharacteristics,
  };
})(observer(CurrentTariffContainer));
