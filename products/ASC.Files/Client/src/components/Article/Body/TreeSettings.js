import React from "react";
import { TreeMenu, TreeNode, Icons, toastr, utils, Badge } from "asc-web-components";
import styled from "styled-components";
import { api, constants } from "asc-web-common";
const { files } = api;
const { FolderType, ShareAccessRights } = constants;


const StyledTreeMenu = styled(TreeMenu)`
  .rc-tree-node-content-wrapper{
    background: ${props => !props.dragging && "none !important"};
  }
  
  .rc-tree-node-selected {
    background: #DFE2E3 !important;
  }
`;

class TreeSettings extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      drop: false
    };
  }

  switcherIcon = (obj) => {
    if(obj.expanded) {
      return <Icons.ExpanderDownIcon size="scale" isfill color="dimgray" />;
    } else {
      return <Icons.ExpanderRightIcon size="scale" isfill color="dimgray" />;
    }
  }

  onSelect = (e, r) => {
    console.log(e, r)
  }

  renderTreeNode = () => {
    const { t } = this.props;
    return (
      <TreeNode
        id="settings"
        key="settings"
        title={t('settingsMenuTitle')}
        icon={<Icons.SettingsIcon size="scale" isfill color="dimgray" />}
      >
        <TreeNode
          id='common-settings'
          key='common-settings'
          title={t('settingCommonSettings')}
          icon={false}
          isLeaf={true}

        ></TreeNode>
        <TreeNode
          id='admin-settings'
          key='admin-settings'
          title={t('settingsAdminSettings')}
        ></TreeNode>
        <TreeNode
          id='connected-cloud'
          key='connected-cloud'
          title={t('settingsConnectedCloud')}
        ></TreeNode>
      </TreeNode>
    );
  }

  render() {
    const { drop } = this.state;

    const nodes = this.renderTreeNode();

    return (
      <StyledTreeMenu
        className="settings-tree-menu"
        switcherIcon={this.switcherIcon}
        onSelect={this.onSelect}
        onClick={()=>console.log('111')}
        showIcon={true}
      >
        {nodes}
      </StyledTreeMenu>
    );
  }
}

export default TreeSettings;