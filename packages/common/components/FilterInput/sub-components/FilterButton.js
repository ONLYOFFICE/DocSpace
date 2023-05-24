import React from "react";
import FilterReactSvrUrl from "PUBLIC_DIR/images/filter.react.svg?url";

import IconButton from "@docspace/components/icon-button";
import { Base } from "@docspace/components/themes";

import FilterBlock from "./FilterBlock";

import StyledButton from "./StyledButton";

import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";
const FilterButton = ({
  t,
  onFilter,
  getFilterData,

  selectedFilterValue,

  filterHeader,
  selectorLabel,

  isPersonalRoom,
  isRooms,
  isAccounts,
  id,
  title,
}) => {
  const [showFilterBlock, setShowFilterBlock] = React.useState(false);

  const changeShowFilterBlock = React.useCallback(() => {
    setShowFilterBlock((value) => !value);
  }, [setShowFilterBlock]);

  return (
    <>
      <StyledButton id={id} onClick={changeShowFilterBlock} title={title}>
        <IconButton iconName={FilterReactSvrUrl} size={16} />
        {selectedFilterValue && selectedFilterValue.length > 0 && (
          <ColorTheme themeId={ThemeType.IndicatorFilterButton} />
        )}
      </StyledButton>

      {showFilterBlock && (
        <FilterBlock
          t={t}
          filterHeader={filterHeader}
          selectedFilterValue={selectedFilterValue}
          hideFilterBlock={changeShowFilterBlock}
          getFilterData={getFilterData}
          onFilter={onFilter}
          selectorLabel={selectorLabel}
          isPersonalRoom={isPersonalRoom}
          isRooms={isRooms}
          isAccounts={isAccounts}
        />
      )}
    </>
  );
};

export default React.memo(FilterButton);
