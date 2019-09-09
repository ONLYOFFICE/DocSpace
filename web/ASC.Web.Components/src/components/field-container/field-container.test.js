import React from 'react';
import { mount } from 'enzyme';
import FieldContainer from '.';
import TextInput from '../text-input';

describe('<FieldContainer />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <FieldContainer labelText="Name:">
        <TextInput value="" onChange={(e) => console.log(e.target.value)} />
      </FieldContainer>
    );

    expect(wrapper).toExist();
  });
});
