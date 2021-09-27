import React from 'react';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router';
import CatalogItem from '@appserver/components/catalog-item';
import { inject, observer } from 'mobx-react';
import { withTranslation } from 'react-i18next';
import { combineUrl } from '@appserver/common/utils';
import config from '../../../../package.json';
import { AppServerConfig } from '@appserver/common/constants';
import withLoader from '../../../HOCs/withLoader';
import { isTablet } from '@appserver/components/utils/device';

const PureSettingsItems = ({
  match,
  expandedSetting,
  setSelectedNode,
  setExpandSettingsTree,
  setSelectedFolder,
  history,
  setIsLoading,
  t,
  showText,
  homepage,
  setShowText,
}) => {
  const { setting } = match.params;
  const iconUrl = '/static/images/settings.react.svg';

  React.useEffect(() => {
    setIsLoading(true);
    setSelectedNode([setting]);
    setIsLoading(false);
  }, [setting, setIsLoading, setSelectedNode]);

  React.useEffect(() => {
    const { setting } = match.params;
    if (setting && !expandedSetting) setExpandSettingsTree(['settings']);
  }, [match, expandedSetting, setExpandSettingsTree]);

  const onClick = () => {
    setSelectedFolder(null);

    setSelectedNode(['common']);
    if (!expandedSetting || expandedSetting[0] !== 'settings') setExpandSettingsTree(`settings`);
    if (isTablet()) setShowText(false);
    return history.push(combineUrl(AppServerConfig.proxyURL, homepage, '/settings/common'));

    // if (selectedTreeNode[0] !== path) {
    //   setSelectedNode(section);
    //   return history.push(
    //     combineUrl(AppServerConfig.proxyURL, config.homepage, `/settings/${path}`),
    //   );
    // }
  };

  const isActive = () => {
    return window.location.pathname.indexOf('/settings') > 0;
  };

  return (
    <CatalogItem
      id="settings"
      key="settings"
      text={t('Common:Settings')}
      icon={iconUrl}
      showText={showText}
      onClick={onClick}
      isActive={isActive()}
    />
  );
};

const SettingsItems = withTranslation(['Settings', 'Common'])(
  withRouter(withLoader(PureSettingsItems)(<></>)),
);

export default inject(
  ({ auth, filesStore, settingsStore, treeFoldersStore, selectedFolderStore }) => {
    const { setIsLoading } = filesStore;
    const { setSelectedFolder } = selectedFolderStore;
    const { selectedTreeNode, setSelectedNode } = treeFoldersStore;
    const { expandedSetting, setExpandSettingsTree } = settingsStore;
    return {
      selectedTreeNode,
      expandedSetting,
      setIsLoading,
      setSelectedFolder,
      setSelectedNode,
      setExpandSettingsTree,
      showText: auth.settingsStore.showText,
      setShowText: auth.settingsStore.setShowText,
      homepage: config.homepage,
    };
  },
)(observer(SettingsItems));
