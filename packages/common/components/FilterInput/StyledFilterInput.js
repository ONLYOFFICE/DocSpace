import styled, { css } from "styled-components";

import { isDesktop } from "react-device-detect";

import SearchInput from "@docspace/components/search-input";

const StyledFilterInput = styled.div`
  width: 100%;

  display: flex;

  flex-direction: column;

  margin: 0;
  padding: 0;

  .filter-input_filter-row {
    width: 100%;
    height: 32px;

    display: flex;
    align-items: center;
    justify-content: start;

    margin-bottom: 8px;
  }

  .filter-input_selected-row {
    width: 100%;
    min-height: 32px;

    display: flex;
    flex-direction: row;
    align-items: center;
    flex-wrap: wrap;

    margin-bottom: 8px;
  }
`;

const StyledSearchInput = styled(SearchInput)`
  width: 100%;
`;

export { StyledFilterInput, StyledSearchInput };
