import React, {useCallback, useMemo} from "react";
import { connect } from "react-redux";
import { fetchPeople } from "../../../../../store/people/actions";
import { Paging } from "asc-web-components";
import Filter from '../../../../../helpers/filter';

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


const SectionPagingContent = ({ fetchPeople, filter }) => {
  const onNextClick = useCallback(e => {
    if(!filter.hasNext) {
      e.preventDefault();
      return;
    }
    console.log("Next Clicked", e);
    const newFilter = Filter.nextPage(filter);
    fetchPeople(newFilter);
  }, [filter, fetchPeople]);

  const onPrevClick = useCallback(e => {
    if(!filter.hasPrev) {
      e.preventDefault();
      return;
    }
    console.log("Prev Clicked", e);
    const newFilter = Filter.prevPage(filter);
    fetchPeople(newFilter);
  }, [filter, fetchPeople]);

  const onChangePageSize = useCallback(pageCount => {
    const newFilter = new Filter(0, pageCount, filter.total, filter.sortby, filter.sortorder, filter.employeestatus, filter.activationstatus);
    fetchPeople(newFilter);
  }, [filter, fetchPeople]);

  const perPageItems = useMemo(() => [
    {
      key: "25",
      label: "25 per page",
      onClick: () => onChangePageSize(25)
    },
    {
      key: "50",
      label: "50 per page",
      onClick: () => onChangePageSize(50)
    },
    {
      key: "100",
      label: "100 per page",
      onClick: () => onChangePageSize(100)
    }
  ], [onChangePageSize]);

  console.log("SectionPagingContent render", filter);
  return (
    <Paging
      previousLabel="Previous"
      nextLabel="Next"
      // pageItems={pageItems}
      perPageItems={perPageItems}
      displayItems={false}
      disablePrevious={!filter.hasPrev}
      disableNext={!filter.hasNext}
      previousAction={onPrevClick}
      nextAction={onNextClick}
      openDirection="top"
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
