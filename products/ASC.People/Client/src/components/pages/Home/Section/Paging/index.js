import React from "react";
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

const perPageItems = [
  {
    key: "25",
    label: "25 per page",
    onClick: () => console.log("set paging 25 action")
  },
  {
    key: "50",
    label: "50 per page",
    onClick: () => console.log("set paging 50 action")
  },
  {
    key: "100",
    label: "100 per page",
    onClick: () => console.log("set paging 100 action")
  }
];

const SectionPagingContent = ({ fetchPeople, filter }) => {
  const onNextClick = e => {
    if(!filter.hasNext) {
      e.preventDefault();
      return;
    }
    console.log("Next Clicked", e);
    const newFilter = Filter.nextPage(filter);
    fetchPeople(newFilter);
  };
  const onPrevClick = e => {
    if(!filter.hasPrev) {
      e.preventDefault();
      return;
    }
    console.log("Prev Clicked", e);
    const newFilter = Filter.prevPage(filter);
    fetchPeople(newFilter);
  }
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
