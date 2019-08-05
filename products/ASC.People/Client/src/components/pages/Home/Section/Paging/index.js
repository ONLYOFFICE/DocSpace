import React, {useCallback, useMemo} from "react";
import { connect } from "react-redux";
import { fetchPeople } from "../../../../../store/people/actions";
import { Paging } from "asc-web-components";

/*const pageItems = [
  {
    key: "1",
    label: "1 of 5",
    onClick: () => console.log("set paging 1 of 5")
  },
  {
    key: "2",
    label: "2 of 5",
    onClick: () => console.log("set paging 2 of 5")
  },
  {
    key: "3",
    label: "3 of 5",
    onClick: () => console.log("set paging 3 of 5")
  },
  {
    key: "4",
    label: "4 of 5",
    onClick: () => console.log("set paging 4 of 5")
  },
  {
    key: "5",
    label: "5 of 5",
    onClick: () => console.log("set paging 5 of 5")
  }
];*/

const SectionPagingContent = ({ fetchPeople, filter, onLoading, selectedCount }) => {
  const onNextClick = useCallback(e => {
    if(!filter.hasNext()) {
      e.preventDefault();
      return;
    }
    console.log("Next Clicked", e);

    const newFilter = filter.clone();
    newFilter.page++;
    
    onLoading(true);
    fetchPeople(newFilter)
      .finally(() => onLoading(false));

  }, [filter, fetchPeople, onLoading]);

  const onPrevClick = useCallback(e => {
    if(!filter.hasPrev()) {
      e.preventDefault();
      return;
    }

    console.log("Prev Clicked", e);

    const newFilter = filter.clone();
    newFilter.page--;

    onLoading(true);
    fetchPeople(newFilter)
      .finally(() => onLoading(false));

  }, [filter, fetchPeople, onLoading]);

  const onChangePageSize = useCallback(pageItem => {
    console.log("Paging onChangePageSize", pageItem);
    
    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.pageCount = pageItem.key;

    onLoading(true);
    fetchPeople(newFilter)
      .finally(() => onLoading(false));

  }, [filter, fetchPeople, onLoading]);

  /*const onChangePage = useCallback(pageItem => {
    console.log("Paging onChangePage", pageItem);
  }, []);*/

  const countItems = useMemo(() => [
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
  ], []);

  console.log("SectionPagingContent render", filter);
  return (
    <Paging
      previousLabel="Previous"
      nextLabel="Next"
      //pageItems={pageItems}
      //onSelectPage={onChangePage}
      countItems={countItems}
      onSelectCount={onChangePageSize}
      displayItems={false}
      disablePrevious={!filter.hasPrev()}
      disableNext={!filter.hasNext()}
      previousAction={onPrevClick}
      nextAction={onNextClick}
      openDirection="top"
      //selectedPage={} //FILTER CURRENT PAGE
      selectedCount={selectedCount} //FILTER PAGE COUNT
    />
  );
};

function mapStateToProps(state) {
  return {
    filter: state.people.filter,
    selectedCount: state.people.filter.pageCount
  };
}

export default connect(
  mapStateToProps,
  { fetchPeople }
)(SectionPagingContent);
