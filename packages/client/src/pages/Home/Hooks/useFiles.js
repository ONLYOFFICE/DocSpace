import React from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

import FilesFilter from "@docspace/common/api/files/filter";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import { getGroup } from "@docspace/common/api/groups";
import { getUserById } from "@docspace/common/api/people";

import { Events, RoomSearchArea } from "@docspace/common/constants";
import { getObjectByLocation } from "@docspace/common/utils";

import { getCategoryType, getCategoryUrl } from "SRC_DIR/helpers/utils";
import { CategoryType } from "SRC_DIR/helpers/constants";

const useFiles = ({
  t,

  dragging,
  setDragging,
  disableDrag,
  uploadEmptyFolders,
  startUpload,

  fetchFiles,
  fetchRooms,
  setIsLoading,

  isAccountsPage,
  isSettingsPage,

  location,

  playlist,

  getFileInfo,
  setToPreviewFile,
  setIsPreview,

  setIsUpdatingRowItem,

  gallerySelected,
  removeFirstUrl,
}) => {
  const navigate = useNavigate();

  const fetchDefaultFiles = () => {
    const filter = FilesFilter.getDefault();

    const url = getCategoryUrl(CategoryType.Personal);

    navigate(`${url}?${filter.toUrlParams()}`);
  };

  const fetchDefaultRooms = () => {
    const filter = RoomsFilter.getDefault();

    const categoryType = getCategoryType(location);

    const url = getCategoryUrl(categoryType);

    filter.searchArea =
      categoryType === CategoryType.Shared
        ? RoomSearchArea.Active
        : RoomSearchArea.Archive;

    navigate(`${url}?${filter.toUrlParams()}`);
  };

  const onDrop = (files, uploadToFolder) => {
    dragging && setDragging(false);

    if (disableDrag) return;

    const emptyFolders = files.filter((f) => f.isEmptyDirectory);

    if (emptyFolders.length > 0) {
      uploadEmptyFolders(emptyFolders, uploadToFolder).then(() => {
        const onlyFiles = files.filter((f) => !f.isEmptyDirectory);
        if (onlyFiles.length > 0) startUpload(onlyFiles, uploadToFolder, t);
      });
    } else {
      startUpload(files, uploadToFolder, t);
    }
  };

  React.useEffect(() => {
    if (isAccountsPage || isSettingsPage) return;
    setIsLoading(true);

    if (!window.location.href.includes("#preview")) {
      // localStorage.removeItem("isFirstUrl");
      // Media viewer
      removeFirstUrl();
    }

    const categoryType = getCategoryType(location);

    let filterObj = null;
    let isRooms = false;

    if (window.location.href.indexOf("/#preview") > 1 && playlist.length < 1) {
      const pathname = window.location.href;
      const fileId = pathname.slice(pathname.indexOf("#preview") + 9);

      setTimeout(() => {
        getFileInfo(fileId)
          .then((data) => {
            const canOpenPlayer =
              data.viewAccessability.ImageView ||
              data.viewAccessability.MediaView;
            const file = { ...data, canOpenPlayer };
            setToPreviewFile(file, true);
            setIsPreview(true);
          })
          .catch((err) => {
            toastr.error(err);
            fetchDefaultFiles();
          });
      }, 1);

      return setIsLoading(false);
    }

    if (window.location.href.indexOf("/#preview") > 1)
      return setIsLoading(false);

    const isRoomFolder = getObjectByLocation(window.location)?.folder;

    if (
      (categoryType == CategoryType.Shared ||
        categoryType == CategoryType.SharedRoom ||
        categoryType == CategoryType.Archive) &&
      !isRoomFolder
    ) {
      filterObj = RoomsFilter.getFilter(window.location);

      isRooms = true;

      if (!filterObj) {
        fetchDefaultRooms();

        return;
      }
    } else {
      filterObj = FilesFilter.getFilter(window.location);

      if (!filterObj) {
        fetchDefaultFiles();

        return;
      }
    }

    if (!filterObj) return;

    let dataObj = { filter: filterObj };

    if (filterObj && filterObj.authorType) {
      const authorType = filterObj.authorType;
      const indexOfUnderscore = authorType.indexOf("_");
      const type = authorType.slice(0, indexOfUnderscore);
      const itemId = authorType.slice(indexOfUnderscore + 1);

      if (itemId) {
        dataObj = {
          type,
          itemId,
          filter: filterObj,
        };
      } else {
        filterObj.authorType = null;
        dataObj = { filter: filterObj };
      }
    }

    if (filterObj && filterObj.subjectId) {
      const type = "user";
      const itemId = filterObj.subjectId;

      if (itemId) {
        dataObj = {
          type,
          itemId,
          filter: filterObj,
        };
      } else {
        filterObj.subjectId = null;
        dataObj = { filter: filterObj };
      }
    }

    if (!dataObj) return;

    const { filter, itemId, type } = dataObj;
    const newFilter = filter
      ? filter.clone()
      : isRooms
      ? RoomsFilter.getDefault()
      : FilesFilter.getDefault();
    const requests = [Promise.resolve(newFilter)];

    if (type === "group") {
      requests.push(getGroup(itemId));
    } else if (type === "user") {
      requests.push(getUserById(itemId));
    }

    axios
      .all(requests)
      .catch((err) => {
        if (isRooms) {
          Promise.resolve(RoomsFilter.getDefault());
        } else {
          Promise.resolve(FilesFilter.getDefault());
        }

        //console.warn("Filter restored by default", err);
      })
      .then((data) => {
        const filter = data[0];
        const result = data[1];
        if (result) {
          const type = result.displayName ? "user" : "group";
          const selectedItem = {
            key: result.id,
            label: type === "user" ? result.displayName : result.name,
            type,
          };
          if (!isRooms) {
            filter.selectedItem = selectedItem;
          }
        }

        if (filter) {
          if (isRooms) {
            return fetchRooms(
              null,
              filter,
              undefined,
              undefined,
              undefined,
              true
            );
          } else {
            const folderId = filter.folder;
            return fetchFiles(folderId, filter);
          }
        }

        return Promise.resolve();
      })
      .then(() => {
        if (gallerySelected) {
          setIsUpdatingRowItem(false);

          const event = new Event(Events.CREATE);

          const payload = {
            extension: "docxf",
            id: -1,
            fromTemplate: true,
            title: gallerySelected.attributes.name_form,
          };

          event.payload = payload;

          window.dispatchEvent(event);
        }
      })
      .finally(() => {
        setIsLoading(false);
      });
  }, [location.pathname, location.search, isAccountsPage, isSettingsPage]);

  return { onDrop };
};

export default useFiles;
