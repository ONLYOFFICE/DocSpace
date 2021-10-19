import React from 'react';
import PropTypes from 'prop-types';
import { inject, observer } from 'mobx-react';
import CatalogItem from '@appserver/components/catalog-item';
import { FolderType } from '@appserver/common/constants';
import { withTranslation } from 'react-i18next';
import withLoader from '../../../HOCs/withLoader';
import Loaders from '@appserver/common/components/Loaders';

const Items = ({ data, showText, pathParts, selectedTreeNode, onClick, onBadgeClick }) => {
  const isActive = (item) => {
    if (selectedTreeNode.length > 0) {
      if (pathParts && pathParts.includes(item.id)) return true;
      if (selectedTreeNode[0] === '@my' && item.key === '0-0') return true;
      return `${item.id}` === selectedTreeNode[0];
    }
  };
  const getEndOfBlock = (item) => {
    switch (item.key) {
      case '0-3':
      case '0-5':
      case '0-6':
        return true;
      default:
        return false;
    }
  };

  const getFolderIcon = (item) => {
    let iconUrl = 'images/catalog.folder.react.svg';

    switch (item.rootFolderType) {
      case FolderType.USER:
        iconUrl = '/static/images/catalog.user.react.svg';
        break;
      case FolderType.SHARE:
        iconUrl = '/static/images/catalog.shared.react.svg';
        break;
      case FolderType.COMMON:
        iconUrl = '/static/images/catalog.portfolio.react.svg';
        break;
      case FolderType.Favorites:
        iconUrl = '/static/images/catalog.favorites.react.svg';
        break;
      case FolderType.Recent:
        iconUrl = '/static/images/catalog.recent.react.svg';
        break;
      case FolderType.Privacy:
        iconUrl = '/static/images/catalog.private.react.svg';
        break;
      case FolderType.TRASH:
        iconUrl = '/static/images/catalog.trash.react.svg';
        break;
      default:
        break;
    }

    switch (item.providerKey) {
      case 'GoogleDrive':
        iconUrl = '/static/images/cloud.services.google.drive.react.svg';
        break;
      case 'Box':
        iconUrl = '/static/images/cloud.services.box.react.svg';
        break;
      case 'DropboxV2':
        iconUrl = '/static/images/cloud.services.dropbox.react.svg';
        break;
      case 'OneDrive':
        iconUrl = '/static/images/cloud.services.onedrive.react.svg';
        break;
      case 'SharePoint':
        iconUrl = '/static/images/cloud.services.onedrive.react.svg';
        break;
      case 'kDrive':
        iconUrl = '/static/images/catalog.folder.react.svg';
        break;
      case 'Yandex':
        iconUrl = '/static/images/catalog.folder.react.svg';
        break;
      case 'NextCloud':
        iconUrl = '/static/images/cloud.services.nextcloud.react.svg';
        break;
      case 'OwnCloud':
        iconUrl = '/static/images/catalog.folder.react.svg';
        break;
      case 'WebDav':
        iconUrl = '/static/images/catalog.folder.react.svg';
        break;
      default:
        break;
    }

    return iconUrl;
  };

  const getItem = (data) => {
    const items = data.map((item) => {
      const showBadge = item.newItems ? item.newItems > 0 && true : false;

      return (
        <CatalogItem
          key={item.id}
          id={item.id}
          icon={getFolderIcon(item)}
          showText={showText}
          text={item.title}
          isActive={isActive(item)}
          onClick={onClick}
          isEndOfBlock={getEndOfBlock(item)}
          showBadge={showBadge}
          labelBadge={showBadge ? item.newItems : null}
          onClickBadge={onBadgeClick}
        />
      );
    });
    return items;
  };

  return <>{getItem(data)}</>;
};

Items.propTypes = {
  data: PropTypes.array,
  showText: PropTypes.bool,
  selectedTreeNode: PropTypes.array,
  onClick: PropTypes.func,
  onClickBadge: PropTypes.func,
};

export default inject(({ auth, treeFoldersStore, selectedFolderStore }) => {
  const { treeFolders, selectedTreeNode } = treeFoldersStore;

  return {
    showText: auth.settingsStore.showText,
    pathParts: selectedFolderStore.pathParts,
    data: treeFolders,
    selectedTreeNode,
  };
})(
  withTranslation(['Home', 'Common'])(
    withLoader(observer(Items))(<Loaders.DocumentCatalogFolderLoader />),
  ),
);
