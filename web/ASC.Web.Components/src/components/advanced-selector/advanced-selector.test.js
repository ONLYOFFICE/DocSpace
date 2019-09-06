import React from 'react';
import { mount } from 'enzyme';
import AdvancedSelector from '.';

describe('<AdvancedSelector />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <AdvancedSelector 
        placeholder="Search users"
        onSearchChanged={(e) => console.log(e.target.value)}
        options={[]}
        isMultiSelect={false}
        buttonLabel="Add members"
        onSelect={(selectedOptions) => console.log("onSelect", selectedOptions)}
        />
    );

    expect(wrapper).toExist();
  });
});
