import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import Loaders from "@docspace/common/components/Loaders";

import { Consumer } from "@docspace/components/utils/context";

import withPeopleLoader from "SRC_DIR/HOCs/withPeopleLoader";

import PeopleRowContainer from "./RowView/PeopleRowContainer";
import TableView from "./TableView/TableContainer";

import RoomSelector from "SRC_DIR/components/RoomSelector";

class SectionBodyContent extends React.Component {
  componentDidMount() {
    window.addEventListener("mousedown", this.onMouseDown);
  }

  componentWillUnmount() {
    window.removeEventListener("mousedown", this.onMouseDown);
  }

  onMouseDown = (e) => {
    const { setSelection, setBufferSelection } = this.props;
    if (
      (e.target.closest(".scroll-body") &&
        !e.target.closest(".user-item") &&
        !e.target.closest(".not-selectable") &&
        !e.target.closest(".info-panel") &&
        !e.target.closest(".table-container_group-menu")) ||
      e.target.closest(".files-main-button") ||
      e.target.closest(".add-button") ||
      e.target.closest(".search-input-block")
    ) {
      setSelection([]);
      setBufferSelection(null);
    }
  };

  render() {
    const { tReady, viewAs } = this.props;

    return (
      <Consumer>
        {(context) =>
          viewAs === "table" ? (
            <>
              <TableView sectionWidth={context.sectionWidth} tReady={tReady} />
            </>
          ) : (
            <>
              <PeopleRowContainer
                sectionWidth={context.sectionWidth}
                tReady={tReady}
              />
            </>
          )
        }
      </Consumer>
    );
  }
}

export default inject(({ peopleStore }) => {
  const { viewAs } = peopleStore;

  const { setSelection, setBufferSelection } = peopleStore.selectionStore;

  return { viewAs, setSelection, setBufferSelection };
})(
  withTranslation(["People", "Common", "PeopleTranslations"])(
    withPeopleLoader(observer(SectionBodyContent))(
      <Loaders.Rows isRectangle={false} />
    )
  )
);
