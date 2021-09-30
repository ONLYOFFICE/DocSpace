import React from 'react';
import { withTranslation } from 'react-i18next';
import Filter from '@appserver/common/api/people/filter';
import Loaders from '@appserver/common/components/Loaders';
import { inject, observer } from 'mobx-react';
import { getSelectedGroup } from '../../../helpers/people-helpers';
import { withRouter } from 'react-router';
import { isMobile } from '@appserver/components/utils/device';
import { isMobileOnly } from 'react-device-detect';
import config from '../../../../package.json';
import { combineUrl } from '@appserver/common/utils';
import { AppServerConfig } from '@appserver/common/constants';
import CatalogItem from '@appserver/components/catalog-item';

const departmentsIcon = 'images/departments.group.react.svg';
const groupIcon = '/static/images/catalog.folder.react.svg';

const CatalogBodyContent = ({
  selectedKeys,
  groups,
  setFirstLoad,
  toggleShowText,
  showText,
  groupsCaption,
  history,
  filter,
  selectGroup,
  isVisitor,
  isLoaded,
  setDocumentTitle,
}) => {
  React.useEffect(() => {
    changeTitleDocument();
    setFirstLoad(false);
  }, []);

  React.useEffect(() => {
    changeTitleDocument();
  }, [selectedKeys[0]]);

  const changeTitleDocument = (id) => {
    const currentGroup = getSelectedGroup(groups, id === 'root' ? selectedKeys[0] : id);
    currentGroup ? setDocumentTitle(currentGroup.name) : setDocumentTitle();
  };

  const isActive = (group) => {
    if (group === selectedKeys[0]) return true;
    return false;
  };
  const onClick = (data) => {
    const isRoot = data === 'departments';
    const groupId = isRoot ? 'root' : data;
    changeTitleDocument(groupId);

    if (window.location.pathname.indexOf('/people/filter') > 0) {
      selectGroup(groupId);
      if (isMobileOnly || isMobile()) toggleShowText();
    } else {
      const newFilter = isRoot ? Filter.getDefault() : filter.clone();

      if (!isRoot) newFilter.group = groupId;

      const urlFilter = newFilter.toUrlParams();
      const url = combineUrl(AppServerConfig.proxyURL, config.homepage, `/filter?${urlFilter}`);
      history.push(url);
      if (isMobileOnly || isMobile()) toggleShowText();
    }
  };

  const getItems = (data) => {
    const items = data.map((group) => {
      return (
        <CatalogItem
          key={group.id}
          id={group.id}
          icon={groupIcon}
          text={group.name}
          showText={showText}
          showInitial={true}
          onClick={onClick}
          isActive={isActive(group.id)}
        />
      );
    });
    return items;
  };
  return (
    !isVisitor &&
    (!isLoaded ? (
      <Loaders.TreeFolders />
    ) : (
      <>
        <CatalogItem
          key={'root'}
          id={'departments'}
          icon={departmentsIcon}
          onClick={onClick}
          text={groupsCaption}
          showText={showText}
          isActive={isActive('root')}
        />
        {getItems(groups)}
      </>
    ))
  );
};

const BodyContent = withTranslation('Article')(withRouter(CatalogBodyContent));

export default inject(({ auth, peopleStore }) => {
  const { settingsStore, isLoaded, setDocumentTitle, isAdmin } = auth;
  const { customNames, showText, toggleShowText } = settingsStore;
  const {
    groupsStore,
    selectedGroupStore,
    editingFormStore,
    filterStore,
    loadingStore,
  } = peopleStore;
  const { filter } = filterStore;
  const { groups } = groupsStore;
  const { groupsCaption } = customNames;
  const { isEdit, setIsVisibleDataLossDialog } = editingFormStore;
  const { selectedGroup, selectGroup } = selectedGroupStore;
  const selectedKeys = selectedGroup ? [selectedGroup] : ['root'];
  const { setFirstLoad, isLoading } = loadingStore;
  return {
    setDocumentTitle,
    isLoaded,
    isVisitor: auth.userStore.user.isVisitor,
    isAdmin,
    groups,
    groups,
    groupsCaption,
    selectedKeys,
    selectGroup,
    isEdit,
    setIsVisibleDataLossDialog,
    isLoading,
    filter,
    setFirstLoad,
    showText,
    toggleShowText,
  };
})(observer(BodyContent));
