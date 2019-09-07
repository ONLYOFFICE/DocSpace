import React from 'react';
import { mount } from 'enzyme';
import ComboBox from '.';

describe('<ComboBox />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <ComboBox
        options={[]} // An empty array will enable advancedOptions
        advancedOptions={[]}
        onSelect={option => console.log("Selected option", option)}
        selectedOption={{
          key: 0,
          label: "Select"
        }}
        isDisabled={false}
        scaled={false}
        size="content"
        directionX="right"
      />
    );

    expect(wrapper).toExist();
  });
});
