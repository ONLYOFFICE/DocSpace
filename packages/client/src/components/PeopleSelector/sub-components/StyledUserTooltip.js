import styled from "styled-components";

const StyledUserTooltip = styled.div`
  width: 233px;
  min-height: 63px;
  display: grid;
  grid-template-columns: 33px 1fr;
  grid-template-rows: 1fr;
  grid-column-gap: 12px;

  .block-info {
    display: grid;
    grid-template-rows: 1fr;

    .email-text {
      padding-bottom: 8px;
    }
  }
`;

export default StyledUserTooltip;
