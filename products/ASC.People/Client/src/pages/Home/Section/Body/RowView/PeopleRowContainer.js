import React from "react";
import { inject, observer } from "mobx-react";
import RowContainer from "@appserver/components/row-container";
import { Consumer } from "@appserver/components/utils/context";
import SimpleUserRow from "./SimpleUserRow";
import EmptyScreen from "../EmptyScreen";

const PeopleRowContainer = ({ peopleList }) => {
  return peopleList.length > 0 ? (
    <Consumer>
      {(context) => (
        <RowContainer className="people-row-container" useReactWindow={false}>
          {peopleList.map((item) => (
            <SimpleUserRow
              key={item.id}
              item={item}
              sectionWidth={context.sectionWidth}
            />
          ))}
        </RowContainer>
      )}
    </Consumer>
  ) : (
    <EmptyScreen />
  );
};

export default inject(({ peopleStore }) => {
  const { peopleList } = peopleStore.usersStore;

  return {
    peopleList,
  };
})(observer(PeopleRowContainer));
