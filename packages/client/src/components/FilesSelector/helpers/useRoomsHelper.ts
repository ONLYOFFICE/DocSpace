import React from "react";

// @ts-ignore
import { getRooms } from "@docspace/common/api/rooms";
// @ts-ignore
import RoomsFilter from "@docspace/common/api/rooms/filter";
// @ts-ignore
import { RoomsType } from "@docspace/common/constants";
// @ts-ignore
import { iconSize32 } from "@docspace/common/utils/image-helpers";

import { PAGE_COUNT, defaultBreadCrumb } from "../utils";

import { BreadCrumb, Item, useRoomsHelperProps } from "../FilesSelector.types";

const getRoomLogo = (roomType: number) => {
  let path = "";
  switch (roomType) {
    case RoomsType.CustomRoom:
      path = "custom.svg";
      break;

    case RoomsType.EditingRoom:
      path = "editing.svg";
      break;
  }

  return iconSize32.get(path);
};

const convertRoomsToItems = (rooms: any) => {
  const items = rooms.map((room: any) => {
    const {
      id,
      title,
      roomType,
      logo,
      filesCount,
      foldersCount,
      security,
      parentId,
      rootFolderType,
    } = room;

    const icon = logo.medium ? logo.medium : getRoomLogo(roomType);

    return {
      id,
      label: title,
      title,
      icon,
      filesCount,
      foldersCount,
      security,
      parentId,
      rootFolderType,
      isFolder: true,
    };
  });

  return items;
};

const useRoomsHelper = ({
  setIsNextPageLoading,
  setHasNextPage,
  setTotal,
  setItems,
  setBreadCrumbs,
  setIsRoot,
  isFirstLoad,
  setIsBreadCrumbsLoading,
  searchValue,
}: useRoomsHelperProps) => {
  const getRoomList = React.useCallback(
    async (startIndex: number, isInit?: boolean, search?: string | null) => {
      setIsNextPageLoading(true);

      const filterValue = search
        ? search
        : search === null
        ? ""
        : searchValue || "";

      const page = startIndex / PAGE_COUNT;

      const filter = RoomsFilter.getDefault();

      filter.page = page;
      filter.pageCount = PAGE_COUNT;

      filter.filterValue = filterValue;

      const rooms = await getRooms(filter);

      const { folders, total, count, current } = rooms;

      const { title, id } = current;

      if (isInit) {
        const breadCrumbs: BreadCrumb[] = [
          { ...defaultBreadCrumb },
          { label: title, id, isRoom: true },
        ];

        setBreadCrumbs(breadCrumbs);

        setIsBreadCrumbsLoading(false);
      }

      const itemList: Item[] = convertRoomsToItems(folders);

      setHasNextPage(count === PAGE_COUNT);

      if (isFirstLoad || startIndex === 0) {
        setTotal(total);
        setItems(itemList);
      } else {
        setItems((prevState: Item[] | null) => {
          if (prevState) return [...prevState, ...itemList];
          return [...itemList];
        });
      }

      setIsNextPageLoading(false);
      setIsRoot(false);
    },
    [isFirstLoad, searchValue]
  );
  return { getRoomList };
};

export default useRoomsHelper;
