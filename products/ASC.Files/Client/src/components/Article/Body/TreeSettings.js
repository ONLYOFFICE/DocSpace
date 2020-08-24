import React from "react";
import { connect } from 'react-redux';
import { withRouter } from 'react-router';
import { TreeMenu, TreeNode, Icons } from "asc-web-components";
import styled from "styled-components";       
import { history, utils } from "asc-web-common";
import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";

import { setSelectedSetting, setExpandSettingsTree } from '../../../store/files/actions';

const i18n = createI18N({
  page: "Settings",
  localesPath: "pages/Settings"
})

const { changeLanguage } = utils;

const StyledTreeMenu = styled(TreeMenu)`
  margin-top: 20px !important;
  
  .rc-tree-node-selected {
    background: #DFE2E3 !important;
  }

  .settings-node {
    margin-left: 8px !important;
  }
`;

class PureTreeSettings extends React.Component {
  constructor(props) {
    super(props);
  }

  componentDidMount() {
    const { match, setSelectedSetting, setExpandSettingsTree } = this.props;
    const { setting } = match.params;
    setSelectedSetting([setting]);
    setExpandSettingsTree(['settings']);
  }

  switcherIcon = (obj) => {
    if (obj.isLeaf) {
      return null;
    }
    if(obj.expanded) {
      return <Icons.ExpanderDownIcon size="scale" isfill color="dimgray" />;
    } else {
      return <Icons.ExpanderRightIcon size="scale" isfill color="dimgray" />;
    }
  }

  onSelect = (section) => {
    const { setSelectedSetting, setExpandSettingsTree } = this.props;
    const path = section[0];

    if(path === 'settings') {
      setSelectedSetting(['common']);
      setExpandSettingsTree(section);
      return history.push('/products/files/settings/common');
    } 
    
    setSelectedSetting(section);
    return history.push(`/products/files/settings/${path}`);    
  }

  onExpand = (data) => {
    const { setExpandSettingsTree, setSelectedSetting } = this.props;
    setSelectedSetting(['common']);
    setExpandSettingsTree(data);
  }

  renderTreeNode = () => {
    const { t } = this.props;
    return (
      <TreeNode
        id="settings"
        key="settings"
        title={t('treeSettingsMenuTitle')}
        isLeaf={false}
        icon={<Icons.SettingsIcon size="scale" isfill color="dimgray" />}
      >
        <TreeNode
          className="settings-node"
          id='common-settings'
          key='common'
          isLeaf={true}
          title={t('treeSettingsCommonSettings')}
        />
        <TreeNode
          className="settings-node"
          id='admin-settings'
          key='admin'
          isLeaf={true}
          title={t('treeSettingsAdminSettings')}
        />
        <TreeNode
        selectable={true}
          className="settings-node"
          id='connected-clouds'
          key='thirdParty'
          isLeaf={true}
          title={t('treeSettingsConnectedCloud')}
        />
      </TreeNode>
    );
  }

  render() {
    const { 
      selectedSetting,
      expandedSetting
    } = this.props;
    const nodes = this.renderTreeNode();

    return (
      <StyledTreeMenu
        expandedKeys={expandedSetting}
        selectedKeys={selectedSetting}
        defaultExpandParent={false}
        className="settings-tree-menu"
        switcherIcon={this.switcherIcon}
        onSelect={this.onSelect}
        showIcon={true}
        onExpand={this.onExpand}
      >
        {nodes}
      </StyledTreeMenu>
    );
  }
}

const TreeSettingsContainer = withTranslation()(PureTreeSettings);

const TreeSettings = props => {
  changeLanguage(i18n);
  return (
    <I18nextProvider i18n={i18n}>
      <TreeSettingsContainer { ...props } />
    </I18nextProvider>
  );
}

function mapStateToProps(state) {
  const { 
    selectedSetting,
    expandedSetting
   } = state.files;
  return {
    selectedSetting,
    expandedSetting
  }
}

export default connect(
  mapStateToProps, { 
    setSelectedSetting,
    setExpandSettingsTree 
  })(withRouter(TreeSettings));