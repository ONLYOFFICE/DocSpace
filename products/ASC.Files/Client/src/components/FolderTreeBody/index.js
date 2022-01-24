import React from 'react';
import { inject, observer } from 'mobx-react';
import { useTranslation } from 'react-i18next';
import Loader from '@appserver/components/loader';
import Text from '@appserver/components/text';
import Scrollbar from '@appserver/components/scrollbar';
import TreeFolders from '../Article/Body/TreeFolders';
import { StyledSelectFolderPanel } from '../panels/StyledPanels';
const FolderTreeBody = ({
  isLoadingData,
  expandedKeys,
  folderList,
  onSelect,
  withoutProvider,
  certainFolders,
  isAvailable,
  filter,
  selectedKeys,
  heightContent,
  displayType,
  isHeaderChildren,
  theme,
}) => {
  const { t } = useTranslation(['SelectFolder', 'Common']);
  return (
    <>
      {!isLoadingData ? (
        isAvailable ? (
          <StyledSelectFolderPanel
            theme={theme}
            heightContent={heightContent}
            displayType={displayType}
            isHeaderChildren={isHeaderChildren}>
            <div className="select-folder-dialog_tree-folder">
              <Scrollbar theme={theme} id="folder-tree-scroll-bar">
                <TreeFolders
                  theme={theme}
                  expandedPanelKeys={expandedKeys}
                  data={folderList}
                  filter={filter}
                  onSelect={onSelect}
                  withoutProvider={withoutProvider}
                  certainFolders={certainFolders}
                  selectedKeys={selectedKeys}
                  needUpdate={false}
                />
              </Scrollbar>
            </div>
          </StyledSelectFolderPanel>
        ) : (
          <StyledSelectFolderPanel
            theme={theme}
            heightContent={heightContent}
            isHeaderChildren={isHeaderChildren}>
            <div className="tree-folder-empty-list select-folder-dialog_tree-folder">
              <Text as="span">{t('NotAvailableFolder')}</Text>
            </div>
          </StyledSelectFolderPanel>
        )
      ) : (
        <StyledSelectFolderPanel theme={theme} heightContent={heightContent}>
          <div className="tree-folder-Loader" key="loader">
            <Loader
              theme={theme}
              type="oval"
              size="16px"
              style={{
                display: 'inline',
                marginRight: '10px',
                marginTop: '16px',
              }}
            />
            <Text theme={theme} as="span">{`${t('Common:LoadingProcessing')} ${t(
              'Common:LoadingDescription',
            )}`}</Text>
          </div>
        </StyledSelectFolderPanel>
      )}
    </>
  );
};

FolderTreeBody.defaultProps = {
  isAvailable: true,
  isLoadingData: false,
};

export default inject(({ filesStore, treeFoldersStore, selectedFolderStore }) => {
  const { filter, isLoading } = filesStore;
  const { expandedPanelKeys } = treeFoldersStore;
  return {
    expandedKeys: expandedPanelKeys ? expandedPanelKeys : null,
    filter,
    isLoading,
  };
})(observer(FolderTreeBody));
