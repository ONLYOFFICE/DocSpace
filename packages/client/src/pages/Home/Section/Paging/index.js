import React, { useCallback, useMemo, useEffect } from "react";
import { isMobile } from "react-device-detect";
import { useLocation } from "react-router-dom";
import Paging from "@docspace/components/paging";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

const SectionPagingContent = ({
  filter,
  files,
  folders,
  fetchFiles,
  fetchRooms,
  setIsLoading,
  selectedCount,
  selectedFolderId,
  tReady,
  totalPages,
  setPageItemsLength,
  isHidePagination,
  isRooms,
  accountsFilter,
  fetchPeople,
}) => {
  const location = useLocation();

  const isAccountsPage = location.pathname.includes("accounts");

  const { t } = useTranslation(["Files", "People"]);
  const onNextClick = useCallback(
    (e) => {
      if (isAccountsPage) {
        if (!accountsFilter.hasNext()) {
          e.preventDefault();
          return;
        }
        //console.log("Next Clicked", e);

        const newFilter = accountsFilter.clone();
        newFilter.page++;

        setIsLoading(true);
        fetchPeople(newFilter).finally(() => setIsLoading(false));
      } else {
        if (!filter.hasNext()) {
          e.preventDefault();
          return;
        }
        //console.log("Next Clicked", e);

        const newFilter = filter.clone();
        newFilter.page++;

        setIsLoading(true);
        if (isRooms) {
          fetchRooms(selectedFolderId, newFilter).finally(() =>
            setIsLoading(false)
          );
        } else {
          fetchFiles(selectedFolderId, newFilter).finally(() =>
            setIsLoading(false)
          );
        }
      }
    },
    [
      isAccountsPage,
      accountsFilter,
      fetchPeople,
      filter,
      selectedFolderId,
      setIsLoading,
      fetchFiles,
      isRooms,
    ]
  );

  const onPrevClick = useCallback(
    (e) => {
      if (isAccountsPage) {
        if (!accountsFilter.hasPrev()) {
          e.preventDefault();
          return;
        }

        //console.log("Prev Clicked", e);

        const newFilter = accountsFilter.clone();
        newFilter.page--;

        setIsLoading(true);
        fetchPeople(newFilter).finally(() => setIsLoading(false));
      } else {
        if (!filter.hasPrev()) {
          e.preventDefault();
          return;
        }

        //console.log("Prev Clicked", e);

        const newFilter = filter.clone();
        newFilter.page--;

        setIsLoading(true);
        if (isRooms) {
          fetchRooms(selectedFolderId, newFilter).finally(() =>
            setIsLoading(false)
          );
        } else {
          fetchFiles(selectedFolderId, newFilter).finally(() =>
            setIsLoading(false)
          );
        }
      }
    },
    [
      isAccountsPage,
      accountsFiler,
      fetchPeople,
      filter,
      selectedFolderId,
      setIsLoading,
      fetchFiles,
    ]
  );

  const onChangePageSize = useCallback(
    (pageItem) => {
      //console.log("Paging onChangePageSize", pageItem);
      if (isAccountsPage) {
        //console.log("Paging onChangePageSize", pageItem);

        const newFilter = accountsFilter.clone();
        newFilter.page = 0;
        newFilter.pageCount = pageItem.key;

        setIsLoading(true);
        fetchPeople(newFilter).finally(() => setIsLoading(false));
      } else {
        const newFilter = filter.clone();
        newFilter.page = 0;
        newFilter.pageCount = pageItem.key;

        setIsLoading(true);
        if (isRooms) {
          fetchRooms(selectedFolderId, newFilter).finally(() =>
            setIsLoading(false)
          );
        } else {
          fetchFiles(selectedFolderId, newFilter).finally(() =>
            setIsLoading(false)
          );
        }
      }
    },
    [
      isAccountsPage,
      accountsFilter,
      fetchPeople,
      filter,
      selectedFolderId,
      setIsLoading,
      fetchFiles,
    ]
  );

  const onChangePage = useCallback(
    (pageItem) => {
      //console.log("Paging onChangePage", pageItem);
      if (isAccountsPage) {
        const newFilter = accountsFilter.clone();
        newFilter.page = pageItem.key;

        setIsLoading(true);
        fetchPeople(newFilter).finally(() => setIsLoading(false));
      } else {
        const newFilter = filter.clone();
        newFilter.page = pageItem.key;

        setIsLoading(true);
        if (isRooms) {
          fetchRooms(selectedFolderId, newFilter).finally(() =>
            setIsLoading(false)
          );
        } else {
          fetchFiles(selectedFolderId, newFilter).finally(() =>
            setIsLoading(false)
          );
        }
      }
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
    if (isAccountsPage) {
      const totalPages = Math.ceil(
        accountsFilter.total / accountsFilter.pageCount
      );
      return [...Array(totalPages).keys()].map((item) => {
        return {
          key: item,
          label: t("Common:PageOfTotalPage", {
            page: item + 1,
            totalPage: totalPages,
          }),
        };
      });
    } else {
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
    }
  }, [
    filter.total,
    filter.pageCount,
    t,
    totalPages,
    accountsFilter.total,
    accountsFilter.pageCount,
  ]);

  useEffect(() => {
    if (isAccountsPage) return;
    setPageItemsLength(pageItems.length);
  }, [pageItems, isAccountsPage]);

  const emptyPageSelection = {
    key: 0,
    label: t("Common:PageOfTotalPage", { page: 1, totalPage: 1 }),
  };

  const emptyCountSelection = {
    key: 0,
    label: t("Common:CountPerPage", { count: 25 }),
  };

  const selectedPageItem = isAccountsPage
    ? pageItems.find((x) => x.key === accountsFilter.page) || emptyPageSelection
    : pageItems.find((x) => x.key === filter.page) || emptyPageSelection;
  const selectedCountItem = isAccountsPage
    ? countItems.find((x) => x.key === accountsFilter.pageCount) ||
      emptyCountSelection
    : countItems.find((x) => x.key === filter.pageCount) || emptyCountSelection;

  //console.log("SectionPagingContent render", filter);

  const showCountItem = useMemo(() => {
    if (isAccountsPage) return false;

    if (files && folders)
      return (
        files.length + folders.length === filter.pageCount || filter.total > 25
      );
  }, [isAccountsPage, files, folders, filter, pageItems]);

  return !tReady ||
    (filter.total <= filter.pageCount && filter.total < 26) ||
    isHidePagination ? (
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
      disablePrevious={
        isAccountsPage ? !accountsFilter.hasPrev() : !filter.hasPrev()
      }
      disableNext={
        isAccountsPage ? !accountsFilter.hasNext() : !filter.hasNext()
      }
      disableHover={isMobile}
      previousAction={onPrevClick}
      nextAction={onNextClick}
      openDirection="both"
      selectedPageItem={selectedPageItem} //FILTER CURRENT PAGE
      selectedCountItem={selectedCountItem} //FILTER PAGE COUNT
      showCountItem={showCountItem}
    />
  );
};

export default inject(
  ({
    auth,
    peopleStore,
    filesStore,
    selectedFolderStore,
    treeFoldersStore,
  }) => {
    const {
      files,
      folders,
      fetchFiles,
      fetchRooms,
      filter,
      roomsFilter,
      setIsLoading,
      setPageItemsLength,
      isHidePagination,
    } = filesStore;

    const { isRoomsFolder, isArchiveFolder } = treeFoldersStore;

    const isRooms = isRoomsFolder || isArchiveFolder;

    const currentFilter = isRooms ? roomsFilter.clone() : filter.clone();

    const totalPages = Math.ceil(currentFilter.total / currentFilter.pageCount);

    const { usersStore, filterStore } = peopleStore;
    const { filter: accountsFilter } = filterStore;

    const { getUsersList: fetchPeople } = usersStore;

    return {
      files,
      folders,
      selectedFolderId: selectedFolderStore.id,
      filter: currentFilter,

      totalPages,

      setIsLoading,
      fetchFiles,
      fetchRooms,

      setPageItemsLength,
      isHidePagination,
      isRooms,

      accountsFilter,
      fetchPeople,
    };
  }
)(observer(SectionPagingContent));
