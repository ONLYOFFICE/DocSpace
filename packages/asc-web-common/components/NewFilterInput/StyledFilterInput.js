import styled, { css } from "styled-components";

import SearchInput from "@appserver/components/search-input";

const StyledFilterInput = styled.div`
  width: 100%;
  max-width: ${(props) => props.sectionWidth}px;
  height: 32px;

  display: flex;
  align-items: center;
  justify-content: start;

  margin: 0;
  padding: 0;

  .sort-combo-box {
    width: 32px;
    height: 32px;

    margin-left: 8px;

    .dropdown-container {
      min-width: 200px;
      margin-top: 3px;
    }

    .optionalBlock {
      display: flex;
      align-items: center;
      justify-content: center;

      margin-right: 0;
    }

    .combo-buttons_arrow-icon {
      display: none;
    }

    .backdrop-active {
      display: none;
    }
  }
`;

const StyledSearchInput = styled(SearchInput)`
  width: 100%;
`;

export { StyledFilterInput, StyledSearchInput };
