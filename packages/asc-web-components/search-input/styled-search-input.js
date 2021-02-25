import styled from "styled-components";
import Base from "../themes/base";

const StyledSearchInput = styled.div`
  font-family: Open Sans;
  font-style: normal;

  .search-input-block {
    & > input {
      font-size: ${(props) => props.theme.searchInput.fontSize};
      font-weight: ${(props) => props.theme.searchInput.fontWeight};
    }
  }
`;

StyledSearchInput.defaultProps = { theme: Base };
export default StyledSearchInput;
