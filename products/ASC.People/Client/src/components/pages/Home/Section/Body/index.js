import React from "react";
import RowContainer from "@appserver/components/row-container";
import { Consumer } from "@appserver/components/utils/context";
import toastr from "@appserver/common/components/Toast/toastr";
import Loaders from "@appserver/common/components/Loaders";

import EmptyScreen from "./EmptyScreen";
import { inject, observer } from "mobx-react";
import SimpleUserRow from "./SimpleUserRow";
import Dialogs from "./Dialogs";
import { isMobile } from "react-device-detect";

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

  render() {
    // console.log("Home SectionBodyContent render()");
    const { isLoaded, peopleList, isLoading } = this.props;

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
              {peopleList.map((person) => (
                <SimpleUserRow
                  key={person.id}
                  person={person}
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
      <EmptyScreen />
    );
  }
}

export default inject(({ auth, peopleStore }) => ({
  isLoaded: auth.isLoaded,
  fetchPeople: peopleStore.usersStore.getUsersList,
  peopleList: peopleStore.usersStore.peopleList,

  filter: peopleStore.filterStore.filter,
  isLoading: peopleStore.isLoading,
}))(observer(SectionBodyContent));
