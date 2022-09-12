import React from "react";
import { withTranslation } from "react-i18next";

import api from "@docspace/common/api";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import { RoomsType } from "@docspace/common/constants";

import Loaders from "@docspace/common/components/Loaders";

import Selector from "@docspace/components/selector";
import { Backdrop } from "@docspace/components";

const pageCount = 100;

const getRoomLogo = (roomType) => {
  const path = `images/icons/32`;
  switch (roomType) {
    case RoomsType.CustomRoom:
      return `${path}/room/custom.svg`;
    case RoomsType.FillingFormsRoom:
      return `${path}/room/filling.form.svg`;
    case RoomsType.EditingRoom:
      return `${path}/room/editing.svg`;
    case RoomsType.ReadOnlyRoom:
      return `${path}/room/view.only.svg`;
    case RoomsType.ReviewRoom:
      return `${path}/room/review.svg`;
  }
};

const convertToItems = (folders) => {
  const items = folders.map((folder) => {
    const { id, title, roomType, logo } = folder;

    const icon = logo.original ? logo.original : getRoomLogo(roomType);

    return { id, label: title, icon };
  });

  return items;
};

const RoomSelector = ({
  t,
  id,
  className,
  style,

  excludeItems,

  headerLabel,
  onBackClick,

  searchPlaceholder,
  onSearch,
  onClearSearch,

  onSelect,
  isMultiSelect,
  selectedItems,
  acceptButtonLabel,
  onAccept,

  withSelectAll,
  selectAllLabel,
  selectAllIcon,
  onSelectAll,

  withAccessRights,
  accessRights,
  selectedAccessRight,
  onAccessRightsChange,

  withCancelButton,
  cancelButtonLabel,
  onCancel,

  emptyScreenImage,
  emptyScreenHeader,
  emptyScreenDescription,
  searchEmptyScreenImage,
  searchEmptyScreenHeader,
  searchEmptyScreenDescription,
}) => {
  const [isFirstLoad, setIsFirstLoad] = React.useState(true);
  const [searchValue, setSearchValue] = React.useState("");
  const [hasNextPage, setHasNextPage] = React.useState(false);
  const [isNextPageLoading, setIsNextPageLoading] = React.useState(false);

  const [total, setTotal] = React.useState(0);

  const [items, setItems] = React.useState([]);

  const timeoutRef = React.useRef(null);

  const onSearchAction = React.useCallback(
    (value) => {
      onSearch && onSearch(value);
      setSearchValue(() => {
        setIsFirstLoad(true);

        return value;
      });
    },
    [onSearch]
  );

  const onClearSearchAction = React.useCallback(() => {
    onClearSearch && onClearSearch();
    setSearchValue(() => {
      setIsFirstLoad(true);

      return "";
    });
  }, [onClearSearch]);

  const onLoadNextPage = React.useCallback(
    (startIndex) => {
      setIsNextPageLoading(true);

      const page = startIndex / pageCount;

      const filter = RoomsFilter.getDefault();

      filter.page = page;
      filter.pageCount = pageCount;

      filter.filterValue = searchValue ? searchValue : null;

      api.rooms
        .getRooms(filter)
        .then(({ folders, total, count }) => {
          const rooms = convertToItems(folders);

          const itemList = rooms.filter((x) => !excludeItems.includes(x.id));

          setHasNextPage(count === pageCount);

          if (isFirstLoad) {
            setTotal(total);
            setItems(itemList);
          } else {
            setItems((value) => [...value, ...itemList]);
          }
        })
        .finally(() => {
          if (isFirstLoad) {
            setTimeout(() => {
              setIsFirstLoad(false);
            }, 500);
          }

          setIsNextPageLoading(false);
        });
    },
    [isFirstLoad, excludeItems, searchValue]
  );

  React.useEffect(() => {
    onLoadNextPage(0);
  }, [searchValue]);

  return (
    <Selector
      id={id}
      className={className}
      style={style}
      headerLabel={headerLabel || t("RoomList")}
      onBackClick={onBackClick}
      searchPlaceholder={searchPlaceholder || t("Common:Search")}
      searchValue={searchValue}
      onSearch={onSearchAction}
      onClearSearch={onClearSearchAction}
      onSelect={onSelect}
      items={items}
      acceptButtonLabel={acceptButtonLabel || t("Common:SelectAction")}
      onAccept={onAccept}
      withCancelButton={withCancelButton}
      cancelButtonLabel={cancelButtonLabel || t("Common:CancelButton")}
      onCancel={onCancel}
      isMultiSelect={isMultiSelect}
      selectedItems={selectedItems}
      withSelectAll={withSelectAll}
      selectAllLabel={selectAllLabel}
      selectAllIcon={selectAllIcon}
      onSelectAll={onSelectAll}
      withAccessRights={withAccessRights}
      accessRights={accessRights}
      selectedAccessRight={selectedAccessRight}
      onAccessRightsChange={onAccessRightsChange}
      emptyScreenImage={emptyScreenImage || "images/empty_screen_corporate.png"}
      emptyScreenHeader={emptyScreenHeader || t("EmptyRoomsHeader")}
      emptyScreenDescription={
        emptyScreenDescription || t("EmptyRoomsDescription")
      }
      searchEmptyScreenImage={
        searchEmptyScreenImage || "images/empty_screen_corporate.png"
      }
      searchEmptyScreenHeader={
        searchEmptyScreenHeader || t("SearchEmptyRoomsHeader")
      }
      searchEmptyScreenDescription={
        searchEmptyScreenDescription || t("SearchEmptyRoomsDescription")
      }
      totalItems={total}
      hasNextPage={hasNextPage}
      isNextPageLoading={isNextPageLoading}
      loadNextPage={onLoadNextPage}
      isLoading={isFirstLoad}
      searchLoader={<Loaders.SelectorSearchLoader />}
      rowLoader={
        <Loaders.SelectorRowLoader
          isMultiSelect={isMultiSelect}
          isContainer={isFirstLoad}
          isUser={false}
        />
      }
    />
  );
};

RoomSelector.defaultProps = { excludeItems: [] };

export default withTranslation(["RoomSelector", "Common"])(RoomSelector);
