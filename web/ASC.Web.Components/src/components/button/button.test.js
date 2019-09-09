import React from 'react';
import { mount } from 'enzyme';
import Button from '.';

describe('<Button />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Button size='base' isDisabled={false} onClick={() => alert('Button clicked')} label="OK" />
    );

    expect(wrapper).toExist();
  });
});
