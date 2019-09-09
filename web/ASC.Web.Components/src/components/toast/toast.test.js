import React from 'react';
import { mount } from 'enzyme';
import Toast from '.';
import toastr from './toastr';

describe('<Textarea />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <Toast>
        {toastr.success('Some text for toast')}
      </Toast>
    );

    expect(wrapper).toExist();
  });
});
