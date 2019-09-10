import React from 'react';
import { mount } from 'enzyme';
import InputBlock from '.';
import Button from '../button';

describe('<IconButton />', () => {
  it('renders without error', () => {
    const mask = [/\d/, /\d/, '/', /\d/, /\d/, '/', /\d/, /\d/, /\d/, /\d/];
    const wrapper = mount(
      <InputBlock mask={mask} iconName={"SearchIcon"} onIconClick={event => alert(event.target.value)}
        onChange={event => alert(event.target.value)} >
        <Button size='base' isDisabled={false} onClick={() => alert('Button clicked')} label="OK" />
      </InputBlock>
    );

    expect(wrapper).toExist();
  });
});
