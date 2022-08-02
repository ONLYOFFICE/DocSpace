import React from "react";

import SearchInput from "@docspace/components/search-input";

const Search = ({
  isDisabled,
  searchPlaceHolderLabel,
  searchValue,
  onSearchChange,
  onSearchReset,
}) => {
  return (
    <div className="header-options">
      <SearchInput
        className="options_searcher"
        isDisabled={isDisabled}
        size="base"
        scale={true}
        isNeedFilter={false}
        placeholder={searchPlaceHolderLabel}
        value={searchValue}
        onChange={onSearchChange}
        onClearSearch={onSearchReset}
      />
    </div>
  );
};

export default React.memo(Search);
