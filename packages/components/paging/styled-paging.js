import styled from "styled-components";
import Base from "../themes/base";

const StyledPaging = styled.div`
  display: flex;
  flex-direction: row;
  justify-content: flex-start;

  & > button {
    margin-right: ${(props) => props.theme.paging.button.marginRight};
    max-width: ${(props) => props.theme.paging.button.maxWidth};
  }
`;
StyledPaging.defaultProps = { theme: Base };

const StyledOnPage = styled.div`
  margin-left: ${(props) => props.theme.paging.comboBox.marginLeft};
  margin-right: ${(props) => props.theme.paging.comboBox.marginRight};

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
  margin-right: ${(props) => props.theme.paging.page.marginRight};

  .manualWidth {
    .dropdown-container {
      width: ${(props) => props.theme.paging.page.width};
    }
  }
`;
StyledPage.defaultProps = { theme: Base };

export { StyledPage, StyledOnPage, StyledPaging };
