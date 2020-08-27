import React from "react";
import { connect } from 'react-redux';
import { withRouter } from 'react-router';
import { TreeMenu, TreeNode, Icons } from "asc-web-components";
import styled from "styled-components";       
import { history, utils } from "asc-web-common";
import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";

import { setSelectedNode, setExpandSettingsTree } from '../../../store/files/actions';

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
    const { match, setSelectedNode, setExpandSettingsTree } = this.props;
    const { setting } = match.params;
    setSelectedNode([setting]);
    if (setting)
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
    const { setSelectedNode, setExpandSettingsTree } = this.props;
    const path = section[0];

    if(path === 'settings') {
      setSelectedNode(['common']);
      setExpandSettingsTree(section);
      return history.push('/products/files/settings/common');
    } 
    
    setSelectedNode(section);
    return history.push(`/products/files/settings/${path}`);    
  }

  onExpand = (data) => {
    const { setExpandSettingsTree } = this.props;
    setExpandSettingsTree(data);
  }

  renderTreeNode = () => {
    const { t, thirdParty } = this.props;
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
        {thirdParty 
          ? <TreeNode
              selectable={true}
              className="settings-node"
              id='connected-clouds'
              key='thirdParty'
              isLeaf={true}
              title={t('treeSettingsConnectedCloud')}
            /> 
          : null 
        }
      </TreeNode>
    );
  }

  render() {
    const { 
      selectedTreeNode,
      expandedSetting
    } = this.props;
    const nodes = this.renderTreeNode();

    return (
      <StyledTreeMenu
        expandedKeys={expandedSetting}
        selectedKeys={selectedTreeNode}
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
    selectedTreeNode,
    settingsTree
   } = state.files;

   const {
    expandedSetting,
    thirdParty
   } = settingsTree;

  return {
    selectedTreeNode,
    expandedSetting,
    thirdParty
  }
}

export default connect(
  mapStateToProps, { 
    setSelectedNode,
    setExpandSettingsTree 
  })(withRouter(TreeSettings));