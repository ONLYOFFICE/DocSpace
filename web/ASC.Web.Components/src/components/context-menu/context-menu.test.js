import React from 'react';
import { mount } from 'enzyme';
import ContextMenu from '.';

describe('<ContextMenu />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <ContextMenu targetAreaId='rowContainer' options={[]} />
    );

    expect(wrapper).toExist();
  });
});
