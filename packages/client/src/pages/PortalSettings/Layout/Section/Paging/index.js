import React, { useCallback, useMemo } from "react";
import { isMobile } from "react-device-detect";
import Paging from "@docspace/components/paging";
import Loaders from "@docspace/common/components/Loaders";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

const SectionPagingContent = ({
  fetchPeople,
  filter,
  setIsLoading,
  selectedCount,
  isLoaded,
  t,
}) => {
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
      fetchPeople(newFilter).finally(() => setIsLoading(false));
    },
    [filter, fetchPeople, setIsLoading]
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
      fetchPeople(newFilter).finally(() => setIsLoading(false));
    },
    [filter, fetchPeople, setIsLoading]
  );

  const onChangePageSize = useCallback(
    (pageItem) => {
      console.log("Paging onChangePageSize", pageItem);

      const newFilter = filter.clone();
      newFilter.page = 0;
      newFilter.pageCount = pageItem.key;

      setIsLoading(true);
      fetchPeople(newFilter).finally(() => setIsLoading(false));
    },
    [filter, fetchPeople, setIsLoading]
  );

  const onChangePage = useCallback(
    (pageItem) => {
      console.log("Paging onChangePage", pageItem);

      const newFilter = filter.clone();
      newFilter.page = pageItem.key;

      setIsLoading(true);
      fetchPeople(newFilter).finally(() => setIsLoading(false));
    },
    [filter, fetchPeople, setIsLoading]
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
    const totalPages = Math.ceil(filter.total / filter.pageCount);
    return [...Array(totalPages).keys()].map((item) => {
      return {
        key: item,
        label: t("Common:PageOfTotalPage", {
          page: item + 1,
          totalPage: totalPages,
        }),
      };
    });
  }, [filter.total, filter.pageCount, t]);

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

  return isLoaded ? (
    !filter || filter.total < filter.pageCount ? (
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
      />
    )
  ) : (
    <Loaders.Filter />
  );
};

export default inject(({ auth, setup }) => ({
  isLoaded: auth.isLoaded,
  fetchPeople: setup.updateListAdmins,
  filter: setup.security.accessRight.filter,
  setIsLoading: setup.setIsLoading,
}))(withTranslation(["Settings", "Common"])(observer(SectionPagingContent)));
