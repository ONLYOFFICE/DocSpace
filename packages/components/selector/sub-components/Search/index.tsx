import React from "react";

import SearchInput from "../../../search-input";

import { SearchProps } from "./Search.types";

const Search = React.memo(
  ({ placeholder, value, onSearch, onClearSearch }: SearchProps) => {
    return (
      <SearchInput
        className="search-input"
        placeholder={placeholder}
        value={value}
        onChange={onSearch}
        onClearSearch={onClearSearch}
      />
    );
  }
);

export default Search;
