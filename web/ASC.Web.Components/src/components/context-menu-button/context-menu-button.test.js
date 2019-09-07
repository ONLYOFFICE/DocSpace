import React from 'react';
import { mount } from 'enzyme';
import ContextMenuButton from '.';

describe('<ContextMenuButton />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <ContextMenuButton
          title="Actions"
          getData={() => [{key: 'key', label: 'label', onClick: () => alert('label')}]}
      />
    );

    expect(wrapper).toExist();
  });
});
