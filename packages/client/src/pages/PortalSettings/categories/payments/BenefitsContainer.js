import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { inject, observer } from "mobx-react";
import { smallTablet } from "@docspace/components/utils/device";

const StyledBody = styled.div`
  border-radius: 12px;
  border: 1px solid #f8f9f9;
  max-width: 320px;

  @media ${smallTablet} {
    max-width: 600px;
  }

  padding: 24px;
  box-sizing: border-box;
  background: #f8f9f9;

  p {
    margin-bottom: 24px;
  }
  .payment-benefits_text {
    margin-bottom: 20px;
  }
  .payment-benefits {
    display: flex;
    margin-bottom: 16px;
    align-items: baseline;
    p {
      margin-left: 8px;
    }
  }
`;

const BenefitsContainer = ({ t, availableTariffs }) => {
  return (
    <StyledBody>
      <Text
        fontSize={"16px"}
        fontWeight={"600"}
        className="payment-benefits_text"
        noSelect
      >
        {t("Benefits")}
      </Text>
      {availableTariffs.map((item, index) => {
        return (
          <div className="payment-benefits" key={index}>
            <Text noSelect>{"*" + item}</Text>
          </div>
        );
      })}
    </StyledBody>
  );
};

export default inject(({ payments }) => {
  const { tariffsInfo } = payments;

  const availableTariffs = ["first", "second", "third", "forth", "five"];
  return { tariffsInfo, availableTariffs };
})(observer(BenefitsContainer));
