import { inject, observer } from "mobx-react";
import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";

import SeveralItems from "./SeveralItems";
import SingleItem from "./SingleItem";
import { StyledInfoRoomBody } from "./styles/styles.js";

const InfoPanelBodyContent = ({
    t,
    selectedItems,
    bufferSelectedItem,
    getFolderInfo,
    getIcon,
    getFolderIcon,
    getShareUsers,
    onSelectItem,
    setSharingPanelVisible,
    isRecycleBinFolder,
}) => {
    if (selectedItems.length) {
    }

    return (
        <StyledInfoRoomBody>
            {selectedItems.length === 0 && !bufferSelectedItem ? (
                <div className="no-item">
                    <h4>{t("NoItemsSelected")}</h4>
                </div>
            ) : selectedItems.length === 1 || bufferSelectedItem ? (
                <SingleItem
                    selectedItem={selectedItems[0] || bufferSelectedItem}
                    isRecycleBinFolder={isRecycleBinFolder}
                    onSelectItem={onSelectItem}
                    setSharingPanelVisible={setSharingPanelVisible}
                    getFolderInfo={getFolderInfo}
                    getIcon={getIcon}
                    getFolderIcon={getFolderIcon}
                    getShareUsers={getShareUsers}
                />
            ) : (
                <SeveralItems selectedItems={selectedItems} getIcon={getIcon} />
            )}
        </StyledInfoRoomBody>
    );
};

export default inject(
    ({
        filesStore,
        formatsStore,
        filesActionsStore,
        dialogsStore,
        treeFoldersStore,
    }) => {
        const selectedItems = JSON.parse(JSON.stringify(filesStore.selection));
        const bufferSelectedItem = JSON.parse(
            JSON.stringify(filesStore.bufferSelection)
        );
        const { getFolderInfo, getShareUsers } = filesStore;

        const { getIcon, getFolderIcon } = formatsStore.iconFormatsStore;
        const { onSelectItem } = filesActionsStore;
        const { setSharingPanelVisible } = dialogsStore;
        const { isRecycleBinFolder } = treeFoldersStore;

        return {
            bufferSelectedItem,
            selectedItems,
            getFolderInfo,
            getShareUsers,
            getIcon,
            getFolderIcon,
            onSelectItem,
            setSharingPanelVisible,
            isRecycleBinFolder,
        };
    }
)(withRouter(withTranslation(["InfoPanel"])(observer(InfoPanelBodyContent))));
