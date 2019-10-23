import React from 'react';
import { utils } from 'asc-web-components';
import { connect } from 'react-redux';
import {
  TreeMenu,
  TreeNode,
  Icons
} from "asc-web-components";
import { setNewSelectedNode } from '../../../../../store/auth/actions';
import { withRouter } from "react-router";

const getItems = data => {
  return data.map(item => {
    if (item.children && item.children.length) {
      return (
        <TreeNode
          title={item.title}
          key={item.key}
          icon={item.icon && React.createElement(Icons[item.icon], {
            size: 'scale',
            isfill: true,
            color: 'dimgray',
          })}
        >
          {getItems(item.children)}
        </TreeNode>
      );
    }
    return (
      <TreeNode
        key={item.key}
        title={item.title}
        icon={item.icon && React.createElement(Icons[item.icon], {
          size: 'scale',
          isfill: true,
          color: 'dimgray',
        })}
      />
    );
  });
};

const getObjectForState = (key, title, subtitle, link) => {
  const selectedInfo = {
    selectedKey: key,
    selectedTitle: title,
    selectedSubtitle: subtitle,
    selectedLink: link,
  };
  return selectedInfo;
}

const getKeyByLink = (data, linkArr) => {
  const length = linkArr.length;
  if (length === 1 || !linkArr[1].length) {
    const arrLength = data.length;
    for (let i = 0; i < arrLength; i++) {
      if (data[i].link === linkArr[0]) {
        return data[i].children ? data[i].children[0].key : data[i].key;
      }
    }
  } else if (length === 2) {
    const arrLength = data.length;
    let key;

    for (let i = 0; i < arrLength; i++) {
      if (data[i].link === linkArr[0]) {
        key = i;
        break;
      }
    }

    const selectedArr = data[key].children;
    const childrenLength = selectedArr.length;
    for (let i = 0; i < childrenLength; i++) {
      if (selectedArr[i].link === linkArr[1]) {
        return selectedArr[i].key;
      }

    }
  }
  return '0-0';
}

class ArticleBodyContent extends React.Component {

  constructor(props) {
    super(props);

    const { data, selectedKeys, match, history } = props;
    const fullSettingsUrl = props.match.url;
    const locationPathname = props.location.pathname;

    if (locationPathname === fullSettingsUrl) {
      const newPath = match.path + this.getSelectedLinkByKey(selectedKeys[0]);
      history.push(newPath);
      return;
    }

    const fullSettingsUrlLength = fullSettingsUrl.length;

    const resultPath = locationPathname.slice(fullSettingsUrlLength + 1);
    const arrayOfParams = resultPath.split('/');

    const key = getKeyByLink(data, arrayOfParams);
    const title = this.getSelectedTitleByKey(key[0]);
    const subtitle = this.getSelectedTitleByKey(key);
    const link = this.getSelectedLinkByKey(key);

    this.sendNewSelectedData([key], title, subtitle, link);
    const path = match.path + link;
    history.push(path);
  }

  componentDidUpdate() {
    const { selectedKeys, match, history } = this.props;
    const settingsPath = this.getSelectedLinkByKey(selectedKeys[0]);
    const newPath = match.path + settingsPath;
    history.push(newPath);
  }

  shouldComponentUpdate(nextProps) {
    if (!utils.array.isArrayEqual(nextProps.selectedKeys, this.props.selectedKeys)) {
      return true;
    }

    if (!utils.array.isArrayEqual(nextProps.data, this.props.data)) {
      return true;
    }

    return false;
  }

  sendNewSelectedData = (key, title, subtitle, link) => {
    const { setNewSelectedNode } = this.props;
    const data = getObjectForState(key, title, subtitle, link);
    setNewSelectedNode(data);
  }

  getSelectedTitleByKey = key => {
    const { data } = this.props;
    const length = key.length;
    if (length === 1) {
      return data[key].title;
    }
    else if (length === 3) {
      return data[key[0]].children[key[2]].title;
    }
  }

  getSelectedLinkByKey = key => {
    const { data } = this.props;
    const length = key.length;
    if (length === 1) {
      return '/' + data[key].link;
    }
    else if (length === 3) {
      return '/' + data[key[0]].link + '/' + data[key[0]].children[key[2]].link;
    }
  }
  onSelect = value => {
    const { data, selectedKeys } = this.props;

    if (value) {
      if (utils.array.isArrayEqual(value, selectedKeys)) {

        return;
      }
      const selectedKey = value[0];
      if (selectedKey.length === 3) {
        const selectedTitle = this.getSelectedTitleByKey(selectedKey[0]);
        const selectedSubtitle = this.getSelectedTitleByKey(selectedKey);
        const selectedLink = this.getSelectedLinkByKey(selectedKey);
        this.sendNewSelectedData(value, selectedTitle, selectedSubtitle, selectedLink);
      }
      else if (selectedKey.length === 1 && (selectedKey.toString() !== selectedKeys.toString()[0] || selectedKeys.toString()[2] !== '0')) {
        const selectedKeys = data[value].children ? [`${value.toString()}-0`] : value;
        const selectedTitle = this.getSelectedTitleByKey(selectedKey);
        const selectedSubtitle = this.getSelectedTitleByKey(selectedKeys[0]);
        const selectedLink = this.getSelectedLinkByKey(selectedKeys[0]);
        this.sendNewSelectedData(selectedKeys, selectedTitle, selectedSubtitle, selectedLink);
      }
    }
  };

  switcherIcon = obj => {
    if (obj.isLeaf) {
      return null;
    }
    if (obj.expanded) {
      return (
        <Icons.ExpanderDownIcon size="scale" isfill={true} color="dimgray" />
      );
    } else {
      return (
        <Icons.ExpanderRightIcon size="scale" isfill={true} color="dimgray" />
      );
    }
  };

  render() {
    const { data, selectedKeys } = this.props;

    console.log("SettingsTreeMenu", this.props);

    return (
      <TreeMenu
        className="people-tree-menu"
        checkable={false}
        draggable={false}
        disabled={false}
        multiple={false}
        showIcon={true}
        defaultExpandAll={true}
        switcherIcon={this.switcherIcon}
        onSelect={this.onSelect}
        selectedKeys={selectedKeys}
      >
        {getItems(data)}
      </TreeMenu>
    );
  };
};

function mapStateToProps(state) {
  return {
    data: state.auth.settings.settingsTree.list,
    selectedKeys: state.auth.settings.settingsTree.selectedKey,
    selectedTitle: state.auth.settings.settingsTree.selectedTitle,
    selectedSubtitle: state.auth.settings.settingsTree.selectedSubtitle,
    selectedLink: state.auth.settings.settingsTree.selectedLink,
  };
}

export default connect(mapStateToProps, { setNewSelectedNode })(withRouter(ArticleBodyContent));