import styled from "styled-components";
import Base from "../themes/base";

const StyledPaging = styled.div`
  display: flex;
  flex-direction: row;
  justify-content: flex-start;

  & > button {
    ${({ theme }) =>
      theme.interfaceDirection === "rtl"
        ? `margin-left: ${theme.paging.button.marginRight};`
        : `margin-right: ${theme.paging.button.marginRight};`}

    max-width: ${(props) => props.theme.paging.button.maxWidth};
  }
`;
StyledPaging.defaultProps = { theme: Base };

const StyledOnPage = styled.div`
  ${({ theme }) =>
    theme.interfaceDirection === "rtl"
      ? `
        margin-right: ${theme.paging.comboBox.marginLeft};
        margin-left: ${theme.paging.comboBox.marginRight};
      `
      : `
        margin-left: ${theme.paging.comboBox.marginLeft};
        margin-right: ${theme.paging.comboBox.marginRight};
      `}

  .hideDisabled {
    div[disabled] {
      display: none;
    }
  }

  @media (max-width: 450px) {
    display: none;
  }
`;
StyledOnPage.defaultProps = { theme: Base };

const StyledPage = styled.div`
  ${({ theme }) =>
    theme.interfaceDirection === "rtl"
      ? `margin-left: ${theme.paging.page.marginRight};`
      : `margin-right: ${theme.paging.page.marginRight};`}

  .manualWidth {
    .dropdown-container {
      width: ${(props) => props.theme.paging.page.width};
    }
  }
`;
StyledPage.defaultProps = { theme: Base };

export { StyledPage, StyledOnPage, StyledPaging };
