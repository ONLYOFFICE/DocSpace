import React from 'react';

import SearchInput from '@appserver/components/search-input';

import { StyledSelectorSearchInput } from './StyledSelector';

const Search = ({ placeholder, onSearch }) => {
  return (
    <StyledSelectorSearchInput>
      <SearchInput onChange={onSearch} placeholder={placeholder} />
    </StyledSelectorSearchInput>
  );
};

export default React.memo(Search);
