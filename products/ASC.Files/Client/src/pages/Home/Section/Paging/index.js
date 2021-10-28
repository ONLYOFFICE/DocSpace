import React, { useCallback, useMemo } from "react";
import { isMobile } from "react-device-detect";
import Paging from "@appserver/components/paging";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

const SectionPagingContent = ({
  filter,
  files,
  folders,
  fetchFiles,
  setIsLoading,
  selectedCount,
  selectedFolderId,
  tReady,
  totalPages,
}) => {
  const { t } = useTranslation("Home");
  const onNextClick = useCallback(
    (e) => {
      if (!filter.hasNext()) {
        e.preventDefault();
        return;
      }
      console.log("Next Clicked", e);

      const newFilter = filter.clone();
      newFilter.page++;

      setIsLoading(true);
      fetchFiles(selectedFolderId, newFilter).finally(() =>
        setIsLoading(false)
      );
    },
    [filter, selectedFolderId, setIsLoading, fetchFiles]
  );

  const onPrevClick = useCallback(
    (e) => {
      if (!filter.hasPrev()) {
        e.preventDefault();
        return;
      }

      console.log("Prev Clicked", e);

      const newFilter = filter.clone();
      newFilter.page--;

      setIsLoading(true);
      fetchFiles(selectedFolderId, newFilter).finally(() =>
        setIsLoading(false)
      );
    },
    [filter, selectedFolderId, setIsLoading, fetchFiles]
  );

  const onChangePageSize = useCallback(
    (pageItem) => {
      console.log("Paging onChangePageSize", pageItem);

      const newFilter = filter.clone();
      newFilter.page = 0;
      newFilter.pageCount = pageItem.key;

      setIsLoading(true);
      fetchFiles(selectedFolderId, newFilter).finally(() =>
        setIsLoading(false)
      );
    },
    [filter, selectedFolderId, setIsLoading, fetchFiles]
  );

  const onChangePage = useCallback(
    (pageItem) => {
      console.log("Paging onChangePage", pageItem);

      const newFilter = filter.clone();
      newFilter.page = pageItem.key;

      setIsLoading(true);
      fetchFiles(selectedFolderId, newFilter).finally(() =>
        setIsLoading(false)
      );
    },
    [filter, selectedFolderId, setIsLoading, fetchFiles]
  );

  const countItems = useMemo(
    () => [
      {
        key: 25,
        label: t("Common:CountPerPage", { count: 25 }),
      },
      {
        key: 50,
        label: t("Common:CountPerPage", { count: 50 }),
      },
      {
        key: 100,
        label: t("Common:CountPerPage", { count: 100 }),
      },
    ],
    [t]
  );

  const pageItems = useMemo(() => {
    if (filter.total < filter.pageCount) return [];
    return [...Array(totalPages).keys()].map((item) => {
      return {
        key: item,
        label: t("Common:PageOfTotalPage", {
          page: item + 1,
          totalPage: totalPages,
        }),
      };
    });
  }, [filter.total, filter.pageCount, t, totalPages]);

  const emptyPageSelection = {
    key: 0,
    label: t("Common:PageOfTotalPage", { page: 1, totalPage: 1 }),
  };

  const emptyCountSelection = {
    key: 0,
    label: t("Common:CountPerPage", { count: 25 }),
  };

  const selectedPageItem =
    pageItems.find((x) => x.key === filter.page) || emptyPageSelection;
  const selectedCountItem =
    countItems.find((x) => x.key === filter.pageCount) || emptyCountSelection;

  //console.log("SectionPagingContent render", filter);

  const showCountItem = useMemo(() => {
    if (files && folders)
      return (
        files.length + folders.length === filter.pageCount || filter.total > 25
      );
  }, [files, folders, filter, pageItems]);

  return !tReady || (filter.total < filter.pageCount && filter.total < 26) ? (
    <></>
  ) : (
    <Paging
      previousLabel={t("Common:Previous")}
      nextLabel={t("Common:Next")}
      pageItems={pageItems}
      onSelectPage={onChangePage}
      countItems={countItems}
      onSelectCount={onChangePageSize}
      displayItems={false}
      disablePrevious={!filter.hasPrev()}
      disableNext={!filter.hasNext()}
      disableHover={isMobile}
      previousAction={onPrevClick}
      nextAction={onNextClick}
      openDirection="top"
      selectedPageItem={selectedPageItem} //FILTER CURRENT PAGE
      selectedCountItem={selectedCountItem} //FILTER PAGE COUNT
      showCountItem={showCountItem}
    />
  );
};

export default inject(({ filesStore, selectedFolderStore }) => {
  const { files, folders, fetchFiles, filter, setIsLoading } = filesStore;

  const totalPages = Math.ceil(filter.total / filter.pageCount);

  return {
    files,
    folders,
    selectedFolderId: selectedFolderStore.id,
    filter,
    totalPages,

    setIsLoading,
    fetchFiles,
  };
})(observer(SectionPagingContent));
