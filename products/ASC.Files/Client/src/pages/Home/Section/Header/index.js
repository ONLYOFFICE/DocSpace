import React from 'react';
import copy from 'copy-to-clipboard';
import styled, { css } from 'styled-components';
import { withRouter } from 'react-router';
import toastr from 'studio/toastr';
import Loaders from '@appserver/common/components/Loaders';
import Headline from '@appserver/common/components/Headline';
import { FilterType, FileAction } from '@appserver/common/constants';
import { withTranslation } from 'react-i18next';
import { isMobile } from 'react-device-detect';
import ContextMenuButton from '@appserver/components/context-menu-button';
import DropDownItem from '@appserver/components/drop-down-item';
import GroupButtonsMenu from '@appserver/components/group-buttons-menu';
import IconButton from '@appserver/components/icon-button';
import { tablet, desktop, isTablet } from '@appserver/components/utils/device';
import { Consumer } from '@appserver/components/utils/context';
import { inject, observer } from 'mobx-react';
import Navigation from '@appserver/common/components/Navigation';

class SectionHeaderContent extends React.Component {
  constructor(props) {
    super(props);
    this.state = { navigationItems: [] };
  }

  onCreate = (format) => {
    this.props.setAction({
      type: FileAction.Create,
      extension: format,
      id: -1,
    });
  };

  createDocument = () => this.onCreate('docx');

  createSpreadsheet = () => this.onCreate('xlsx');

  createPresentation = () => this.onCreate('pptx');

  createFolder = () => this.onCreate();

  uploadToFolder = () => console.log('Upload To Folder click');

  getContextOptionsPlus = () => {
    const { t } = this.props;

    return [
      {
        key: 'new-document',
        label: t('NewDocument'),
        onClick: this.createDocument,
      },
      {
        key: 'new-spreadsheet',
        label: t('NewSpreadsheet'),
        onClick: this.createSpreadsheet,
      },
      {
        key: 'new-presentation',
        label: t('NewPresentation'),
        onClick: this.createPresentation,
      },
      {
        key: 'new-folder',
        label: t('NewFolder'),
        onClick: this.createFolder,
      },
      { key: 'separator', isSeparator: true },
      {
        key: 'make-invitation-link',
        label: t('UploadToFolder'),
        onClick: this.uploadToFolder,
        disabled: true,
      },
    ];
  };

  createLinkForPortalUsers = () => {
    const { currentFolderId } = this.props;
    const { t } = this.props;

    copy(`${window.location.origin}/products/files/filter?folder=${currentFolderId}`);

    toastr.success(t('Translations:LinkCopySuccess'));
  };

  onMoveAction = () => {
    this.props.setIsFolderActions(true);
    this.props.setBufferSelection(this.props.currentFolderId);
    return this.props.setMoveToPanelVisible(true);
  };
  onCopyAction = () => {
    this.props.setIsFolderActions(true);
    this.props.setBufferSelection(this.props.currentFolderId);
    return this.props.setCopyPanelVisible(true);
  };
  downloadAction = () => {
    this.props.setBufferSelection(this.props.currentFolderId);
    this.props.setIsFolderActions(true);
    this.props
      .downloadAction(this.props.t('Translations:ArchivingData'), [this.props.currentFolderId])
      .catch((err) => toastr.error(err));
  };

  renameAction = () => console.log('renameAction click');
  onOpenSharingPanel = () => {
    this.props.setBufferSelection(this.props.currentFolderId);
    this.props.setIsFolderActions(true);
    return this.props.setSharingPanelVisible(true);
  };

  onDeleteAction = () => {
    const {
      t,
      deleteAction,
      confirmDelete,
      setDeleteDialogVisible,
      isThirdPartySelection,
      currentFolderId,
      getFolderInfo,
      setBufferSelection,
    } = this.props;

    this.props.setIsFolderActions(true);

    if (confirmDelete || isThirdPartySelection) {
      getFolderInfo(currentFolderId).then((data) => {
        setBufferSelection(data);
        setDeleteDialogVisible(true);
      });
    } else {
      const translations = {
        deleteOperation: t('Translations:DeleteOperation'),
        deleteFromTrash: t('Translations:DeleteFromTrash'),
        deleteSelectedElem: t('Translations:DeleteSelectedElem'),
      };

      deleteAction(translations, [currentFolderId], true).catch((err) => toastr.error(err));
    }
  };

  onEmptyTrashAction = () => this.props.setEmptyTrashDialogVisible(true);

  getContextOptionsFolder = () => {
    const { t, personal } = this.props;

    return [
      {
        key: 'sharing-settings',
        label: t('SharingSettings'),
        onClick: this.onOpenSharingPanel,
        disabled: personal ? true : false,
      },
      {
        key: 'link-portal-users',
        label: t('LinkForPortalUsers'),
        onClick: this.createLinkForPortalUsers,
        disabled: personal ? true : false,
      },
      { key: 'separator-2', isSeparator: true },
      {
        key: 'move-to',
        label: t('MoveTo'),
        onClick: this.onMoveAction,
        disabled: false,
      },
      {
        key: 'copy',
        label: t('Translations:Copy'),
        onClick: this.onCopyAction,
        disabled: false,
      },
      {
        key: 'download',
        label: t('Common:Download'),
        onClick: this.downloadAction,
        disabled: false,
      },
      {
        key: 'rename',
        label: t('Rename'),
        onClick: this.renameAction,
        disabled: true,
      },
      {
        key: 'delete',
        label: t('Common:Delete'),
        onClick: this.onDeleteAction,
        disabled: false,
      },
    ];
  };

  onBackToParentFolder = () => {
    const { setIsLoading, parentId, filter, fetchFiles } = this.props;
    setIsLoading(true);
    fetchFiles(parentId, filter).finally(() => setIsLoading(false));
  };

  onCheck = (checked) => {
    this.props.setSelected(checked ? 'all' : 'none');
  };

  onSelect = (item) => {
    this.props.setSelected(item.key);
  };

  onClose = () => {
    this.props.setSelected('close');
  };

  getMenuItems = () => {
    const { t, getHeaderMenu, cbMenuItems, getCheckboxItemLabel } = this.props;

    const headerMenu = getHeaderMenu(t);
    const children = cbMenuItems.map((key, index) => {
      const label = getCheckboxItemLabel(t, key);
      return <DropDownItem key={key} label={label} data-index={index} />;
    });

    let menu = [
      {
        label: t('Common:Select'),
        isDropdown: true,
        isSeparator: true,
        isSelect: true,
        fontWeight: 'bold',
        children,
        onSelect: this.onSelect,
      },
    ];

    menu = [...menu, ...headerMenu];

    return menu;
  };

  onClickFolder = (data) => {
    const { setSelectedNode, setIsLoading, fetchFiles } = this.props;
    setSelectedNode(data);
    setIsLoading(true);
    fetchFiles(data, null, true, false)
      .catch((err) => toastr.error(err))
      .finally(() => setIsLoading(false));
  };

  render() {
    //console.log("Body header render");

    const {
      tReady,
      isRootFolder,
      title,
      canCreate,
      isDesktop,
      isTabletView,
      personal,
      navigationPath,
    } = this.props;
    const menuItems = this.getMenuItems();

    return (
      <Navigation
        isRootFolder={isRootFolder}
        canCreate={canCreate}
        title={title}
        isDesktop={isDesktop}
        isTabletView={isTabletView}
        personal={personal}
        tReady={tReady}
        menuItems={menuItems}
        navigationItems={navigationPath}
        getContextOptionsPlus={this.getContextOptionsPlus}
        getContextOptionsFolder={this.getContextOptionsFolder}
        onClose={this.onClose}
        onClick={this.onCheck}
        onClickFolder={this.onClickFolder}
        onBackToParentFolder={this.onBackToParentFolder}
      />
    );
  }
}

export default inject(
  ({
    auth,
    filesStore,
    dialogsStore,
    selectedFolderStore,
    treeFoldersStore,
    filesActionsStore,
    settingsStore,
  }) => {
    const {
      setSelected,
      setSelection,
      fileActionStore,
      fetchFiles,
      filter,
      canCreate,
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      isThirdPartySelection,
      setIsLoading,
      viewAs,
      cbMenuItems,
      getCheckboxItemLabel,
      getFolderInfo,
      setBufferSelection,
    } = filesStore;
    const { setAction } = fileActionStore;
    const {
      setSharingPanelVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setDeleteDialogVisible,
      setIsFolderActions,
    } = dialogsStore;

    const { deleteAction, downloadAction, getHeaderMenu } = filesActionsStore;

    return {
      isDesktop: auth.settingsStore.isDesktopClient,
      isRootFolder: selectedFolderStore.parentId === 0,
      title: selectedFolderStore.title,
      parentId: selectedFolderStore.parentId,
      currentFolderId: selectedFolderStore.id,
      pathParts: selectedFolderStore.pathParts,
      navigationPath: selectedFolderStore.navigationPath,
      filter,
      canCreate,
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      isThirdPartySelection,
      isTabletView: auth.settingsStore.isTabletView,
      confirmDelete: settingsStore.confirmDelete,
      personal: auth.settingsStore.personal,
      viewAs,
      cbMenuItems,
      setSelectedNode: treeFoldersStore.setSelectedNode,
      getFolderInfo,

      setSelected,
      setSelection,
      setAction,
      setIsLoading,
      fetchFiles,
      setSharingPanelVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setBufferSelection,
      setIsFolderActions,
      deleteAction,
      setDeleteDialogVisible,
      downloadAction,
      getHeaderMenu,
      getCheckboxItemLabel,
    };
  },
)(withTranslation(['Home', 'Common', 'Translations'])(withRouter(observer(SectionHeaderContent))));
