import React from "react";
import RowContainer from "@appserver/components/row-container";
import { Consumer } from "@appserver/components/utils/context";
//import toastr from "studio/toastr";
import Loaders from "@appserver/common/components/Loaders";

import EmptyScreen from "./EmptyScreen";
import { inject, observer } from "mobx-react";
import SimpleUserRow from "./SimpleUserRow";
import Dialogs from "./Dialogs";
import { isMobile } from "react-device-detect";

const SectionBodyContent = ({ isLoaded, peopleList, isLoading, isRefresh }) => {
  return !isLoaded || (isMobile && isLoading) || isRefresh ? (
    <Loaders.Rows isRectangle={false} />
  ) : peopleList.length > 0 ? (
    <>
      <Consumer>
        {(context) => (
          <RowContainer className="people-row-container" useReactWindow={false}>
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
};

export default inject(({ auth, peopleStore }) => ({
  isLoaded: auth.isLoaded,
  isRefresh: peopleStore.isRefresh,
  peopleList: peopleStore.usersStore.peopleList,
  isLoading: peopleStore.isLoading,
}))(observer(SectionBodyContent));
