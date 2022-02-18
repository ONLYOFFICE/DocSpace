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
`;

const StyledSearchInput = styled(SearchInput)`
  width: 100%;
`;

export { StyledFilterInput, StyledSearchInput };
