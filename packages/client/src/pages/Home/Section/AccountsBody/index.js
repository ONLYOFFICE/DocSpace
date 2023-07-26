import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { useLocation } from "react-router-dom";

import { Consumer } from "@docspace/components/utils/context";

import withLoader from "SRC_DIR/HOCs/withLoader";

import PeopleRowContainer from "./RowView/PeopleRowContainer";
import TableView from "./TableView/TableContainer";

const SectionBodyContent = (props) => {
  const {
    tReady,
    accountsViewAs,
    setSelection,
    setBufferSelection,
    setChangeOwnerDialogVisible,
  } = props;
  const location = useLocation();

  useEffect(() => {
    window.addEventListener("mousedown", onMouseDown);

    if (location?.state?.openChangeOwnerDialog) {
      setChangeOwnerDialogVisible(true);
    }

    return () => {
      window.removeEventListener("mousedown", onMouseDown);
    };
  }, []);

  const onMouseDown = (e) => {
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
};

export default inject(({ peopleStore }) => {
  const { viewAs: accountsViewAs } = peopleStore;

  const { setSelection, setBufferSelection } = peopleStore.selectionStore;
  const { setChangeOwnerDialogVisible } = peopleStore.dialogStore;

  return {
    accountsViewAs,
    setSelection,
    setBufferSelection,
    setChangeOwnerDialogVisible,
  };
})(
  withTranslation(["People", "Common", "PeopleTranslations"])(
    withLoader(observer(SectionBodyContent))()
  )
);
