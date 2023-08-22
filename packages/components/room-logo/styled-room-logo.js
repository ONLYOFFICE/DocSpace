import styled, { css } from "styled-components";

const StyledContainer = styled.div`
  width: 32px;
  height: 32px;

  min-width: 32px;
  min-height: 32px;

  display: flex;
  align-items: center;
  justify-content: center;

  ${({ theme }) =>
    theme.interfaceDirection === "rtl"
      ? `margin-left: 12px;`
      : `margin-right: 12px;`}

  .room-logo_checkbox {
    display: none;

    .checkbox {
      ${({ theme }) =>
        theme.interfaceDirection === "rtl"
          ? `margin-left: 0;`
          : `margin-right: 0;`}
    }
  }
`;

const StyledLogoContainer = styled.div`
  width: 32px;
  height: 32px;

  .room-logo_icon {
    border-radius: 6px;
  }
`;

export { StyledContainer, StyledLogoContainer };
