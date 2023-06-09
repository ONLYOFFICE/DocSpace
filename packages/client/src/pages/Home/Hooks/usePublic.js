import React, { useEffect } from "react";
import axios from "axios";
import FilesFilter from "@docspace/common/api/files/filter";

const usePublic = ({ roomId, location, fetchFiles, setIsLoading }) => {
  useEffect(() => {
    let filterObj = null;

    filterObj = FilesFilter.getFilter(window.location);

    if (filterObj?.folder === "@my") {
      filterObj.folder = roomId;
    }

    if (!filterObj) return;
    setIsLoading(true);

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

    if (!dataObj) return;

    const { filter } = dataObj;
    const newFilter = filter ? filter.clone() : FilesFilter.getDefault();
    const requests = [Promise.resolve(newFilter)];

    axios
      .all(requests)
      .catch((err) => {
        Promise.resolve(FilesFilter.getDefault());

        //console.warn("Filter restored by default", err);
      })
      .then((data) => {
        const filter = data[0];

        if (filter) {
          const folderId = filter.folder;
          return fetchFiles(folderId, filter);
        }

        return Promise.resolve();
      })
      .finally(() => {
        setIsLoading(false);
      });
  }, [location.pathname, location.search]);
};

export default usePublic;
