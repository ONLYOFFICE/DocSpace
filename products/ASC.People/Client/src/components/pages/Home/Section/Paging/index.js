import React, { useCallback, useMemo } from "react";
import { connect } from "react-redux";
import { fetchPeople } from "../../../../../store/people/actions";
import { Paging } from "asc-web-components";

const SectionPagingContent = ({
  fetchPeople,
  filter,
  onLoading,
  selectedCount
}) => {
  const onNextClick = useCallback(
    e => {
      if (!filter.hasNext()) {
        e.preventDefault();
        return;
      }
      console.log("Next Clicked", e);

      const newFilter = filter.clone();
      newFilter.page++;

      onLoading(true);
      fetchPeople(newFilter).finally(() => onLoading(false));
    },
    [filter, fetchPeople, onLoading]
  );

  const onPrevClick = useCallback(
    e => {
      if (!filter.hasPrev()) {
        e.preventDefault();
        return;
      }

      console.log("Prev Clicked", e);

      const newFilter = filter.clone();
      newFilter.page--;

      onLoading(true);
      fetchPeople(newFilter).finally(() => onLoading(false));
    },
    [filter, fetchPeople, onLoading]
  );

  const onChangePageSize = useCallback(
    pageItem => {
      console.log("Paging onChangePageSize", pageItem);

      const newFilter = filter.clone();
      newFilter.page = 0;
      newFilter.pageCount = pageItem.key;

      onLoading(true);
      fetchPeople(newFilter).finally(() => onLoading(false));
    },
    [filter, fetchPeople, onLoading]
  );

  const onChangePage = useCallback(
    pageItem => {
      console.log("Paging onChangePage", pageItem);

      const newFilter = filter.clone();
      newFilter.page = pageItem.key;

      onLoading(true);
      fetchPeople(newFilter).finally(() => onLoading(false));
    },
    [filter, fetchPeople, onLoading]
  );

  const countItems = useMemo(
    () => [
      {
        key: 25,
        label: "25 per page"
      },
      {
        key: 50,
        label: "50 per page"
      },
      {
        key: 100,
        label: "100 per page"
      }
    ],
    []
  );

  const pageItems = useMemo(() => {
    const totalPages = Math.round(filter.total / filter.pageCount);
    return [...Array(totalPages).keys()].map(
      item => {
        return { key: item, label: `${item+1} of ${totalPages}` };
      }
    );
  }, [filter.total, filter.pageCount]);

  console.log("SectionPagingContent render", filter);
  return (
    <Paging
      previousLabel="Previous"
      nextLabel="Next"
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
      selectedPage={filter.page} //FILTER CURRENT PAGE
      selectedCount={filter.pageCount} //FILTER PAGE COUNT
      emptyPagePlaceholder="1 of 1"
    />
  );
};

function mapStateToProps(state) {
  return {
    filter: state.people.filter
  };
}

export default connect(
  mapStateToProps,
  { fetchPeople }
)(SectionPagingContent);
