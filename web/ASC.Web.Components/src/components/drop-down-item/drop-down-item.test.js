import React from 'react';
import { mount } from 'enzyme';
import DropDownItem from '.';

describe('<DropDownItem />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <DropDownItem 
        isSeparator={false}
        isHeader={false} 
        label='Button 1' 
        icon='NavLogoIcon' 
        onClick={() => console.log('Button 1 clicked')} />
    );

    expect(wrapper).toExist();
  });
});
