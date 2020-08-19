import React from "react";
import { TreeMenu, TreeNode, Icons } from "asc-web-components";
import styled from "styled-components";       
import { history, utils } from "asc-web-common";
import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";

const i18n = createI18N({
  page: "SettingsTree",
  localesPath: "pages/Settings"
})

const { changeLanguage } = utils;

const StyledTreeMenu = styled(TreeMenu)`
  margin-top: 20px !important;

  .rc-tree-node-content-wrapper{
    background: ${props => !props.dragging && "none !important"};
  }
  
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

    this.state = {
      drop: false
    };
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
    const path = section[0];
    if(path === 'settings')
      return history.push('/products/files/settings/common-settings');
    return history.push(`/products/files/settings/${path}`);
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
          key='common-settings'
          isLeaf={true}
          title={t('treeSettingsCommonSettings')}
        />
        <TreeNode
          className="settings-node"
          id='admin-settings'
          key='admin-settings'
          isLeaf={true}
          title={t('treeSettingsAdminSettings')}
        />
        <TreeNode
          className="settings-node"
          id='connected-clouds'
          key='connected-clouds'
          isLeaf={true}
          title={t('treeSettingsConnectedCloud')}
        />
      </TreeNode>
    );
  }

  render() {
    const nodes = this.renderTreeNode();

    return (
      <StyledTreeMenu
        defaultExpandParent={false}
        className="settings-tree-menu"
        switcherIcon={this.switcherIcon}
        onSelect={this.onSelect}
        showIcon={true}
        onExpand={() => console.log('expand')}
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

export default TreeSettings;