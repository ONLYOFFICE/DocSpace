import React from 'react';
import { mount } from 'enzyme';
import IconButton from '.';

describe('<IconButton />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <IconButton size='25' isDisabled={false} onClick={() => alert('Clicked')} iconName={"SearchIcon"} isFill={true} />
    );

    expect(wrapper).toExist();
  });
});
