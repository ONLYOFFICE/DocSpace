import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { inject, observer } from "mobx-react";
import { smallTablet } from "@docspace/components/utils/device";

const StyledBody = styled.div`
  border-radius: 12px;
  border: ${(props) => props.theme.client.settings.payment.border};
  max-width: 320px;

  padding: 24px;
  box-sizing: border-box;
  background: ${(props) =>
    props.theme.client.settings.payment.backgroundBenefitsColor};

  p {
    margin-bottom: 24px;
  }
  .payment-benefits_text {
    margin-bottom: 20px;
  }
  .payment-benefits {
    margin-bottom: 14px;
    align-items: flex-start;
    display: grid;
    grid-template-columns: 24px 1fr;
    grid-gap: 10px;
    p {
      margin-bottom: 0;
    }
    svg {
      path {
        //fill: red !important;
      }
    }
  }
`;

const BenefitsContainer = ({ t, features, theme }) => {
  return (
    <StyledBody className="benefits-container">
      <Text
        fontSize={"16px"}
        fontWeight={"600"}
        className="payment-benefits_text"
        noSelect
      >
        {t("Benefits")}
      </Text>
      {features.map((item, index) => {
        if (!item.title || !item.image) return;
        return (
          <div className="payment-benefits" key={index}>
            <div dangerouslySetInnerHTML={{ __html: item.image }} />
            <Text noSelect>{item.title}</Text>
          </div>
        );
      })}
    </StyledBody>
  );
};

export default inject(({ auth }) => {
  const { paymentQuotasStore, settingsStore } = auth;
  const { theme } = settingsStore;
  const { portalPaymentQuotasFeatures } = paymentQuotasStore;

  return {
    theme,
    features: portalPaymentQuotasFeatures,
  };
})(observer(BenefitsContainer));
