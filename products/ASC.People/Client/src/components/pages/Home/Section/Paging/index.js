import React from "react";
import { connect } from "react-redux";
import { getPeople } from "../../../../../actions/peopleActions";
import { Paging } from "asc-web-components";
import Filter from '../../../../../helpers/filter';

const pageItems = [
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
];

const perPageItems = [
  {
    key: "1-1",
    label: "25 per page",
    onClick: () => console.log("set paging 25 action")
  },
  {
    key: "1-2",
    label: "50 per page",
    onClick: () => console.log("set paging 50 action")
  },
  {
    key: "1-3",
    label: "100 per page",
    onClick: () => console.log("set paging 100 action")
  }
];

const SectionPagingContent = ({ users, getPeople }) => {
  console.log("SectionPagingContent render", users);
  return (
    <Paging
      previousLabel="Previous"
      nextLabel="Next"
      pageItems={pageItems}
      perPageItems={perPageItems}
      previousAction={e => {
        console.log("Prev Clicked", e);
        getPeople(new Filter(1, 25));
      }}
      nextAction={e => {
        console.log("Next Clicked", e);
        getPeople(new Filter(2, 25));
      }}
      openDirection="top"
    />
  );
};

function mapStateToProps(state) {
  return {
    users: state.people.users,
    isLoaded: state.auth.isLoaded
  };
}

export default connect(
  mapStateToProps,
  { getPeople }
)(SectionPagingContent);
