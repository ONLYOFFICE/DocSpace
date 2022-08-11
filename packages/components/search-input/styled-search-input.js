import styled from "styled-components";
import Base from "../themes/base";

const StyledSearchInput = styled.div`
  font-family: Open Sans;
  font-style: normal;

  .search-input-block {
    max-height: 32px;

    & > input {
      font-size: ${(props) => props.theme.searchInput.fontSize};
      font-weight: ${(props) => props.theme.searchInput.fontWeight};
    }
  }

  svg {
    path {
      fill: ${(props) => props.theme.searchInput.iconColor};
    }
  }
  &:hover {
    svg {
      path {
        fill: ${(props) => props.theme.searchInput.hoverIconColor};
      }
    }
  }
`;

StyledSearchInput.defaultProps = { theme: Base };
export default StyledSearchInput;
