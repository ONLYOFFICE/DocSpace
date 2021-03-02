import React from "react";
import { withRouter } from "react-router";
import { withTranslation, Trans } from "react-i18next";
//import styled from "styled-components";

import RowContainer from "@appserver/components/row-container";
import { Consumer } from "@appserver/components/utils/context";
//import { isArrayEqual } from "@appserver/components/utils/array";

//import equal from "fast-deep-equal/react";

import toastr from "@appserver/common/components/Toast/toastr";
//import { EmployeeStatus } from "@appserver/common/constants";
import Loaders from "@appserver/common/components/Loaders";

import EmptyScreen from "./sub-components/EmptyScreen";
import { inject, observer } from "mobx-react";
import SimpleUserRow from "./SimpleUserRow";
import Dialogs from "./Dialogs";

class SectionBodyContent extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      isLoadedSection: true,
    };
  }

  componentDidMount() {
    const { isLoaded, fetchPeople, filter, peopleList } = this.props;
    if (!isLoaded || peopleList.length > 0) return;

    this.setState({ isLoadedSection: false });

    fetchPeople(filter)
      .catch((error) => {
        toastr.error(error);
      })
      .finally(() => this.setState({ isLoadedSection: isLoaded }));
  }

  //findUserById = (id) => this.props.peopleList.find((man) => man.id === id);

  onResetFilter = () => {
    const { onLoading, resetFilter } = this.props;
    onLoading(true);
    resetFilter(true).finally(() => onLoading(false));
  };

  /*needForUpdate = (currentProps, nextProps) => {
    if (currentProps.checked !== nextProps.checked) {
      return true;
    }
    if (currentProps.status !== nextProps.status) {
      return true;
    }
    if (currentProps.sectionWidth !== nextProps.sectionWidth) {
      return true;
    }
    if (!equal(currentProps.data, nextProps.data)) {
      return true;
    }
    if (!isArrayEqual(currentProps.contextOptions, nextProps.contextOptions)) {
      return true;
    }
    return false;
  };*/

  render() {
    // console.log("Home SectionBodyContent render()");
    const {
      isLoaded,
      peopleList,
      //history,
      t,
      //filter,
      //widthProp,
      isMobile,
      //selectGroup,
      isLoading,
      //isAdmin,
      //currentUserId,
      isEmptyGroup,
      //isUserSelected,
    } = this.props;

    const { isLoadedSection } = this.state;

    return !isLoaded || (isMobile && isLoading) || !isLoadedSection ? (
      <Loaders.Rows isRectangle={false} />
    ) : peopleList.length > 0 ? (
      <>
        <Consumer>
          {(context) => (
            <RowContainer
              className="people-row-container"
              useReactWindow={false}
            >
              {peopleList.map((man) => (
                <SimpleUserRow
                  man={man}
                  sectionWidth={context.sectionWidth}
                  isMobile={isMobile}
                />
              ))}
            </RowContainer>
          )}
        </Consumer>
        <Dialogs />
      </>
    ) : (
      <EmptyScreen
        t={t}
        onResetFilter={this.onResetFilter}
        isEmptyGroup={isEmptyGroup}
      />
    );
  }
}

export default inject(({ auth, peopleStore }) => ({
  isLoaded: auth.isLoaded,
  //isAdmin: auth.isAdmin,
  currentUserId: auth.userStore.user.id,
  fetchPeople: peopleStore.usersStore.getUsersList,
  peopleList: peopleStore.usersStore.peopleList,

  filter: peopleStore.filterStore.filter,
  resetFilter: peopleStore.resetFilter,

  isLoading: peopleStore.isLoading,
  isEmptyGroup: peopleStore.selectedGroupStore.isEmptyGroup,
}))(observer(withRouter(withTranslation("Home")(SectionBodyContent))));
