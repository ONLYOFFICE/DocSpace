import React from 'react';
import { mount } from 'enzyme';
import MainButton from '.';
import Button from '../button';

describe('<MainButton />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <MainButton text='Button' isDisabled={false} isDropdown={true}>
          <div>Some button</div>
          <Button label='Some button' />
      </MainButton>
    );

    expect(wrapper).toExist();
  });
});
