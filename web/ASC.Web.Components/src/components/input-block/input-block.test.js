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

  it('test base size props', () => {
    const wrapper = mount(
      <InputBlock iconName="SearchIcon" size="base">
      </InputBlock>
    );

    expect(wrapper.prop('size')).toBe('base');
  });

  it('test middle size props', () => {
    const wrapper = mount(
      <InputBlock iconName="SearchIcon" size="middle" >
      </InputBlock>
    );

    expect(wrapper.prop('size')).toBe('middle');
  });

  it('test big size props', () => {
    const wrapper = mount(
      <InputBlock iconName="SearchIcon" size="big" >
      </InputBlock>
    );

    expect(wrapper.prop('size')).toBe('big');
  });

  it('test huge size props', () => {
    const wrapper = mount(
      <InputBlock iconName="SearchIcon" size="huge" >
      </InputBlock>
    );

    expect(wrapper.prop('size')).toBe('huge');
  });

  it('test iconSize props', () => {
    const wrapper = mount(
      <InputBlock iconName="SearchIcon" iconSize={18} >
      </InputBlock>
    );

    expect(wrapper.prop('iconSize')).toBe(18);
  });

  it('test empty props', () => {
    const wrapper = mount(
      <InputBlock>
      </InputBlock>
    );

    expect(wrapper).toExist();
  });

  it('accepts id', () => {
    const wrapper = mount(
      <InputBlock id="testId" />
    );

    expect(wrapper.prop('id')).toEqual('testId');
  });

  it('accepts className', () => {
    const wrapper = mount(
      <InputBlock className="test" />
    );

    expect(wrapper.prop('className')).toEqual('test');
  });

  it('accepts style', () => {
    const wrapper = mount(
      <InputBlock style={{ color: 'red' }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty('color', 'red');
  });
  it('call onChange', () => {
    const onChange = jest.fn();
    const wrapper = mount(<InputBlock iconName="SearchIcon" size="huge" onChange={onChange}/>);
    const input = wrapper.find("input");
    input.first().simulate("change", { target: { value: "test" } });
    expect(onChange).toHaveBeenCalled();
  });
  it('call onIconClick', () => {
    const onIconClick = jest.fn();
    const wrapper = mount(<InputBlock iconName="SearchIcon" size="huge" isDisabled={false} onIconClick={onIconClick}/>);
    const input = wrapper.find(".append div");
    input.first().simulate('click');
    expect(onIconClick).toHaveBeenCalled();
  });
  it('not call onChange', () => {
    const onChange = jest.fn();
    const wrapper = mount(<InputBlock iconName="SearchIcon" size="huge"/>);
    const input = wrapper.find("input");
    input.first().simulate("change", { target: { value: "test" } });
    expect(onChange).not.toHaveBeenCalled();
  });
  it('not call onIconClick', () => {
    const onIconClick = jest.fn();
    const wrapper = mount(<InputBlock iconName="SearchIcon" size="huge" isDisabled={true} onIconClick={onIconClick}/>);
    const input = wrapper.find(".append div");
    input.first().simulate('click');
    expect(onIconClick).not.toHaveBeenCalled();
  });
  
});
