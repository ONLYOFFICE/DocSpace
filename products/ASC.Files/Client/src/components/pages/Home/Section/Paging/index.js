import React, { useCallback, useMemo } from "react";
import { connect } from "react-redux";
import { fetchFiles, setIsLoading } from "../../../../../store/files/actions";
import {
  getFilter,
  getSelectedFolderId,
} from "../../../../../store/files/selectors";
import { Paging } from "asc-web-components";
import { useTranslation } from "react-i18next";

const SectionPagingContent = ({
  filter,
  fetchFiles,
  setIsLoading,
  selectedCount,
  selectedFolderId,
}) => {
  const { t } = useTranslation();
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
        label: t("CountPerPage", { count: 25 }),
      },
      {
        key: 50,
        label: t("CountPerPage", { count: 50 }),
      },
      {
        key: 100,
        label: t("CountPerPage", { count: 100 }),
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
        label: t("PageOfTotalPage", { page: item + 1, totalPage: totalPages }),
      };
    });
  }, [filter.total, filter.pageCount, t]);

  const emptyPageSelection = {
    key: 0,
    label: t("PageOfTotalPage", { page: 1, totalPage: 1 }),
  };

  const emptyCountSelection = {
    key: 0,
    label: t("CountPerPage", { count: 25 }),
  };

  const selectedPageItem =
    pageItems.find((x) => x.key === filter.page) || emptyPageSelection;
  const selectedCountItem =
    countItems.find((x) => x.key === filter.pageCount) || emptyCountSelection;

  //console.log("SectionPagingContent render", filter);

  return filter.total < filter.pageCount ? (
    <></>
  ) : (
    <Paging
      previousLabel={t("PreviousPage")}
      nextLabel={t("NextPage")}
      pageItems={pageItems}
      onSelectPage={onChangePage}
      countItems={countItems}
      onSelectCount={onChangePageSize}
      displayItems={false}
      disablePrevious={!filter.hasPrev()}
      disableNext={!filter.hasNext()}
      previousAction={onPrevClick}
      nextAction={onNextClick}
      openDirection="top"
      selectedPageItem={selectedPageItem} //FILTER CURRENT PAGE
      selectedCountItem={selectedCountItem} //FILTER PAGE COUNT
    />
  );
};

function mapStateToProps(state) {
  return {
    filter: getFilter(state),
    selectedFolderId: getSelectedFolderId(state),
  };
}

export default connect(mapStateToProps, { fetchFiles, setIsLoading })(
  SectionPagingContent
);
