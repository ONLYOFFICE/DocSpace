import styled, { css } from "styled-components";

import { isDesktop } from "react-device-detect";

import SearchInput from "@appserver/components/search-input";

const StyledFilterInput = styled.div`
  width: 100%;
  height: 32px;

  display: flex;
  align-items: center;
  justify-content: start;

  margin: 0;
  padding: 0;

  ${isDesktop &&
  css`
    margin-bottom: 6px;
  `}
`;

const StyledSearchInput = styled(SearchInput)`
  width: 100%;
`;

export { StyledFilterInput, StyledSearchInput };
