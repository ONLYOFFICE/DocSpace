import React from 'react';
import { mount } from 'enzyme';
import TreeMenu from '.';
import TreeNode from './sub-components/tree-node';

describe('<TreeMenu />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <TreeMenu
        checkable={false}
        draggable={true}
        disabled={false}
        multiple={false}
        showIcon={true}
        showLine={false}
      >
        <TreeNode title="Parent" key="0-0">
          <TreeNode 
            title="Child" 
            key="0-0-0"
          ></TreeNode>
        </TreeNode>
      </TreeMenu>
    );

    expect(wrapper).toExist();
  });
});
