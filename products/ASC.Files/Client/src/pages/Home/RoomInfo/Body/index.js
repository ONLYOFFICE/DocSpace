import { inject, observer } from "mobx-react";
import React from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";

const RoomInfoBodyContent = (selectedFilesStore) => {
    return <div>{selectedFilesStore.fileInfo}</div>;
};

export default inject(({ selectedFilesStore }) => {
    //const { selectedFilesStore } = filesStore;
    console.log(selectedFilesStore);
    return { selectedFilesStore };
})(
    withRouter(
        withTranslation(["Home", "Common", "Translations"])(
            observer(RoomInfoBodyContent)
        )
    )
);
