import styled from "styled-components";

const StyledBenefitsBody = styled.div`
  margin: 20px 0;
  border-radius: 12px;
  border: ${(props) => props.theme.client.settings.payment.border};
  max-width: 660px;

  padding: 23px;

  background: ${(props) =>
    props.theme.client.settings.payment.backgroundBenefitsColor};

  .benefits-title {
    margin-bottom: 20px;
  }
  .payments-benefits {
    display: grid;
    grid-template-columns: 24px 1fr;
    gap: 10px;
    .benefits-svg {
      display: flex;
      align-items: flex-start;
      padding-top: 4px;
    }
    .benefits-description {
      p:first-child {
        margin-bottom: 2px;
      }
    }
  }
  .payments-benefits ~ .payments-benefits {
    margin-top: 12px;
  }
`;

const StyledComponent = styled.div`
  max-width: 660px;
  .official-documentation {
    margin: 12px 0;
  }
  .upgrade-info {
    margin-bottom: 24px;
  }
`;

const StyledContactComponent = styled.div`
  margin-top: 16px;
  max-width: 504px;
  .payments_contact {
    display: flex;
    width: 100%;
    p {
      margin-right: 4px;
    }
    a {
      text-decoration: underline;
    }
  }
`;
export { StyledBenefitsBody, StyledComponent, StyledContactComponent };
