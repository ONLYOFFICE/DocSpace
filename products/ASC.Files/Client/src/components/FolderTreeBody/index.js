import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import Scrollbar from "@appserver/components/scrollbar";
import { StyledTree } from "../panels/SelectionPanel/StyledSelectionPanel";
import TreeFolders from "./TreeFolders";
const FolderTreeBody = ({
  expandedKeys,
  folderTree,
  onSelect,
  withoutProvider,
  certainFolders,
  isAvailable,
  filter,
  selectedKeys,
  theme,
  isDisableTree,
  parentId,
  isLoadingNodes,
}) => {
  const { t } = useTranslation(["SelectFolder", "Common"]);

  useEffect(() => {
    const selectedNode = document.getElementsByClassName(
      "rc-tree-node-selected"
    )[0];
    if (selectedNode) {
      document
        .querySelector("#folder-tree-scroll-bar > .scroll-body")
        .scrollTo(0, selectedNode.offsetTop);
    }
  }, []);

  return (
    <>
      {isAvailable ? (
        <StyledTree theme={theme} isLoadingNodes={isLoadingNodes}>
          <div className="selection-panel_tree-folder">
            <Scrollbar id="folder-tree-scroll-bar" stype="mediumBlack">
              <TreeFolders
                isPanel={true}
                expandedPanelKeys={expandedKeys}
                data={folderTree}
                filter={filter}
                onSelect={onSelect}
                withoutProvider={withoutProvider}
                certainFolders={certainFolders}
                selectedKeys={selectedKeys}
                disabled={isDisableTree}
                needUpdate={false}
                defaultExpandAll
                parentId={parentId}
              />
            </Scrollbar>
          </div>
        </StyledTree>
      ) : (
        <StyledTree>
          <div className="selection-panel_empty-folder">
            <Text as="span">{t("NotAvailableFolder")}</Text>
          </div>
        </StyledTree>
      )}
    </>
  );
};

FolderTreeBody.defaultProps = {
  isAvailable: true,
  isLoadingData: false,
};

export default inject(
  ({ filesStore, treeFoldersStore, selectedFolderStore }) => {
    const { filter, isLoading } = filesStore;
    const { expandedPanelKeys, isLoadingNodes } = treeFoldersStore;

    const expandedKeysProp = expandedPanelKeys
      ? expandedPanelKeys
      : selectedFolderStore.pathParts;
    const expandedKeys = expandedKeysProp?.map((item) => item.toString());
    !expandedPanelKeys && expandedKeys?.pop();
    return {
      expandedKeys: expandedKeys,

      filter,
      isLoading,
      isLoadingNodes,
    };
  }
)(observer(FolderTreeBody));
