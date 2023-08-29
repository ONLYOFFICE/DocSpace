import React from "react";

import { convertFilesToItems, convertFoldersToItems } from "./useFilesHelper";

import {
  Item,
  setItemsCallback,
  useSocketHelperProps,
} from "../FilesSelector.types";
import { convertRoomsToItems } from "./useRoomsHelper";

const useSocketHelper = ({
  socketHelper,
  socketSubscribersId,
  setItems,
  setBreadCrumbs,
  setTotal,
  disabledItems,
  filterParam,
}: useSocketHelperProps) => {
  const subscribedId = React.useRef<null | number>(null);

  const subscribe = (id: number) => {
    const roomParts = `DIR-${id}`;

    if (socketSubscribersId.has(roomParts)) return (subscribedId.current = id);

    if (subscribedId.current && !socketSubscribersId.has(roomParts)) {
      unsubscribe(subscribedId.current, false);
    }

    socketHelper.emit({
      command: "subscribe",
      data: {
        roomParts: `DIR-${id}`,
        individual: true,
      },
    });

    subscribedId.current = id;
  };

  const unsubscribe = (id: number, clear = true) => {
    if (clear) {
      subscribedId.current = null;
    }

    if (id && !socketSubscribersId.has(`DIR-${id}`)) {
      socketHelper.emit({
        command: "unsubscribe",
        data: {
          roomParts: `DIR-${id}`,
          individual: true,
        },
      });
    }
  };

  const addItem = React.useCallback((opt: any) => {
    if (!opt?.data) return;

    const data = JSON.parse(opt.data);

    if (
      data.folderId
        ? data.folderId !== subscribedId.current
        : data.parentId !== subscribedId.current
    )
      return;

    let item: null | Item = null;

    if (opt?.type === "file") {
      item = convertFilesToItems([data], filterParam)[0];
    } else if (opt?.type === "folder") {
      item = !!data.roomType
        ? convertRoomsToItems([data])[0]
        : convertFoldersToItems([data], disabledItems, filterParam)[0];
    }

    const callback: setItemsCallback = (value: Item[] | null) => {
      if (!item || !value) return value;

      if (opt.type === "folder") {
        setTotal((value) => value + 1);

        return [item, ...value];
      }

      if (opt.type === "file") {
        let idx = 0;

        for (let i = 0; i < value.length - 1; i++) {
          if (!value[i].isFolder) break;

          idx = i + 1;
        }

        const newValue = [...value];

        newValue.splice(idx, 0, item);

        setTotal((value) => value + 1);

        return newValue;
      }

      return value;
    };

    setItems(callback);
  }, []);

  const updateItem = React.useCallback((opt: any) => {
    if (!opt?.data) return;

    const data = JSON.parse(opt.data);

    if (
      ((data.folderId && data.folderId !== subscribedId.current) ||
        (data.parentId && data.parentId !== subscribedId.current)) &&
      data.id !== subscribedId.current
    )
      return;

    let item: null | Item = null;

    if (opt?.type === "file") {
      item = convertFilesToItems([data], filterParam)[0];
    } else if (opt?.type === "folder") {
      item = !!data.roomType
        ? convertRoomsToItems([data])[0]
        : convertFoldersToItems([data], disabledItems, filterParam)[0];
    }

    if (item?.id === subscribedId.current) {
      return setBreadCrumbs((value) => {
        if (!value) return value;

        const newValue = [...value];

        if (newValue[newValue.length - 1].id === item?.id) {
          newValue[newValue.length - 1].label = item.label;
        }

        return newValue;
      });
    }

    const callback: setItemsCallback = (value: Item[] | null) => {
      if (!item || !value) return value;

      if (opt.type === "folder") {
        const idx = value.findIndex((v) => v.id === item?.id && v.isFolder);

        if (idx > -1) {
          const newValue = [...value];

          newValue.splice(idx, 1, item);

          return newValue;
        }

        setBreadCrumbs((breadCrumbsValue) => {
          return breadCrumbsValue;
        });
      }

      if (opt.type === "file") {
        const idx = value.findIndex((v) => v.id === item?.id && !v.isFolder);

        if (idx > -1) {
          const newValue = [...value];

          newValue.splice(idx, 1, item);

          return [...newValue];
        }
      }

      return value;
    };

    setItems(callback);
  }, []);

  const deleteItem = React.useCallback((opt: any) => {
    const callback: setItemsCallback = (value: Item[] | null) => {
      if (!value) return value;

      if (opt.type === "folder") {
        const newValue = value.filter((v) => +v.id !== +opt?.id || !v.isFolder);

        if (newValue.length !== value.length) {
          setTotal((value) => value - 1);
        }

        return newValue;
      }
      if (opt.type === "file") {
        const newValue = value.filter((v) => +v.id !== +opt?.id || v.isFolder);

        if (newValue.length !== value.length) {
          setTotal((value) => value - 1);
        }

        return newValue;
      }

      return value;
    };

    setItems(callback);
  }, []);

  React.useEffect(() => {
    socketHelper.on("s:modify-folder", async (opt: any) => {
      switch (opt?.cmd) {
        case "create":
          addItem(opt);
          break;
        case "update":
          updateItem(opt);
          break;
        case "delete":
          deleteItem(opt);
          break;
      }
    });
  }, [addItem, updateItem, deleteItem]);

  return { subscribe, unsubscribe };
};

export default useSocketHelper;
