import React from "react";

import SearchInput from "../../../search-input";

const Search = React.memo(({ placeholder, value, onSearch, onClearSearch }) => {
  return (
    <SearchInput
      className="search-input"
      placeholder={placeholder}
      value={value}
      onChange={onSearch}
      onClearSearch={onClearSearch}
    />
  );
});

export default Search;
