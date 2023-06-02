import EmptyScreenCorporateSvgUrl from "PUBLIC_DIR/images/empty_screen_corporate.svg?url";
import React from "react";
import { withTranslation } from "react-i18next";

import api from "@docspace/common/api";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import { RoomsType } from "@docspace/common/constants";
import { iconSize32 } from "@docspace/common/utils/image-helpers";

import Loaders from "@docspace/common/components/Loaders";

import Selector from "@docspace/components/selector";

const pageCount = 100;

const getRoomLogo = (roomType) => {
  let path = "";
  switch (roomType) {
    case RoomsType.CustomRoom:
      path = "custom.svg";
      break;
    case RoomsType.FillingFormsRoom:
      path = "filling.form.svg";
      break;
    case RoomsType.EditingRoom:
      path = "editing.svg";
      break;
    case RoomsType.ReadOnlyRoom:
      path = "view.only.svg";
      break;
    case RoomsType.ReviewRoom:
      path = "review.svg";
      break;
  }

  return iconSize32.get(path);
};

const convertToItems = (folders) => {
  const items = folders.map((folder) => {
    const { id, title, roomType, logo } = folder;

    const icon = logo.medium ? logo.medium : getRoomLogo(roomType);

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
      emptyScreenImage={emptyScreenImage || EmptyScreenCorporateSvgUrl}
      emptyScreenHeader={emptyScreenHeader || t("EmptyRoomsHeader")}
      emptyScreenDescription={
        emptyScreenDescription || t("EmptyRoomsDescription")
      }
      searchEmptyScreenImage={
        searchEmptyScreenImage || EmptyScreenCorporateSvgUrl
      }
      searchEmptyScreenHeader={
        searchEmptyScreenHeader || t("Common:NotFoundTitle")
      }
      searchEmptyScreenDescription={
        searchEmptyScreenDescription || t("Common:SearchEmptyRoomsDescription")
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
