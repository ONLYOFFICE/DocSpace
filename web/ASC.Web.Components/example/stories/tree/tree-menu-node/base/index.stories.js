import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, boolean, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import styled from '@emotion/styled';
import { TreeMenu, TreeNode } from 'asc-web-components';
import { Icons } from 'asc-web-components';


const TreeMenuStory = props => {
  const switcherIcon = (obj) => {
    if (obj.isLeaf) {
      return null;
    }
    const StyledTreeExpanderDownIcon = styled(Icons.ExpanderDownIcon)`
      margin: 0px 0px 8px 5px;
      height: 8px;
      width: 8px;
    `;
    const StyledTreeExpanderRightIcon = styled(Icons.ExpanderRightIcon)`
      margin: 0px 0px 8px 5px;
      height: 8px;
      width: 8px;
    `;
    if (obj.expanded) {
      return (<StyledTreeExpanderDownIcon size="small" color="dimgray"></StyledTreeExpanderDownIcon>);
    } else {
      return (<StyledTreeExpanderRightIcon size="small" color="dimgray"></StyledTreeExpanderRightIcon>);
    }
  };

  const StyledPeopleIcon = styled(Icons.PeopleIcon)`
    margin: 0px 3px 2px 0px;
  `;
  const sizeValue = select('size', ['small', 'middle', 'big'], 'middle');

  return (
    <div style={{width: "250px", margin: "20px"}}>
      <TreeMenu
        checkable={false}
        draggable={true}
        disabled={false}
        multiple={false}
        showIcon={true}
        showLine={false}

        switcherIcon={switcherIcon}
      >
        <TreeNode size="big" title="Parent" key="0-0" icon={<StyledPeopleIcon size="medium" color="dimgray"/>}>
          <TreeNode 
            size={sizeValue}
            isUnderline={boolean('isUnderline', true)}

            title="Child" 
            key="0-0-0"
          ></TreeNode>
        </TreeNode>
      </TreeMenu>
    </div>
  ); 
};


storiesOf('Components|Tree', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('Tree menu node', () => <TreeMenuStory />);
  

