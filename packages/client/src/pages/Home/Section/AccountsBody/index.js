import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import { Consumer } from "@docspace/components/utils/context";

import withLoader from "SRC_DIR/HOCs/withLoader";

import PeopleRowContainer from "./RowView/PeopleRowContainer";
import TableView from "./TableView/TableContainer";

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
      window?.getSelection()?.removeAllRanges();
    }
  };

  render() {
    const { tReady, accountsViewAs } = this.props;

    return (
      <Consumer>
        {(context) =>
          accountsViewAs === "table" ? (
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
  const { viewAs: accountsViewAs } = peopleStore;

  const { setSelection, setBufferSelection } = peopleStore.selectionStore;

  return { accountsViewAs, setSelection, setBufferSelection };
})(
  withTranslation(["People", "Common", "PeopleTranslations"])(
    withLoader(observer(SectionBodyContent))()
  )
);
