import React, {useState} from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, boolean} from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import styled from '@emotion/styled';
import { TreeMenu, TreeNode } from 'asc-web-components';
import { Icons } from 'asc-web-components';
import { action } from '@storybook/addon-actions';

const treeData = [
  { key: '0-0', title: 'Departments', root: true, children:
    [
      { key: '0-0-0', title: 'Development', root: false},
      { key: '0-0-1', title: 'Direction', root: false },
      { key: '0-0-2', title: 'Marketing', root: false },
      { key: '0-0-3', title: 'Support', root: false }
    ],
  },
];

const TreeMenuStory = props => {
  const { data } = props;

  const [gData, setGData] = useState(data);
  const [autoExpandParent, setAutoExpandParent] = useState(true);
  
  const [expandedKeys, setExpandedKeys] = useState(['0-0-key', '0-0-0-key', '0-0-0-0-key']);
  
  const onDragStart = (info) => {
    info.event.persist();
  };

  const onDragEnter = (info) => {
    setExpandedKeys(info.expandedKeys);
  }

  const onDrop = (info) => {
      info.event.persist();
      const dropKey = info.node.props.eventKey;
      const dragKey = info.dragNode.props.eventKey;
      const dropPos = info.node.props.pos.split('-');
      const dropPosition = info.dropPosition - Number(dropPos[dropPos.length - 1]);

      const loop = (data, key, callback) => {
          data.forEach((item, index, arr) => {
              if (item.key === key) {
                  callback(item, index, arr);
                  return;
              }
              if (item.children) {
                  loop(item.children, key, callback);
              }
          });
      };
      const data = [...gData];

      let dragObj;
      loop(data, dragKey, (item, index, arr) => {
          arr.splice(index, 1);
          dragObj = item;
      });

      if (!info.dropToGap) {
          loop(data, dropKey, (item) => {
              item.children = item.children || [];
              item.children.push(dragObj);
          });
      } else if (
          (info.node.props.children || []).length > 0 &&  
          info.node.props.expanded &&                     
          dropPosition === 1                              
      ) {
          loop(data, dropKey, (item) => {
              item.children = item.children || [];
              item.children.unshift(dragObj);
          });
      } else {
          let ar;
          let i;
          loop(data, dropKey, (item, index, arr) => {
              ar = arr;
              i = index;
          });
          if (dropPosition === -1) {
              ar.splice(i, 0, dragObj);
          } else {
              ar.splice(i + 1, 0, dragObj);
          }
      }
      setGData(data);
  };
  const onExpand = (expandedKeys) => {
    setExpandedKeys(expandedKeys);
    setAutoExpandParent(false);
  }

  const StyledCatalogDepartmentsIcon = styled(Icons.CatalogDepartmentsIcon)`
    height: 16px;
    width: 16px;
    margin: 0px 3px 2px 0px;
  `;
  const StyledCatalogFolderIcon = styled(Icons.CatalogFolderIcon)`
    height: 16px;
    width: 16px;
    margin: 0px 3px 2px 0px;
  `;

  const loop = data => {
      return data.map((item) => {
          if (item.children && item.children.length) {
              return <TreeNode size="big" title={item.title} key={item.key} icon={item.root ? <StyledCatalogDepartmentsIcon size="medium" isfill={true} color="dimgray"/> : ''} >{loop(item.children)}</TreeNode>;
          }
          return <TreeNode size="middle" key={item.key} title={item.title} icon={!item.root ? <StyledCatalogFolderIcon size="medium" isfill={true} color="dimgray"/> : ''} ></TreeNode>;
      });
  };
  
  const switcherIcon = (obj) => {
    if (obj.isLeaf) {
      return null;
    }
    const StyledTreeExpanderDownIcon = styled(Icons.ExpanderDownIcon)`
      margin: 0 0 0 5px;
      height: 8px;
      width: 8px;
    `;
    const StyledTreeExpanderRightIcon = styled(Icons.ExpanderRightIcon)`
      margin: 0 0 2px 5px;
      height: 8px; 
      width: 8px;
    `;
    if (obj.expanded) {
      return <StyledTreeExpanderDownIcon size="small" isfill={true} color="dimgray"></StyledTreeExpanderDownIcon>
    } else {
      return <StyledTreeExpanderRightIcon size="small" isfill={true} color="dimgray"></StyledTreeExpanderRightIcon>
    }
  };
  
  return (
    <div style={{width: "250px", margin: "20px"}}>
      <TreeMenu
        checkable={boolean('checkable', false)}
        draggable={boolean('draggable', false)}
        disabled={boolean('disabled', false)}
        multiple={boolean('multiple', false)}
        showIcon={boolean('showIcon', true)}
        
        defaultExpandAll={boolean('defaultExpandAll', false)}
        defaultExpandParent={boolean('defaultExpandParent', true)}  

        onExpand={onExpand} 
        autoExpandParent={autoExpandParent}
        expandedKeys={expandedKeys}
        
        onDragStart={(info) => onDragStart(info)}
        onDrop={(info) => onDrop(info)}
        onDragEnter={onDragEnter}
        
        switcherIcon={switcherIcon}

        onSelect={action("select")}
        onLoad={action("load")}
        
        onRightClick={action("rightClick")}
      >
        {loop(gData)}
      </TreeMenu>
    </div>
  );
};

storiesOf('Components|Tree', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('Tree menu', () => <TreeMenuStory data={treeData} />);
  

